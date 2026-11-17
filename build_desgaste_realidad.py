"""Desgaste sutil para los muros y pisos de la Realidad (casa con años).

Lee siempre la versión original de cada sprite desde git (HEAD), así que se puede volver a
ejecutar para ajustar la intensidad sin acumular desgaste. Escribe sobre el mismo PNG, sin
cambiar tamaño ni .meta.

Solo usa tonos más oscuros o apagados de los colores que ya tiene cada sprite.

    python build_desgaste_realidad.py              # todos
    python build_desgaste_realidad.py --intensidad 0.7
    python build_desgaste_realidad.py --git-ref HEAD --solo ParedBanoIzq
"""
import argparse
import hashlib
import io
import re
import subprocess
import zlib
from pathlib import Path

import numpy as np
from PIL import Image

CARPETA = Path("Assets/_Carencia/Sprites/Escenarios/Realidad")

# nombre -> tipo
SPRITES = {
    "SPR_ESC_ParedPasilloIzq_R": "pared",
    "SPR_ESC_ParedPasilloDer_R": "pared",
    "SPR_ESC_ParedIzq_R": "pared",
    "SPR_ESC_ParedDer_R": "pared",
    "SPR_ESC_ParedEsquina_R": "pared",
    "SPR_ESC_ParedBanoIzq_R": "azulejo",
    "SPR_ESC_ParedBanoDer_R": "azulejo",
    "SPR_ESC_MuroBajoIzq_R": "muro_bajo",
    "SPR_ESC_MuroBajoDer_R": "muro_bajo",
    "SPR_ESC_LosaBeige_R": "losa",
    "SPR_ESC_LosaBanoClara_R": "losa",
    "SPR_ESC_LosaBanoOscura_R": "losa",
    "SPR_ESC_SueloDamero_R": "losa",
}

REMATE = {(250, 236, 206), (212, 186, 146)}  # canto superior de los muros

BAYER4 = (np.array([[0, 8, 2, 10], [12, 4, 14, 6], [3, 11, 1, 9], [15, 7, 13, 5]]) + 0.5) / 16


# ---------------------------------------------------------------- utilidades

def leer_original(nombre, ref):
    ruta = (CARPETA / f"{nombre}.png").as_posix()
    datos = subprocess.run(["git", "cat-file", "--filters", f"{ref}:{ruta}"],  # --filters aplica LFS
                           capture_output=True, check=True).stdout
    return np.array(Image.open(io.BytesIO(datos)).convert("RGBA"))


def ruido(h, w, escala, rng, octavas=2):
    """Ruido de valor suave en [0,1]."""
    total = np.zeros((h, w))
    amp, suma = 1.0, 0.0
    for o in range(octavas):
        s = max(2.0, escala / (2 ** o))
        gh, gw = int(h / s) + 3, int(w / s) + 3
        g = rng.random((gh, gw))
        ys = np.arange(h) / s
        xs = np.arange(w) / s
        y0, x0 = ys.astype(int), xs.astype(int)
        fy, fx = ys - y0, xs - x0
        fy = fy * fy * (3 - 2 * fy)
        fx = fx * fx * (3 - 2 * fx)
        a = g[y0][:, x0]
        b = g[y0][:, x0 + 1]
        c = g[y0 + 1][:, x0]
        d = g[y0 + 1][:, x0 + 1]
        top = a + (b - a) * fx
        bot = c + (d - c) * fx
        total += amp * (top + (bot - top) * fy[:, None])
        suma += amp
        amp *= 0.5
    return total / suma


def bayer(h, w):
    return np.tile(BAYER4, (h // 4 + 1, w // 4 + 1))[:h, :w]


def lum(rgb):
    return rgb[..., 0] * 0.299 + rgb[..., 1] * 0.587 + rgb[..., 2] * 0.114


class Lienzo:
    def __init__(self, img):
        self.img = img
        self.rgb0 = img[..., :3].astype(float)  # colores originales
        self.h, self.w = img.shape[:2]
        self.opaco = img[..., 3] > 0

    def tono(self, mask, mult):
        """Aplica un tono derivado del color original de cada píxel."""
        mult = np.asarray(mult, dtype=float)
        nuevo = np.clip(self.rgb0 * mult, 0, 255).round().astype(np.uint8)
        self.img[mask, :3] = nuevo[mask]


# multiplicadores de tono
SUCIO1 = (0.94, 0.93, 0.91)
SUCIO2 = (0.86, 0.84, 0.81)
HUMEDAD = (0.93, 0.90, 0.80)
HUMEDAD2 = (0.85, 0.81, 0.70)
GRIETA = (0.72, 0.69, 0.66)
CLARO = (1.04, 1.04, 1.03)


def coordenadas_cara(opaco):
    """Para cada columna: fila superior e inferior opacas (siguen la inclinación isométrica)."""
    h, w = opaco.shape
    arriba = np.full(w, -1)
    abajo = np.full(w, -1)
    for x in range(w):
        ys = np.nonzero(opaco[:, x])[0]
        if len(ys):
            arriba[x], abajo[x] = ys[0], ys[-1]
    return arriba, abajo


# ---------------------------------------------------------------- muros

def clasificar_muro(lz):
    rgb = lz.rgb0
    l = lum(rgb)
    contorno = lz.opaco & (l < 60)
    remate = lz.opaco & np.isin(rgb.astype(int) @ [65536, 256, 1],
                                [r * 65536 + g * 256 + b for r, g, b in REMATE])
    zoclo = lz.opaco & ~contorno & ~remate & ((rgb[..., 0] - rgb[..., 2]) > 45) & (l < 170)
    pintura = lz.opaco & ~contorno & ~remate & ~zoclo
    return contorno, remate, zoclo, pintura


def altura_sobre_zoclo(lz, zoclo, pintura):
    """Distancia en píxeles desde el borde superior del zoclo (o del fondo) hacia arriba."""
    h, w = lz.h, lz.w
    alt = np.full((h, w), 999.0)
    for x in range(w):
        ys_z = np.nonzero(zoclo[:, x])[0]
        ys_p = np.nonzero(pintura[:, x])[0]
        if len(ys_p) == 0:
            continue
        base = ys_z[0] if len(ys_z) else ys_p[-1] + 1
        alt[:, x] = base - np.arange(h)
    return alt


def grieta(lz, permitido, rng, largo, x0=None, y0=None):
    ys, xs = np.nonzero(permitido)
    if len(ys) == 0:
        return
    if x0 is None:
        i = rng.integers(len(ys))
        x0, y0 = xs[i], ys[i]
    x, y = x0, y0
    mask = np.zeros_like(permitido)
    dx = rng.choice([-1, 1])
    for _ in range(largo):
        if 0 <= y < lz.h and 0 <= x < lz.w and permitido[y, x]:
            mask[y, x] = True
        y += 1
        r = rng.random()
        if r < 0.35:
            x += dx
        elif r < 0.45:
            x -= dx
        if rng.random() < 0.12:  # ramita
            bx, by = x, y
            for _ in range(rng.integers(2, 5)):
                bx += -dx
                by += rng.integers(0, 2)
                if 0 <= by < lz.h and 0 <= bx < lz.w and permitido[by, bx]:
                    mask[by, bx] = True
    lz.tono(mask, GRIETA)


def mancha(lz, permitido, rng, cx, cy, rx, ry, tono1, tono2):
    yy, xx = np.mgrid[0:lz.h, 0:lz.w]
    n = ruido(lz.h, lz.w, 3, rng)
    d = ((xx - cx) / rx) ** 2 + ((yy - cy) / ry) ** 2 + (n - 0.5) * 0.9
    lz.tono(permitido & (d < 1.0) & (d >= 0.55), tono2)  # borde más oscuro (aureola)
    lz.tono(permitido & (d < 0.55), tono1)
    # escurrido hacia abajo
    for _ in range(rng.integers(1, 3)):
        x = int(cx + rng.integers(-int(rx * 0.6), int(rx * 0.6) + 1))
        largo = rng.integers(4, int(ry * 2) + 5)
        m = np.zeros_like(permitido)
        m[int(cy):min(lz.h, int(cy) + int(ry) + largo), x:x + 1] = True
        lz.tono(permitido & m, tono1)


def desgastar_pared(lz, rng, k):
    contorno, remate, zoclo, pintura = clasificar_muro(lz)
    h, w = lz.h, lz.w
    th = bayer(h, w)
    alt = altura_sobre_zoclo(lz, zoclo, pintura)

    # 1. pintura moteada / descolorida
    n = ruido(h, w, 10, rng, 3)
    lz.tono(pintura & (n > 1 - 0.30 * k) & (th < 0.55), SUCIO1)

    # 2. suciedad que sube desde el zoclo
    s = np.clip(1 - alt / (26 * k + 4), 0, 1) ** 1.6
    s = s * (0.6 + 0.8 * ruido(h, w, 5, rng))
    lz.tono(pintura & (s > th), SUCIO1)
    lz.tono(pintura & (s > th + 0.65), SUCIO2)

    # 3. manchas de humedad
    ys, xs = np.nonzero(pintura & (alt > 20))
    if len(ys) and rng.random() < 0.85 * k + 0.15:
        for _ in range(1 if lz.w < 40 else rng.integers(1, 3)):
            i = rng.integers(len(ys))
            mancha(lz, pintura, rng, xs[i], ys[i], rng.uniform(3, 6), rng.uniform(4, 9), HUMEDAD, HUMEDAD2)

    # 4. grietas finas
    for _ in range(int(round(rng.uniform(0.5, 1.5) * k))):
        grieta(lz, pintura & (alt > 15), rng, rng.integers(8, 22))

    # 5. desconchones (yeso claro con sombra abajo)
    cand = pintura & (rng.random((h, w)) < 0.004 * k)
    for y, x in zip(*np.nonzero(cand)):
        lz.tono(np.pad(np.ones((1, 2), bool), ((y, h - y - 1), (x, max(0, w - x - 2))))[:h, :w] & pintura, CLARO)
        if y + 1 < h and pintura[y + 1, x]:
            lz.img[y + 1, x, :3] = np.clip(lz.rgb0[y + 1, x] * SUCIO2, 0, 255).astype(np.uint8)

    # 6. zoclo raspado
    desgastar_zoclo(lz, zoclo, rng, k)

    # 7. remate: algo de polvo
    nr = ruido(h, w, 4, rng)
    lz.tono(remate & (nr > 1 - 0.25 * k) & (th < 0.5), SUCIO1)


def desgastar_zoclo(lz, zoclo, rng, k):
    h, w = lz.h, lz.w
    n = ruido(h, w, 6, rng)
    lz.tono(zoclo & (n > 1 - 0.3 * k) & (bayer(h, w) < 0.5), SUCIO1)
    for _ in range(int(3 * k * max(1, w / 24))):
        ys, xs = np.nonzero(zoclo)
        if not len(ys):
            return
        i = rng.integers(len(ys))
        y, x = ys[i], xs[i]
        largo = rng.integers(2, 5)
        m = np.zeros_like(zoclo)
        m[y, x:x + largo] = True
        lz.tono(m & zoclo, CLARO if rng.random() < 0.5 else SUCIO2)


def desgastar_azulejo(lz, rng, k):
    contorno, remate, _, resto = clasificar_muro(lz)
    h, w = lz.h, lz.w
    th = bayer(h, w)
    l = lum(lz.rgb0)
    # junta = el tono más oscuro de la pared
    if resto.any():
        junta = resto & (l <= l[resto].min() + 1)
    else:
        junta = resto
    azulejo = resto & ~junta
    arriba, abajo = coordenadas_cara(lz.opaco)
    alt = (abajo[None, :] - np.arange(h)[:, None]).astype(float)

    # juntas sucias, más hacia abajo
    sj = np.clip(1 - alt / (120 * k + 20), 0, 1) * 0.9 + 0.15 * ruido(h, w, 6, rng)
    lz.tono(junta & (sj > th), SUCIO2)

    # sarro / agua en la parte baja
    s = np.clip(1 - alt / (45 * k + 6), 0, 1) ** 1.5 * (0.5 + ruido(h, w, 5, rng))
    lz.tono(azulejo & (s > th), HUMEDAD)
    lz.tono(azulejo & (s > th + 0.7), HUMEDAD2)

    # moteado general muy suave
    n = ruido(h, w, 9, rng, 3)
    lz.tono(azulejo & (n > 1 - 0.18 * k) & (th < 0.4), SUCIO1)

    # un azulejo estrellado
    for _ in range(max(1, int(round(k)))):
        grieta(lz, azulejo & (alt > 40), rng, rng.integers(6, 13))

    # escurridos de agua desde una junta
    ys, xs = np.nonzero(junta & (alt > 60))
    for _ in range(int(round(2 * k))):
        if not len(ys):
            break
        i = rng.integers(len(ys))
        x, y = xs[i], ys[i]
        largo = rng.integers(6, 20)
        m = np.zeros_like(azulejo)
        m[y:y + largo, x:x + 1] = True
        lz.tono(m & azulejo, HUMEDAD)

    nr = ruido(h, w, 4, rng)
    lz.tono(remate & (nr > 1 - 0.25 * k) & (th < 0.5), SUCIO1)


def desgastar_muro_bajo(lz, rng, k):
    contorno, remate, zoclo, pintura = clasificar_muro(lz)
    h, w = lz.h, lz.w
    th = bayer(h, w)
    cara = zoclo | pintura
    n = ruido(h, w, 6, rng)
    lz.tono(cara & (n > 1 - 0.35 * k) & (th < 0.6), SUCIO1)
    desgastar_zoclo(lz, cara, rng, k)
    nr = ruido(h, w, 4, rng)
    lz.tono(remate & (nr > 1 - 0.3 * k) & (th < 0.5), SUCIO1)


# ---------------------------------------------------------------- pisos

def desgastar_losa(lz, rng, k):
    """Muy ligero: los pisos se repiten en el tilemap y un detalle fuerte se nota en patrón."""
    h, w = lz.h, lz.w
    th = bayer(h, w)
    l = lum(lz.rgb0)
    fondo_oscuro = l[lz.opaco].mean() < 110
    sucio = (0.90, 0.89, 0.87) if fondo_oscuro else SUCIO1
    cy, cx = (h - 1) / 2, (w - 1) / 2
    yy, xx = np.mgrid[0:h, 0:w]
    # distancia "diamante" al centro: 0 centro, 1 borde
    dd = np.abs(xx - cx) / (w / 2) + np.abs(yy - cy) / (h / 2)

    # moteado suave
    n = ruido(h, w, max(4, w / 8), rng, 2)
    lz.tono(lz.opaco & (n > 1 - 0.22 * k) & (th < 0.45), sucio)

    # orillas de la losa más sucias (juntas)
    borde = lz.opaco & (dd > 0.86) & (th < 0.35 * k)
    lz.tono(borde, sucio)

    # una o dos picaduras pequeñas
    for _ in range(int(round(1.5 * k))):
        ys, xs = np.nonzero(lz.opaco & (dd < 0.75))
        i = rng.integers(len(ys))
        y, x = ys[i], xs[i]
        m = np.zeros_like(lz.opaco)
        m[y, x] = True
        if rng.random() < 0.5 and x + 1 < w:
            m[y, x + 1] = True
        lz.tono(m & lz.opaco, SUCIO2 if not fondo_oscuro else (0.84, 0.83, 0.81))


# ---------------------------------------------------------------- variante B
# Otro tipo de desgaste: sin manchas redondas; pintura levantada, raspones de muebles,
# moho en una esquina baja.

MOHO = (0.86, 0.89, 0.80)
MOHO2 = (0.76, 0.80, 0.70)


def linea_en_cara(lz, permitido, arriba, x0, v, largo, tono):
    """Trazo que sigue la inclinación de la cara (v = filas bajo el borde superior)."""
    m = np.zeros_like(permitido)
    for x in range(x0, min(lz.w, x0 + largo)):
        y = arriba[x] + v
        if arriba[x] >= 0 and 0 <= y < lz.h:
            m[y, x] = True
    lz.tono(m & permitido, tono)


def pintura_levantada(lz, permitido, rng, cx, cy):
    yy, xx = np.mgrid[0:lz.h, 0:lz.w]
    n = ruido(lz.h, lz.w, 2.5, rng)
    rx, ry = rng.uniform(2, 4.5), rng.uniform(2, 5)
    d = ((xx - cx) / rx) ** 2 + ((yy - cy) / ry) ** 2 + (n - 0.5) * 1.2
    dentro = permitido & (d < 1)
    lz.tono(dentro, CLARO)
    # sombra en el borde inferior del desprendimiento
    abajo = np.zeros_like(dentro)
    abajo[1:] = dentro[:-1]
    lz.tono(abajo & ~dentro & permitido, SUCIO2)


def moho_esquina(lz, permitido, alt, rng, k):
    h, w = lz.h, lz.w
    th = bayer(h, w)
    lado = rng.integers(2)
    xs = np.arange(w)
    dist_lado = (xs if lado == 0 else (w - 1 - xs))[None, :].astype(float)
    m = np.clip(1 - dist_lado / (w * 0.45), 0, 1) * np.clip(1 - alt / (30 * k + 6), 0, 1)
    m = m * (0.5 + ruido(h, w, 4, rng))
    lz.tono(permitido & (m > th + 0.1), MOHO)
    lz.tono(permitido & (m > th + 0.6), MOHO2)


def desgastar_pared_b(lz, rng, k):
    contorno, remate, zoclo, pintura = clasificar_muro(lz)
    h, w = lz.h, lz.w
    th = bayer(h, w)
    alt = altura_sobre_zoclo(lz, zoclo, pintura)
    arriba, _ = coordenadas_cara(lz.opaco)

    # pintura vieja con vetas verticales tenues (se ve "lavada")
    n = ruido(h * 1, w, 6, rng, 2)
    vetas = ruido(h, w, 3, rng)[:, :] * 0 + rng.random(w)[None, :]
    lz.tono(pintura & (n > 1 - 0.22 * k) & (vetas > 0.55) & (th < 0.5), SUCIO1)

    # raspones de sillas/muebles a media altura
    for _ in range(int(round(rng.uniform(2, 4) * k * max(1, w / 30)))):
        ys, xs = np.nonzero(pintura & (alt > 12) & (alt < 40))
        if not len(ys):
            break
        i = rng.integers(len(ys))
        x0 = xs[i]
        v = ys[i] - arriba[x0]
        linea_en_cara(lz, pintura, arriba, x0, v, rng.integers(3, 10), SUCIO2 if rng.random() < 0.7 else SUCIO1)

    # pintura levantada
    ys, xs = np.nonzero(pintura & (alt > 30))
    for _ in range(int(round(rng.uniform(1, 2.5) * k * max(1, w / 40)))):
        if not len(ys):
            break
        i = rng.integers(len(ys))
        pintura_levantada(lz, pintura, rng, xs[i], ys[i])

    # moho en una esquina baja
    moho_esquina(lz, pintura, alt, rng, k)

    # grieta corta que baja desde el remate
    filas_rem = np.nonzero(remate.any(axis=1))[0]
    if len(filas_rem):
        for _ in range(int(round(k))):
            x0 = rng.integers(2, max(3, w - 2))
            col = np.nonzero(pintura[:, x0])[0]
            if len(col):
                grieta(lz, pintura, rng, rng.integers(6, 14), x0, col[0])

    desgastar_zoclo(lz, zoclo, rng, k)
    nr = ruido(h, w, 4, rng)
    lz.tono(remate & (nr > 1 - 0.3 * k) & (th < 0.5), SUCIO1)


def desgastar_azulejo_b(lz, rng, k):
    contorno, remate, _, resto = clasificar_muro(lz)
    h, w = lz.h, lz.w
    th = bayer(h, w)
    l = lum(lz.rgb0)
    junta = resto & (l <= l[resto].min() + 1) if resto.any() else resto
    azulejo = resto & ~junta
    arriba, abajo = coordenadas_cara(lz.opaco)
    alt = (abajo[None, :] - np.arange(h)[:, None]).astype(float)

    # juntas sucias parejas (todo el muro)
    lz.tono(junta & (ruido(h, w, 5, rng) > 0.45) & (th < 0.6 * k + 0.1), SUCIO2)

    # sarro en una franja a media altura (donde salpica el lavabo)
    centro = rng.uniform(70, 130)
    s = np.clip(1 - np.abs(alt - centro) / (25 * k + 5), 0, 1) * (0.4 + ruido(h, w, 4, rng))
    lz.tono(azulejo & (s > th + 0.15), HUMEDAD)

    # azulejos despostillados: esquinita gris
    for _ in range(int(round(2 * k))):
        ys, xs = np.nonzero(azulejo & (alt > 20))
        i = rng.integers(len(ys))
        y, x = ys[i], xs[i]
        m = np.zeros_like(azulejo)
        m[y:y + 2, x:x + 2] = True
        m[y, x + 2:x + 3] = True
        lz.tono(m & azulejo, GRIETA)

    # moho en la base
    moho_esquina(lz, azulejo | junta, alt, rng, k * 0.8)

    nr = ruido(h, w, 4, rng)
    lz.tono(remate & (nr > 1 - 0.25 * k) & (th < 0.5), SUCIO1)


def desgastar_muro_bajo_b(lz, rng, k):
    contorno, remate, zoclo, pintura = clasificar_muro(lz)
    h, w = lz.h, lz.w
    th = bayer(h, w)
    cara = zoclo | pintura
    arriba, _ = coordenadas_cara(cara)
    for _ in range(int(round(4 * k))):
        x0 = rng.integers(0, w)
        if arriba[x0] < 0:
            continue
        linea_en_cara(lz, cara, arriba, x0, rng.integers(2, 20), rng.integers(4, 12),
                      SUCIO2 if rng.random() < 0.6 else CLARO)
    n = ruido(h, w, 4, rng)
    lz.tono(cara & (n > 1 - 0.25 * k) & (th < 0.5), SUCIO1)
    lz.tono(remate & (ruido(h, w, 4, rng) > 1 - 0.3 * k) & (th < 0.5), SUCIO1)


def desgastar_losa_b(lz, rng, k):
    h, w = lz.h, lz.w
    th = bayer(h, w)
    l = lum(lz.rgb0)
    oscuro = l[lz.opaco].mean() < 110
    sucio = (0.90, 0.89, 0.87) if oscuro else SUCIO1
    sucio2 = (0.82, 0.81, 0.79) if oscuro else SUCIO2
    cy, cx = (h - 1) / 2, (w - 1) / 2
    yy, xx = np.mgrid[0:h, 0:w]
    dd = np.abs(xx - cx) / (w / 2) + np.abs(yy - cy) / (h / 2)

    # zona gastada por pisadas (centro más opaco)
    n = ruido(h, w, max(3, w / 10), rng, 2)
    lz.tono(lz.opaco & (dd < 0.7) & (n > 0.5) & (th < 0.45 * k), sucio)

    # mancha difusa
    ys, xs = np.nonzero(lz.opaco & (dd < 0.55))
    i = rng.integers(len(ys))
    d = ((xx - xs[i]) / (w * 0.12)) ** 2 + ((yy - ys[i]) / (h * 0.12)) ** 2 + (ruido(h, w, 3, rng) - 0.5)
    lz.tono(lz.opaco & (d < 1) & (th < 0.7), sucio)

    # grieta fina diagonal
    if rng.random() < 0.8 * k:
        x, y = int(cx + rng.integers(-w // 6, w // 6 + 1)), int(cy + rng.integers(-h // 6, h // 6 + 1))
        dx = rng.choice([-2, 2])
        m = np.zeros_like(lz.opaco)
        for _ in range(int(rng.integers(5, 10) * max(1, w / 64))):
            if 0 <= y < h and 0 <= x < w:
                m[y, x] = True
            x += dx // 2 if rng.random() < 0.8 else 0
            y += 1 if rng.random() < 0.5 else 0
        lz.tono(m & lz.opaco & (dd < 0.9), sucio2)

    lz.tono(lz.opaco & (dd > 0.86) & (th < 0.4 * k), sucio)


# ---------------------------------------------------------------- vistas de inspección
# Imágenes de primer plano (640x360): la pared se ve más cerca, así que hay menos
# detalles y más grandes que en los tiles.

VISTAS = {
    "Assets/_Carencia/UI/Sprites/Vistas/UI_Television.png": {
        (242, 222, 184), (234, 194, 138), (172, 108, 66), (110, 62, 38), (220, 178, 124)},
}


def desgastar_vista(lz, rng, k):
    contorno, remate, zoclo, pintura = clasificar_muro(lz)
    h, w = lz.h, lz.w
    th = bayer(h, w)
    alt = altura_sobre_zoclo(lz, zoclo, pintura)

    # pintura moteada en manchas grandes y suaves
    n = ruido(h, w, 40, rng, 3)
    lz.tono(pintura & (n > 1 - 0.28 * k) & (th < 0.5), SUCIO1)

    # suciedad que sube desde el zoclo
    s = np.clip(1 - alt / (55 * k + 8), 0, 1) ** 1.6 * (0.6 + 0.8 * ruido(h, w, 14, rng))
    lz.tono(pintura & (s > th), SUCIO1)
    lz.tono(pintura & (s > th + 0.65), SUCIO2)

    # manchas de humedad con escurrido
    ys, xs = np.nonzero(pintura & (alt > 70))
    for _ in range(int(round(2 * k))):
        if not len(ys):
            break
        i = rng.integers(len(ys))
        mancha(lz, pintura, rng, xs[i], ys[i], rng.uniform(7, 13), rng.uniform(9, 16), HUMEDAD, HUMEDAD2)

    # grietas
    for _ in range(int(round(2 * k))):
        grieta(lz, pintura & (alt > 50), rng, rng.integers(25, 50))

    # pintura levantada
    ys, xs = np.nonzero(pintura & (alt > 40))
    for _ in range(int(round(5 * k))):
        if not len(ys):
            break
        i = rng.integers(len(ys))
        pintura_levantada(lz, pintura, rng, xs[i], ys[i])

    # raspones horizontales a la altura de los muebles
    ys, xs = np.nonzero(pintura & (alt > 8) & (alt < 70))
    for _ in range(int(round(8 * k))):
        if not len(ys):
            break
        i = rng.integers(len(ys))
        y, x = ys[i], xs[i]
        m = np.zeros_like(pintura)
        m[y, x:x + rng.integers(6, 22)] = True
        lz.tono(m & pintura, SUCIO2 if rng.random() < 0.7 else SUCIO1)

    # moho en una esquina baja
    lado = rng.integers(2)
    dist = (np.arange(w) if lado == 0 else (w - 1 - np.arange(w)))[None, :].astype(float)
    mm = np.clip(1 - dist / 110, 0, 1) * np.clip(1 - alt / (70 * k + 10), 0, 1) * (0.5 + ruido(h, w, 10, rng))
    lz.tono(pintura & (mm > th + 0.1), MOHO)
    lz.tono(pintura & (mm > th + 0.6), MOHO2)

    # zoclo
    n = ruido(h, w, 20, rng)
    lz.tono(zoclo & (n > 1 - 0.3 * k) & (th < 0.5), SUCIO1)
    ys, xs = np.nonzero(zoclo)
    for _ in range(int(round(14 * k))):
        if not len(ys):
            break
        i = rng.integers(len(ys))
        y, x = ys[i], xs[i]
        m = np.zeros_like(zoclo)
        m[y, x:x + rng.integers(4, 14)] = True
        lz.tono(m & zoclo, CLARO if rng.random() < 0.5 else SUCIO2)


def procesar_vista(ruta, colores_pared, ref, k, destino=None):
    """Solo toca los píxeles con colores de pared; la tele y lo demás quedan igual."""
    img = leer_original_ruta(ruta, ref)
    rgb = img[..., :3].astype(int)
    clave = rgb[..., 0] * 65536 + rgb[..., 1] * 256 + rgb[..., 2]
    es_pared = np.isin(clave, [r * 65536 + g * 256 + b for r, g, b in colores_pared])
    trabajo = img.copy()
    trabajo[~es_pared, 3] = 0
    lz = Lienzo(trabajo)
    desgastar_vista(lz, np.random.default_rng(zlib.crc32(ruta.encode())), k)
    # PantallaJuego.ContinuarFondo toma el color de la pared y del piso de la columna izquierda
    # para rellenar los lados: esas columnas quedan con su color original.
    es_pared[:, :3] = False
    img[es_pared, :3] = lz.img[es_pared, :3]
    salida = Path(destino) / Path(ruta).name if destino else Path(ruta)
    Image.fromarray(img, "RGBA").save(salida)
    print(f"{Path(ruta).stem}: vista")


def leer_original_ruta(ruta, ref):
    datos = subprocess.run(["git", "cat-file", "--filters", f"{ref}:{ruta}"],
                           capture_output=True, check=True).stdout
    return np.array(Image.open(io.BytesIO(datos)).convert("RGBA"))


# ---------------------------------------------------------------- main

FUNCIONES = {
    "A": {
        "pared": desgastar_pared,
        "azulejo": desgastar_azulejo,
        "muro_bajo": desgastar_muro_bajo,
        "losa": desgastar_losa,
    },
    "B": {
        "pared": desgastar_pared_b,
        "azulejo": desgastar_azulejo_b,
        "muro_bajo": desgastar_muro_bajo_b,
        "losa": desgastar_losa_b,
    },
}


def nombre_variante(nombre, v):
    """SPR_ESC_LosaBeige_R + A -> SPR_ESC_LosaBeigeSucioA_R"""
    base, dim = nombre.rsplit("_", 1)
    return f"{base}Sucio{v}_{dim}"


def escribir_meta(nombre_orig, nombre_nuevo, carpeta):
    """Copia el .meta del original (mismos PPU, pivote y filtro) con GUID propio y estable."""
    meta = (CARPETA / f"{nombre_orig}.png.meta").read_text(encoding="utf-8")
    viejo = re.search(r"^guid: (\w+)", meta, re.M).group(1)
    nuevo = hashlib.md5(f"carencia/{nombre_nuevo}".encode()).hexdigest()
    meta = meta.replace(viejo, nuevo)
    sid = hashlib.md5(f"sprite/{nombre_nuevo}".encode()).hexdigest()
    meta = re.sub(r"(spriteID: )\w+", lambda m: m.group(1) + sid, meta)
    (carpeta / f"{nombre_nuevo}.png.meta").write_text(meta, encoding="utf-8")
    return nuevo


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--intensidad", type=float, default=1.0)
    ap.add_argument("--git-ref", default="HEAD")
    ap.add_argument("--solo", default="")
    ap.add_argument("--salida", default="", help="carpeta alternativa (para vista previa)")
    a = ap.parse_args()
    destino = Path(a.salida) if a.salida else CARPETA
    destino.mkdir(parents=True, exist_ok=True)

    for nombre, tipo in SPRITES.items():
        if a.solo and a.solo not in nombre:
            continue
        original = leer_original(nombre, a.git_ref)
        Image.fromarray(original, "RGBA").save(destino / f"{nombre}.png")  # limpia
        for v, semilla in (("A", nombre), ("B", nombre + "/B")):
            lz = Lienzo(original.copy())
            rng = np.random.default_rng(zlib.crc32(semilla.encode()))
            FUNCIONES[v][tipo](lz, rng, a.intensidad)
            nv = nombre_variante(nombre, v)
            Image.fromarray(lz.img, "RGBA").save(destino / f"{nv}.png")
            if not a.salida:
                escribir_meta(nombre, nv, destino)
        print(f"{nombre}: {tipo} (limpia, A, B)")

    for ruta, colores in VISTAS.items():
        if a.solo and a.solo not in ruta:
            continue
        procesar_vista(ruta, colores, a.git_ref, a.intensidad, a.salida or None)


if __name__ == "__main__":
    main()
