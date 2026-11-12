import os
from PIL import Image, ImageDraw

def create_kit():
    escenarios = r"Assets\_Carencia\Sprites\Escenarios\Plantillas"
    muebles = r"Assets\_Carencia\Sprites\Objetos\Muebles"
    puertas = r"Assets\_Carencia\Sprites\Objetos\Puertas"
    os.makedirs(escenarios, exist_ok=True)
    os.makedirs(muebles, exist_ok=True)
    os.makedirs(puertas, exist_ok=True)

    # Palette
    C_TRANS = (0, 0, 0, 0)
    C_OUTLINE = (28, 22, 34, 255)       # Contorno oscuro retro
    C_OUTLINE_SOFT = (58, 44, 62, 255)  # Contorno secundario
    
    # Suelo madera / baldosa
    C_FLOOR_TOP = (182, 142, 102, 255)
    C_FLOOR_LIGHT = (205, 168, 128, 255)
    C_FLOOR_SHADOW = (148, 110, 78, 255)
    C_FLOOR_SEAM = (110, 80, 55, 255)
    
    # Pared
    C_WALL_BASE = (220, 212, 198, 255)
    C_WALL_LIGHT = (238, 232, 222, 255)
    C_WALL_SHADOW = (185, 176, 162, 255)
    C_ZOCALO_BASE = (98, 62, 42, 255)
    C_ZOCALO_LIGHT = (135, 92, 64, 255)
    C_ZOCALO_SHADOW = (65, 38, 24, 255)

    # Madera muebles (Mesa y Armario)
    C_WOOD_LIGHT = (165, 115, 78, 255)
    C_WOOD_BASE = (132, 88, 56, 255)
    C_WOOD_SHADOW = (92, 58, 34, 255)
    C_WOOD_DARK = (60, 36, 20, 255)
    C_METAL = (210, 195, 140, 255)      # Manijas doradas / laton

    # -------------------------------------------------------------
    # 1. TILE SUELO (48x24) - Paso 2:1 isométrico oficial (48 PPU)
    # -------------------------------------------------------------
    img_suelo = Image.new("RGBA", (48, 24), C_TRANS)
    pixels_suelo = img_suelo.load()

    for x in range(48):
        if x < 24:
            top_y = 11 - (x // 2)
            bot_y = 12 + (x // 2)
        else:
            top_y = (x - 24) // 2
            bot_y = 23 - ((x - 24) // 2)
        
        for y in range(top_y, bot_y + 1):
            if y == top_y or y == bot_y:
                pixels_suelo[x, y] = C_OUTLINE
            elif y == top_y + 1:
                pixels_suelo[x, y] = C_FLOOR_LIGHT
            elif y == bot_y - 1:
                pixels_suelo[x, y] = C_FLOOR_SHADOW
            else:
                # Textura de tablones sutiles
                if (x + y) % 6 == 0:
                    pixels_suelo[x, y] = C_FLOOR_SHADOW
                else:
                    pixels_suelo[x, y] = C_FLOOR_TOP

    # Puntos extremos limpios
    pixels_suelo[0, 11] = C_OUTLINE
    pixels_suelo[0, 12] = C_OUTLINE
    pixels_suelo[47, 11] = C_OUTLINE
    pixels_suelo[47, 12] = C_OUTLINE
    pixels_suelo[23, 0] = C_OUTLINE
    pixels_suelo[24, 0] = C_OUTLINE
    pixels_suelo[23, 23] = C_OUTLINE
    pixels_suelo[24, 23] = C_OUTLINE

    img_suelo.save(os.path.join(escenarios, "tile_suelo_plantilla.png"))
    print("Creado tile_suelo_plantilla.png (48x24)")

    # -------------------------------------------------------------
    # 2. PARED BASE (48x154) - 130 px de altura de pared (2.40 m)
    # -------------------------------------------------------------
    # Pared diagonal izquierda: va subiendo hacia la derecha (+1 Y por cada 2 X)
    # Altura vertical de pared libre = 130 px
    # Con el desnivel de 24 px en 48 px de ancho, el lienzo total mide 48x154 px.
    img_pared = Image.new("RGBA", (48, 154), C_TRANS)
    pix_pared = img_pared.load()

    for x in range(48):
        top_y = x // 2
        bot_y = 130 + (x // 2)
        for y in range(top_y, bot_y + 1):
            if y == top_y or y == bot_y or x == 0 or x == 47:
                pix_pared[x, y] = C_OUTLINE
            elif y >= bot_y - 8:
                # Zócalo / rodapié
                if y == bot_y - 8:
                    pix_pared[x, y] = C_OUTLINE_SOFT
                elif y == bot_y - 7:
                    pix_pared[x, y] = C_ZOCALO_LIGHT
                else:
                    pix_pared[x, y] = C_ZOCALO_BASE
            elif y == top_y + 1 or y == top_y + 2:
                # Cornisa superior
                pix_pared[x, y] = C_WALL_LIGHT
            else:
                # Fondo pared con sombras verticales
                if x % 12 == 0:
                    pix_pared[x, y] = C_WALL_SHADOW
                else:
                    pix_pared[x, y] = C_WALL_BASE

    img_pared.save(os.path.join(escenarios, "pared_base_plantilla.png"))
    print("Creado pared_base_plantilla.png (48x154)")

    # -------------------------------------------------------------
    # 3. PARED CON HUECO DE PUERTA (48x154)
    # -------------------------------------------------------------
    img_pared_p = img_pared.copy()
    pix_pp = img_pared_p.load()

    # Vano de puerta: 108 px de alto (2.0 m) de x=4 a x=43 (ancho 40 px)
    for x in range(4, 44):
        bot_y = 130 + (x // 2)
        door_top = bot_y - 108
        for y in range(door_top, bot_y + 1):
            if x == 4 or x == 43 or y == door_top:
                pix_pp[x, y] = C_OUTLINE
            else:
                # Hueco interior oscuro (vano)
                pix_pp[x, y] = (20, 16, 26, 255)

    img_pared_p.save(os.path.join(escenarios, "pared_puerta_plantilla.png"))
    print("Creado pared_puerta_plantilla.png (48x154)")

    # -------------------------------------------------------------
    # 4. PUERTA Y MARCO (48x132) - En capa independiente
    # -------------------------------------------------------------
    # Altura de puerta: 108 px (2.0 m) + 24 px desnivel = 132 px
    img_puerta = Image.new("RGBA", (48, 132), C_TRANS)
    pix_door = img_puerta.load()

    for x in range(4, 44):
        bot_y = 108 + (x // 2)
        door_top = bot_y - 108
        for y in range(door_top, bot_y + 1):
            # Marco exterior
            if x in (4, 5, 42, 43) or y in (door_top, door_top + 1):
                pix_door[x, y] = C_ZOCALO_BASE
            elif y == bot_y:
                pix_door[x, y] = C_OUTLINE
            else:
                # Paneles de la puerta
                is_panel_top = (8 <= x <= 39) and (door_top + 10 <= y <= door_top + 50)
                is_panel_bot = (8 <= x <= 39) and (door_top + 58 <= y <= bot_y - 8)
                if is_panel_top or is_panel_bot:
                    if x in (8, 39) or y in (door_top + 10, door_top + 50, door_top + 58, bot_y - 8):
                        pix_door[x, y] = C_WOOD_SHADOW
                    else:
                        pix_door[x, y] = C_WOOD_LIGHT
                else:
                    pix_door[x, y] = C_WOOD_BASE

    # Picaporte / Manija metálica
    manija_x = 38
    manija_y = 108 + (manija_x // 2) - 52
    pix_door[manija_x, manija_y] = C_METAL
    pix_door[manija_x - 1, manija_y] = C_METAL
    pix_door[manija_x, manija_y + 1] = C_OUTLINE

    # Cartelito base "NO PASAR"
    for cx in range(16, 32):
        cy_base = 108 + (cx // 2) - 78
        for cy in range(cy_base, cy_base + 12):
            if cx in (16, 31) or cy in (cy_base, cy_base + 11):
                pix_door[cx, cy] = (60, 50, 40, 255)
            else:
                pix_door[cx, cy] = (240, 235, 210, 255)

    img_puerta.save(os.path.join(puertas, "puerta_marco_plantilla.png"))
    print("Creado puerta_marco_plantilla.png (48x132)")

    # -------------------------------------------------------------
    # 5. MESA ISOMETRICA (48x65) - 0.75 m de alto
    # -------------------------------------------------------------
    img_mesa = Image.new("RGBA", (48, 65), C_TRANS)
    draw_mesa = ImageDraw.Draw(img_mesa)

    # Patas de la mesa (4 postes verticales)
    patas = [(8, 28, 55), (38, 43, 55), (10, 39, 63), (40, 54, 63)] # x, y_top, y_bot
    for px, py_top, py_bot in patas:
        draw_mesa.rectangle([px, py_top, px + 2, py_bot], fill=C_WOOD_SHADOW, outline=C_OUTLINE)

    # Tablero superior
    rombo_top = [(24, 18), (44, 28), (24, 38), (4, 28)]
    draw_mesa.polygon([(4, 28), (24, 38), (24, 42), (4, 32)], fill=C_WOOD_BASE, outline=C_OUTLINE)
    draw_mesa.polygon([(24, 38), (44, 28), (44, 32), (24, 42)], fill=C_WOOD_SHADOW, outline=C_OUTLINE)
    draw_mesa.polygon(rombo_top, fill=C_WOOD_LIGHT, outline=C_OUTLINE)

    # Florero
    draw_mesa.rectangle([22, 23, 26, 28], fill=(220, 220, 240, 255), outline=C_OUTLINE)
    draw_mesa.point([(24, 20), (23, 21), (25, 21)], fill=(120, 190, 90, 255))

    img_mesa.save(os.path.join(muebles, "mesa_isometrica_plantilla.png"))
    print("Creado mesa_isometrica_plantilla.png (48x65)")

    # -------------------------------------------------------------
    # 6. ARMARIO ISOMETRICO (48x132) - 2 m de alto
    # -------------------------------------------------------------
    img_armario = Image.new("RGBA", (48, 132), C_TRANS)
    draw_arm = ImageDraw.Draw(img_armario)

    # Cara frontal (Left face)
    poly_front = [(6, 28), (36, 43), (36, 126), (6, 111)]
    draw_arm.polygon(poly_front, fill=C_WOOD_BASE, outline=C_OUTLINE)

    # División de puertas
    draw_arm.line([(21, 35), (21, 118)], fill=C_OUTLINE, width=1)
    draw_arm.point([(19, 78), (23, 80)], fill=C_METAL)

    # Cara lateral derecha
    poly_side = [(36, 43), (46, 38), (46, 121), (36, 126)]
    draw_arm.polygon(poly_side, fill=C_WOOD_SHADOW, outline=C_OUTLINE)

    # Cara superior (techo)
    poly_top = [(16, 23), (46, 38), (36, 43), (6, 28)]
    draw_arm.polygon(poly_top, fill=C_WOOD_LIGHT, outline=C_OUTLINE)

    # Cajas arriba del armario
    caja_front = [(14, 18), (26, 24), (26, 32), (14, 26)]
    caja_side = [(26, 24), (32, 21), (32, 29), (26, 32)]
    caja_top = [(20, 15), (32, 21), (26, 24), (14, 18)]
    draw_arm.polygon(caja_front, fill=(195, 170, 130, 255), outline=C_OUTLINE)
    draw_arm.polygon(caja_side, fill=(155, 130, 95, 255), outline=C_OUTLINE)
    draw_arm.polygon(caja_top, fill=(225, 205, 160, 255), outline=C_OUTLINE)

    img_armario.save(os.path.join(muebles, "armario_isometrico_plantilla.png"))
    print("Creado armario_isometrico_plantilla.png (48x132)")

    # -------------------------------------------------------------
    # 7. PALETA DE COLORES (128x32)
    # -------------------------------------------------------------
    img_paleta = Image.new("RGBA", (128, 32), C_TRANS)
    draw_pal = ImageDraw.Draw(img_paleta)
    colores = [
        C_OUTLINE, C_OUTLINE_SOFT, C_WALL_BASE, C_WALL_LIGHT,
        C_WALL_SHADOW, C_ZOCALO_BASE, C_ZOCALO_LIGHT, C_FLOOR_TOP,
        C_FLOOR_LIGHT, C_FLOOR_SHADOW, C_WOOD_LIGHT, C_WOOD_BASE,
        C_WOOD_SHADOW, C_WOOD_DARK, C_METAL, (240, 235, 210, 255)
    ]
    for i, col in enumerate(colores):
        x = (i % 8) * 16
        y = (i // 8) * 16
        draw_pal.rectangle([x, y, x + 15, y + 15], fill=col, outline=C_OUTLINE)
    
    img_paleta.save(os.path.join(escenarios, "paleta_colores_carencia.png"))
    print("Creado paleta_colores_carencia.png (128x32)")

if __name__ == "__main__":
    create_kit()
