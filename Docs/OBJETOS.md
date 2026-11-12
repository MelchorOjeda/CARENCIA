# Colocación de los objetos de la colección de arte

Inventario de `Assets/_Carencia/Sprites/Objetos` (objetos de Zuri) y propuesta de dónde va cada uno. Casi todos vienen en tres estados con el mismo nombre, así que un mismo objeto sirve para toda la historia:

| Estado del sprite | Dónde se usa |
|---|---|
| `normal` | Casa real: prólogo, 2.º y 3.er despertar (`PRO_Casa`, `R1_Casa`, `R2_Casa`) |
| `sucio_oscuro` (en la bañera, `sucia_oscura`) | Nivel 2, casa abandonada (`N2_Casa`) |
| `agobiante` | Nivel 3, casa de carne (`N3_Casa`) |
| `normal` medio transparente | Nivel 1, laberinto liminal (`N1_Laberinto`): pocos objetos, separados y fuera de lugar |

Orientación: `frente_derecha` va contra la pared trasera izquierda (mira hacia abajo a la derecha), `frente_izquierda` contra la pared trasera derecha, y `espalda_*` cuando el jugador lo ve por detrás (por ejemplo, el sillón que mira a la TV). `frente_recto` es una vista de frente, para acercamientos en primera persona.

**Estado:** ✅ ya colocado · 🔜 propuesto, falta colocarlo.

## Baño

| Objeto | Casa real | Nivel 2 / Nivel 3 | Momento del guion | Estado |
|---|---|---|---|---|
| `banera` (cortina cerrada, media, abierta y hoja de animación de 13 cuadros) | Tina con la cortina en estado 0, 1 y 2 según la visita | Estado 2 sucia; 2.5 y 3 (abierta) de carne | P4, N2.2, N2.3, N3.1, N3.2, N3.8 | ✅ |
| `lavamanos` | Lavabo | Sucio / de carne | P4 (agua en la cara) | ✅ |
| `retrete` | Taza | Sucio / de carne | N1.1 (inodoro amarillento) | ✅ |
| `gabinete` (puerta cerrada y abierta) | Espejo con gabinete: cerrado siempre, abierto mientras se ve la vista del gabinete | Sucio / de carne | P5, N1.1, N2.2, N2.3, N3.1, N3.2 | ✅ cerrado · 🔜 abierto al usarlo |
| `toallas_perchero` | Cuatro toallas de cuatro colores | Tiesas de mugre / de piel | P4, N2.2, N2.3, N3.1, N3.2 | ✅ (falta versión de 3 toallas) |
| `Gabinete/Productos` (18 productos: sertralina, clorpromazina, risperidona, naproxeno, parches, serum, jade…) | Contenido de la vista del gabinete en primera persona | Frascos vacíos (N2) y llenos de dientes (N3) | P5 y revisiones del gabinete | 🔜 reemplazar los dibujos actuales de la vista |
| `botella_pastillas_*` (3 colores, 4 niveles de llenado) | Repisa del gabinete; "unas cuantas" en la casa real | "Vacío" en el Nivel 2 | P5, N2.3 ("Están vacíos") | 🔜 |
| `cepillos_vaso`, `pasta_dental`, `jabon_dispensador` | Sobre el lavamanos | Sucios / de carne | Ambiente | 🔜 |
| `papel_bano` | Junto al retrete | Ídem | Ambiente | 🔜 |
| `cesto_ropa_sucia` | Esquina libre del baño, junto a la puerta | Ídem | Ambiente | 🔜 |
| `alcohol_etilico`, `kit_emergencia` | Repisa o pared del baño | Ídem | Ambiente | 🔜 |
| `espejo_ovalado`, `espejo_marco_dorado`, `espejo_rectangular` | Pasillo o cuarto de Elaki (el del baño es el gabinete) | Ídem | Ambiente | 🔜 |

## Cocina

| Objeto | Casa real | Nivel 2 / Nivel 3 | Momento del guion | Estado |
|---|---|---|---|---|
| `refrigerador` | Refrigerador de la cocina | Sucio / con ojos | Vista interior con E | ✅ |
| `refrigerador_abierto_frente_recto` | Vista en primera persona; en la casa real del Prólogo es `PF_VistaRefrigerador`, con la barra de la cocina y 7 objetos con contorno al pasar el mouse | Comida podrida / "cosas extrañas" | N3.5 (refrigerador) | ✅ niveles 2 y 3 · ✅ Prólogo · 🔜 R1 y R2 |
| `estufa` | Sobre la barra, en el lugar de la estufa dibujada | Sucia / de carne | Ambiente | ✅ |
| `lavaplatos` | Junto a la estufa, si se separa la barra | Sucio; en el Nivel 3, el "fregadero lleno de uñas" | N3.5 | ✅ Nivel 1 · 🔜 casa y niveles 2 y 3 |
| `dispensador_agua` | Junto al refrigerador | Ídem | Ambiente | ✅ |
| `platos_grupo`, `plato` | Sobre la barra; `plato` para los tres platos del desayuno | Cuatro platos con comida y moscas (N2) y con órganos (N3) | P2, N1.8, N2.6, N2.11, N3.5 | ✅ grupo · 🔜 platos de la mesa |
| `cazuela_barro`, `olla_*` (aluminio, barro, express, peltre, tamalera), `sarten` | Barra y estufa | Ídem | Ambiente | ✅ cazuela · 🔜 el resto |
| `taza_cafe`, `vaso`, `cuchara`, `cuchillo` | Mesa del desayuno (taza del padre) | Ídem | P2, N2.11 ("Esta comida no tiene sabor") | 🔜 |

## Comedor y sala

| Objeto | Casa real | Nivel 2 / Nivel 3 | Momento del guion | Estado |
|---|---|---|---|---|
| `mesa_comedor` | Mesa cuadrada del desayuno, con mantel | Sucia / de carne | P2, N1.8, N2.6, N2.11, N3.5 | 🔜 reemplazar la actual |
| `mesa_centro` | Mesa de centro de la sala | Ídem | Ambiente | 🔜 |
| `mesa_madera_sencilla` | Nivel 1: las cuatro sillas sin mesa la dejan fuera; o mesa de la cocina | Ídem | N1.4 | 🔜 |
| `sillon_sala`, `sillon_sala_cojines` (`espalda_*` mirando a la TV) | Sillón frente a la TV | Sucio / de piel con costuras | N3.5: al sentarse empieza el anuncio | ✅ niveles 1 y 3 · 🔜 casa real y Nivel 2 |
| `mueble_tv_con_television`, `television_antigua`, `mueble_tv` | TV de la sala | Con estática (N2), la que pasa el anuncio (N3) | N2.6, N3.5, D3 (noticiero) | 🔜 |
| `control_remoto` | Sobre el sillón o la mesa de centro | Ídem | Ambiente | 🔜 |
| `alfombra_persa`, `alfombra_sarape` | Bajo la mesa de centro y en la entrada | Ídem | Ambiente | 🔜 |
| `cojin`, `cojines_par` | Sobre el sillón | Ídem | Ambiente | 🔜 |
| `periodico_abierto`, `periodico_primera_plana`, `periodico_enrollado`, `periodico_monton` | Periódico del padre en cada desayuno (abierto) y montón junto al sillón | Montón sucio en la sala del Nivel 2 | P2, N1.8, N2.11 | 🔜 reemplazar el blockout |

## Escritorio y computadora (entrada de la sala)

| Objeto | Casa real | Nivel 2 / Nivel 3 | Momento del guion | Estado |
|---|---|---|---|---|
| `escritorio_completo`, `computadora_completa`, `monitor`, `pc_torre`, `teclado`, `mouse` | Computadora de la entrada | Encendida con el video | N2.7 (video y perro) | 🔜 reemplazar el escritorio actual |
| `silla_oficina`, `silla_gamer` | Silla de la computadora | Sucia | N2.7 (Elaki se sienta) | 🔜 |
| `escritorio_frontal_*` (13 variantes) | Vista en primera persona del escritorio del cuarto de Elaki | — | Vista con E del escritorio | 🔜 alternativa a la vista actual |
| `calendario_escritorio` | Escritorio del cuarto de Elaki | Ídem | Refuerza el 31 de octubre | 🔜 |
| `calendario_pared` | Junto a la puerta del baño del otro lado | Sucio / de carne (el de piel) | N1.2, N2.3, N3.2 | ✅ |
| `archivero` | Junto al escritorio de la computadora | Ídem | Ambiente | 🔜 |
| `libro_*`, `libros_apilados`, `libros_fila`, `libros_grupo` | Escritorio y librero del cuarto de Elaki | Ídem | Ambiente | 🔜 |

## Cuarto de Elaki

| Objeto | Casa real | Nivel 1 (cuarto liminal) | Momento del guion | Estado |
|---|---|---|---|---|
| `cama` | Las camas actuales ya tienen sus cuatro estados; esta sirve para el cuarto liminal | Cama sola, en medio del cuarto vacío | N1.3 ("Es mi cuarto… ¿o no?") | 🔜 cuarto liminal |
| `ropero` | Pared del cuarto | Ropero lejos de todo | P1, N1.3 | 🔜 |
| `despertador_campanas`, `despertador_radio` | Buró junto a la cama | — | P1, N1.7 (despertares) | 🔜 |
| `lampara_buro`, `lampara_lectura` | Buró y escritorio | — | Ambiente | 🔜 |
| `buro` (MueblesMadera) | Junto a la cama, con el vaso y la receta del 2.º despertar | — | N1.7 | 🔜 |

## Pasillo

| Objeto | Casa real | Nivel 2 / Nivel 3 | Momento del guion | Estado |
|---|---|---|---|---|
| `alacena_pared`, `alacena_comida`, `vitrina_antigua` | Alacena grande que tapa la puerta del cuarto de Margarita | Ídem | P1, N1.7, N2.12 (se abre: escoba y limpieza) | 🔜 reemplazar el armario actual |
| `mesita_cuadrada`, `mesita_redonda` | Mesa pequeña bajo la ventana del fondo | Ídem | Vacía en P1 y N1.7; con el altar en N2.12 | 🔜 |
| `veladora_apagada`, `velas_*`, `geranio_bote` | Altar del perro: veladoras consumidas y flores secas | — | N2.12 | 🔜 completar el altar |
| `ventana_con_cortina`, `ventana_sin_cortina` | Ventana del fondo con vista a edificios | — | P1 | 🔜 |
| `ventana_tapiada` | — | Tapiada después de ver la barda (N2) y al hombre armado (N3) | N2.5, N3.7 | ✅ |
| `puerta_adolescente` | Puerta del cuarto de Elaki (y la de Margarita en el Nivel 1) | Sucia / de carne | N1.5–N1.6 (la puerta del fondo) | 🔜 |
| `puerta` | Baño, entrada y demás puertas | Ídem; la del baño con el corazón deformado en el Nivel 3 | N3 | 🔜 |
| `foto_familia_sofa` | — | Fotografía de los cuatro (N2), marco de piel (N3) | N2.4 | ✅ |
| `foto_margarita` | Escondida: no hay fotos de ella en la casa real | Al fondo del pasillo del Nivel 2 | N2.4 | 🔜 |
| `foto_boda`, `foto_abuelos`, `foto_bebe`, `foto_graduacion`, `foto_hermanos`, `foto_salon_escolar` | Fotos del pasillo de la casa real (sin Margarita) | Versiones sucias en el Nivel 2 | P1, N2.4 | 🔜 |
| `foto_cabeza_glitch`, `foto_cara_ancha`, `foto_cara_embarrada`, `foto_ojos_negros`, `foto_ojo`, `foto_doble_exposicion`, `foto_dobles`, `foto_mujer_cabello`, `foto_nina_velo`, `foto_senor_sonrisa`, `foto_victoriana`, `foto_manos` | — | Cuadros deformes "a la izquierda, hacia la sala" del Nivel 2 y marcos de piel del Nivel 3 | N2.4, N3.3 | ✅ dos · 🔜 el resto |
| `foto_cuarto_vacio`, `foto_pasillo`, `foto_polaroid_flash`, `foto_polaroid_puerta` | — | Cuadros del laberinto (Nivel 1) y del pasillo del Nivel 2 | N1.3, N1.4 | 🔜 |

Las fotos vienen solo de frente (`frente_recto`). Se usan como acercamiento en primera persona al revisar un cuadro de la pared; en la pared queda el marco inclinado que ya existe.

## Generales (cualquier cuarto)

| Objeto | Dónde | Nivel 2 / Nivel 3 | Estado |
|---|---|---|---|
| `caja_chica`, `caja_mediana`, `caja_grande`, `cajas_apiladas`, `cajas_monton` | Casa abandonada del Nivel 2 (cuartos llenos de cajas); en la casa real, encima del ropero | Muchas en el Nivel 2 | 🔜 |
| `planta_espada`, `potus_colgante`, `sabila`, `palma`, `nopal`, `geranio_bote` | Sala, comedor y pasillo de la casa real | Secas (N2) / de carne (N3) | 🔜 |
| `enchufe_apagador`, `enchufe_contacto` | Paredes de todos los cuartos | Ídem | 🔜 |
| `foco_encendido`, `foco_apagado`, `foco_ahorrador` | Techo del baño ("luz blanca cálida de foco", P4) y pasillo | Apagados en el Nivel 2 | 🔜 |
| `llave`, `llaves_par` | Mesita de la entrada | Ídem | 🔜 |
| `librero_con_libros`, `librero_vacio`, `estante_pared`, `comoda` | Sala y cuarto de Elaki; `librero_vacio` en el Nivel 1 | Ídem | 🔜 |

## Lo que falta en la colección para el guion

- Toallero con **tres** toallas (N2.2: "Solo hay tres").
- Platos con comida podrida y con órganos, y el "fregadero lleno de uñas" (se puede resolver con `lavaplatos_agobiante`).
- Marcos con piel estirada y cabello saliendo de los cajones (Nivel 3).
- La barda con las mujeres y con el hombre armado, los carteles de búsqueda, el noticiero y el edificio (vistas y desenlace).
