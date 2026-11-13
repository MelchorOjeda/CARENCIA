# Claves de diálogo

Cada línea del guion unificado v1 (Kris, 9 de noviembre) ya está en su lugar dentro del juego con su **clave**. El texto de cada clave vive en `Assets/_Carencia/Resources/SO_Dialogo_Guion.asset`; una clave que todavía no tiene texto se ve entre corchetes, por ejemplo `[P1_mama]`, y avisa en la consola.

## Cómo funciona

- En las secuencias, cada línea es un paso `Dialogo` cuyo campo `texto` es la clave.
- La caja es un rectángulo negro translúcido al pie de la pantalla, con el nombre de quien habla arriba (en dorado) y el texto en Special Elite. El tamaño sigue a Opciones → Accesibilidad → Tamaño de texto.
- El texto se escribe letra por letra con el sonido `Audio/Efectos/SFX_Dialogo_Escritura`, que suena en loop solo mientras se escribe y sigue el volumen de Efectos.
- Enter, clic o E (también Esc): la primera pulsación completa la página; la siguiente pasa a la siguiente página o cierra la línea.
- Al terminar la línea, Elaki queda como estaba: si estaba sentada en la mesa, sigue sentada.

## Cómo editar un texto

Selecciona `SO_Dialogo_Guion` en el Project y, en el Inspector, busca la clave en la lista **Lineas**:

| Campo | Para qué |
|---|---|
| Clave | La misma del paso `Dialogo`. Si no coincide letra por letra, se verá `[clave]` |
| Quien | Nombre que aparece arriba del texto (Elaki, Mamá, Papá…). Vacío para textos sin voz, como el periódico |
| Texto | Lo que se escribe. Una **línea en blanco** separa páginas: úsala en textos de más de dos renglones. Vacío si el guion dice "Sin texto": el juego se salta la línea |
| Pensamiento | Marcado para lo que Elaki piensa y no dice: se ve en cursiva y con la etiqueta *pensamiento* junto al nombre |

Arriba de la lista están la fuente, el sonido y la velocidad (letras por segundo; 0 muestra cada página completa de golpe).

La prueba `ClavesDelGuionTests` (Test Runner → EditMode) falla si una clave de las escenas no tiene línea, si una clave se repite o si una página pasa de 120 letras.

## Decisiones al pasar el guion v1

Las 71 claves ya tienen el texto del guion unificado v1, incluidas las propuestas [P]. Cambios para revisar:

- Los nombres son **Mamá** y **Papá** (el guion alterna "Madre" y "Mamá"); la voz del pasillo es **Voz**; el anuncio, **Televisión**; el D3, **Noticiero**. El periódico, D4 y D5 van sin nombre.
- Todo lo que el guion marca como "Pensamiento" o comentario de Elaki al revisar un objeto va en cursiva.
- Ortografía: "¿Debería ir?" (N1.1), "Qué extraño… no se abre." (N1.1), "Cuautitlán Izcalli" e "Izcalli" (D3 y D4).
- N2.11 · Lugar vacío: se quitó la acotación "(Suspira.)"; el suspiro queda para el audio.
- `P4_lavabo_2` está vacío ("Sin texto, solo agua"): el juego se salta la línea.
- Los textos largos (periódicos, anuncio, noticiero, D4) están divididos en páginas.
- `R_puerta_cerrada` (puerta cerrada de la casa real en PRO_Casa, R1_Casa y R2_Casa, secuencia `Sec_puerta_cerrada`): no viene en el guion v1. Por ahora es el pensamiento "Está cerrada.", una propuesta para revisar con el equipo.

Al agregar un paso `Dialogo` con una clave nueva en una escena, agrega también su línea en `SO_Dialogo_Guion` y corre `ClavesDelGuionTests`: si falta, el juego muestra `[clave]`.
- Los objetos que se revisan más de una vez usan dos claves: `_1` para la primera vez y `_2` para las siguientes. Si el guion dice "Se repite", se usa una sola clave sin número.
- Los objetos marcados como **obligatorio** avanzan la historia cuando se revisan todos.

## Prólogo (`PRO_Casa`)

| Clave | Momento del guion | Quién |
|---|---|---|
| `P1_mama` | P1 · Toc, toc, toc y el llamado a comer | Mamá, fuera de cuadro |
| `P2_periodico` | P2 · Periódico del padre (opcional, se repite) | Acercamiento |
| `P2_elaki_1` | P2 · Primera pregunta por M-M-A-A-R (al revisar el lugar vacío) | Elaki |
| `P2_elaki_2` | P2 · Segunda pregunta | Elaki |
| `P2_madre_1` | P2 · Respuesta de la madre | Madre |
| `P2_padre_1` | P2 · Respuesta del padre sin bajar el periódico | Padre |
| `P2_elaki_3` | P2 · "Pero… ¿cómo?…" | Elaki |
| `P3_pensamiento` | P3 · Mareo | Pensamiento |
| `P4_lavabo_1` / `P4_lavabo_2` | P4 · Lavabo | Pensamiento / sin texto |
| `P4_toallas_1` / `P4_toallas_2` | P4 · Toallas | Pensamiento |
| `P4_cortina` | P4 · Cortina (se repite) | Pensamiento |
| `P6_pensamiento_1` / `P6_pensamiento_2` | P6 · Portal en el espejo | Pensamiento |

### Gabinete (P5)

Al hacer clic en un objeto de la vista del gabinete, su pensamiento sale en la misma caja de diálogo, siempre abajo y centrada como en el resto del juego. La clave es `P5_` más el **nombre del objeto** en la vista: si alguien lo renombra, la prueba `TextosDeVistasTests` avisa. Los mismos textos sirven en el baño real del Prólogo, R1 y R2. En `N1_Laberinto` la vista tiene **Mostrar Textos** apagado, porque ahí las etiquetas son símbolos ilegibles.

| Clave | Objeto | Guion P5 |
|---|---|---|
| `P5_SertralinaCajas` | Cajas de sertralina | Depresión |
| `P5_SertralinaFrasco` | Frasco de sertralina | Ansiedad |
| `P5_ClorpromazinaBote` | Bote chico de clorpromazina | Para dormir |
| `P5_ClorpromazinaFrasco` | Frasco grande de clorpromazina | Esquizofrenia |
| `P5_Risperidona` | Risperidona | Esquizofrenia |
| `P5_Blister` | Blíster de naproxeno y paracetamol | Naproxeno y paracetamol |
| `P5_Pads` | Parches juveniles | Parches juveniles |
| `P5_Rastrillo` | Rastrillo | Sin texto |

### Refrigerador del Prólogo (REF)

Misma mecánica que el gabinete, en la vista `PF_VistaRefrigerador`: al pasar el mouse el objeto se marca con el contorno amarillo y al hacer clic sale el pensamiento de Elaki. La clave es `REF_` más el nombre del objeto. Tono apático.

| Clave | Objeto | Texto |
|---|---|---|
| `REF_Frijoles` | Olla azul | La olla con frijoles de hace dos semanas. Mejor ni la destapo. |
| `REF_AguaJamaica` | Jarra roja | Agua de jamaica. Ya ni sabe a nada. |
| `REF_Arroz` | Tupper naranja | Arroz de… no sé de cuándo. |
| `REF_Huevos` | Huevos | Huevos. Tendría que cocinar. Mmm, no. |
| `REF_Pay` | Pay del estante | Un pedazo de pay. No tengo ganas. |
| `REF_Jitomates` | Jitomates del cajón | Esto mmm… no me gusta tanto. |
| `REF_Refresco` | Botella verde de la puerta | Refresco sin gas. Como todo. |

## Nivel 1 (`N1_Laberinto`, `N1_Pasillo`)

| Clave | Momento del guion |
|---|---|
| `N1_1_toallas_1` / `N1_1_toallas_2` | N1.1 · Toallas |
| `N1_1_gabinete_1` / `N1_1_gabinete_2` | N1.1 · Gabinete con símbolos (se abre con el espejo) |
| `N1_1_cortina_1` | N1.1 · Cortina: al abrirla aparece el pasaje |
| `N1_1_puerta` | N1.1 · Puerta que no se abre |
| `N1_2_calendario_1` / `N1_2_calendario_2` | N1.2 · Calendario del 31 |
| `N1_3_cama` | N1.3 · Cama del cuarto liminal |
| `N1_4_pared` | N1.4 · Pared de frente |
| `N1_4_sillas_1` / `N1_4_sillas_2` | N1.4 · Cuatro sillas sin mesa |
| `N1_4_ya_pase` | N1.4 · Tercera vuelta al cuarto de lavado (automático) |
| `N1_OBJ_<objeto>` | Nivel 1 · Pensamiento al revisar con E cada objeto regado (61 claves, una por tipo de objeto: `N1_OBJ_taza_cafe`, `N1_OBJ_lavadora`…). **Textos provisionales**, pendientes de revisión |
| `R_escritorio` / `R_television` | Casa real · Pensamiento al cerrar la vista del escritorio y de la tele. **Textos provisionales** |
| `N1_5_pensamiento` | N1.5 · "¿M-M-A-A-R?" al entrar al pasillo |
| `N1_6_ayuda` | N1.6 · Golpe y "Ayuda." frente a la puerta |

Sin clave por ahora: el espejo del Nivel 1 ("Ya es solo un espejo") abre el gabinete, y la segunda línea de la cortina no aplica porque la cortina se queda abierta.

## Segundo despertar (`R1_Casa`)

| Clave | Momento del guion |
|---|---|
| `N1_7_pensamiento` | N1.7 · Al despertar |
| `N1_8_periodico` | N1.8 · Periódico (sección de música, opcional) |
| `N1_8_madre_1` | N1.8 · Pregunta de la madre |
| `N1_8_elaki_1` | N1.8 · Respuesta de Elaki |
| `N1_8_elaki_piensa` | N1.8 · Pensamiento |
| `N2_1_pensamiento` | N2.1 · Decide volver al baño |
| `N2_2_toallas_1` / `N2_2_toallas_2` | N2.2 · Solo hay tres toallas |
| `N2_2_tina` | N2.2 · Cortina y tina (se repite) |
| `N2_2_gabinete` | N2.2 · Gabinete (con E) |

## Nivel 2 (`N2_Casa`)

| Clave | Momento del guion |
|---|---|
| `N2_3_toallas` | N2.3 · Toallas tiesas |
| `N2_3_gabinete` | N2.3 · Gabinete (con E) |
| `N2_3_calendario` | N2.3 · Calendario junto a la puerta |
| `N2_4_foto_1` / `N2_4_foto_2` | N2.4 · Fotografía familiar |
| `N2_6_platos_1` / `N2_6_platos_2` | N2.6 · Cuatro platos |
| `N2_8_pensamiento` | N2.8 · "No puede ser…" al terminar el video |
| `N2_9_pensamiento` | N2.9 · "¿Eres tú?" al terminar el giro |

## Tercer despertar (`R2_Casa`)

| Clave | Momento del guion | |
|---|---|---|
| `N2_10_pensamiento` | N2.10 · Al despertar | |
| `N2_11_lugar` | N2.11 · Lugar vacío | Obligatorio |
| `N2_11_madre` | N2.11 · Madre | Obligatorio |
| `N2_11_comida` | N2.11 · Comida | Obligatorio |
| `N2_11_periodico` | N2.11 · Periódico | Obligatorio |
| `N2_11_final` | N2.11 · "No voy a dejar de buscarla." al revisar los cuatro | |
| `N2_12_alacena_1` / `N2_12_alacena_2` | N2.12 · Alacena | |
| `N2_12_cocina` | N2.12 · Si intenta ir a la cocina | |
| `N3_1_toallas` | N3.1 · Siguen siendo cuatro | |
| `N3_1_tina` | N3.1 · Cortina y tina | |
| `N3_1_gabinete_1` / `N3_1_gabinete_2` | N3.1 · Los parches ya no están | |

El sollozo frente al altar (N2.12) no lleva texto.

## Nivel 3 (`N3_Casa`)

| Clave | Momento del guion |
|---|---|
| `N3_2_toallas` | N3.2 · Toallas de piel |
| `N3_2_gabinete` | N3.2 · Frascos llenos de dientes (con E) |
| `N3_5_platos` | N3.5 · Cuatro platos con órganos |
| `N3_5_sillon` | N3.5 · Al sentarse en el sillón |
| `N3_5_anuncio` | N3.5 · (sin uso: el anuncio del SITATYR ahora solo se oye, `SFX_N3_AnuncioTele`, y al terminar la tele se apaga) |
| `N3_3_parpadeo` | N3.3 · Al salir del baño la luz parpadea y aparece en la cocina |
| `N3_3_cocina` | N3.3 · Apéndices en las paredes de la cocina |
| `N3_3_sellada` | N3.3 · Puerta de la cocina sellada con carne |
| `N3_4_refri` | N3.4 · El refrigerador: carne que late; se abre la membrana de la puerta |
| `N3_5_cuarto` | N3.5 · Llega a su cuarto deforme |
| `N3_5_cama` | N3.5 · (sin uso: se quitaron las manos bajo la cama) |
| `N3_5_libre` | N3.5 · (sin uso) |
| `N3_6_ventana` | N3.6 · El pasillo: apéndices que lo cruzan; hay que esperar a que se escondan |
| `N3_7_ojos` | N3.7 · (sin uso: se quitaron los ojos del pasillo) |
| `N3_8_pensamiento` | N3.8 · "Te encontré." |

## Desenlace (`DES_Final`)

| Clave | Momento del guion |
|---|---|
| `D3_noticiero` | D3 · Voz del noticiero del 25 de diciembre |
| `D4_texto` | D4 · Pantalla de texto sobre el joven ahogado |
| `D5_cierre` | D5 · Cierre: el mismo día en que la encontraron |

Las imágenes del desenlace, la vista de la barda y el hombre armado son marcadores con su descripción hasta que llegue su arte o su video.
