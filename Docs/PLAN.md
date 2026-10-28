# Plan de trabajo — Carencia

Meta: demo jugable (prólogo, tres niveles y desenlace) en `main` con la etiqueta `v0.1.0`,
construido con escenas y objetos tradicionales, con gitflow, commits claros y los recursos de arte, audio y video
dentro del repositorio.

El trabajo se ordena en **fases**. Una fase termina cuando se cumple su criterio de cierre; dentro de cada fase, cada
integrante sigue sus pasos en el orden indicado. Cada paso es una rama `git flow feature start` → `git flow feature finish`
(ver [CONTRIBUTING.md](../CONTRIBUTING.md)).

## Integrantes

| Integrante | Rol | Áreas | Carpetas donde trabaja |
|---|---|---|---|
| Kristal Palma | Líder | Seguimiento en ClickUp y revisión contra el GDD | — (no usa el repositorio) |
| Ideny López y Melchor Ojeda | Programadores | `core`, `player`, `camara`, `interaccion`, `narrativa`, `enemigos`, `efectos`, `progreso`, `escenas` | `Scripts/` (salvo UI y Dialogue), `Scenes/`, `Settings/`, `Data/` |
| Zurisarai Delgado | UI/UX | `ui`, `dialogos` | `Scripts/UI`, `Scripts/Dialogue`, `UI/`, `Prefabs/UI`, `Data/Dialogos`, `Scenes/Menu` |
| Yareni Arellano | Arte | `arte` | `Sprites/`, `Tilemaps/`, `Animations/`, `Materials/`, `Prefabs/Escenarios`, `Prefabs/Personajes` |
| Armando Balam | Audio | `audio` | `Audio/` |
| Por confirmar | Video | `video` | `Videos/` |

Los programadores trabajan juntos sobre una sola lista de tareas: toman los pasos en orden y se reparten en el momento
quién abre cada rama. Si dos pasos se pueden hacer a la vez, están marcados con ⇄.

## Fases

| Fase | Objetivo | Se cierra cuando |
|---|---|---|
| 0. Preparación | Repositorio, ramas y equipos listos | Todos clonaron, abrieron el proyecto y cerraron una feature de prueba |
| 1. Base jugable | Sistemas que usan todos los niveles | En una escena de prueba Elaki camina, interactúa, lee un diálogo, cruza una puerta a otra escena y pausa |
| 2. Prólogo | Primer tramo completo | Del menú se llega al baño, se abre el gabinete y se cruza el espejo |
| 3. Nivel 1 | Primera ruptura y regreso a la realidad | Se cruza el segundo portal |
| 4. Nivel 2 | Persecuciones, daño y segundo despertar | Se sobreviven ambas persecuciones y se cruza el tercer portal |
| 5. Nivel 3 y desenlace | Final del demo | Se ve "Fin del demo" y se vuelve al menú |
| 6. Entrega | Versión `v0.1.0` en `main` | Build de Windows probado y `git flow release finish v0.1.0` subido |

Las fases 1 a 5 son de **funcionalidades nuevas** (`feature/`). En la fase 6 ya no entra nada nuevo: solo correcciones
dentro de `release/v0.1.0`.

## Programadores (Ideny y Melchor)

### Fase 0 — Preparación

1. Crear `develop`, subir la estructura y las convenciones (rama `feature/docs/estructura-y-convenciones`):
   ```bash
   git switch -c develop
   git push -u origin develop
   git flow init -d
   git flow feature start docs/estructura-y-convenciones
   git add Assets/_Carencia Assets/_Carencia.meta
   git commit -m "chore(proyecto): crea la estructura de carpetas de _Carencia"
   git add Docs/GDD
   git commit -m "docs(gdd): agrega copia del GDD al commit 9a56be4"
   git add CONTRIBUTING.md Docs/ESTRUCTURA.md Docs/PLAN.md README.md
   git commit -m "docs(proyecto): agrega convenciones de gitflow, estructura y plan"
   git flow feature finish docs/estructura-y-convenciones
   git push origin develop
   ```
2. En GitHub: invitar a los demás como colaboradores (`Settings → Collaborators`) y poner `develop` como rama por
   defecto (`Settings → General → Default branch`).
3. Configurar el proyecto: Input Actions en `Settings/` (WASD, flechas, Shift, E, Esc, clic), capas de orden de
   sprites y `Build Settings` vacío → `feature/core/configuracion-inicial`.

### Fase 1 — Base jugable

4. `feature/player/movimiento-wasd`: `PlayerController` con colisiones y velocidad constante en diagonal. ⇄ con 5.
5. `feature/core/estado-y-carga-de-escenas`: clase estática `EstadoJuego` (banderas, salud, punto de entrada) y
   `CargadorDeEscenas` con fundido; puerta que carga otra escena y coloca a Elaki en su punto de entrada.
6. `feature/interaccion/objetos-con-tecla-e`: `Interactable`, detección por proximidad, indicador "E + acción". ⇄ con 7.
7. `feature/camara/seguimiento-cercano`: cámara que sigue a Elaki y acercamiento al interactuar.
8. `feature/player/animaciones-8-direcciones`: Animator con las hojas de Yareni (requiere su paso 1).
9. `feature/core/prefab-sistemas`: `PF_Sistemas` con EventSystem, Canvas, `AudioSource` de ambiente y efectos de
   pantalla (fundido). Integrar la caja de diálogo de Zurisarai (requiere su paso 2).
10. `feature/interaccion/vista-de-inspeccion`: vista fija con objetos seleccionables con el ratón (gabinete).
11. `feature/progreso/guardado-json`: una ranura automática, puntos de control y botón Continuar del menú.

### Fase 2 — Prólogo

12. `feature/escenas/prologo`: escenas `PRO_Habitacion`, `PRO_Comedor`, `PRO_Bano` con los prefabs de escenario de
    Yareni y `PF_Sistemas`; agregarlas a Build Settings.
13. `feature/narrativa/eventos-prologo`: despertar y desayuno como cinemáticas (Timeline o corrutinas), el control
    empieza en el baño, gabinete obligatorio, espejo que se deforma y portal al nivel 1.

### Fase 3 — Nivel 1

14. `feature/escenas/nivel01`: `N1_Bano`, `N1_Pasillo`, `N1_Sala`, y las escenas de regreso a la realidad.
15. `feature/narrativa/eventos-nivel01`: voz femenina, puerta del fondo, desmayo, despertar, desayuno con el
    periódico, gotera, cortina en estado 1 y segundo portal.

### Fase 4 — Nivel 2

16. `feature/escenas/nivel02`: `N2_Pasillo`, `N2_Sala` y las escenas del segundo despertar.
17. `feature/efectos/manchas-de-dano` y `feature/player/salud`: daño con manchas rojas progresivas, sin barra de vida. ⇄ con 18.
18. `feature/narrativa/monitor-y-video`: Elaki se sienta, video con barra de progreso, la cámara se aleja y revela al
    perro (requiere el video del paso 1 de Video).
19. `feature/enemigos/perro-persecucion`: persecución que termina al llegar al pasillo.
20. `feature/progreso/derrota-y-reintento`: muerte y reintento desde el punto de control.
21. `feature/enemigos/margarita-persecucion`: aparición, transformación y persecución hasta el fondo del pasillo;
    gusanos sin daño.
22. `feature/narrativa/segundo-despertar`: habitación, desayuno con el periódico del 20 de noviembre, altar del perro,
    armario, cortina en estado 2 y tercer portal.

### Fase 5 — Nivel 3 y desenlace

23. `feature/escenas/nivel03`: `N3_Pasillo`, `N3_Sala`, `N3_Bano`.
24. `feature/narrativa/eventos-nivel03`: televisión, rastro de agua, calendario, puerta con corazón, Margarita en la
    bañera y cruce del último portal.
25. `feature/escenas/desenlace`: `DES_Carteles`, `DES_Noticiero` (hallazgo y 25 de diciembre), `DES_Edificio` y
    "Fin del demo".

### Fase 6 — Entrega

26. `git flow release start v0.1.0`. A partir de aquí solo correcciones, una por commit.
27. Jugar el demo completo con todo el equipo y corregir lo encontrado dentro de la rama de release.
28. Generar el build de Windows y probarlo fuera del editor.
29. Actualizar `README.md` (controles y créditos), cerrar con `git flow release finish v0.1.0` y
    `git push origin main develop --tags`.

## Zurisarai (UI/UX)

| Orden | Fase | Rama | Qué entrega |
|---|---|---|---|
| 1 | 1 | `feature/ui/menu-principal` | Escena `MENU_Principal`: Jugar, Continuar, Opciones, Créditos y Salir (la carga de escenas la conectan los programadores) |
| 2 | 1 | `feature/dialogos/caja-de-dialogo` | `DialogueBox` (nombre, texto que aparece poco a poco, avanzar con Espacio o clic) y el ScriptableObject `SO_Dialogo` |
| 3 | 1 | `feature/ui/indicador-de-interaccion` | Diseño del indicador "E + acción" y del aviso "Acércate más" |
| 4 | 2 | `feature/dialogos/datos-prologo` | Diálogo del desayuno y pensamientos del gabinete como `SO_Dialogo_*`, textos del GDD |
| 5 | 2 | `feature/ui/pausa` | Pausa con Esc: Reanudar, Opciones y Volver al menú principal (con confirmación) |
| 6 | 2 | `feature/ui/opciones-audio` | Volumen general, música, ambiente, efectos y voces (con el Mixer de Armando) |
| 7 | 3 | `feature/ui/opciones-accesibilidad` | Tamaño de texto, indicadores de sonido, reducir destellos y movimiento, restablecer valores |
| 8 | 3 | `feature/dialogos/datos-nivel01` | Diálogos del nivel 1 |
| 9 | 4 | `feature/ui/pantalla-derrota` | "No lograste escapar" con Reintentar |
| 10 | 4 | `feature/dialogos/datos-nivel02` | Pensamientos del segundo desayuno y textos del periódico y del armario |
| 11 | 5 | `feature/dialogos/datos-nivel03` | Diálogo de Margarita en la bañera y textos de los noticieros |
| 12 | 5 | `feature/ui/creditos-y-fin-del-demo` | Pantalla de créditos y pantalla "Fin del demo" |

## Yareni (Arte)

| Orden | Fase | Rama | Qué entrega |
|---|---|---|---|
| 1 | 1 | `feature/arte/sprites-protagonista` | Hojas de Elaki en 8 direcciones (caminar y quieto) en `Sprites/Personajes` |
| 2 | 1 | `feature/arte/tiles-realidad` | Tiles y paleta de la casa en la realidad (`PAL_R_Casa`) |
| 3 | 2 | `feature/arte/escenarios-prologo` | `PF_EscenarioPROHabitacion`, `PF_EscenarioPROComedor`, `PF_EscenarioPROBano` con muebles y colliders |
| 4 | 2 | `feature/arte/padres-y-gabinete` | Padres sentados, objetos del gabinete, espejo normal y deformado |
| 5 | 3 | `feature/arte/dimension1` | Tiles y escenarios de la dimensión 1; cortina en estado 1 |
| 6 | 4 | `feature/arte/dimension2` | Tiles y escenarios de la dimensión 2; televisión y monitor |
| 7 | 4 | `feature/arte/perro-y-margarita` | Perro deformado y Margarita (de espaldas, de frente, deformada) con animaciones |
| 8 | 4 | `feature/arte/segundo-despertar` | Madre cansada, periódico, altar del perro, armario; cortina en estado 2 |
| 9 | 5 | `feature/arte/dimension3` | Dimensión 3 orgánica: paredes de carne, ojos, corazón; cortina en estado 3 |
| 10 | 5 | `feature/arte/desenlace` | Carteles de desaparecidos y vista superior del edificio |

## Armando (Audio)

| Orden | Fase | Rama | Qué entrega |
|---|---|---|---|
| 1 | 1 | `feature/audio/mixer-y-base` | `Audio/Mixer/MIX_Carencia` con grupos Música, Ambiente, Efectos y Voces; `SFX_Pasos`, `SFX_Puerta_Abrir` |
| 2 | 1 | `feature/audio/musica-menu` | `MUS_Menu` |
| 3 | 2 | `feature/audio/prologo` | Ambiente de la casa, gabinete, palanca del escusado, gotera, latidos, respiración, murmullos |
| 4 | 3 | `feature/audio/nivel01` | Ambiente de la dimensión 1, voz femenina "ayuda", estática de televisión, golpe seco |
| 5 | 4 | `feature/audio/nivel02` | Ambiente de la dimensión 2, música de persecución, rugido, mordida, chillido, gusanos, llanto |
| 6 | 5 | `feature/audio/nivel03-y-desenlace` | Sonidos de carne, himno distorsionado, goteo con eco, corte abrupto, noticiero, sirenas |

## Video (responsable por confirmar)

| Orden | Fase | Rama | Qué entrega |
|---|---|---|---|
| 1 | 4 | `feature/video/monitor-nivel02` | `VID_N2_Monitor` (el video perturbador del monitor) |
| 2 | 5 | `feature/video/noticieros` | `VID_DES_Noticiero` (hallazgo de Margarita y noticia del 25 de diciembre) |
| 3 | 5 | `feature/video/edificio` | `VID_DES_Edificio` (vista superior con personas, sirenas y adornos navideños) |

Formato recomendado: MP4 (H.264), 1280×720, y que cada archivo pese lo menos posible por la cuota de Git LFS.

## Dependencias entre integrantes

| Paso que espera | Necesita antes |
|---|---|
| Programadores 8 (animaciones) | Yareni 1 |
| Programadores 9 (caja de diálogo integrada) | Zurisarai 2 |
| Programadores 12 (escenas del prólogo) | Yareni 3 |
| Programadores 18 (monitor) | Video 1 (mientras tanto, un video provisional) |
| Zurisarai 6 (opciones de audio) | Armando 1 (Mixer) |
| Programadores 21 (Margarita) | Yareni 7 |

Si algo no está listo, el paso que espera usa un recurso provisional y se sustituye después en una rama
`feature/<area>/...` del responsable del recurso.

## Rutina del equipo

- **Al empezar a trabajar:** `git pull` en `develop` y avisar en el grupo qué rama vas a abrir y **qué escenas vas a
  tocar**. Una escena tiene un solo dueño a la vez.
- **Al terminar un paso:** revisar la lista de la sección 7 de CONTRIBUTING.md, `git flow feature finish` y
  `git push origin develop`.

Prioridad de entrega, de mayor a menor:

1. Fases 1 y 2: base jugable, prólogo, menú y guardado.
2. Fase 3: nivel 1.
3. Fase 4: nivel 2 con ambas persecuciones.
4. Fase 5: nivel 3 y desenlace.
5. Opciones de accesibilidad, créditos, pulido visual y sonoro, videos definitivos.

Lo que no esté cerrado en `develop` al iniciar la fase 6 no entra en `v0.1.0` y queda para la siguiente versión.
