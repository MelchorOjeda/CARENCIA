import os
from PIL import Image, ImageDraw

def create_remaining_sprites():
    escenarios = r"Assets\_Carencia\Sprites\Escenarios\Plantillas"
    puertas = r"Assets\_Carencia\Sprites\Objetos\Puertas"
    decoracion = r"Assets\_Carencia\Sprites\Objetos\Decoracion"
    os.makedirs(escenarios, exist_ok=True)
    os.makedirs(puertas, exist_ok=True)
    os.makedirs(decoracion, exist_ok=True)

    C_TRANS = (0, 0, 0, 0)
    C_OUTLINE = (28, 22, 34, 255)
    C_OUTLINE_SOFT = (58, 44, 62, 255)

    # Pared y zocalo
    C_WALL_BASE = (210, 202, 188, 255)
    C_WALL_LIGHT = (235, 228, 218, 255)
    C_WALL_SHADOW = (175, 166, 152, 255)
    C_ZOCALO_BASE = (98, 62, 42, 255)
    C_ZOCALO_LIGHT = (135, 92, 64, 255)

    # Maderas
    C_WOOD_LIGHT = (165, 115, 78, 255)
    C_WOOD_BASE = (132, 88, 56, 255)
    C_WOOD_SHADOW = (92, 58, 34, 255)
    C_WOOD_DARK = (60, 36, 20, 255)
    C_METAL = (210, 195, 140, 255)

    # Ventana
    C_GLASS_BASE = (120, 160, 190, 255)
    C_GLASS_LIGHT = (180, 220, 245, 255)
    C_GLASS_SHADOW = (80, 115, 145, 255)
    C_WIN_FRAME = (240, 238, 230, 255)
    C_WIN_SHADOW = (180, 175, 165, 255)

    # -------------------------------------------------------------
    # 1. PARED DERECHA CON VENTANA (48x154) - Fondo del pasillo
    # -------------------------------------------------------------
    # La pared derecha sube hacia la derecha: pendiente opuesta (-1 Y cada 2 X)
    # Altura vertical de pared libre = 130 px
    # bot_y(x) = 153 - (x // 2)   (de 153 a 130)
    # top_y(x) = 23 - (x // 2)    (de 23 a 0)
    img_win_wall = Image.new("RGBA", (48, 154), C_TRANS)
    pix_ww = img_win_wall.load()

    for x in range(48):
        bot_y = 153 - (x // 2)
        top_y = 23 - (x // 2)
        for y in range(top_y, bot_y + 1):
            if y == top_y or y == bot_y or x == 0 or x == 47:
                pix_ww[x, y] = C_OUTLINE
            elif y >= bot_y - 8:
                if y == bot_y - 8:
                    pix_ww[x, y] = C_OUTLINE_SOFT
                elif y == bot_y - 7:
                    pix_ww[x, y] = C_ZOCALO_LIGHT
                else:
                    pix_ww[x, y] = C_ZOCALO_BASE
            elif y in (top_y + 1, top_y + 2):
                pix_ww[x, y] = C_WALL_LIGHT
            else:
                pix_ww[x, y] = C_WALL_SHADOW if (x % 12 == 0) else C_WALL_BASE

    # Ventana en la pared derecha (de x=8 a x=39, ancho 32 px)
    # Altura ventana: desde bot_y - 35 hasta bot_y - 95 (60 px de alto)
    for x in range(8, 40):
        bot_y = 153 - (x // 2)
        w_bot = bot_y - 35
        w_top = bot_y - 95
        for y in range(w_top, w_bot + 1):
            is_frame_outer = (x in (8, 9, 38, 39) or y in (w_top, w_top + 1, w_bot, w_bot - 1))
            is_cross_bar = (x in (23, 24) or y in ((w_top + w_bot) // 2, (w_top + w_bot) // 2 + 1))
            
            if is_frame_outer:
                pix_ww[x, y] = C_WIN_FRAME
            elif is_cross_bar:
                pix_ww[x, y] = C_WIN_SHADOW
            else:
                # Vidrio con reflejo diagonal
                if (x + y) % 6 in (0, 1):
                    pix_ww[x, y] = C_GLASS_LIGHT
                elif (x + y) % 6 in (2, 3):
                    pix_ww[x, y] = C_GLASS_BASE
                else:
                    pix_ww[x, y] = C_GLASS_SHADOW

    # Repisa / alféizar de la ventana
    for x in range(6, 42):
        bot_y = 153 - (x // 2)
        sill_y = bot_y - 34
        pix_ww[x, sill_y] = C_WIN_FRAME
        pix_ww[x, sill_y + 1] = C_OUTLINE

    img_win_wall.save(os.path.join(escenarios, "pared_ventana_plantilla.png"))
    print("Creado pared_ventana_plantilla.png (48x154)")

    # -------------------------------------------------------------
    # 2. CUADRO FOTO GRUPAL PRIMARIA (29x36) - Según MEDIDAS.md
    # -------------------------------------------------------------
    img_foto_p = Image.new("RGBA", (29, 36), C_TRANS)
    pix_fp = img_foto_p.load()

    for x in range(2, 27):
        top_y = 2 + (x // 4)
        bot_y = top_y + 24
        for y in range(top_y, bot_y + 1):
            if x in (2, 3, 25, 26) or y in (top_y, top_y + 1, bot_y, bot_y - 1):
                pix_fp[x, y] = C_WOOD_SHADOW
            elif x in (4, 24) or y in (top_y + 2, bot_y - 2):
                pix_fp[x, y] = (245, 242, 235, 255) # Passepartout
            else:
                if y >= bot_y - 6:
                    pix_fp[x, y] = (110, 90, 75, 255) # Niños
                elif y in (bot_y - 9, bot_y - 8, bot_y - 7) and x % 3 == 0:
                    pix_fp[x, y] = (220, 180, 150, 255) # Caritas
                else:
                    pix_fp[x, y] = (190, 175, 155, 255) # Fondo

    img_foto_p.save(os.path.join(decoracion, "cuadro_foto_primaria.png"))
    print("Creado cuadro_foto_primaria.png (29x36)")

    # -------------------------------------------------------------
    # 3. CUADRO FOTO FAMILIAR / BODA (24x44) - Según MEDIDAS.md
    # -------------------------------------------------------------
    img_foto_f = Image.new("RGBA", (24, 44), C_TRANS)
    pix_ff = img_foto_f.load()

    for x in range(2, 22):
        top_y = 2 + (x // 4)
        bot_y = top_y + 32
        for y in range(top_y, bot_y + 1):
            if x in (2, 21) or y in (top_y, bot_y):
                pix_ff[x, y] = C_METAL # Marco dorado
            elif x in (3, 20) or y in (top_y + 1, bot_y - 1):
                pix_ff[x, y] = C_WOOD_DARK
            else:
                if y in (top_y + 8, top_y + 9) and x in (8, 9, 14, 15):
                    pix_ff[x, y] = (230, 200, 180, 255) # Rostros
                elif y > top_y + 9:
                    if x <= 11:
                        pix_ff[x, y] = (40, 40, 50, 255) # Traje
                    else:
                        pix_ff[x, y] = (235, 235, 245, 255) # Vestido novia
                else:
                    pix_ff[x, y] = (160, 150, 140, 255)

    img_foto_f.save(os.path.join(decoracion, "cuadro_foto_familiar.png"))
    print("Creado cuadro_foto_familiar.png (24x44)")

    # -------------------------------------------------------------
    # 4. CALENDARIO OCTUBRE (14x29) - Según MEDIDAS.md
    # -------------------------------------------------------------
    img_cal = Image.new("RGBA", (14, 29), C_TRANS)
    pix_cal = img_cal.load()

    for x in range(1, 13):
        top_y = 1 + (x // 4)
        bot_y = top_y + 22
        for y in range(top_y, bot_y + 1):
            if x in (1, 12) or y in (top_y, bot_y):
                pix_cal[x, y] = C_OUTLINE_SOFT
            elif y in (top_y + 1, top_y + 2):
                pix_cal[x, y] = (190, 40, 40, 255) # Cabecera roja
            elif y in (top_y + 3, top_y + 4):
                pix_cal[x, y] = (250, 245, 235, 255)
            elif y in (top_y + 5, top_y + 6) and (x % 2 == 0):
                pix_cal[x, y] = (130, 130, 130, 255)
            else:
                if y >= bot_y - 6 and x in (5, 6, 8, 9):
                    pix_cal[x, y] = (180, 30, 30, 255) # Número 31
                else:
                    pix_cal[x, y] = (255, 255, 250, 255)

    img_cal.save(os.path.join(decoracion, "calendario_octubre.png"))
    print("Creado calendario_octubre.png (14x29)")

    # -------------------------------------------------------------
    # 5. PUERTA 1: ELAKI (48x132) - Con cartel NO PASAR y stickers
    # -------------------------------------------------------------
    marco_path = os.path.join(puertas, "puerta_marco_plantilla.png")
    img_elaki = Image.open(marco_path).copy()
    pix_el = img_elaki.load()

    # Cordel del cartel "NO PASAR"
    cord_x = 24
    cord_top = 108 + (cord_x // 2) - 82
    pix_el[cord_x, cord_top] = C_OUTLINE
    pix_el[cord_x - 3, cord_top + 3] = C_OUTLINE
    pix_el[cord_x + 3, cord_top + 3] = C_OUTLINE

    # Texto rojo aproximado en el cartel
    for cx in range(20, 28):
        cy = 108 + (cx // 2) - 72
        if cx in (20, 21, 23, 25, 27):
            pix_el[cx, cy] = (180, 30, 30, 255)

    # Stickers (fantasmita, gato amarillo, etc.)
    st1_x, st1_y = 18, 108 + (18 // 2) - 58
    for dx in range(4):
        for dy in range(4):
            pix_el[st1_x + dx, st1_y + dy] = (100, 180, 240, 255)
    pix_el[st1_x + 1, st1_y + 1] = C_OUTLINE
    pix_el[st1_x + 2, st1_y + 1] = C_OUTLINE

    st2_x, st2_y = 19, 108 + (19 // 2) - 40
    for dx in range(5):
        for dy in range(4):
            pix_el[st2_x + dx, st2_y + dy] = (240, 210, 80, 255)
    pix_el[st2_x + 1, st2_y - 1] = (240, 210, 80, 255)
    pix_el[st2_x + 3, st2_y - 1] = (240, 210, 80, 255)
    pix_el[st2_x + 1, st2_y + 1] = C_OUTLINE
    pix_el[st2_x + 3, st2_y + 1] = C_OUTLINE

    img_elaki.save(os.path.join(puertas, "puerta_elaki.png"))
    print("Creado puerta_elaki.png (48x132)")

    # -------------------------------------------------------------
    # 6. PUERTA 2: BAÑO (48x132) - Con cristal esmerilado
    # -------------------------------------------------------------
    img_bano = Image.open(marco_path).copy()
    pix_ba = img_bano.load()

    # Quitar cartel de no pasar (reemplazar con madera)
    for cx in range(16, 32):
        cy_base = 108 + (cx // 2) - 78
        for cy in range(cy_base, cy_base + 12):
            pix_ba[cx, cy] = C_WOOD_LIGHT

    # Panel superior de cristal esmerilado
    for cx in range(14, 34):
        cy_base = 108 + (cx // 2) - 95
        for cy in range(cy_base, cy_base + 35):
            if cx in (14, 33) or cy in (cy_base, cy_base + 34):
                pix_ba[cx, cy] = C_OUTLINE_SOFT
            else:
                pix_ba[cx, cy] = (175, 205, 215, 255) if (cx + cy) % 3 == 0 else (195, 220, 230, 255)

    # Letrero de metal "BAÑO"
    for lx in range(21, 27):
        ly = 108 + (lx // 2) - 56
        pix_ba[lx, ly] = C_METAL

    img_bano.save(os.path.join(puertas, "puerta_bano.png"))
    print("Creado puerta_bano.png (48x132)")

    # -------------------------------------------------------------
    # 7. PUERTA 3: HERMANA MARGARITA (48x132)
    # -------------------------------------------------------------
    img_marg = Image.open(marco_path).copy()
    pix_mg = img_marg.load()

    for cx in range(16, 32):
        cy_base = 108 + (cx // 2) - 78
        for cy in range(cy_base, cy_base + 12):
            pix_mg[cx, cy] = C_WOOD_LIGHT

    for px in range(6, 42):
        bot_y = 108 + (px // 2)
        top_y = bot_y - 104
        for py in range(top_y, bot_y):
            c = pix_mg[px, py]
            if c != C_TRANS and c != C_METAL:
                pix_mg[px, py] = (max(0, c[0] - 12), max(0, c[1] - 10), max(0, c[2] - 14), 255)

    img_marg.save(os.path.join(puertas, "puerta_margarita.png"))
    print("Creado puerta_margarita.png (48x132)")

if __name__ == "__main__":
    create_remaining_sprites()
