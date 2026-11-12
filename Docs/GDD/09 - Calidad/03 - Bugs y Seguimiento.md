# Bugs y Seguimiento

## Descripción de la sección

Registra problemas encontrados durante las pruebas y permite darles seguimiento.

## Reporte de bug

### BUG-01 — Elaki atraviesa las paredes y las paredes se dibujan encima de él

**Título:** Elaki atraviesa las paredes y las paredes se superponen al personaje.

**Versión:** Compilación de desarrollo del 30 de octubre de 2026.

**Nivel / escena:** Casa real, en las habitaciones y el pasillo.

**Pasos para reproducir:**

1. Iniciar una partida en la casa real.
2. Caminar hacia una de las paredes de la habitación o del pasillo.
3. Mantener presionada la tecla de movimiento contra la pared.

**Resultado esperado:** Elaki se detiene al llegar a la pared y se dibuja delante o detrás de ella según su posición en la vista isométrica.

**Resultado actual:** Elaki atravesaba algunas paredes y, al acercarse a ellas, la pared se dibujaba encima del personaje y lo ocultaba.

**Frecuencia:** Siempre

**Severidad:** Alta

**Evidencia:** Corrección registrada en el repositorio del proyecto: ajuste de la altura y las colisiones de los muros.

**Estado:** Cerrado

## Registro

| ID     | Bug                                                                                                                                         | Severidad | Responsable                   | Estado  |
| ------ | ------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ----------------------------- | ------- |
| BUG-01 | Elaki atravesaba algunas paredes y las paredes se dibujaban encima del personaje.                                                           | Alta      | Melchor Emmanuel Ojeda Cauich | Cerrado |



> **Navegación:** [[00 - Índice]] · ← [[02 - Pruebas]] · [[04 - Criterios de Aceptación]] →