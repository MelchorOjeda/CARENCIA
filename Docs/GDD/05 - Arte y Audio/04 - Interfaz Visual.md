# Interfaz Visual

## Descripción de la sección

Documenta la información que el juego muestra al jugador y las reglas visuales de la interfaz.

> **Fuente de las imágenes:** página *Menu Carencia* del archivo de Figma del proyecto.

## HUD

Carencia **no tiene HUD**. Es un juego de terror y la interfaz en pantalla durante la exploración rompería la inmersión, así que no se muestran vida, energía, munición, mapa ni recursos.

La información que el jugador necesita se comunica dentro del propio mundo (iluminación, sonido, objetos del escenario) y sólo aparece interfaz cuando el juego se detiene: menú principal, pausa, opciones, derrota y fin del demo.

| Elemento del HUD tradicional | ¿Se usa? | Cómo se resuelve |
| ---------------------------- | -------- | ---------------- |
| Vida / Energía / Munición | No | No existen como recursos visibles. |
| Objetivo | No | Lo guía la narrativa y el escenario. |
| Mapa / Recursos | No | La exploración es parte de la tensión. |
| Indicadores contextuales | Sólo en menús | La ayuda de teclas aparece en la parte inferior de cada pantalla de menú. |
| Progreso | Sólo en pausa | Nivel actual y último guardado automático, en la esquina inferior derecha. |

## Jerarquía visual

1. **Primero:** el título de la pantalla (*Carencia*, *Pausa*, *Opciones*, *Perdiste*) en Barrio con efecto glitch.
2. **Segundo:** la opción enfocada, en color hueso y marcada con el rasguño rojo a la izquierda.
3. **Tercero:** el resto de las opciones en gris niebla, el cartel de *Desaparecid?* y la ayuda de teclas al pie de la pantalla.

![Menú principal](img/UI02_menu_principal.jpg)

## Estados

| Estado | Cómo se ve |
| ------ | ---------- |
| Normal | Texto en gris niebla `#8A9BA8`. |
| Enfocado | Texto en hueso `#ECE6D8` con rasguño rojo `#C21A1F` a la izquierda. En pausa, también una franja roja de fondo. |
| Deshabilitado | Texto tachado y atenuado (por ejemplo, *Continuar* sin partida guardada). |
| Advertencia / confirmación | Nota de papel al centro de la pantalla, con el fondo oscurecido al 78 %. La opción segura (*No*) es la enfocada por defecto. |
| Cambios sin guardar | Sello rojo **SIN GUARDAR** sobre la hoja de Opciones. |
| Error / derrota | Pantalla *Perdiste* con manchas rojas de daño y el juego desenfocado detrás. |
| Carga | Barra de progreso delgada bajo el texto *Reiniciando desde el punto de control…*. |

![Componentes y sus estados](img/UI17_componentes.jpg)

## Convenciones

**Tipografía:**

- **Barrio** para títulos. Logo *CARENCIA*: 300 px en la portada y 190 px en el menú y fin del demo. *Pausa* y *Opciones*: 150 px. *Perdiste*: 190 px.
- **Special Elite** para todo lo demás. Opciones de menú: 68 px. Subtexto de opción: 24 px. Notas: pregunta 38 px y detalle 24 px. Hoja de opciones: ajuste 30 px y valor 28 px. Ayuda de controles: 26 px.
- Ambas son de Google Fonts (licencia OFL) y se usan en Unity con un Font Asset de TextMeshPro por fuente.

![Tipografía](img/UI19_tipografia.png)

**Iconografía:** no hay íconos. Las teclas se muestran como texto dentro de un recuadro (`↑ ↓`, `Enter / Clic`, `Esc`, `Q E`, `G`, `R`). Los elementos gráficos son de papel y pixel art: cartel de desaparecida, notas, hoja de opciones, sello y rasguño de selección.

**Tamaños mínimos:** el texto más pequeño en pantalla es de 24 px sobre una resolución base de 1920 × 1080.

**Contraste:** medido con la fórmula de WCAG 2.

| Combinación | Contraste |
| ----------- | --------- |
| Hueso `#ECE6D8` sobre noche `#0A1018` | 15.3 : 1 |
| Niebla `#8A9BA8` sobre noche `#0A1018` | 6.7 : 1 |
| Tinta `#1B1B1E` sobre papel `#C9C1AE` | 9.6 : 1 |
| Sangre `#C21A1F` sobre noche `#0A1018` | 3.1 : 1 (sólo para texto grande o acentos) |

![Paleta de colores](img/UI18_paleta.png)

**Animaciones:** los títulos usan un glitch de tres copias desplazadas (rojo `#C21A1F` en x +12, azul `#5FA8C8` en x 0 y hueso `#ECE6D8` en x +5 encima). Al pausar o perder, el juego se queda detrás desenfocado y oscurecido.

## Pantallas

### Portada

![Portada](img/UI01_portada.jpg)

### Menú principal

Con partida guardada, el cartel muestra el último rastro (nivel y tiempo jugado). Sin partida, *Continuar* aparece deshabilitado.

![Menú principal con Continuar enfocado](img/UI03_menu_continuar.jpg)

![Menú principal sin partida guardada](img/UI04_menu_sin_partida.jpg)

![Confirmar empezar desde el principio](img/UI06_confirmar_nueva_partida.jpg)

![Confirmar salir del juego](img/UI05_confirmar_salir.jpg)

### Pausa

![Pausa](img/UI07_pausa.jpg)

![Confirmar volver al menú principal](img/UI08_pausa_volver_al_menu.jpg)

### Configuración

La hoja de Opciones tiene cuatro pestañas (Audio, Video, Controles y Accesibilidad). A la izquierda se describe la opción enfocada.

![Opciones: Audio](img/UI09_opciones_audio.jpg)

![Opciones: Video](img/UI10_opciones_video.jpg)

![Opciones: Controles](img/UI11_opciones_controles.jpg)

![Opciones: Accesibilidad](img/UI12_opciones_accesibilidad.jpg)

![Confirmar restablecer valores predeterminados](img/UI13_opciones_restablecer.jpg)

### Inventario

No aplica: el juego no tiene inventario visible.

### Mapa

No aplica: el juego no tiene mapa.

### Victoria (fin del demo)

![Fin del demo](img/UI16_fin_del_demo.jpg)

### Derrota

![Derrota](img/UI14_derrota.jpg)

![Reiniciando desde el punto de control](img/UI15_derrota_reiniciando.jpg)


> **Navegación:** [[00 - Índice]] · ← [[03 - Entornos]] · [[05 - Audio y Música]] →
