import os
from PIL import Image, ImageDraw

def create_gabinete_sprites():
    out_dir = r"Assets\_Carencia\Sprites\Objetos\Decoracion"
    os.makedirs(out_dir, exist_ok=True)

    C_TRANS = (0, 0, 0, 0)
    C_OUTLINE = (28, 22, 34, 255)
    C_OUTLINE_SOFT = (58, 44, 62, 255)

    # Azulejos pared del baño
    C_TILE_BG = (175, 190, 205, 255)
    C_TILE_LIGHT = (195, 210, 225, 255)
    C_TILE_SEAM = (140, 155, 175, 255)

    # Gabinete (Esmalte metálico blanco / crema de baño retro)
    C_CAB_OUTER = (235, 235, 240, 255)
    C_CAB_SHADOW = (180, 185, 195, 255)
    C_CAB_DARK = (130, 135, 145, 255)
    C_CAB_INSIDE = (215, 220, 228, 255)
    C_CAB_INNER_SHADOW = (165, 170, 180, 255)

    # Espejo puerta abierta (lado izquierdo)
    C_MIRROR_GLASS = (200, 225, 245, 255)
    C_MIRROR_LIGHT = (235, 245, 255, 255)
    C_MIRROR_DARK = (150, 175, 200, 255)
    C_METAL_HINGE = (210, 195, 140, 255)

    # Medicamentos
    # Sertralina (cajas blancas con azul)
    C_SERT_BOX = (245, 245, 250, 255)
    C_SERT_BLUE = (60, 110, 185, 255)
    C_SERT_SHADOW = (190, 195, 210, 255)

    # Clorpromazina (frasco boticario ámbar y frasco alto)
    C_CLOR_AMBER = (170, 105, 45, 255)
    C_CLOR_AMBER_L = (205, 140, 75, 255)
    C_CLOR_AMBER_D = (115, 65, 25, 255)
    C_CLOR_WHITE_JAR = (225, 225, 230, 255)
    C_CLOR_LABEL = (245, 240, 225, 255)

    # Risperidona (frasco blanco con tapa roja + bola de algodón)
    C_RISP_BOTTLE = (235, 235, 240, 255)
    C_RISP_CAP = (195, 50, 45, 255)
    C_RISP_LABEL = (250, 250, 250, 255)
    C_COTTON_WHITE = (240, 245, 250, 255)
    C_COTTON_SHADOW = (185, 195, 205, 255)

    # Naproxeno / Paracetamol (caja verde/blanca + blister plateado con pastillas)
    C_NAPROX_BOX = (230, 240, 230, 255)
    C_NAPROX_GREEN = (50, 140, 90, 255)
    C_BLISTER_FOIL = (215, 225, 235, 255)
    C_BLISTER_SHADOW = (160, 175, 190, 255)
    C_PILL_DOT = (250, 250, 255, 255)

    # Higiene / Personal
    # Pads desmaquillantes (cilindro horizontal con etiqueta rosa/menta)
    C_PADS_CYL = (235, 235, 240, 255)
    C_PADS_LABEL = (240, 190, 205, 255)
    C_PADS_PRINT = (75, 130, 100, 255)
    C_PADS_SHADOW = (175, 180, 190, 255)

    # Rastrillo de afeitar (mango estriado celeste/plástico, cabezal angular con cuchilla)
    C_RAZOR_HANDLE = (90, 170, 190, 255)
    C_RAZOR_HANDLE_L = (140, 210, 225, 255)
    C_RAZOR_HEAD = (70, 140, 160, 255)
    C_RAZOR_BLADE = (230, 235, 245, 255)
    C_RAZOR_SHADOW = (45, 95, 110, 255)

    # =============================================================
    # 1. PANTALLA COMPLETA DE INSPECCIÓN (320 x 180 px)
    # =============================================================
    img_scene = Image.new("RGBA", (320, 180), C_TILE_BG)
    draw_s = ImageDraw.Draw(img_scene)

    # Azulejos del baño en la pared de fondo (rejilla de 16x16 px)
    for x in range(0, 320, 16):
        draw_s.line([(x, 0), (x, 180)], fill=C_TILE_SEAM, width=1)
    for y in range(0, 180, 16):
        draw_s.line([(0, y), (320, y)], fill=C_TILE_SEAM, width=1)

    # Sombra oscura proyectada por el gabinete en la pared
    draw_s.rectangle([68, 14, 254, 168], fill=(120, 135, 150, 180))

    # Puerta con espejo abierta hacia la izquierda:
    # Marco de la puerta de x=18 a x=68, y=16 a y=164 (perspectiva de puerta abatida a la izq)
    poly_door = [(18, 22), (68, 16), (68, 164), (18, 158)]
    draw_s.polygon(poly_door, fill=C_CAB_OUTER, outline=C_OUTLINE)
    poly_mirror = [(22, 26), (64, 20), (64, 160), (22, 154)]
    draw_s.polygon(poly_mirror, fill=C_MIRROR_GLASS, outline=C_OUTLINE_SOFT)

    # Reflejo diagonal sutil en el espejo
    draw_s.polygon([(26, 30), (45, 27), (55, 145), (36, 148)], fill=C_MIRROR_LIGHT)

    # Bisagras metálicas (3 bisagras a la izquierda del marco)
    for hy in [28, 88, 148]:
        draw_s.rectangle([65, hy, 70, hy + 8], fill=C_METAL_HINGE, outline=C_OUTLINE)
        draw_s.point([(67, hy + 2), (67, hy + 5)], fill=C_OUTLINE)

    # -------------------------------------------------------------
    # Gabinete principal (Caja vista 100% de frente)
    # x: 70 a 250 (180 px de ancho)
    # y: 16 a 164 (148 px de alto)
    # -------------------------------------------------------------
    cab_x0, cab_y0, cab_x1, cab_y1 = 70, 16, 250, 164
    border_thick = 5

    # Marco exterior blanco
    draw_s.rectangle([cab_x0, cab_y0, cab_x1, cab_y1], fill=C_CAB_OUTER, outline=C_OUTLINE)
    draw_s.rectangle([cab_x0 + 1, cab_y0 + 1, cab_x1 - 1, cab_y1 - 1], outline=C_CAB_SHADOW)

    # Fondo interior del gabinete (ligeramente más oscuro con sombras en las esquinas)
    in_x0, in_y0, in_x1, in_y1 = cab_x0 + border_thick, cab_y0 + border_thick, cab_x1 - border_thick, cab_y1 - border_thick
    draw_s.rectangle([in_x0, in_y0, in_x1, in_y1], fill=C_CAB_INSIDE, outline=C_OUTLINE)

    # Sombra interna superior y lateral
    draw_s.rectangle([in_x0, in_y0, in_x1, in_y0 + 4], fill=C_CAB_INNER_SHADOW)
    draw_s.rectangle([in_x0, in_y0, in_x0 + 4, in_y1], fill=C_CAB_INNER_SHADOW)

    # -------------------------------------------------------------
    # Estantes y divisiones internas:
    # Altura útil interior = 138 px
    # - Estante 1 (superior): y = 21 a 63 (42 px)
    # - Balda 1: y = 64 a 68 (5 px de grosor)
    # - Estante 2 (medio): y = 69 a 111 (42 px)
    # - Balda 2: y = 112 a 116 (5 px de grosor)
    # - Estante 3 (inferior, ancho corrido): y = 117 a 159 (42 px)
    #
    # Columna central divisoria (para estantes 1 y 2): x = 157 a 162
    # -------------------------------------------------------------
    shelf1_y = 64
    shelf2_y = 112
    col_x = (in_x0 + in_x1) // 2  # ~160

    # Balda 1
    draw_s.rectangle([in_x0, shelf1_y, in_x1, shelf1_y + 4], fill=C_CAB_OUTER, outline=C_OUTLINE)
    draw_s.line([(in_x0, shelf1_y + 4), (in_x1, shelf1_y + 4)], fill=C_CAB_DARK)

    # Balda 2
    draw_s.rectangle([in_x0, shelf2_y, in_x1, shelf2_y + 4], fill=C_CAB_OUTER, outline=C_OUTLINE)
    draw_s.line([(in_x0, shelf2_y + 4), (in_x1, shelf2_y + 4)], fill=C_CAB_DARK)

    # Partición vertical central (solo en estantes superior y medio)
    draw_s.rectangle([col_x - 2, in_y0, col_x + 2, shelf2_y], fill=C_CAB_OUTER, outline=C_OUTLINE)
    draw_s.line([(col_x + 2, in_y0), (col_x + 2, shelf2_y)], fill=C_CAB_DARK)

    # =============================================================
    # CONTENIDO DE CADA COMPARTIMENTO
    # =============================================================

    # -------------------------------------------------------------
    # 1. ESTANTE SUPERIOR IZQUIERDO: SERTRALINA
    # -------------------------------------------------------------
    # Caja vertical (izq): x=82..98, y=28..63 (16 px ancho, 35 px alto)
    draw_s.rectangle([82, 28, 98, 63], fill=C_SERT_BOX, outline=C_OUTLINE)
    draw_s.rectangle([83, 30, 97, 36], fill=C_SERT_BLUE)  # Franja azul superior
    draw_s.rectangle([83, 48, 97, 50], fill=C_SERT_BLUE)  # Franja fina
    # Texto representativo "Sertralina"
    for tx in [85, 87, 89, 91, 93, 95]:
        draw_s.point([(tx, 40), (tx, 42)], fill=C_OUTLINE_SOFT)

    # Caja inclinada detrás (centro):
    poly_box2 = [(103, 33), (120, 36), (117, 63), (100, 63)]
    draw_s.polygon(poly_box2, fill=C_SERT_SHADOW, outline=C_OUTLINE)
    draw_s.line([(103, 42), (119, 45)], fill=C_SERT_BLUE, width=2)

    # Dos cajas acostadas/apiladas (der): x=122..153
    # Caja abajo: y=53..63
    draw_s.rectangle([122, 53, 153, 63], fill=C_SERT_BOX, outline=C_OUTLINE)
    draw_s.rectangle([123, 55, 128, 61], fill=C_SERT_BLUE)
    draw_s.line([(131, 57), (148, 57)], fill=C_OUTLINE_SOFT)
    # Caja arriba: y=42..52
    draw_s.rectangle([124, 42, 154, 52], fill=C_SERT_BOX, outline=C_OUTLINE)
    draw_s.rectangle([125, 44, 130, 50], fill=C_SERT_BLUE)
    draw_s.line([(133, 46), (150, 46)], fill=C_OUTLINE_SOFT)

    # -------------------------------------------------------------
    # 2. ESTANTE SUPERIOR DERECHO: CLORPROMAZINA
    # -------------------------------------------------------------
    # Frasco de vidrio ámbar boticario (izq): x=172..196, y=28..63
    # Tapa de vidrio/rosca
    draw_s.rectangle([178, 28, 190, 33], fill=C_CLOR_AMBER_L, outline=C_OUTLINE)
    # Cuello frasco
    draw_s.rectangle([181, 34, 187, 37], fill=C_CLOR_AMBER_D, outline=C_OUTLINE)
    # Cuerpo redondeado
    draw_s.rectangle([173, 38, 195, 63], fill=C_CLOR_AMBER, outline=C_OUTLINE)
    # Reflejo luz vidrio
    draw_s.line([(175, 40), (175, 60)], fill=C_CLOR_AMBER_L)
    # Etiqueta cuadrada con texto
    draw_s.rectangle([176, 43, 192, 57], fill=C_CLOR_LABEL, outline=C_OUTLINE_SOFT)
    draw_s.line([(178, 47), (190, 47)], fill=(120, 40, 30, 255))
    draw_s.line([(178, 51), (188, 51)], fill=C_OUTLINE_SOFT)

    # Frasco cilíndrico blanco alto (der): x=208..234, y=24..63
    draw_s.rectangle([211, 24, 231, 30], fill=C_CLOR_WHITE_JAR, outline=C_OUTLINE) # Tapa
    draw_s.rectangle([209, 31, 233, 63], fill=C_CLOR_WHITE_JAR, outline=C_OUTLINE) # Cuerpo
    draw_s.line([(211, 33), (211, 61)], fill=(255, 255, 255, 255), width=2) # Brillo
    draw_s.line([(231, 33), (231, 61)], fill=C_CAB_SHADOW, width=2) # Sombra
    # Etiqueta
    draw_s.rectangle([213, 38, 230, 56], fill=(245, 245, 235, 255), outline=C_OUTLINE_SOFT)
    draw_s.line([(215, 43), (227, 43)], fill=(50, 60, 80, 255))
    draw_s.line([(215, 48), (225, 48)], fill=C_OUTLINE_SOFT)

    # -------------------------------------------------------------
    # 3. ESTANTE MEDIO IZQUIERDO: RISPERIDONA
    # -------------------------------------------------------------
    # Frasco cilíndrico de Risperidona (izq): x=82..108, y=74..111
    # Tapa acanalada de seguridad roja/naranja
    draw_s.rectangle([86, 74, 104, 82], fill=C_RISP_CAP, outline=C_OUTLINE)
    for rx in range(88, 104, 3):
        draw_s.line([(rx, 75), (rx, 81)], fill=(240, 110, 100, 255))
    # Cuerpo del frasco
    draw_s.rectangle([84, 83, 106, 111], fill=C_RISP_BOTTLE, outline=C_OUTLINE)
    # Etiqueta grande
    draw_s.rectangle([86, 88, 104, 105], fill=C_RISP_LABEL, outline=C_OUTLINE_SOFT)
    draw_s.line([(88, 92), (102, 92)], fill=(200, 40, 40, 255), width=2) # Franja roja
    draw_s.line([(88, 96), (100, 96)], fill=C_OUTLINE)                    # "RISPERIDONA"
    draw_s.line([(88, 100), (96, 100)], fill=C_OUTLINE_SOFT)

    # Bola / bolsa de algodón medicinal (der): x=116..148, y=86..111
    draw_s.ellipse([118, 86, 146, 111], fill=C_COTTON_WHITE, outline=C_OUTLINE)
    draw_s.arc([122, 90, 138, 105], start=45, end=225, fill=C_COTTON_SHADOW, width=2)
    draw_s.arc([130, 94, 144, 108], start=10, end=180, fill=C_COTTON_SHADOW, width=1)

    # -------------------------------------------------------------
    # 4. ESTANTE MEDIO DERECHO: NAPROXENO / PARACETAMOL
    # -------------------------------------------------------------
    # Caja de analgésicos de fondo: x=170..202, y=74..111
    draw_s.rectangle([172, 75, 202, 111], fill=C_NAPROX_BOX, outline=C_OUTLINE)
    draw_s.rectangle([173, 76, 201, 84], fill=C_NAPROX_GREEN) # Banda verde
    draw_s.line([(175, 90), (198, 90)], fill=C_OUTLINE)
    draw_s.line([(175, 95), (192, 95)], fill=C_OUTLINE_SOFT)

    # Blíster de pastillas inclinado en primer plano: x=194..242, y=78..111
    poly_blister = [(202, 80), (242, 88), (230, 111), (190, 103)]
    draw_s.polygon(poly_blister, fill=C_BLISTER_FOIL, outline=C_OUTLINE)

    # Alvéolos/burbujas redondas del blíster con pastillas blancas dentro
    pills = [
        (205, 87), (216, 90), (227, 93),
        (201, 96), (212, 99), (223, 102)
    ]
    for px, py in pills:
        draw_s.ellipse([px - 3, py - 3, px + 3, py + 3], fill=C_PILL_DOT, outline=C_BLISTER_SHADOW)
        draw_s.point([(px - 1, py - 1)], fill=(255, 255, 255, 255))

    # -------------------------------------------------------------
    # 5. ESTANTE INFERIOR (ANCHO CORRIDO): HIGIENE Y RASTRILLO
    # -------------------------------------------------------------
    # A) Tubo de Pads desmaquillantes acostado (izq y centro):
    # Cilindro horizontal de x=82 a x=176, y=128 a y=156 (ancho 94 px, alto 28 px)
    draw_s.rectangle([88, 128, 168, 156], fill=C_PADS_CYL, outline=C_OUTLINE)
    # Tapa / base derecha redonda
    draw_s.ellipse([162, 128, 174, 156], fill=(245, 245, 250, 255), outline=C_OUTLINE)
    # Textura de discos de algodón apilados visibles en el extremo
    for dx in range(165, 172, 2):
        draw_s.line([(dx, 134), (dx, 150)], fill=C_CAB_SHADOW)

    # Etiqueta decorativa sobre el tubo (franja rosa con estampado botánico)
    draw_s.rectangle([96, 130, 152, 154], fill=C_PADS_LABEL, outline=C_OUTLINE_SOFT)
    # Motivo de hojitas verdes
    draw_s.line([(102, 142), (116, 142)], fill=C_PADS_PRINT, width=1)
    draw_s.point([(106, 138), (112, 138), (109, 146)], fill=C_PADS_PRINT)
    # Texto "COTTON / PADS"
    draw_s.line([(122, 137), (146, 137)], fill=C_OUTLINE_SOFT)
    draw_s.line([(122, 145), (140, 145)], fill=C_OUTLINE_SOFT)

    # Sombra del cilindro
    draw_s.line([(88, 155), (168, 155)], fill=C_CAB_DARK)

    # B) Rastrillo de afeitar (inclinado de centro-izq hacia arriba-der):
    # Mango: de (140, 145) a (205, 122)
    # Cabezal de afeitar angular en (205, 122): ancho 24 px
    # Dibujar mango primero
    draw_s.line([(140, 145), (205, 122)], fill=C_OUTLINE, width=6)
    draw_s.line([(140, 145), (205, 122)], fill=C_RAZOR_HANDLE, width=4)
    draw_s.line([(140, 144), (205, 121)], fill=C_RAZOR_HANDLE_L, width=1) # Brillo
    # Estrías del mango
    for step in range(5, 55, 6):
        gx = 142 + int(step * 0.94)
        gy = 144 - int(step * 0.33)
        draw_s.point([(gx, gy)], fill=C_OUTLINE_SOFT)

    # Cabezal del rastrillo (rectángulo girado ~30 grados)
    poly_head = [(198, 114), (222, 122), (218, 134), (194, 126)]
    draw_s.polygon(poly_head, fill=C_RAZOR_HEAD, outline=C_OUTLINE)
    # Cuchilla metálica afilada
    draw_s.polygon([(200, 117), (220, 124), (218, 128), (198, 121)], fill=C_RAZOR_BLADE, outline=C_OUTLINE_SOFT)
    draw_s.line([(201, 118), (219, 124)], fill=(255, 255, 255, 255), width=1) # Destello de filo

    # C) Sobre de hojas de navaja / repuestos a la derecha: x=220..242, y=148..157
    poly_blade = [(222, 150), (242, 151), (239, 157), (219, 156)]
    draw_s.polygon(poly_blade, fill=(240, 240, 245, 255), outline=C_OUTLINE)
    draw_s.line([(224, 153), (237, 153)], fill=(180, 50, 50, 255)) # Marca roja

    # Guardar la pantalla completa de inspección
    scene_path = os.path.join(out_dir, "gabinete_inspeccion_pantalla.png")
    img_scene.save(scene_path)
    print("Creado gabinete_inspeccion_pantalla.png (320x180)")

    # =============================================================
    # 2. SOLO EL GABINETE CON PUERTA (Con fondo transparente)
    # Para usar directamente como SpriteRenderer o UI Image
    # =============================================================
    img_gab = Image.new("RGBA", (240, 160), C_TRANS)
    # Copiar desde img_scene la región del gabinete y la puerta
    # x: 14 a 254 (ancho 240 px)
    # y: 10 a 170 (alto 160 px)
    crop_gab = img_scene.crop((14, 10, 254, 170))
    # Para aislarlo con canal alfa, creamos la máscara del gabinete
    mask_gab = Image.new("L", (240, 160), 0)
    draw_m = ImageDraw.Draw(mask_gab)
    # Máscara puerta:
    draw_m.polygon([(18 - 14, 22 - 10), (68 - 14, 16 - 10), (68 - 14, 164 - 10), (18 - 14, 158 - 10)], fill=255)
    # Máscara caja:
    draw_m.rectangle([cab_x0 - 14, cab_y0 - 10, cab_x1 - 14, cab_y1 - 10], fill=255)
    img_gab.paste(crop_gab, (0, 0), mask_gab)
    gab_path = os.path.join(out_dir, "gabinete_solo.png")
    img_gab.save(gab_path)
    print("Creado gabinete_solo.png (240x160)")

if __name__ == "__main__":
    create_gabinete_sprites()

