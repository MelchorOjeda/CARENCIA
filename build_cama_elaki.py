"""
Script de generacion de sprites de la Cama de Elaki para 'Carencia'.
Version perfeccionada con drapeados organicos y pixel-art artesanal.
Medidas oficiales segun Docs/MEDIDAS.md y assets cuarto.png:
- 2 x 4 celdas isometricas (1.0 x 2.0 m reales).
- 48 PPU, Point (no filter), Single, None compression.
- Lienzo: 144 x 116 px.
"""

import os
from PIL import Image, ImageDraw

def build_elaki_beds():
    output_dir = r"Assets\_Carencia\Sprites\Objetos\Muebles"
    os.makedirs(output_dir, exist_ok=True)

    W, H = 144, 116

    # Paleta de colores Carencia (fiel a assets cuarto.png)
    C_TRANS = (0, 0, 0, 0)
    C_OUTLINE = (28, 20, 32, 255)
    C_OUTLINE_SOFT = (56, 40, 58, 255)

    # Madera Castaño Cálido
    C_WOOD_HIGHLIGHT = (210, 150, 100, 255)
    C_WOOD_LIGHT = (176, 116, 72, 255)
    C_WOOD_BASE = (134, 82, 48, 255)
    C_WOOD_SHADOW = (92, 54, 30, 255)
    C_WOOD_DARK = (52, 28, 16, 255)
    C_WOOD_DEEP = (34, 18, 12, 255)

    # Edredón Acolchado Índigo (Elaki)
    C_QUILT_HIGHLIGHT = (124, 148, 202, 255)
    C_QUILT_LIGHT = (96, 118, 172, 255)
    C_QUILT_BASE = (64, 78, 126, 255)
    C_QUILT_MID = (48, 58, 100, 255)
    C_QUILT_SHADOW = (34, 42, 74, 255)
    C_QUILT_DARK = (24, 30, 54, 255)
    C_QUILT_SEAM = (18, 22, 42, 255)

    # Sábanas y Almohadas (Marfil / Melocotón suave)
    C_SHEET_HIGHLIGHT = (255, 250, 244, 255)
    C_SHEET_LIGHT = (244, 230, 218, 255)
    C_SHEET_BASE = (224, 206, 192, 255)
    C_SHEET_SHADOW = (184, 162, 146, 255)
    C_SHEET_CREASE = (144, 122, 108, 255)
    C_SHEET_DARK = (102, 84, 74, 255)

    # Colchón base lateral
    C_MAT_SIDE = (215, 200, 188, 255)
    C_MAT_SHADOW = (170, 154, 142, 255)

    # Sombras arrojadas
    C_FLOOR_SHADOW = (20, 14, 24, 95)
    C_FLOOR_SHADOW_DEEP = (16, 10, 20, 160)

    # Puntos de la estructura
    X1, Y1 = 12, 66
    X2, Y2 = 56, 44
    X3, Y3 = 96, 108
    X4, Y4 = 140, 86

    def draw_bed_background(draw, img):
        """Dibuja sombra del suelo, cabecera y pata del fondo."""
        poly_floor_shadow = [
            (X1 - 2, Y1),
            (X2, Y2 - 2),
            (X4 + 2, Y4),
            (X3, Y3 + 2)
        ]
        shadow_overlay = Image.new("RGBA", (W, H), C_TRANS)
        draw_sh = ImageDraw.Draw(shadow_overlay)
        draw_sh.polygon(poly_floor_shadow, fill=C_FLOOR_SHADOW)
        poly_deep_sh = [
            (X1 + 10, Y1 + 4),
            (X2 + 8, Y2 + 2),
            (X4 - 10, Y4 - 2),
            (X3 - 6, Y3 - 4)
        ]
        draw_sh.polygon(poly_deep_sh, fill=C_FLOOR_SHADOW_DEEP)
        img.alpha_composite(shadow_overlay)

        # Pata P2
        draw.rectangle([X2 - 2, 5, X2 + 3, Y2], fill=C_WOOD_DARK, outline=C_OUTLINE)
        draw.line([(X2 - 1, 6), (X2 - 1, Y2 - 1)], fill=C_WOOD_SHADOW)
        draw.polygon([(X2 - 3, 5), (X2 + 4, 5), (X2 + 1, 1), (X2 - 1, 1)], fill=C_WOOD_BASE, outline=C_OUTLINE)

        # Cabecera
        poly_headboard = [
            (X1 + 2, 20),
            (X2 - 1, 6),
            (X2 - 1, Y2 - 16),
            (X1 + 2, Y1 - 16)
        ]
        draw.polygon(poly_headboard, fill=C_WOOD_BASE, outline=C_OUTLINE)

        # Tablero interior rebajado
        poly_hb_inner = [
            (X1 + 4, 24),
            (X2 - 3, 10),
            (X2 - 3, Y2 - 20),
            (X1 + 4, Y1 - 20)
        ]
        draw.polygon(poly_hb_inner, fill=C_WOOD_SHADOW, outline=C_OUTLINE_SOFT)

        for frac in (0.25, 0.50, 0.75):
            yl = int(24 + (Y1 - 20 - 24) * frac)
            yr = int(10 + (Y2 - 20 - 10) * frac)
            draw.line([(X1 + 4, yl), (X2 - 3, yr)], fill=C_WOOD_LIGHT)
            draw.line([(X1 + 4, yl + 1), (X2 - 3, yr + 1)], fill=C_WOOD_DARK)

        poly_hb_crown = [
            (X1 + 1, 18),
            (X2, 4),
            (X2 + 1, 7),
            (X1 + 3, 21)
        ]
        draw.polygon(poly_hb_crown, fill=C_WOOD_LIGHT, outline=C_OUTLINE)

        # Pata P1
        draw.rectangle([X1 - 2, 16, X1 + 3, Y1], fill=C_WOOD_BASE, outline=C_OUTLINE)
        draw.line([(X1 - 1, 17), (X1 - 1, Y1 - 1)], fill=C_WOOD_LIGHT)
        draw.line([(X1 + 2, 17), (X1 + 2, Y1 - 1)], fill=C_WOOD_DARK)
        draw.polygon([(X1 - 3, 16), (X1 + 4, 16), (X1 + 1, 12), (X1 - 1, 12)], fill=C_WOOD_HIGHLIGHT, outline=C_OUTLINE)

    def draw_bed_foreground(draw):
        """Dibuja larguero izquierdo, larguero piecero y patas delanteras."""
        poly_rail_left = [
            (X1 + 2, Y1 - 16),
            (X3 - 2, Y3 - 16),
            (X3 - 2, Y3 - 8),
            (X1 + 2, Y1 - 8)
        ]
        draw.polygon(poly_rail_left, fill=C_WOOD_BASE, outline=C_OUTLINE)
        draw.line([(X1 + 2, Y1 - 16), (X3 - 2, Y3 - 16)], fill=C_WOOD_LIGHT)
        draw.line([(X1 + 2, Y1 - 8), (X3 - 2, Y3 - 8)], fill=C_WOOD_DARK)

        poly_rail_foot = [
            (X3 + 2, Y3 - 16),
            (X4 - 2, Y4 - 16),
            (X4 - 2, Y4 - 8),
            (X3 + 2, Y3 - 8)
        ]
        draw.polygon(poly_rail_foot, fill=C_WOOD_SHADOW, outline=C_OUTLINE)
        draw.line([(X3 + 2, Y3 - 16), (X4 - 2, Y4 - 16)], fill=C_WOOD_BASE)
        draw.line([(X3 + 2, Y3 - 8), (X4 - 2, Y4 - 8)], fill=C_WOOD_DARK)

        # Pata P4
        draw.rectangle([X4 - 2, Y4 - 20, X4 + 3, Y4], fill=C_WOOD_SHADOW, outline=C_OUTLINE)
        draw.line([(X4 - 1, Y4 - 19), (X4 - 1, Y4 - 1)], fill=C_WOOD_BASE)
        draw.line([(X4 + 2, Y4 - 19), (X4 + 2, Y4 - 1)], fill=C_WOOD_DARK)
        draw.polygon([(X4 - 3, Y4 - 20), (X4 + 4, Y4 - 20), (X4 + 1, Y4 - 23), (X4 - 1, Y4 - 23)], fill=C_WOOD_BASE, outline=C_OUTLINE)

        # Pata P3
        draw.rectangle([X3 - 2, Y3 - 20, X3 + 3, Y3], fill=C_WOOD_BASE, outline=C_OUTLINE)
        draw.line([(X3 - 1, Y3 - 19), (X3 - 1, Y3 - 1)], fill=C_WOOD_LIGHT)
        draw.line([(X3 + 2, Y3 - 19), (X3 + 2, Y3 - 1)], fill=C_WOOD_DARK)
        draw.polygon([(X3 - 3, Y3 - 20), (X3 + 4, Y3 - 20), (X3 + 1, Y3 - 23), (X3 - 1, Y3 - 23)], fill=C_WOOD_HIGHLIGHT, outline=C_OUTLINE)

    def draw_pillow(draw, px, py, width=17, dent=False, crooked=False):
        """Dibuja almohada redondeada con iluminación suave."""
        if crooked:
            poly_p = [
                (px - 1, py + 2),
                (px + width - 1, py - width // 2 - 2),
                (px + width + 10, py - width // 2 + 5),
                (px + 10, py + 9)
            ]
        else:
            poly_p = [
                (px, py),
                (px + width, py - width // 2),
                (px + width + 11, py - width // 2 + 6),
                (px + 11, py + 6)
            ]
        draw.polygon(poly_p, fill=C_SHEET_BASE, outline=C_OUTLINE)
        poly_p_top = [(p[0], p[1] - 1) for p in poly_p]
        draw.polygon(poly_p_top, fill=C_SHEET_LIGHT)
        cx = sum(p[0] for p in poly_p) // 4
        cy = sum(p[1] for p in poly_p) // 4
        draw.line([(cx - 4, cy), (cx + 4, cy - 4)], fill=C_SHEET_HIGHLIGHT)

        if dent:
            draw.ellipse([cx - 4, cy - 3, cx + 4, cy + 2], fill=C_SHEET_SHADOW, outline=C_SHEET_CREASE)
            draw.line([(cx - 5, cy), (cx - 2, cy - 2)], fill=C_SHEET_CREASE)
            draw.line([(cx + 2, cy - 2), (cx + 5, cy)], fill=C_SHEET_CREASE)
        else:
            draw.point([(cx, cy - 1)], fill=C_SHEET_SHADOW)

    # -------------------------------------------------------------
    # 1. VARIANTE HECHA (Cama tendida e impecable)
    # -------------------------------------------------------------
    img_hecha = Image.new("RGBA", (W, H), C_TRANS)
    draw_h = ImageDraw.Draw(img_hecha)
    draw_bed_background(draw_h, img_hecha)

    # Caída lateral izquierda (toda azul índigo cubriendo hasta el larguero)
    poly_drape_left = [
        (14, 36),
        (94, 78),
        (94, Y3 - 16),
        (14, Y1 - 16)
    ]
    draw_h.polygon(poly_drape_left, fill=C_QUILT_BASE, outline=C_OUTLINE)
    for dx in range(24, 92, 12):
        t = (dx - 14) / (94.0 - 14.0)
        ty = int(36 + (78 - 36) * t)
        by = int(50 + (92 - 50) * t)
        draw_h.line([(dx, ty + 1), (dx, by)], fill=C_QUILT_LIGHT)
        draw_h.line([(dx + 1, ty + 1), (dx + 1, by)], fill=C_QUILT_DARK)

    # Caída piecero
    poly_drape_foot = [
        (94, 78),
        (138, 56),
        (138, Y4 - 16),
        (94, Y3 - 16)
    ]
    draw_h.polygon(poly_drape_foot, fill=C_QUILT_SHADOW, outline=C_OUTLINE)
    draw_h.line([(95, 78), (137, 57)], fill=C_QUILT_BASE)

    # Almohadas gemelas
    draw_pillow(draw_h, 17, 32, width=17, dent=False)
    draw_pillow(draw_h, 34, 23, width=17, dent=False)

    # Sábana doblada blanca sobre el edredón
    poly_sheet_fold = [
        (26, 42),
        (58, 26),
        (66, 30),
        (34, 46)
    ]
    draw_h.polygon(poly_sheet_fold, fill=C_SHEET_LIGHT, outline=C_OUTLINE)
    draw_h.line([(27, 42), (58, 26)], fill=C_SHEET_HIGHLIGHT)
    draw_h.line([(34, 46), (66, 30)], fill=C_SHEET_CREASE)
    # Caída lateral blanca del doblado
    poly_sheet_side = [(26, 42), (34, 46), (34, 54), (26, 50)]
    draw_h.polygon(poly_sheet_side, fill=C_SHEET_SHADOW, outline=C_OUTLINE)

    # Superficie superior del edredón
    poly_quilt_top = [(34, 46), (66, 30), (138, 56), (94, 78)]
    draw_h.polygon(poly_quilt_top, fill=C_QUILT_BASE, outline=C_OUTLINE)

    # Rejilla acolchada (2 columnas x 3 filas)
    p_top_mid = ((34 + 66) // 2, (46 + 30) // 2)      # (50, 38)
    p_bot_mid = ((94 + 138) // 2, (78 + 56) // 2)    # (116, 67)
    draw_h.line([p_top_mid, p_bot_mid], fill=C_QUILT_SEAM, width=1)

    for frac in (0.33, 0.66):
        ql_x = int(34 + (94 - 34) * frac)
        ql_y = int(46 + (78 - 46) * frac)
        qr_x = int(66 + (138 - 66) * frac)
        qr_y = int(30 + (56 - 30) * frac)
        draw_h.line([(ql_x, ql_y), (qr_x, qr_y)], fill=C_QUILT_SEAM, width=1)

    quilt_centers = [
        (48, 46), (68, 56), (88, 66),
        (72, 38), (92, 48), (112, 58)
    ]
    for (cx, cy) in quilt_centers:
        draw_h.point([(cx, cy)], fill=C_QUILT_HIGHLIGHT)
        draw_h.point([(cx + 1, cy)], fill=C_QUILT_LIGHT)
        draw_h.point([(cx - 1, cy)], fill=C_QUILT_LIGHT)
        draw_h.point([(cx, cy - 1)], fill=C_QUILT_LIGHT)
        draw_h.point([(cx, cy + 1)], fill=C_QUILT_MID)

    draw_bed_foreground(draw_h)

    path_hecha = os.path.join(output_dir, "cama_elaki_hecha.png")
    img_hecha.save(path_hecha)
    print("Guardado: cama_elaki_hecha.png (144x116)")

    # -------------------------------------------------------------
    # 2. VARIANTE DESTENDIDA / ABIERTA (Cinemática despertar Prólogo)
    # -------------------------------------------------------------
    img_dest = Image.new("RGBA", (W, H), C_TRANS)
    draw_d = ImageDraw.Draw(img_dest)
    draw_bed_background(draw_d, img_dest)

    # Caída piecero
    draw_d.polygon(poly_drape_foot, fill=C_QUILT_SHADOW, outline=C_OUTLINE)

    # Caída lateral: mitad superior es colchón/sábana bajera crema, mitad inferior edredón azul
    poly_side_sheet = [(14, 36), (54, 56), (54, Y1 - 16 + 18), (14, Y1 - 16)]
    draw_d.polygon(poly_side_sheet, fill=C_MAT_SIDE, outline=C_OUTLINE)
    poly_side_quilt = [(54, 56), (94, 78), (94, Y3 - 16), (54, Y1 - 16 + 18)]
    draw_d.polygon(poly_side_quilt, fill=C_QUILT_BASE, outline=C_OUTLINE)
    draw_d.line([(55, 57), (55, Y1 - 16 + 17)], fill=C_QUILT_DARK)

    # Almohadas: almohada derecha con la marca de la cabeza hundida
    draw_pillow(draw_d, 17, 32, width=17, dent=False)
    draw_pillow(draw_d, 34, 23, width=17, dent=True)

    # SÁBANA BAJERA AL DESCUBIERTO (Área superior completa donde descansó Elaki)
    # Cubre desde las almohadas hasta la curva ondulada del edredón retirado
    poly_exposed = [
        (14, 36),
        (55, 22),
        (102, 45),
        (54, 56)
    ]
    draw_d.polygon(poly_exposed, fill=C_SHEET_BASE, outline=C_OUTLINE)
    # Arrugas suaves realistas de tela donde durmió
    draw_d.line([(24, 40), (46, 31)], fill=C_SHEET_LIGHT)
    draw_d.line([(25, 41), (47, 32)], fill=C_SHEET_CREASE)
    draw_d.line([(38, 46), (72, 34)], fill=C_SHEET_LIGHT)
    draw_d.line([(39, 47), (73, 35)], fill=C_SHEET_CREASE)
    draw_d.line([(50, 50), (84, 38)], fill=C_SHEET_SHADOW)
    draw_d.line([(32, 48), (48, 54)], fill=C_SHEET_CREASE)

    # EDREDÓN RETIRADO (Cubre desde la curva diagonal ondulada hasta el piecero)
    poly_duvet_body = [
        (54, 56),
        (102, 45),
        (138, 56),
        (94, 78)
    ]
    draw_d.polygon(poly_duvet_body, fill=C_QUILT_BASE, outline=C_OUTLINE)

    # PLIEGUE ONDULADO DEL EMBOZO (Doble vuelta orgánica del edredón)
    # Puntos que forman el borde ondulado del edredón plegado
    wave_points_top = [(50, 54), (64, 48), (78, 51), (92, 43), (104, 47)]
    wave_points_bot = [(104, 52), (92, 48), (78, 56), (64, 53), (50, 59)]
    poly_wave_fold = wave_points_top + wave_points_bot
    draw_d.polygon(poly_wave_fold, fill=C_SHEET_LIGHT, outline=C_OUTLINE)
    for p1, p2 in zip(wave_points_top[:-1], wave_points_top[1:]):
        draw_d.line([p1, p2], fill=C_SHEET_HIGHLIGHT)
    for p1, p2 in zip(wave_points_bot[:-1], wave_points_bot[1:]):
        draw_d.line([p1, p2], fill=C_SHEET_CREASE)

    # Sombras y relieves de acolchado en la parte inferior del edredón
    draw_d.line([(68, 62), (108, 50)], fill=C_QUILT_SEAM)
    draw_d.line([(76, 70), (122, 54)], fill=C_QUILT_SEAM)
    draw_d.line([(88, 66), (116, 67)], fill=C_QUILT_SEAM)
    draw_d.point([(78, 65)], fill=C_QUILT_LIGHT)
    draw_d.point([(98, 58)], fill=C_QUILT_LIGHT)
    draw_d.point([(88, 73)], fill=C_QUILT_LIGHT)
    draw_d.point([(108, 67)], fill=C_QUILT_LIGHT)

    draw_bed_foreground(draw_d)

    path_dest = os.path.join(output_dir, "cama_elaki_destendida.png")
    img_dest.save(path_dest)
    print("Guardado: cama_elaki_destendida.png (144x116)")

    # -------------------------------------------------------------
    # 3. VARIANTE DESORDENADA / PESADILLA (Tras segundo despertar)
    # -------------------------------------------------------------
    img_pesad = Image.new("RGBA", (W, H), C_TRANS)
    draw_p = ImageDraw.Draw(img_pesad)
    draw_bed_background(draw_p, img_pesad)

    # Caída lateral caótica
    draw_p.polygon(poly_side_sheet, fill=C_MAT_SIDE, outline=C_OUTLINE)
    draw_p.polygon(poly_side_quilt, fill=C_QUILT_BASE, outline=C_OUTLINE)
    draw_p.polygon(poly_drape_foot, fill=C_QUILT_SHADOW, outline=C_OUTLINE)

    # Almohada torcida y agitada
    draw_pillow(draw_p, 15, 34, width=17, dent=False, crooked=False)
    draw_pillow(draw_p, 31, 26, width=17, dent=True, crooked=True)

    # Sábana bajera muy revuelta con pliegues diagonales pronunciados
    draw_p.polygon(poly_exposed, fill=C_SHEET_BASE, outline=C_OUTLINE)
    for l in [((22, 39), (48, 30)), ((34, 45), (66, 33)), ((46, 51), (78, 37)), ((30, 48), (56, 42)), ((42, 55), (68, 48))]:
        draw_p.line(l, fill=C_SHEET_CREASE)
        draw_p.line([(l[0][0], l[0][1] - 1), (l[1][0], l[1][1] - 1)], fill=C_SHEET_LIGHT)

    # Edredón torcido
    poly_messy_quilt = [
        (58, 58),
        (106, 43),
        (138, 59),
        (92, 81)
    ]
    draw_p.polygon(poly_messy_quilt, fill=C_QUILT_BASE, outline=C_OUTLINE)
    draw_p.line([(62, 60), (108, 45)], fill=C_QUILT_DARK, width=2)
    draw_p.line([(64, 59), (109, 44)], fill=C_QUILT_HIGHLIGHT)
    draw_p.line([(76, 68), (124, 52)], fill=C_QUILT_DARK, width=2)
    draw_p.line([(78, 67), (125, 51)], fill=C_QUILT_LIGHT)

    # Pico de sábana colgando desordenadamente por el piecero
    poly_sheet_hang = [
        (114, 58),
        (128, 53),
        (124, 72),
        (111, 68)
    ]
    draw_p.polygon(poly_sheet_hang, fill=C_SHEET_LIGHT, outline=C_OUTLINE)
    draw_p.line([(112, 67), (123, 71)], fill=C_SHEET_SHADOW)

    draw_bed_foreground(draw_p)

    path_pesad = os.path.join(output_dir, "cama_elaki_desordenada.png")
    img_pesad.save(path_pesad)
    print("Guardado: cama_elaki_desordenada.png (144x116)")

    # -------------------------------------------------------------
    # 4. VARIANTE BASE (Estructura con colchón desnudo)
    # -------------------------------------------------------------
    img_base = Image.new("RGBA", (W, H), C_TRANS)
    draw_b = ImageDraw.Draw(img_base)
    draw_bed_background(draw_b, img_base)

    poly_mat_side = [(14, 36), (94, 78), (94, Y3 - 16), (14, Y1 - 16)]
    draw_b.polygon(poly_mat_side, fill=C_MAT_SIDE, outline=C_OUTLINE)
    poly_mat_foot = [(94, 78), (138, 56), (138, Y4 - 16), (94, Y3 - 16)]
    draw_b.polygon(poly_mat_foot, fill=C_MAT_SHADOW, outline=C_OUTLINE)

    poly_mat_top = [(14, 36), (55, 22), (138, 56), (94, 78)]
    draw_b.polygon(poly_mat_top, fill=C_SHEET_BASE, outline=C_OUTLINE)
    draw_b.line([(15, 36), (54, 23)], fill=C_SHEET_LIGHT)
    draw_b.line([(94, 78), (137, 57)], fill=C_SHEET_LIGHT)

    draw_pillow(draw_b, 17, 32, width=17, dent=False)
    draw_pillow(draw_b, 34, 23, width=17, dent=False)

    draw_bed_foreground(draw_b)

    path_base = os.path.join(output_dir, "cama_elaki_base.png")
    img_base.save(path_base)
    print("Guardado: cama_elaki_base.png (144x116)")

    # -------------------------------------------------------------
    # ARCHIVOS .META EN UNITY (48 PPU, Point, Single, None)
    # -------------------------------------------------------------
    pivot_x = 0.5
    pivot_y = 20.0 / float(H)

    for fname in ["cama_elaki_hecha.png", "cama_elaki_destendida.png", "cama_elaki_desordenada.png", "cama_elaki_base.png"]:
        meta_content = f"""fileFormatVersion: 2
guid: {os.urandom(16).hex()}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 0
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 0
  alignment: 9
  spritePivot: {{x: {pivot_x}, y: {pivot_y:.7f}}}
  spritePixelsToUnits: 48
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationMethod: 0
  spriteTessellationDetail: -1
  spriteGeometrySubdivision: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: 
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable: {{}}
  mipmapLimitGroupName: 
  pSDRemoveMatte: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
        meta_path = os.path.join(output_dir, fname + ".meta")
        with open(meta_path, "w", encoding="utf-8") as f:
            f.write(meta_content)
        print(f"Guardado: {fname}.meta")

if __name__ == "__main__":
    build_elaki_beds()
