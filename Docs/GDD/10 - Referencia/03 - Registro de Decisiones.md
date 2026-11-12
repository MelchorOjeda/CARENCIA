# Registro de Decisiones

## Descripción de la sección

Esta sección conserva las decisiones importantes del proyecto y el razonamiento detrás de ellas.

## Decisión

### DEC-01 — Guion unificado de Carencia

**Fecha:** 9 de noviembre de 2026

**Tema:** Historia y estructura del juego.

**Problema:** El equipo contaba con dos versiones del guion, una de dos niveles y otra de tres niveles, con diferencias en el cierre de cada nivel y en el desenlace. Esto generaba contradicciones entre el diseño, el arte, el audio y la programación.

**Opciones consideradas:**

1. Mantener el guion de dos niveles.
2. Mantener el guion de tres niveles.
3. Unificar ambos guiones en uno solo, usando el guion de dos niveles como base, los cierres de nivel del guion de tres niveles y las decisiones tomadas por el equipo.

**Decisión tomada:** Se adoptó la opción 3. El guion unificado define el orden definitivo del prólogo, los tres niveles y el desenlace.

**Motivo:** Conservar la base narrativa más desarrollada y, al mismo tiempo, aprovechar la estructura de tres niveles, en la que cada nivel tiene un cierre propio.

**Consecuencias:** El guion unificado se convierte en la referencia de la historia para todo el equipo. Cada nivel inicia en el baño real y se cruza por el espejo, la muerte se limita a las persecuciones del Nivel 2 y el desenlace se presenta como una secuencia sin control del jugador. Los cambios derivados se registran en [[05 - Gestión de Cambios]].

**Responsable:** Wendy Kristal Palma Góngora.

## Registro

| Fecha                  | Tema                    | Decisión                                                                                                                                                                         | Responsable                                           |
| ---------------------- | ----------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| 20 de octubre de 2026  | Controles               | El personaje se mueve únicamente con WASD o flechas. El clic no desplaza a Elaki y se utiliza para interactuar, avanzar diálogos y navegar por menús y vistas de inspección.     | Ideny López Rodríguez                                 |
| 23 de octubre de 2026  | Motor y renderizado     | Desarrollar Carencia en Unity 6 (versión 6000.6.3f1) con Universal Render Pipeline 2D, sin actualizar la versión durante el desarrollo para evitar conflictos entre integrantes. | Ideny López Rodríguez                                 |
| 23 de octubre de 2026  | Nombre de la hermana    | Todo el juego utiliza el nombre Margarita en diálogos, textos y recursos, y la distorsión del nombre durante el desayuno se ajustó a ese nombre.                                 | Wendy Kristal Palma Góngora                           |
| 23 de octubre de 2026  | Mascota de la familia   | La mascota de la familia es un perro, de acuerdo con las hojas de concepto, y su manifestación es la criatura canina del Nivel 2.                                                | Wendy Kristal Palma Góngora                           |
| 23 de octubre de 2026  | Reglas de derrota       | La muerte solo es posible durante la persecución de la criatura canina y el encuentro con la Manifestación de Margarita, que funciona como transición narrativa.                 | Melchor Emmanuel Ojeda Cauich e Ideny López Rodríguez |
| 23 de octubre de 2026  | Control de versiones    | El repositorio se trabaja con Gitflow: una rama por funcionalidad o corrección, integración en `develop` y versiones entregables en `main`.                                      | Melchor Emmanuel Ojeda Cauich                         |
| 27 de octubre de 2026  | Guardado                | Se utiliza una sola ranura de guardado automático con una copia de respaldo. Si el guardado falla, se avisa al jugador y se conserva el último guardado válido.                  | Ideny López Rodríguez                                 |
| 27 de octubre de 2026  | Organización del código | Cada componente de Unity se escribe en un archivo con el mismo nombre de su clase, para evitar componentes con scripts faltantes en los prefabs.                                 | Ideny López Rodríguez                                 |
| 3 de noviembre de 2026 | Historia y estructura   | Se adopta el guion unificado como referencia de la historia.                                                                                                                     | Wendy Kristal Palma Góngora                           |


> **Navegación:** [[00 - Índice]] · ← [[02 - Referencias e Inspiración]] · [[04 - Historial de Versiones]] →
