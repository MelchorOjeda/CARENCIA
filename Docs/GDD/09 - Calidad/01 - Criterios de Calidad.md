# Criterios de Calidad

## Descripción de la sección

Define qué significa que una característica esté correctamente terminada.

## Calidad funcional

- [x] Funciona según las reglas documentadas en el GDD: controles, interacciones, secuencias, persecuciones y guardado.
- [x] No produce errores conocidos críticos que impidan avanzar en la historia.

## Calidad de experiencia

- [x] El jugador entiende el objetivo de cada momento mediante los pensamientos de Elaki, los indicadores de interacción y las señales visuales y sonoras.
- [x] Las respuestas del juego son claras: cada interacción produce un texto, un acercamiento, un sonido o un cambio en el escenario.
- [x] La dificultad es coherente: el peligro se concentra en las persecuciones del Nivel 2 y el resto del recorrido se centra en la exploración.
- [x] La interfaz es comprensible: los menús pueden recorrerse con teclado y ratón, y los controles pueden consultarse desde las opciones.

## Calidad técnica

- [x] Rendimiento aceptable: el ejecutable para Windows inicia correctamente y se mantiene estable durante el recorrido, sin bloqueos al cambiar de escena.
- [x] Sin errores críticos de compilación ni de ejecución.
- [x] Guardado estable: el progreso se guarda en los puntos de control, se conserva una copia del último guardado válido y "Continuar" restaura la partida.
- [x] No existen dependencias rotas: las escenas no contienen scripts faltantes, referencias vacías ni puertas sin destino.

## Definición de terminado

Una característica se considera terminada cuando:

1. Cumple el criterio de aceptación definido en el [[04 - Backlog del Proyecto]].
2. Funciona dentro del recorrido completo del juego y no solo en una escena aislada.
3. Se integra correctamente con los sistemas de los que depende, como movimiento, cámara, secuencias, guardado e interfaz.
4. Restaura los controles y el estado del juego al terminar, incluso después de una derrota o al cargar una partida.
5. No genera errores en la consola de Unity ni en el ejecutable para Windows.
6. Cuenta con sus recursos de arte, audio y texto integrados.
7. Fue probada mediante los casos descritos en [[02 - Pruebas]] y obtuvo un resultado correcto.

> **Navegación:** [[00 - Índice]] · ← [[05 - Gestión de Cambios]] · [[02 - Pruebas]] →
