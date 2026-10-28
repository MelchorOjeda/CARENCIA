# Estructura del proyecto Carencia

## Raíz del repositorio

```
CARENCIA/
├── Assets/
│   └── _Carencia/          ← todo lo nuestro
├── Docs/
│   ├── GDD/                ← copia del GDD (ver Docs/GDD/LEEME.md)
│   ├── ESTRUCTURA.md       ← este archivo
│   └── PLAN.md             ← plan de trabajo por fases
├── CONTRIBUTING.md         ← gitflow, commits y nomenclatura
├── README.md
├── Packages/  ProjectSettings/
└── .gitignore  .gitattributes
```

`Docs/` está fuera de `Assets/` a propósito: Unity no lo importa (no crea `.meta` ni lo mete al build) y GitHub lo
muestra como documentación normal.

## Assets/_Carencia

```
Assets/_Carencia/
├── Animations/
│   ├── Personajes/           Elaki, padres, Margarita
│   ├── Enemigos/             perro deformado, manifestaciones
│   └── Objetos/              puertas, corazón, televisión
├── Audio/
│   ├── Ambientes/
│   ├── Efectos/
│   ├── Musica/
│   ├── Voces/
│   └── Mixer/               AudioMixer con grupos Música, Ambiente, Efectos, Voces
├── Data/                    ScriptableObjects con los datos del juego
│   ├── Dialogos/            textos de diálogos y pensamientos
│   ├── Objetos/             datos de objetos inspeccionables (gabinete, periódico…)
│   ├── Enemigos/            velocidad, alcance y daño de cada enemigo
│   └── Niveles/             datos de portales y puntos de control
├── Materials/
├── Prefabs/
│   ├── Personajes/           PF_Elaki, PF_Madre, PF_Padre
│   ├── Enemigos/            
│   ├── Interactuables/       puertas, gabinete, espejo, monitor
│   ├── Escenarios/           un prefab de escenario (Grid + Tilemaps + muebles) por habitación
│   ├── UI/                   caja de diálogo, menú de pausa, botones
│   └── Sistemas/             PF_Sistemas: cámara, audio, diálogos y canvas que lleva cada escena
├── Scenes/
│   ├── Menu/
│   ├── Prologo/
│   ├── Nivel01/
│   ├── Nivel02/
│   ├── Nivel03/
│   ├── Desenlace/
│   └── Pruebas/              escenas personales de prueba; no entran al build
├── Scripts/
│   ├── Core/                carga de escenas, estado del juego, constantes
│   ├── Player/
│   ├── Interaction/
│   ├── Camera/
│   ├── Enemies/
│   ├── Narrative/           eventos de cada nivel, banderas de historia
│   ├── Dialogue/             lógica de diálogos (la caja visual va en UI)
│   ├── Audio/
│   ├── UI/
│   ├── Effects/              fundidos, manchas de daño, espiral, parpadeo
│   ├── Progress/            guardado JSON y puntos de control
│   └── Editor/               herramientas de editor; Unity exige este nombre para que no entren al build
├── Settings/                 Input Actions del juego
├── Sprites/
│   ├── Personajes/          
│   ├── Enemigos/            
│   ├── Escenarios/           Realidad/, Dimension1/, Dimension2/, Dimension3/
│   ├── Objetos/             
│   └── Efectos/             
├── Tilemaps/
│   ├── Palettes/             paletas de tiles para pintar
│   └── Tiles/                assets de tile
├── UI/
│   ├── Sprites/              botones, paneles, iconos
│   └── Fuentes/              fuentes y sus Font Assets de TextMeshPro
└── Videos/
```

Por qué cada adición:

| Carpeta                   | Motivo                                                                                                                                           |
| ------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `Data/`                   | Los diálogos y valores ajustables viven como datos, no escritos dentro de los scripts. Zurisarai y los programadores los editan sin tocar código |
| `Scripts/Dialogue/`       | Separa la lógica de los diálogos (quién habla, en qué orden) de su presentación (`Scripts/UI/`)                                                  |
| `Scripts/Effects/`        | Los efectos de pantalla los usan varios niveles; no pertenecen a un solo sistema                                                                 |
| `Scripts/Editor/`         | Cualquier script que use `UnityEditor` debe estar en una carpeta llamada `Editor`, si no el build falla                                          |
| `Scenes/Pruebas/`         | Cada quien prueba en su propia escena y no toca las escenas reales: evita conflictos de Git                                                      |
| `Prefabs/Escenarios/`     | Yareni arma el escenario dentro de un prefab y los programadores colocan ese prefab en la escena; trabajan en archivos distintos                 |
| `Prefabs/Sistemas/`       | Sustituye al "bootstrap": un prefab con lo que toda escena necesita, colocado a mano en cada escena                                              |
| `Audio/Mixer/`            | Permite los volúmenes por categoría que pide el GDD (Opciones → Audio)                                                                           |
| `Settings/`               | El archivo de Input Actions (WASD, E, Esc, clic)                                                                                                 |
| `UI/Fuentes/`             | Las fuentes y sus Font Assets de TextMeshPro ocupan varios archivos                                                                              |
| Subcarpetas de `Sprites/` | El GDD tiene cuatro versiones de la casa; separar por dimensión evita carpetas con cientos de archivos                                           |

Cada carpeta vacía lleva un archivo `.gitkeep` para que Git la suba. Se puede borrar cuando la carpeta tenga contenido.

Fuera de `Assets/` **no** se suben archivos fuente pesados (proyectos `.flp` de FL Studio, `.kra` de Krita, proyectos de
CapCut). Los `.aseprite` sí pueden ir dentro de `Sprites/`: el paquete Aseprite Importer ya está instalado y Unity los
importa directamente.

## Arquitectura sin bootstrap: escenas y objetos tradicionales

1. **Cada escena es autosuficiente.** Contiene su `Main Camera`, el jugador (`PF_Elaki`), el prefab `PF_Sistemas`
   (EventSystem, Canvas con diálogo, pausa y efectos, AudioSource de ambiente) y el prefab de escenario de la habitación.
   Se puede abrir cualquier escena y pulsar Play.
2. **Una escena por habitación o momento.** Por ejemplo, `Prologo/` contiene `PRO_Habitacion`, `PRO_Comedor` y
   `PRO_Bano`. Las puertas cargan la escena destino con `SceneManager.LoadScene`.
3. **Lo que debe sobrevivir al cambio de escena** (banderas de la historia, salud, punto de entrada) se guarda en una
   clase estática `EstadoJuego` (`Scripts/Core`) y en el guardado JSON (`Scripts/Progress`). No se usa
   `DontDestroyOnLoad`.
4. **Build Settings** lista las escenas en orden: `MENU_Principal`, escenas del prólogo, nivel 1, nivel 2, nivel 3 y
   desenlace. Las de `Pruebas/` nunca se agregan.

