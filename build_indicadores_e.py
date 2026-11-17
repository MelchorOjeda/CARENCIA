"""Indicadores de tecla por nivel: E (16x16) y Esc (26x16) en cuatro estilos.

    python build_indicadores_e.py --vista carpeta   # solo hoja de muestra
    python build_indicadores_e.py                   # escribe los PNG en UI/Sprites
"""
import argparse
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw

SALIDA = Path("Assets/_Carencia/UI/Sprites")
OPACIDAD = round(255 * 0.70)  # 70 % opaco

# letras 6 de ancho x 8 de alto (doble trazo, como la E original)
LETRAS = {
    "E": ["######", "######", "##....", "######", "######", "##....", "######", "######"],
    "S": [".#####", "######", "##....", "#####.", ".#####", "....##", "######", "#####."],
    "C": [".#####", "######", "##....", "##....", "##....", "##....", "######", ".#####"],
}

ESTILOS = {
    # Prólogo: la tecla de siempre, limpia
    "PRO": dict(borde=(28, 22, 34), fondo=(240, 238, 230), letra=(28, 22, 34), sombra=(214, 208, 196)),
    # Nivel 1: blanco deslavado, poco contraste, sin vida
    "N1": dict(borde=(150, 150, 156), fondo=(226, 226, 224), letra=(156, 156, 160), sombra=(212, 212, 212)),
    # Nivel 2: amarillo enfermo, sucio y oscuro
    "N2": dict(borde=(40, 32, 16), fondo=(178, 156, 84), letra=(48, 38, 18), sombra=(132, 112, 56),
               mancha=(150, 128, 62)),
    # Nivel 3: carne roja con venas, letra de hueso, escurre
    "N3": dict(borde=(30, 6, 10), fondo=(146, 44, 46), letra=(232, 202, 186), sombra=(98, 22, 28),
               vena=(110, 26, 34), gota=(120, 16, 22)),
}


def tecla(ancho, texto, estilo, semilla):
    e = ESTILOS[estilo]
    rng = np.random.default_rng(semilla)
    img = np.zeros((16, ancho, 4), np.uint8)

    def px(x, y, c):
        if 0 <= x < ancho and 0 <= y < 16:
            img[y, x] = (*c, 255)

    alto = 15 if estilo == "N3" else 16  # la carne deja una fila para escurrir
    for y in range(alto):
        for x in range(ancho):
            esquina = (x in (0, ancho - 1)) and (y in (0, alto - 1))
            if esquina:
                continue
            borde = x in (0, ancho - 1) or y in (0, alto - 1)
            px(x, y, e["borde"] if borde else e["fondo"])
    # sombra inferior de la tecla (profundidad)
    for x in range(1, ancho - 1):
        px(x, alto - 2, e["sombra"])
        px(x, alto - 3, e["sombra"])

    if estilo == "N2":
        # manchas de grasa y esquina comida
        for _ in range(int(ancho * 0.6)):
            x, y = rng.integers(1, ancho - 1), rng.integers(1, alto - 3)
            px(x, y, e["mancha"])
        px(1, 1, e["borde"])
        px(ancho - 2, alto - 2, e["borde"])
    if estilo == "N3":
        # venas: caminos irregulares más oscuros
        for _ in range(2 + ancho // 13):
            x, y = int(rng.integers(1, ancho - 1)), 1
            while y < alto - 2:
                px(x, y, e["vena"])
                y += 1
                x += int(rng.integers(-1, 2))
                x = min(max(x, 1), ancho - 2)
        # borde mordido
        for _ in range(2):
            x = int(rng.integers(2, ancho - 2))
            px(x, 0, (0, 0, 0)) ; img[0, x, 3] = 0
        # gotas que escurren por abajo
        for x in rng.choice(np.arange(2, ancho - 2), size=2, replace=False):
            px(int(x), alto, e["gota"])

    # texto centrado
    ancho_txt = len(texto) * 6 + (len(texto) - 1) * 1
    x0 = (ancho - ancho_txt) // 2
    y0 = 4
    for i, ch in enumerate(texto):
        for fy, fila in enumerate(LETRAS[ch]):
            for fx, c in enumerate(fila):
                if c == "#":
                    px(x0 + i * 7 + fx, y0 + fy, e["letra"])
    if estilo == "N1":
        # letra con huecos, como desteñida
        for _ in range(2):
            y, x = rng.integers(y0, y0 + 8), rng.integers(x0, x0 + ancho_txt)
            if tuple(img[y, x, :3]) == e["letra"]:
                px(int(x), int(y), e["fondo"])
    img[img[..., 3] > 0, 3] = OPACIDAD
    return Image.fromarray(img, "RGBA")


def hoja(destino):
    fondos = {"PRO": (226, 206, 170), "N1": (214, 214, 210), "N2": (92, 84, 52), "N3": (60, 18, 22)}
    nombres = {"PRO": "Prólogo", "N1": "Nivel 1", "N2": "Nivel 2", "N3": "Nivel 3"}
    celda_w, celda_h = 300, 330
    lienzo = Image.new("RGB", (celda_w * 4, celda_h + 30), (24, 22, 26))
    d = ImageDraw.Draw(lienzo)
    for i, n in enumerate(ESTILOS):
        x = i * celda_w
        lienzo.paste(fondos[n], (x, 0, x + celda_w, celda_h))
        e = tecla(16, "E", n, 1).resize((160, 160), Image.NEAREST)          # x10
        s = tecla(26, "ESC", n, 2).resize((156, 96), Image.NEAREST)         # x6
        e3 = tecla(16, "E", n, 1).resize((48, 48), Image.NEAREST)           # tamaño de juego aprox.
        s3 = tecla(26, "ESC", n, 2).resize((78, 48), Image.NEAREST)
        lienzo.paste(e, (x + 20, 20), e)
        lienzo.paste(s, (x + 20, 200), s)
        lienzo.paste(e3, (x + 200, 40), e3)
        lienzo.paste(s3, (x + 200, 220), s3)
        d.text((x + 12, celda_h + 8), nombres[n] + "   (derecha: tamaño en juego)", fill=(230, 230, 230))
    lienzo.save(destino)


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--vista", default="")
    a = ap.parse_args()
    if a.vista:
        Path(a.vista).mkdir(parents=True, exist_ok=True)
        hoja(Path(a.vista) / "indicadores_e.png")
        for n in ESTILOS:
            tecla(16, "E", n, 1).save(Path(a.vista) / f"UI_IndicadorE_{n}.png")
            tecla(26, "ESC", n, 2).save(Path(a.vista) / f"UI_IndicadorEsc_{n}.png")
        return
    for n in ESTILOS:
        for nombre, img in ((f"UI_IndicadorE_{n}", tecla(16, "E", n, 1)),
                            (f"UI_IndicadorEsc_{n}", tecla(26, "ESC", n, 2))):
            img.save(SALIDA / f"{nombre}.png")
            escribir_meta(nombre)
        print(n)


def escribir_meta(nombre):
    """Mismos ajustes que la E original (48 PPU, filtro de punto) con GUID propio y estable."""
    import hashlib
    import re
    meta = (SALIDA / "UI_IndicadorE.png.meta").read_text(encoding="utf-8")
    viejo = re.search(r"^guid: (\w+)", meta, re.M).group(1)
    meta = meta.replace(viejo, hashlib.md5(f"carencia/{nombre}".encode()).hexdigest())
    meta = meta.replace("UI_IndicadorE_0", f"{nombre}_0")
    meta = re.sub(r"(spriteID: )\w+", lambda m: m.group(1) + hashlib.md5(f"sprite/{nombre}".encode()).hexdigest(), meta)
    (SALIDA / f"{nombre}.png.meta").write_text(meta, encoding="utf-8")


if __name__ == "__main__":
    main()
