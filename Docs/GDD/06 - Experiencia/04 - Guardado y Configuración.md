# Guardado y Configuración

## Descripción de la sección

Define cómo se conserva el progreso y qué opciones puede modificar el jugador.

## Guardado

**Tipo:** Automático

**Cuándo se guarda:**
- Al completar el prólogo y atravesar el primer portal.
- Al finalizar un nivel y acceder al siguiente.
- Antes de iniciar las persecuciones del Nivel 2.
- Al superar las persecuciones y avanzar al siguiente acontecimiento narrativo.
- Al llegar a otros puntos de control definidos durante el desarrollo.

**Qué datos se guardan:**
- Nivel actual
- Punto de control
- Progreso narrativo
- Interacciones importantes
- Estado del escenario

**Cantidad de ranuras:** Una ranura de guardado para el demo.

**Qué ocurre si el guardado falla:**
El videojuego deberá mostrar un mensaje indicando que no fue posible guardar el progreso.

Se conservará el último guardado válido, evitando sobrescribirlo con datos incompletos o dañados. El jugador podrá continuar jugando, pero deberá ser informado de que su progreso reciente podría no recuperarse al cerrar el juego.

## Configuración
Las opciones estarán disponibles desde el menú principal y el menú de pausa.

| Categoría     | Opciones                                                                                                                                                   |
| ------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Audio         | Volumen general, música, efectos de sonido y voces, si se implementan.                                                                                     |
| Video         | NA                                                                                                                                                         |
| Controles     | Consulta de teclas, reasignación de controles si se implementa.                                                                                            |
| Accesibilidad | Tamaño de texto, subtítulos, contraste, reducción de destellos, reducción de movimiento e indicadores visuales para sonidos importantes, si se implementa. |
| Jugabilidad   | NA                                                                                                                                                         |

## Restablecimiento

¿Qué opciones pueden volver a valores predeterminados?

El jugador podrá restablecer los ajustes de audio, video, controles, accesibilidad y jugabilidad a sus valores originales mediante una opción de Restablecer valores predeterminados.

Esta acción deberá solicitar confirmación antes de aplicarse.

El restablecimiento de la configuración no eliminará la partida guardada ni modificará el progreso de la historia.

> **Navegación:** [[00 - Índice]] · ← [[03 - Accesibilidad]] · [[01 - Arquitectura Técnica]] →
