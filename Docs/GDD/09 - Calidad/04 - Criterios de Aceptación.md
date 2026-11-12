# Criterios de Aceptación

## Descripción de la sección

Los criterios de aceptación permiten decidir objetivamente si una característica está lista.

### CA-01 — Apertura del primer portal

**Dado que:** Elaki se encuentra en el baño real después del primer desayuno.

**Cuando:** el jugador abre y cierra el gabinete del espejo.

**Entonces:** el espejo vuelve a reflejar, se deforma, se abre el portal y el jugador puede cruzarlo hacia el Nivel 1.

### CA-02 — Persecución de la criatura canina

**Dado que:** terminó el video de la computadora del Nivel 2 y el jugador recuperó el control.

**Cuando:** la criatura canina alcanza a Elaki hasta agotar su salud.

**Entonces:** aparece la pantalla de derrota y "Reintentar" regresa a Elaki al punto de control, cerca del final del video.

### CA-03 — Encuentro con la Manifestación de Margarita

**Dado que:** Elaki escapó de la criatura canina y llegó al fondo del pasillo del Nivel 2.

**Cuando:** la Manifestación de Margarita lo alcanza por tercera vez.

**Entonces:** la vista pasa a negro sin pantalla de derrota y Elaki despierta en su cuarto real.

### CA-04 — Continuar una partida

**Dado que:** el jugador alcanzó al menos un punto de control.

**Cuando:** vuelve al menú principal y selecciona "Continuar".

**Entonces:** el juego carga la escena, la habitación, la posición y el progreso guardados, sin repetir eventos ya vistos.

### CA-05 — Desenlace

**Dado que:** Elaki encontró a Margarita en la tina del Nivel 3.

**Cuando:** Elaki entra a la tina.

**Entonces:** comienza el desenlace sin control del jugador, que revela lo ocurrido a Margarita y a Elaki, y al terminar el juego muestra los créditos y regresa al menú principal.

## Criterios del proyecto

- [x] El bucle principal funciona: explorar, inspeccionar, activar eventos y cruzar portales permite avanzar desde el prólogo hasta el desenlace.
- [x] Las condiciones de victoria y derrota funcionan: completar el desenlace termina la partida, y la derrota ante la criatura canina permite reintentar desde el punto de control.
- [x] El progreso puede guardarse automáticamente en los puntos de control y recuperarse con "Continuar".
- [x] Los sistemas principales están integrados: movimiento, interacción, cámara, secuencias, persecuciones, daño, guardado, interfaz y audio.
- [x] Las pruebas críticas están aprobadas, según los casos registrados en [[02 - Pruebas]].
- [x] El rendimiento cumple el objetivo definido: el ejecutable para Windows se mantiene estable durante el recorrido completo.
- [x] No existen errores bloqueantes conocidos, según el registro de [[03 - Bugs y Seguimiento]].


> **Navegación:** [[00 - Índice]] · ← [[03 - Bugs y Seguimiento]] · [[01 - Glosario]] →
