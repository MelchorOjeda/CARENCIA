# Claves de diálogo

Cada línea del guion unificado v1 (Kris, 9 de noviembre) ya está en su lugar dentro del juego, pero sin texto: se ve un cuadro en blanco con su **clave** en la esquina inferior derecha. El equipo de diálogos solo tiene que asignar el texto a cada clave.

## Cómo funciona

- En las secuencias, cada línea es un paso `Dialogo` cuyo campo `texto` es la clave. Avanza con E, Esc o clic.
- Al terminar la línea, Elaki queda como estaba: si estaba sentada en la mesa, sigue sentada.
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

Los textos de cada frasco del gabinete (P5) siguen en la vista del gabinete, como antes.

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
| `N3_5_anuncio` | N3.5 · Voz del anuncio del SITATYR |
| `N3_8_pensamiento` | N3.8 · "Te encontré." |

## Desenlace (`DES_Final`)

| Clave | Momento del guion |
|---|---|
| `D3_noticiero` | D3 · Voz del noticiero del 25 de diciembre |
| `D4_texto` | D4 · Pantalla de texto sobre el joven ahogado |
| `D5_cierre` | D5 · Cierre: el mismo día en que la encontraron |

Las imágenes del desenlace, la vista de la barda, el hombre armado y el anuncio de la TV son marcadores con su descripción hasta que llegue su arte o su video.
