import os
from PIL import Image, ImageDraw

def create_bathroom_sprites():
    muebles_dir = r"Assets\_Carencia\Sprites\Objetos\Muebles"
    decor_dir = r"Assets\_Carencia\Sprites\Objetos\Decoracion"
    plantillas_dir = r"Assets\_Carencia\Sprites\Escenarios\Plantillas"

    os.makedirs(muebles_dir, exist_ok=True)
    os.makedirs(decor_dir, exist_ok=True)
    os.makedirs(plantillas_dir, exist_ok=True)

    C_TRANS = (0, 0, 0, 0)
    C_OUTLINE = (28, 22, 34, 255)
    C_OUTLINE_SOFT = (58, 44, 62, 255)

    # Porcelana sanitaria
    C_WHITE_PURE = (255, 255, 255, 255)
    C_PORC_LIGHT = (245, 242, 238, 255)
    C_PORC_BASE = (230, 226, 220, 255)
    C_PORC_SHADOW = (185, 180, 175, 255)
    C_PORC_DARK = (140, 135, 130, 255)

    # Metales y cromados
    C_CHROME_LIGHT = (245, 250, 255, 255)
    C_CHROME_BASE = (205, 215, 225, 255)
    C_CHROME_SHADOW = (140, 150, 165, 255)

    # Maderas (Marco espejo, toallero)
    C_WOOD_LIGHT = (175, 115, 75, 255)
    C_WOOD_BASE = (135, 88, 55, 255)
    C_WOOD_SHADOW = (92, 58, 35, 255)
    C_WOOD_DARK = (55, 32, 18, 255)

    # Vidrio espejo
    C_GLASS_LIGHT = (235, 245, 255, 255)
    C_GLASS_BASE = (195, 215, 230, 255)
    C_GLASS_SHADOW = (150, 175, 195, 255)

    # Bañera exterior e interior
    C_TUB_OUTER = (190, 185, 182, 255)
    C_TUB_OUTER_SHADOW = (145, 140, 138, 255)
    C_TUB_RIM = (225, 220, 218, 255)
    C_TUB_INNER = (240, 238, 235, 255)
    C_TUB_INNER_SHADOW = (170, 168, 165, 255)
    C_TILE_SEAM = (195, 190, 185, 255)

    # Cortina de baño (Beige cálido fruncido)
    C_CURT_LIGHT = (242, 228, 212, 255)
    C_CURT_BASE = (225, 205, 185, 255)
    C_CURT_FOLD = (185, 155, 135, 255)
    C_CURT_DEEP = (135, 105, 88, 255)

    # Toallas de los 4 integrantes de la familia
    # 1: Padre (Verde bosque)
    # 2: Madre (Naranja terracota)
    # 3: Elaki (Azul índigo)
    # 4: Margarita (Amarillo mostaza)
    TOWEL_COLORS = [
        ((65, 120, 85, 255), (95, 155, 115, 255)),    # Padre
        ((215, 95, 55, 255), (245, 135, 85, 255)),    # Madre
        ((45, 110, 140, 255), (75, 145, 175, 255)),   # Elaki
        ((230, 185, 65, 255), (255, 215, 100, 255))   # Margarita
    ]
    GREY_BASE = (140, 140, 145, 255)
    GREY_LIGHT = (180, 180, 185, 255)

    # Toalla de mano individual
    C_TOWL_YELLOW = (230, 185, 65, 255)
    C_TOWL_YELLOW_L = (255, 215, 100, 255)

    # Alfombra tejida
    C_RUG_BASE = (185, 105, 65, 255)
    C_RUG_LIGHT = (215, 135, 90, 255)
    C_RUG_SHADOW = (140, 75, 45, 255)
    C_RUG_WHITE = (245, 240, 235, 255)

    # -------------------------------------------------------------
    # 1. TILE SUELO DAMERO (48x24) - Mosaico de baldosas blanco y negro
    # -------------------------------------------------------------
    img_damero = Image.new("RGBA", (48, 24), C_TRANS)
    pix_d = img_damero.load()

    C_TILE_W = (240, 235, 228, 255)
    C_TILE_W_L = (255, 252, 248, 255)
    C_TILE_B = (38, 35, 42, 255)
    C_TILE_B_L = (58, 54, 64, 255)

    for x in range(48):
        if x < 24:
            top_y = 11 - (x // 2)
            bot_y = 12 + (x // 2)
        else:
            top_y = (x - 24) // 2
            bot_y = 23 - ((x - 24) // 2)
        
        for y in range(top_y, bot_y + 1):
            if y == top_y or y == bot_y:
                pix_d[x, y] = C_OUTLINE
            else:
                is_black = ((x // 12) + (y // 6)) % 2 == 0
                if is_black:
                    pix_d[x, y] = C_TILE_B_L if (y == top_y + 1 or x % 12 == 0) else C_TILE_B
                else:
                    pix_d[x, y] = C_TILE_W_L if (y == top_y + 1 or x % 12 == 0) else C_TILE_W

    draw_d = ImageDraw.Draw(img_damero)
    draw_d.line([(24, 0), (0, 12)], fill=C_OUTLINE_SOFT)
    draw_d.line([(24, 0), (47, 12)], fill=C_OUTLINE_SOFT)
    draw_d.line([(0, 12), (24, 23)], fill=C_OUTLINE_SOFT)
    draw_d.line([(47, 12), (24, 23)], fill=C_OUTLINE_SOFT)
    draw_d.line([(12, 6), (36, 18)], fill=C_OUTLINE_SOFT)
    draw_d.line([(36, 6), (12, 18)], fill=C_OUTLINE_SOFT)

    damero_path = os.path.join(plantillas_dir, "tile_suelo_damero.png")
    img_damero.save(damero_path)
    print("Creado tile_suelo_damero.png (48x24)")

    # -------------------------------------------------------------
    # 2. INODORO / ESCUSADO (48x65 px) - 1 celda, 0.75 m de alto
    # -------------------------------------------------------------
    img_toilet = Image.new("RGBA", (48, 65), C_TRANS)
    draw_t = ImageDraw.Draw(img_toilet)

    draw_t.ellipse([16, 50, 32, 62], fill=C_PORC_SHADOW, outline=C_OUTLINE)
    draw_t.polygon([(17, 56), (31, 56), (28, 40), (20, 40)], fill=C_PORC_BASE, outline=C_OUTLINE)
    draw_t.ellipse([10, 26, 38, 48], fill=C_PORC_BASE, outline=C_OUTLINE)
    draw_t.ellipse([15, 30, 33, 44], fill=C_PORC_SHADOW, outline=C_OUTLINE_SOFT)
    draw_t.ellipse([18, 34, 30, 41], fill=(160, 195, 215, 255))
    draw_t.ellipse([14, 12, 34, 36], fill=C_PORC_LIGHT, outline=C_OUTLINE)
    draw_t.ellipse([17, 15, 31, 33], fill=C_PORC_BASE, outline=C_OUTLINE_SOFT)
    draw_t.rectangle([11, 16, 37, 36], fill=C_PORC_BASE, outline=C_OUTLINE)
    draw_t.rectangle([9, 13, 39, 17], fill=C_PORC_LIGHT, outline=C_OUTLINE)
    draw_t.rectangle([34, 19, 38, 22], fill=C_CHROME_BASE, outline=C_OUTLINE)
    draw_t.point([(35, 20)], fill=C_CHROME_LIGHT)
    draw_t.rectangle([16, 6, 24, 13], fill=C_WHITE_PURE, outline=C_OUTLINE)
    draw_t.ellipse([16, 5, 24, 8], fill=(230, 230, 235, 255), outline=C_OUTLINE_SOFT)
    draw_t.point([(20, 6)], fill=C_PORC_SHADOW)

    toilet_path = os.path.join(muebles_dir, "bano_inodoro.png")
    img_toilet.save(toilet_path)
    print("Creado bano_inodoro.png (48x65)")

    # -------------------------------------------------------------
    # 3. LAVABO CON PEDESTAL (48x70 px) - 1 celda, 0.85 m de alto
    # -------------------------------------------------------------
    img_sink = Image.new("RGBA", (48, 70), C_TRANS)
    draw_s = ImageDraw.Draw(img_sink)

    draw_s.ellipse([18, 56, 30, 67], fill=C_PORC_SHADOW, outline=C_OUTLINE)
    draw_s.rectangle([21, 34, 27, 58], fill=C_PORC_BASE, outline=C_OUTLINE)
    draw_s.line([(22, 35), (22, 57)], fill=C_PORC_LIGHT)
    draw_s.line([(26, 35), (26, 57)], fill=C_PORC_SHADOW)
    draw_s.ellipse([6, 18, 42, 38], fill=C_PORC_LIGHT, outline=C_OUTLINE)
    draw_s.ellipse([10, 21, 38, 34], fill=C_PORC_BASE, outline=C_OUTLINE_SOFT)
    draw_s.ellipse([22, 28, 26, 31], fill=C_CHROME_SHADOW, outline=C_OUTLINE)
    draw_s.point([(24, 29)], fill=C_OUTLINE)
    draw_s.rectangle([22, 11, 26, 20], fill=C_CHROME_BASE, outline=C_OUTLINE)
    draw_s.line([(23, 11), (23, 19)], fill=C_CHROME_LIGHT)
    draw_s.rectangle([16, 15, 20, 18], fill=C_CHROME_BASE, outline=C_OUTLINE)
    draw_s.point([(18, 16)], fill=(220, 60, 50, 255))
    draw_s.rectangle([28, 15, 32, 18], fill=C_CHROME_BASE, outline=C_OUTLINE)
    draw_s.point([(30, 16)], fill=(60, 120, 220, 255))

    sink_path = os.path.join(muebles_dir, "bano_lavabo.png")
    img_sink.save(sink_path)
    print("Creado bano_lavabo.png (48x70)")

    # -------------------------------------------------------------
    # 4. BAÑERA (192x126 px) - 6x2 celdas, 0.55 m de alto
    # -------------------------------------------------------------
    img_tub = Image.new("RGBA", (192, 126), C_TRANS)
    draw_tb = ImageDraw.Draw(img_tub)

    poly_skirt_l = [(12, 60), (96, 102), (96, 124), (12, 82)]
    draw_tb.polygon(poly_skirt_l, fill=C_TUB_OUTER, outline=C_OUTLINE)
    for sx in range(16, 94, 6):
        sy = 60 + int((sx - 12) * (42 / 84))
        draw_tb.line([(sx, sy + 3), (sx, sy + 20)], fill=(175, 170, 168, 255))

    poly_skirt_r = [(96, 102), (180, 60), (180, 82), (96, 124)]
    draw_tb.polygon(poly_skirt_r, fill=C_TUB_OUTER_SHADOW, outline=C_OUTLINE)

    poly_top_outer = [(96, 18), (180, 60), (96, 102), (12, 60)]
    draw_tb.polygon(poly_top_outer, fill=C_TUB_RIM, outline=C_OUTLINE)

    poly_top_inner = [(96, 26), (170, 60), (96, 94), (22, 60)]
    draw_tb.polygon(poly_top_inner, fill=C_TUB_INNER, outline=C_OUTLINE)

    poly_bottom = [(96, 42), (160, 70), (96, 98), (32, 70)]
    draw_tb.polygon(poly_bottom, fill=C_TUB_INNER_SHADOW, outline=C_OUTLINE_SOFT)

    for lx in range(30, 160, 16):
        draw_tb.line([(lx, 35), (lx, 90)], fill=C_TILE_SEAM)

    draw_tb.rectangle([144, 46, 154, 53], fill=C_CHROME_BASE, outline=C_OUTLINE)
    draw_tb.line([(144, 47), (154, 47)], fill=C_CHROME_LIGHT)
    draw_tb.polygon([(146, 53), (150, 58), (148, 60), (144, 55)], fill=C_CHROME_SHADOW, outline=C_OUTLINE)

    draw_tb.line([(150, 6), (150, 46)], fill=C_CHROME_BASE, width=3)
    draw_tb.line([(150, 6), (150, 46)], fill=C_OUTLINE, width=1)
    draw_tb.polygon([(144, 4), (156, 10), (152, 14), (140, 8)], fill=C_CHROME_BASE, outline=C_OUTLINE)

    tub_path = os.path.join(muebles_dir, "bano_banera.png")
    img_tub.save(tub_path)
    print("Creado bano_banera.png (192x126)")

    # -------------------------------------------------------------
    # 5. CORTINA DE BAÑO (144x126 px) - En capa superior
    # -------------------------------------------------------------
    img_curt = Image.new("RGBA", (144, 126), C_TRANS)
    draw_c = ImageDraw.Draw(img_curt)

    draw_c.line([(14, 28), (130, 2)], fill=C_CHROME_SHADOW, width=3)
    draw_c.line([(14, 28), (80, 61)], fill=C_CHROME_BASE, width=3)
    draw_c.line([(14, 27), (80, 60)], fill=C_CHROME_LIGHT, width=1)

    pliegues = [
        (16, 28, 92), (23, 31, 95), (31, 35, 99),
        (39, 39, 103), (48, 44, 107), (56, 48, 111), (65, 53, 115)
    ]
    for px, py_top, py_bot in pliegues:
        draw_c.polygon([(px - 3, py_top), (px + 4, py_top + 4), (px + 4, py_bot + 4), (px - 3, py_bot)], fill=C_CURT_FOLD, outline=C_OUTLINE)
        draw_c.polygon([(px - 1, py_top), (px + 3, py_top + 2), (px + 3, py_bot + 2), (px - 1, py_bot)], fill=C_CURT_LIGHT)
        for ry in range(py_top + 10, py_bot - 8, 12):
            draw_c.line([(px, ry), (px + 2, ry + 2)], fill=C_CURT_DEEP)
        draw_c.ellipse([px - 2, py_top - 3, px + 2, py_top + 1], outline=C_OUTLINE, fill=C_CHROME_LIGHT)

    curt_path = os.path.join(muebles_dir, "bano_cortina.png")
    img_curt.save(curt_path)
    print("Creado bano_cortina.png (144x126)")

    # -------------------------------------------------------------
    # 6. ESPEJO CON MARCO DE MADERA / GABINETE (36x52 px) - Pared
    # -------------------------------------------------------------
    img_mirror = Image.new("RGBA", (36, 52), C_TRANS)
    draw_m = ImageDraw.Draw(img_mirror)

    draw_m.rectangle([2, 4, 33, 48], fill=C_WOOD_BASE, outline=C_OUTLINE)
    draw_m.rectangle([3, 5, 32, 47], outline=C_WOOD_LIGHT)
    draw_m.rectangle([4, 6, 31, 46], outline=C_WOOD_SHADOW)
    draw_m.rectangle([6, 8, 29, 44], fill=C_GLASS_BASE, outline=C_OUTLINE_SOFT)
    draw_m.polygon([(8, 10), (18, 10), (27, 42), (17, 42)], fill=C_GLASS_LIGHT)
    draw_m.polygon([(20, 10), (25, 10), (29, 25), (27, 25)], fill=C_WHITE_PURE)

    mirror_path = os.path.join(decor_dir, "bano_espejo_pared.png")
    img_mirror.save(mirror_path)
    print("Creado bano_espejo_pared.png (36x52)")

    # -------------------------------------------------------------
    # 7. TOALLERO DE 4 TOALLAS Y SUS VARIANTES SEGÚN NIVEL (48x44 px)
    # -------------------------------------------------------------
    def draw_towel_rack(active_colors, filename):
        w, h = 48, 44
        im = Image.new('RGBA', (w, h), C_TRANS)
        draw = ImageDraw.Draw(im)

        draw.rectangle([2, 5, 45, 10], fill=C_WOOD_BASE, outline=C_OUTLINE)
        draw.rectangle([2, 5, 5, 13], fill=C_WOOD_SHADOW, outline=C_OUTLINE)
        draw.rectangle([42, 5, 45, 13], fill=C_WOOD_SHADOW, outline=C_OUTLINE)

        # 4 Toallas: Padre, Madre, Elaki, Margarita
        x_positions = [
            (6, 13, 32),
            (15, 22, 35),
            (24, 31, 38),
            (33, 40, 34)
        ]

        for i, (x0, x1, y_bot) in enumerate(x_positions):
            is_col = active_colors[i]
            c_base = TOWEL_COLORS[i][0] if is_col else GREY_BASE
            c_light = TOWEL_COLORS[i][1] if is_col else GREY_LIGHT

            draw.rectangle([x0, 8, x1, y_bot], fill=c_base, outline=C_OUTLINE)
            draw.line([(x0 + 1, 9), (x1 - 1, 9)], fill=c_light)
            draw.line([(x0, y_bot - 2), (x1, y_bot - 2)], fill=C_OUTLINE_SOFT)

        out = os.path.join(decor_dir, filename)
        im.save(out)
        print(f"Creado {filename} (48x44)")

    # Generar variantes
    draw_towel_rack([True, True, True, True], "bano_toallero.png")
    draw_towel_rack([True, True, True, True], "bano_toallero_prologo.png")
    draw_towel_rack([False, False, True, True], "bano_toallero_nivel1.png")
    draw_towel_rack([False, False, False, True], "bano_toallero_nivel2_solo_margarita.png")

    # -------------------------------------------------------------
    # 8. TOALLERO PEQUEÑO CON TOALLA DE MANO (18x36 px)
    # -------------------------------------------------------------
    img_hand = Image.new("RGBA", (18, 36), C_TRANS)
    draw_h = ImageDraw.Draw(img_hand)

    draw_h.rectangle([4, 4, 14, 7], fill=(60, 60, 70, 255), outline=C_OUTLINE)
    draw_h.rectangle([3, 7, 15, 33], fill=C_TOWL_YELLOW, outline=C_OUTLINE)
    draw_h.line([(4, 8), (14, 8)], fill=C_TOWL_YELLOW_L)
    draw_h.line([(3, 30), (15, 30)], fill=(185, 145, 45, 255))

    hand_path = os.path.join(decor_dir, "bano_toalla_mano.png")
    img_hand.save(hand_path)
    print("Creado bano_toalla_mano.png (18x36)")

    # -------------------------------------------------------------
    # 9. ALFOMBRA DE BAÑO / TAPETE (72x44 px) - Suelo
    # -------------------------------------------------------------
    img_rug = Image.new("RGBA", (72, 44), C_TRANS)
    draw_r = ImageDraw.Draw(img_rug)

    poly_rug_bot = [(6, 24), (36, 39), (66, 24), (66, 21), (36, 36), (6, 21)]
    draw_r.polygon(poly_rug_bot, fill=C_RUG_SHADOW, outline=C_OUTLINE)
    poly_rug_top = [(36, 6), (66, 21), (36, 36), (6, 21)]
    draw_r.polygon(poly_rug_top, fill=C_RUG_BASE, outline=C_OUTLINE)
    draw_r.line([(22, 14), (52, 29)], fill=C_RUG_WHITE, width=2)
    draw_r.line([(20, 28), (50, 13)], fill=C_RUG_WHITE, width=2)
    for step in range(12, 60, 4):
        draw_r.line([(step, 18), (step + 6, 24)], fill=C_RUG_LIGHT)

    rug_path = os.path.join(decor_dir, "bano_alfombra.png")
    img_rug.save(rug_path)
    print("Creado bano_alfombra.png (72x44)")

if __name__ == "__main__":
    create_bathroom_sprites()
