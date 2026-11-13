# Backlog del Proyecto

## Descripción de la sección

El backlog contiene el trabajo necesario para convertir el diseño en un videojuego funcional.

## Backlog

- [x] T01 — Configuración del proyecto en Unity
- [x] T02 — Cámara isométrica y acercamientos
- [x] T03 — Vistas de inspección
- [x] T04 — Habitaciones, puertas y cambios de espacio
- [x] T05 — Diálogos, textos narrativos y subtítulos, menú principal, pausa, derrota, créditos, opciones y accesibilidad
- [x] T06 — Secuencias narrativas y registro de eventos (diseño de niveles)
- [x] T07 — Progreso, guardado automático y puntos de control
- [x] T08 — Arte, movimiento y animación de personajes
- [x] T09 — Arte de escenarios y objetos
- [x] T10 — Persecuciones, Daño visual del protagonista, derrota y reintento
- [x] T11 — Integración de audio
- [x] T12 — Pruebas del recorrido completo y corrección de errores

## Tareas

### T01 - Configuración del proyecto en Unity

**Descripción:** Preparar el proyecto en Unity 6 con Universal Render Pipeline 2D, Input System, capas de orden de dibujo para la vista isométrica, estructura de carpetas y registro de las escenas del juego en Build Settings.

**Responsable:** Melchor Emmanuel Ojeda Cauich

**Prioridad:** Alta

**Dependencias:** Ninguna.

**Criterio de aceptación:** El proyecto abre y compila sin errores. Las escenas están registradas en Build Settings, y los sprites se ordenan correctamente según su posición en la vista isométrica.

**Estado:** Terminada

### T02 — Cámara isométrica y acercamientos

**Descripción:** Implementar la cámara con encuadre por habitación en perspectiva isométrica, con acercamientos y alejamientos controlados por los eventos narrativos.

**Responsable:** Ideny López Rodríguez

**Prioridad:** Alta. Todos los niveles y secuencias dependen del encuadre para presentar los espacios y los acontecimientos.

**Dependencias:** T01, T08.

**Criterio de aceptación:** La cámara mantiene a Elaki visible durante la exploración y las persecuciones. Durante los eventos narrativos realiza acercamientos suaves al objeto o lugar indicado y recupera su encuadre habitual al terminar. El pixel art se ve nítido.

**Estado:** Terminada

### T03 — Vistas de inspección

**Descripción:** Implementar la interacción con objetos mediante la tecla E o clic, los acercamientos a objetos del escenario y la vista fija en primera persona del gabinete del baño.

**Responsable:** Ideny López Rodríguez

**Prioridad:** Alta.

**Dependencias:** T01, T05.

**Criterio de aceptación:** Al acercarse a un objeto interactivo aparece el indicador con la acción disponible, que desaparece al alejarse. La interacción se activa con E o clic. El gabinete cambia a una vista fija en primera persona, donde cada objeto muestra su pensamiento al seleccionarlo. Esc cierra la vista sin abrir el menú de pausa, y el jugador recupera el control.

**Estado:** Terminada
### T04 — Habitaciones, puertas y cambios de espacio

**Descripción:** Organizar cada escena en habitaciones conectadas, con puertas y aberturas que permiten pasar de un espacio a otro, y con un punto de llegada libre de obstáculos.

**Responsable:** Ideny López Rodríguez

**Prioridad:** Alta.

**Dependencias:** T02, T08.

**Criterio de aceptación:** Elaki cruza entre la habitación, el pasillo, la sala y el baño, y aparece en una posición válida del otro lado. Mantener presionada la tecla de movimiento no lo regresa por la misma puerta. Las puertas cerradas impiden el paso y muestran un aviso.

**Estado:** Terminada

### T05 — Diálogos, textos narrativos y subtítulos, menú principal, pausa, derrota, créditos, opciones y accesibilidad

**Descripción:** Diseñar e implementar la interfaz del juego: cuadro de diálogo para conversaciones y pensamientos de Elaki, subtítulos, menú principal, menú de pausa, pantalla de derrota, créditos y panel de opciones. Incluye la redacción y revisión de los diálogos, pensamientos, pistas y textos de las noticias.

**Responsable:** Zurisarai Delgado Rodríguez. Los textos narrativos están a cargo de Wendy Kristal Palma Góngora, con la participación del equipo de desarrollo.

**Prioridad:** Alta. 

**Dependencias:** T01.

**Criterio de aceptación:**

- Los diálogos muestran el nombre de quien habla, presentan el texto de forma progresiva y avanzan con Enter o clic. 
- El menú principal ofrece Jugar, Continuar, Opciones, Créditos y Salir. "Continuar" solo está disponible si existe una partida guardada, y "Jugar" pide confirmación antes de reemplazarla.
- Esc abre la pausa con Reanudar, Opciones y Volver al menú principal, y un segundo Esc reanuda el juego.
- El panel de opciones permite ajustar el volumen general, de música, de efectos y de voces, la calidad gráfica y el brillo. También permite consultar los controles y volver a ver el tutorial.
- En accesibilidad pueden cambiarse el tamaño de texto y los subtítulos. La opción de restablecer pide confirmación y no afecta la partida guardada.
- Los textos integrados corresponden a la historia del GDD. 


**Estado:** Terminada

### T06 — Secuencias narrativas y registro de eventos (diseño de niveles)

**Descripción:** Construir el prólogo, los tres niveles y el desenlace mediante secuencias y banderas narrativas que bloquean y restauran los controles, mueven la cámara, activan eventos y portales, y registran los acontecimientos completados.

**Responsable:** Ideny López Rodríguez, bajo la coordinación narrativa de Wendy Kristal Palma Góngora.

**Prioridad:** Alta.

**Dependencias:** T02, T03, T04, T05, T08, T09.

**Criterio de aceptación:**

- **Prólogo:** Elaki despierta, los padres niegan la existencia de Margarita durante el desayuno y, en el baño real, la revisión del gabinete abre el primer portal en el espejo.
- **Nivel 1:** Elaki recorre una versión amplia y extraña de la casa siguiendo la voz de Margarita, hasta que se desmaya y despierta en la casa real.
- **Nivel 2:** Elaki cruza a una versión de la casa abandonada, sobrevive a la persecución del perro de la familia y es alcanzado por la Manifestación de Margarita. Al despertar, la casa real muestra nuevas pistas sobre la familia.
- **Nivel 3:** Elaki recorre una casa hecha de carne que lo guía al baño, donde encuentra a Margarita en la tina y entra para alcanzarla.
- **Desenlace:** una secuencia sin control del jugador revela lo ocurrido a Margarita y a Elaki mediante noticias.

**Estado:** Terminada

### T07 — Progreso, guardado automático y puntos de control

**Descripción:** Implementar una ranura de guardado automático en formato JSON que registra escena, habitación, posición y banderas narrativas en los puntos de control, con una copia del último guardado válido.

**Responsable:** Ideny López Rodríguez

**Prioridad:** Alta. 

**Dependencias:** T06.

**Criterio de aceptación:** El juego guarda al entrar a cada nivel, al sentarse frente a la computadora del Nivel 2 y en los momentos narrativos establecidos. "Continuar" restaura la escena, la habitación, la posición y el progreso sin repetir eventos ya vistos. 

**Estado:** Terminada

### T08 — Arte, movimiento y animación de personajes

**Descripción:** Crear e integrar los sprites y animaciones de Elaki, los padres, Margarita, la Manifestación de Margarita y la criatura canina. Implementar el movimiento de Elaki con W, A, S, D y flechas direccionales, y la carrera con Shift.

**Responsable:** Yareny Alexandra Arellano Díaz (arte y animación) e Ideny López Rodríguez (movimiento)

**Prioridad:** Alta. 

**Dependencias:** T01.

**Criterio de aceptación:**

- Elaki se mueve con WASD y con las flechas, mantiene la misma velocidad en diagonal, no atraviesa obstáculos y se detiene al soltar las teclas.
- Al mantener Shift se desplaza más rápido.
- El movimiento se bloquea durante cinemáticas o diálogos, y se restaura al terminar.
- Elaki cuenta con animaciones de espera y caminata en ocho direcciones, carrera y muerte. Los padres aparecen sentados en los desayunos. La criatura canina y la Manifestación de Margarita tienen animaciones de persecución.
- Todos los personajes mantienen una escala coherente con el escenario.

**Estado:** Terminada

### T09 — Arte de escenarios y objetos

**Descripción:** Crear e integrar los escenarios de la casa real y sus tres versiones alteradas, junto con los objetos, los estados de la cortina del baño y las ilustraciones de las vistas de inspección.

**Responsable:** Yareny Alexandra Arellano Díaz (dibujo) , Zurisarai Delgado Rodríguez (dibujo), Melchor Emmanuel Ojeda Cauich (dibujo e integración en Unity), Ideny López Rodríguez (integración en Unity).

**Prioridad:** Alta.

**Dependencias:** T01.

**Criterio de aceptación:** La habitación, el pasillo, la sala con comedor y cocina, y el baño tienen su versión real y sus versiones de cada nivel. Cada dimensión se distingue visualmente de la casa real, y la cortina del baño muestra el estado correspondiente a cada momento de la historia.

**Estado:** Terminada

### T10 — Persecuciones, daño visual del protagonista, derrota y reintento

**Descripción:** Implementar las persecuciones y el daño al personaje, representar el daño mediante manchas rojas progresivas en la pantalla, y permitir reintentar desde el último punto de control tras una derrota.

**Responsable:** Melchor Emmanuel Ojeda Cauich, e Ideny López Rodríguez.

**Prioridad:** Alta. 

**Dependencias:** T05, T06, T07, T08.

**Criterio de aceptación:**

- Los enemigos solo persiguen cuando se activa su evento y buscan a Elaki rodeando los obstáculos.
- La persecución de la criatura canina termina al alcanzar el pasillo.
- La Manifestación de Margarita siempre alcanza a Elaki; al tercer golpe la vista pasa a negro sin pantalla de derrota y da paso al tercer despertar.
- Cada golpe intensifica las manchas rojas en la pantalla.
- Si la criatura canina derrota a Elaki, aparece la pantalla de derrota con el botón "Reintentar", que restaura el último punto de control.
- Solo es posible morir durante estas dos persecuciones.

**Estado:** Terminada

### T11 — Integración de audio

**Descripción:** Crear, editar e integrar los ambientes, efectos y voces de cada nivel, organizados en grupos de mezcla de música, efectos y voces.

**Responsable:** Armando Balam Salaya

**Prioridad:** Media. 

**Dependencias:** T05, T06.

**Criterio de aceptación:** Cada escenario reproduce su ambiente, y los eventos narrativos, persecuciones y noticias reproducen sus sonidos y voces en el momento correspondiente. El volumen de cada grupo responde a los ajustes de opciones.

**Estado:** Terminada

### T12 — Pruebas del recorrido completo y corrección de errores

**Descripción:** Generar el ejecutable para Windows de 64 bits y probar el juego desde el menú hasta los créditos, incluidas las persecuciones, la derrota, el reintento, el guardado, la pausa y las opciones, corrigiendo los errores encontrados.

**Responsable:** Ideny López Rodríguez y Melchor Emmanuel Ojeda Cauich, con la participación del equipo

**Prioridad:** Alta. 

**Dependencias:** T01 a T11.

**Criterio de aceptación:** La compilación termina sin errores y `Carencia.exe` se ejecuta en Windows. El recorrido completo puede jugarse de principio a fin sin errores bloqueantes. Los puntos de control restauran estados válidos, y la pausa y "Continuar" funcionan con teclado real.

**Estado:** Terminada


> **Navegación:** [[00 - Índice]] · ← [[03 - Roadmap]] · [[05 - Gestión de Cambios]] →
