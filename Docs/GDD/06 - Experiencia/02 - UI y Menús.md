# UI y Menús

## Descripción de la sección

Define la estructura funcional de las interfaces y menús.

## Mapa de navegación

```text
Menú principal
├── Nueva partida
├── Continuar
├── Configuración
│   ├── Audio
│   ├── Video
│   ├── Controles
│   └── Accesibilidad
└── Salir
```

```text
MENÚ PRINCIPAL
├── Jugar
│   └── Iniciar desde el prólogo
│  
├── Continuar
│   └── Cargar último punto guardado
├── Opciones
│   ├── Audio
│   ├── Video
│   ├── Controles
│   └── Accesibilidad
└── Salir

DURANTE EL JUEGO
└── Menú de pausa (Esc)
    ├── Reanudar
    ├── Opciones
    └── Volver al menú principal

DERROTA
└── Reiniciar desde el punto de control

FINAL DEL DEMO
└── Volver al menú principal
```
## Pantallas

### Pantalla 01: Menú principal

**Propósito:** Permitir al jugador iniciar una partida, continuar su progreso, modificar las opciones o salir del videojuego.

**Elementos:**

- Nombre o logotipo.
- Fondo visual relacionado con la atmósfera del juego.
- Botones: **Jugar, Continuar, Opciones y Salir**.
- Indicador visual de la opción seleccionada.

**Acciones disponibles:**

| Botón     | Acción                                            |
| --------- | ------------------------------------------------- |
| Jugar     | Inicia una partida desde el prólogo.              |
| Continuar | Retoma la partida desde el último punto guardado. |
| Opciones  | Abre el menú de configuración.                    |
| Salir     | Cierra el videojuego.                             |
**Estados:**
- **Inicial:** Se muestra al abrir el videojuego.
- **Sin partida guardada:** Continuar permanece deshabilitado.
- **Con partida guardada:** Continuar está disponible.
- **Confirmación:** Se solicita autorización antes de reemplazar el progreso existente.

**Transiciones:**
Menú principal → Juego / Opciones / Salir.

### Pantalla 02: Opciones

**Propósito:** Permitir al jugador ajustar las configuraciones del videojuego según sus preferencias.

**Elementos:**

|Categoría|Configuraciones propuestas|
|---|---|
|Audio|Volumen general, música y efectos de sonido.|
|Video|Resolución, modo de pantalla y calidad gráfica, según las opciones compatibles.|
|Controles|Consulta de las teclas utilizadas durante el juego.|
|Accesibilidad|Ajustes de subtítulos, tamaño de texto y otros apoyos que se definan.|

**Acciones disponibles:** Seleccionar una categoría, modificar las configuraciones disponibles y regresar al menú anterior.

**Estados:**

- Configuración actual.
- Configuración modificada.
- Configuración guardada.

**Transiciones:** Menú principal o pausa → Opciones → Menú de origen.

### Pantalla 03 — Menú de pausa

**Propósito:** Permitir al jugador detener temporalmente la experiencia sin perder el progreso.

**Elementos:**

- Título «Pausa».
- Botón **Reanudar**.
- Botón **Opciones**.
- Botón **Volver al menú principal**.

**Acciones disponibles:** Reanudar la partida, modificar configuraciones o regresar al menú principal.

**Estados:**

- **Pausado:** Se detienen las acciones del juego.
- **Opciones abiertas:** El jugador puede modificar configuraciones.
- **Confirmación:** Se solicita autorización para salir de la partida cuando corresponda.

**Transiciones:** Juego → Pausa → Juego / Opciones / Menú principal.

El menú debe poder abrirse con **Esc**, incluso durante las persecuciones y cinemáticas.

### Pantalla 04 — Derrota

**Propósito:** Informar al jugador que ha perdido durante una persecución y permitirle intentarlo nuevamente.

**Elementos:**

- Mensaje breve de derrota.
- Botón Reintentar.
- Presentación visual acorde con la atmósfera del videojuego.

**Acciones disponibles:** Reiniciar desde el último punto de control.

**Estados:**

- Derrota activada.
- Esperando la selección del jugador.
- Reiniciando el punto de control.

**Transiciones:** Persecución → Derrota → Último punto de control.

### Pantalla 05 — Final del demo

**Propósito:** Comunicar que el jugador ha completado el contenido disponible.

**Elementos:**

- Pantalla de cierre con el nombre del videojuego.
- Mensaje «Fin del demo».
- Botón **Volver al menú principal**.

**Acciones disponibles:** Regresar al menú principal.

**Estados:**

- Secuencia final completada.
- Pantalla de cierre visible.

**Transiciones:** Desenlace → Fin del demo → Menú principal.
## Confirmaciones

Documenta acciones destructivas o irreversibles y cómo se confirman.

| Acción                       | ¿Requiere confirmación? | Comportamiento                                                                                   |
| ---------------------------- | ----------------------- | ------------------------------------------------------------------------------------------------ |
| Jugar con progreso existente | Sí                      | Advierte que comenzar desde cero reemplazará la partida guardada, si se utiliza una sola ranura. |
| Salir del videojuego<br>     | sÍ<br>                  | Puede solicitar confirmación para evitar cierres accidentales.                                   |

> **Navegación:** [[00 - Índice]] · ← [[01 - UX y Flujo del Jugador]] · [[03 - Accesibilidad]] →
