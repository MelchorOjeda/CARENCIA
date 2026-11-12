# Gestión de Cambios

## Descripción de la sección

El diseño de un videojuego cambia. Esta sección evita que las modificaciones importantes se pierdan o aparezcan sin evaluar sus consecuencias.

## Solicitud de cambio
### SC-01 — Guion unificado

**Fecha:** 3 de noviembre de 2026

**Cambio propuesto:** Unificar la historia de Carencia en un solo guion que define el orden definitivo del prólogo, los tres niveles y el desenlace.

**Motivo:** El equipo trabajaba con versiones distintas de la historia, con diferencias en el cierre de cada nivel y en el desenlace.

**Beneficio esperado:** Una sola referencia para el diseño, el arte, el audio y la programación, y una progresión narrativa coherente entre los niveles.

**Costo estimado:** Ajustar las secuencias narrativas, los textos de diálogos y pensamientos, y los recursos visuales y sonoros de las escenas afectadas.

**Impacto en otros sistemas:** Secuencias narrativas, diálogos y textos, arte de escenarios y audio.

**Decisión:** Aprobado.

**Responsable:** Wendy Kristal Palma Góngora

### SC-02 — Inicio de cada nivel en el baño real

**Fecha:** 3 de noviembre de 2026

**Cambio propuesto:** Cada nivel inicia en el baño de la casa real y se accede a la versión alterada cruzando el espejo.

**Motivo:** Mantener la comparación constante entre el baño real y sus versiones alteradas a lo largo de toda la historia.

**Beneficio esperado:** El jugador reconoce en cada visita los cambios del baño real, como el número de toallas, el estado de la cortina y el agua de la tina, que preparan la revelación final.

**Costo estimado:** Ajustar el inicio de cada nivel, los estados del baño real en cada visita y los portales del espejo.

**Impacto en otros sistemas:** Secuencias narrativas, portales, arte del baño y progreso.

**Decisión:** Aprobado.

**Responsable:** Ideny López Rodríguez

### SC-03 — Muerte limitada a las persecuciones del Nivel 2

**Fecha:** 3 de noviembre de 2026

**Cambio propuesto:** La muerte solo es posible durante la persecución de la criatura canina y el encuentro con la Manifestación de Margarita. El encuentro con la Manifestación de Margarita no tiene escapatoria y funciona como transición hacia el siguiente despertar.

**Motivo:** Concentrar el peligro en los momentos clave del Nivel 2 y presentar el encuentro con la Manifestación de Margarita como un momento narrativo inevitable.

**Beneficio esperado:** El Nivel 3 se centra en la exploración y la atmósfera, y el encuentro con la Manifestación de Margarita refuerza la contradicción de huir de la persona que se busca.

**Costo estimado:** Retirar el daño de los elementos del Nivel 3 y ajustar el comportamiento y la conclusión del encuentro con la Manifestación de Margarita.

**Impacto en otros sistemas:** Persecuciones, daño visual, derrota y reintento, secuencias narrativas.

**Decisión:** Aprobado.

**Responsable:** Melchor Emmanuel Ojeda Cauich e Ideny López Rodríguez

### SC-04 — Guardado al sentarse frente a la computadora

**Fecha:** 3 de noviembre de 2026

**Cambio propuesto:** El juego guarda cuando Elaki se sienta frente a la computadora del Nivel 2. Si la criatura canina lo alcanza, el reintento comienza cerca del final del video.

**Motivo:** Evitar que el jugador repita el video completo después de cada derrota.

**Beneficio esperado:** Reintentos más ágiles que conservan la tensión de la persecución.

**Costo estimado:** Ajustar el punto de control del Nivel 2 y el reinicio de la secuencia del video.

**Impacto en otros sistemas:** Progreso, puntos de control, persecuciones y secuencia del monitor.

**Decisión:** Aprobado.

**Responsable:** Ideny López Rodríguez

### SC-05 — Nuevo desenlace

**Fecha:** 3 de noviembre de 2026

**Cambio propuesto:** El desenlace se presenta como una secuencia sin control del jugador, con imágenes alternadas de la tina, carteles de búsqueda, una transmisión televisiva, una pantalla de texto y la escena final del edificio.

**Motivo:** Conectar las pistas presentadas durante los niveles con la revelación sobre Margarita y Elaki.

**Beneficio esperado:** Un cierre que permite al jugador reinterpretar la historia desde el inicio y que vincula la experiencia de Elaki con la realidad de las personas desaparecidas.

**Costo estimado:** Construir la secuencia final, sus textos, recursos visuales y sonoros, y la transmisión televisiva.

**Impacto en otros sistemas:** Secuencias narrativas, textos, arte, audio y video.

**Decisión:** Aprobado.

**Responsable:** Ideny López Rodríguez, bajo la coordinación narrativa de Wendy Kristal Palma Góngora

## Registro

| Fecha                  | Cambio                                                                                                                                                           | Motivo                                                                                             | Impacto                                             | Estado       |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------- | --------------------------------------------------- | ------------ |
| 3 de noviembre de 2026 | La sala y el pasillo comparten el mismo espacio y se cruzan caminando, sin fundido ni corte de cámara.                                                           | Dar continuidad al recorrido dentro de la casa.                                                    | Habitaciones, puertas y cámara.                     | Implementado |
| 3 de noviembre de 2026 | El gabinete del baño se revisa en una vista frontal con el arte del equipo y cada medicamento se resalta al pasar el cursor.                                     | Presentar la inspección con los recursos visuales definitivos.                                     | Vistas de inspección, interfaz y prólogo.           | Implementado |
| 3 de noviembre de 2026 | El texto de los diálogos avanza a la siguiente línea en cuanto termina de mostrarse.                                                                             | Evitar esperas innecesarias durante la lectura.                                                    | Diálogos e interfaz.                                | Implementado |
| 3 de noviembre de 2026 | La sala de los niveles 2 y 3 y el baño de los niveles 1 y 3 conservan la distribución de la casa real, con el arte alterado de cada dimensión.                   | Reforzar la sensación de un lugar familiar que se deforma y reutilizar la distribución de la casa. | Arte de escenarios, habitaciones e iluminación.     | Implementado |
| 3 de noviembre de 2026 | La Manifestación de Margarita siempre alcanza a Elaki y el encuentro termina sin pantalla de derrota.                                                            | Presentar el encuentro como un momento narrativo inevitable y no como un desafío que se supera.    | Persecuciones, daño, secuencias y progreso.         | Implementado |
| 3 de noviembre de 2026 | Se adopta el guion unificado como referencia de la historia, con el orden prólogo, Nivel 1, Nivel 2, Nivel 3 y desenlace. (SC-01)                                | Contar con una sola versión de la historia para todo el equipo.                                    | Secuencias narrativas, diálogos, arte y audio.      | Aprobado     |
| 3 de noviembre de 2026 | Cada nivel inicia en el baño real y se cruza por el espejo. (SC-02)                                                                                              | Mantener la comparación entre el baño real y sus versiones alteradas a lo largo del juego.         | Secuencias narrativas, portales y arte del baño.    | Aprobado     |
| 3 de noviembre de 2026 | La muerte solo es posible durante la persecución de la criatura canina y el encuentro con la Manifestación de Margarita. (SC-03)                                 | Concentrar el peligro en los momentos clave del Nivel 2.                                           | Persecuciones, daño y diseño de niveles.            | Aprobado     |
| 3 de noviembre de 2026 | El juego guarda al sentarse frente a la computadora del Nivel 2. Si la criatura canina alcanza a Elaki, el reintento comienza cerca del final del video. (SC-04) | Evitar repetir el video completo después de cada derrota.                                          | Progreso, puntos de control y persecuciones.        | Aprobado     |
| 3 de noviembre de 2026 | El desenlace se presenta con imágenes alternadas de la tina, una transmisión televisiva, una pantalla de texto y la escena final del edificio.                   | Conectar las pistas de los niveles con la revelación sobre Margarita y Elaki.                      | Secuencias narrativas, textos, arte, audio y video. | Aprobado     |

> **Navegación:** [[00 - Índice]] · ← [[04 - Backlog del Proyecto]] · [[01 - Criterios de Calidad]] →
