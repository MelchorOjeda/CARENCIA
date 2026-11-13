# Pruebas

## Descripción de la sección

Define cómo se comprobará que el videojuego funciona como fue diseñado.

## Tipos de prueba

### Pruebas funcionales

En Carencia se comprobaron el movimiento, la interacción con E, las vistas de inspección, la pausa, el menú principal, las opciones, el guardado y la carga de la partida. Se realizaron dentro del editor de Unity y con entrada real de teclado.

### Pruebas de integración

Se comprobó la relación entre secuencias narrativas, cámara, persecuciones, daño, derrota, reintento y puntos de control, así como el paso entre escenas.

### Pruebas de usabilidad

Se revisó que los indicadores de interacción aparezcan y desaparezcan según la distancia, que los menús puedan recorrerse con teclado y que los objetivos se comprendan mediante los pensamientos de Elaki, sin ayuda adicional del equipo.

### Pruebas de rendimiento

Se comprobó la compilación del ejecutable para Windows y su estabilidad durante el recorrido y los cambios de escena.

## Caso de prueba

### CP-01 — Arranque y menú principal

**Objetivo:** Comprobar que el juego inicia en el menú principal y que sus opciones responden al teclado.

**Precondiciones:** Existe una partida guardada.

**Pasos:**

1. Iniciar el juego.

2. Recorrer las opciones del menú con las flechas y confirmar con Enter.

3. Seleccionar "Jugar" y cancelar la confirmación con Esc.

**Resultado esperado:** El juego abre en el menú principal, las opciones responden al teclado y "Jugar" pide confirmación antes de reemplazar la partida guardada.

**Resultado obtenido:** El menú respondió al teclado y "Jugar" mostró la confirmación, que se canceló con Esc.

**Estado:** Correcto

### CP-02 — Movimiento del protagonista

**Objetivo:** Comprobar que Elaki se desplaza con el teclado y respeta los obstáculos.

**Precondiciones:** Partida iniciada con el control habilitado.

**Pasos:**

1. Presionar W, A, S y D en el pasillo.

2. Caminar contra paredes y muebles.

3. Soltar las teclas.

**Resultado esperado:** Elaki se desplaza en la dirección indicada, no atraviesa obstáculos y se detiene al soltar las teclas.

**Resultado obtenido:** Elaki se desplazó en cada dirección, se detuvo ante muebles y paredes y permaneció dentro del área transitable.

**Estado:** Correcto

### CP-03 — Interacción y vista de inspección

**Objetivo:** Comprobar que un objeto se inspecciona con E y que la vista se cierra con Esc.

**Precondiciones:** Elaki se encuentra junto a un objeto interactivo.

**Pasos:**

1. Acercarse al objeto hasta que aparezca el indicador.

2. Presionar E.

3. Presionar Esc.

**Resultado esperado:** Se abre la vista de inspección con los controles de movimiento bloqueados. Esc la cierra sin abrir el menú de pausa.

**Resultado obtenido:** La inspección se abrió con E, bloqueó el movimiento y se cerró con Esc sin abrir la pausa.

**Estado:** Correcto

### CP-04 — Pausa

**Objetivo:** Comprobar que el juego se pausa y se reanuda con Esc.

**Precondiciones:** Partida en curso, sin vistas de inspección abiertas.

**Pasos:**

1. Presionar Esc.

2. Presionar Esc nuevamente.

**Resultado esperado:** Se abre el menú de pausa y el juego se detiene. El segundo Esc reanuda la partida sin desplazar al personaje.

**Resultado obtenido:** La pausa se abrió y el segundo Esc reanudó el juego sin mover a Elaki.

**Estado:** Correcto

### CP-05 — Guardado y "Continuar"

**Objetivo:** Comprobar que la partida se recupera desde el último punto de control.

**Precondiciones:** Haber alcanzado un punto de control.

**Pasos:**

1. Volver al menú principal.

2. Seleccionar "Continuar".

**Resultado esperado:** Se carga la escena, la habitación, la posición y el progreso narrativo guardados, sin repetir eventos ya vistos.

**Resultado obtenido:** "Continuar" cargó la escena, la habitación, la posición y las banderas narrativas guardadas. El archivo de guardado y su copia se escribieron en cada punto de control.

**Estado:** Correcto

### CP-06 — Opciones y restablecimiento

**Objetivo:** Comprobar que las opciones se modifican y se restablecen sin afectar la partida.

**Precondiciones:** Menú principal abierto.

**Pasos:**

1. Abrir "Opciones" y recorrer sus categorías con el teclado.

2. Modificar valores con las flechas y los deslizadores.

3. Restablecer los valores predeterminados y confirmar.

**Resultado esperado:** Los valores cambian y vuelven a sus valores originales al restablecerlos, sin modificar la partida guardada.

**Resultado obtenido:** Las categorías y los valores respondieron al teclado. El restablecimiento devolvió el tamaño de texto y los subtítulos a sus valores originales sin afectar la partida.

**Estado:** Correcto

### CP-07 — Persecución, derrota y reintento

**Objetivo:** Comprobar que la derrota durante la persecución de la criatura canina permite reintentar desde el punto de control.

**Precondiciones:** Haber terminado el video de la computadora del Nivel 2.

**Pasos:**

1. Permitir que la criatura canina alcance a Elaki hasta agotar su salud.

2. Seleccionar "Reintentar" en la pantalla de derrota.

**Resultado esperado:** Cada golpe intensifica las manchas rojas. Al agotarse la salud aparece la pantalla de derrota y "Reintentar" restaura la escena y la persecución desde el punto de control.

**Resultado obtenido:** Los golpes intensificaron las manchas rojas, apareció la pantalla de derrota y el reintento restauró la escena y reinició la persecución.

**Estado:** Correcto

### CP-09 — Recorrido completo

**Objetivo:** Comprobar que el juego puede completarse desde el menú principal hasta los créditos.

**Precondiciones:** Nueva partida.

**Pasos:**

1. Iniciar una nueva partida desde el menú principal.

2. Completar el prólogo, los tres niveles y el desenlace.

3. Esperar el final de los créditos.

**Resultado esperado:** Las escenas se encadenan en orden, cada secuencia restaura los controles y el juego regresa al menú principal.

**Resultado obtenido:** El recorrido se completó de principio a fin y el juego regresó al menú principal.

**Estado:** Correcto

> **Navegación:** [[00 - Índice]] · ← [[01 - Criterios de Calidad]] · [[03 - Bugs y Seguimiento]] →
