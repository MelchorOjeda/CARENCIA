# Cómo trabajamos en el repositorio de Carencia

Reglas para los cinco integrantes que suben cambios (Ideny, Melchor, Zurisarai, Yareni y Armando).
Kristal coordina el proyecto y no trabaja en el repositorio.

## 1. Antes de empezar (una sola vez)

1. Instala **Unity 6000.6.3f1** exactamente. Con otra versión Unity reescribe archivos y genera conflictos.
2. Instala Git y Git LFS, y activa LFS una vez:
   ```bash
   git lfs install
   ```
3. Clona el repositorio y entra a la rama `develop`:
   ```bash
   git clone https://github.com/MelchorOjeda/CARENCIA.git
   cd CARENCIA
   git switch develop
   ```
4. Inicia git flow con los valores por defecto (prefijos `feature/`, `bugfix/`, `release/`, `hotfix/`):
   ```bash
   git flow init -d
   ```
5. Configura tu nombre y correo:
   ```bash
   git config user.name "Nombre Apellido"
   git config user.email "tu-correo@ejemplo.com"
   ```

## 2. Gitflow

| Rama | Para qué sirve | Quién la crea | Se une a |
|---|---|---|---|
| `main` | Solo versiones entregables (con etiqueta `v0.1.0`, `v0.2.0`…) | Programadores, al cerrar una release | — |
| `develop` | Integración; siempre debe abrir y compilar | — | — |
| `feature/<area>/<descripcion>` | Una funcionalidad o un paquete de recursos | Cualquiera | `develop` |
| `bugfix/<area>/<descripcion>` | Corregir un error que está en `develop` | Cualquiera | `develop` |
| `release/vX.Y.Z` | Preparar una entrega: solo correcciones, sin funciones nuevas | Programadores | `main` y `develop` |
| `hotfix/<descripcion>` | Corregir un error urgente de una versión en `main` | Programadores | `main` y `develop` |

**Nadie hace commits directamente en `main` ni en `develop`.** Todo empieza con `git flow ... start` y termina con
`git flow ... finish`, que fusiona la rama en su destino y la borra. No se usan Pull Requests.

### Ciclo de una feature

```bash
# 1. Empezar desde develop actualizado
git switch develop
git pull
git flow feature start player/movimiento-wasd          # crea feature/player/movimiento-wasd

# 2. Trabajar con commits pequeños (ver sección 4)
git add <archivos>
git commit -m "feat(player): agrega movimiento con WASD y flechas a velocidad constante"

# 3. Opcional: publicar la rama para respaldarla o para que otro integrante la pruebe
git flow feature publish player/movimiento-wasd

# 4. Antes de cerrar: traer lo último de develop a tu rama y comprobar que el proyecto abre sin errores
git switch develop
git pull
git switch feature/player/movimiento-wasd
git merge develop                                       # si hay conflictos, se resuelven aquí, en tu rama

# 5. Cerrar la feature y subir develop
git flow feature finish player/movimiento-wasd         # fusiona en develop y borra la rama local
git push origin develop
git push origin --delete feature/player/movimiento-wasd   # solo si la publicaste en el paso 3
```

Las correcciones siguen el mismo ciclo con `git flow bugfix start|finish <area>/<descripcion>`.

Si `git push origin develop` es rechazado porque otro integrante subió antes, haz `git pull` en `develop`, comprueba
que el proyecto abre y vuelve a hacer `git push`.

### Ciclo de una entrega (solo programadores)

```bash
git switch develop
git pull
git flow release start v0.1.0
# solo correcciones; actualizar la versión en Project Settings > Player > Version
git flow release finish v0.1.0          # fusiona en main y en develop, y crea la etiqueta v0.1.0
git push origin main develop --tags
```

Un error urgente en `main` se corrige con `git flow hotfix start <descripcion>` y `git flow hotfix finish <descripcion>`,
y se sube igual que la entrega.

## 3. Nombres de ramas

Formato: `tipo/area/descripcion-en-minusculas-con-guiones`. Una rama = una cosa que se pueda explicar en una frase.

| Área | Responsables | Ejemplos |
|---|---|---|
| `core` | Programadores | `feature/core/carga-de-escenas`, `feature/core/estado-del-juego` |
| `player` | Programadores | `feature/player/movimiento-wasd`, `feature/player/animaciones-8-direcciones` |
| `camara` | Programadores | `feature/camara/seguimiento-cercano` |
| `progreso` | Programadores | `feature/progreso/guardado-json`, `feature/progreso/puntos-de-control` |
| `interaccion` | Programadores | `feature/interaccion/objetos-con-tecla-e`, `feature/interaccion/vista-de-inspeccion` |
| `narrativa` | Programadores | `feature/narrativa/eventos-prologo`, `feature/narrativa/eventos-nivel02` |
| `enemigos` | Programadores | `feature/enemigos/perro-persecucion`, `feature/enemigos/margarita-persecucion` |
| `efectos` | Programadores | `feature/efectos/manchas-de-dano` |
| `escenas` | Programadores | `feature/escenas/prologo-habitacion` |
| `ui` | Zurisarai | `feature/ui/menu-principal`, `feature/ui/opciones-audio`, `feature/ui/caja-de-dialogo` |
| `dialogos` | Zurisarai | `feature/dialogos/datos-prologo` |
| `arte` | Yareni | `feature/arte/sprites-protagonista`, `feature/arte/tiles-bano-realidad` |
| `audio` | Armando | `feature/audio/ambientes-nivel01`, `feature/audio/efectos-persecucion` |
| `video` | Responsable de video | `feature/video/video-monitor-nivel02` |
| `docs` | Cualquiera | `feature/docs/convenciones` |

Programadores: Ideny López y Melchor Ojeda, que trabajan juntos en todas las áreas de código. Antes de empezar una
feature, avisen cuál toma cada uno para no abrir la misma.

Las correcciones usan el mismo formato: `bugfix/player/atraviesa-paredes`, `bugfix/ui/boton-continuar-desactivado`.

## 4. Mensajes de commit

Formato (basado en Conventional Commits, en español):

```
tipo(area): qué cambia, en imperativo y en una línea

Opcional: por qué se hizo y qué hay que saber.
```

| Tipo | Cuándo |
|---|---|
| `feat` | Funcionalidad nueva del juego |
| `fix` | Corrige un error |
| `art` | Sprites, tiles, animaciones o materiales |
| `audio` | Música, ambientes, efectos o voces |
| `video` | Videos y cinemáticas |
| `scene` | Cambios de armado en una escena (colocar objetos, conectar referencias) |
| `refactor` | Reorganiza código sin cambiar lo que hace |
| `docs` | Documentación |
| `chore` | Configuración del proyecto, paquetes, carpetas |

Reglas:

- **Un commit = un cambio con sentido propio.** Ni "avances" con 40 archivos mezclados, ni un commit por cada línea.
- **La primera línea dice qué cambia y dónde**, entre 30 y 72 caracteres, sin punto final.
- **Prohibido:** "cambios", "fix", "avance", "update", "ya quedó", "asdf".
- Si arreglas algo, di qué fallaba: `fix(player): evita que atraviese las paredes en diagonal`.
- Los recursos se suben con su commit propio y su `.meta`.

Ejemplos buenos:

```
feat(player): agrega movimiento con WASD y flechas a velocidad constante
fix(interaccion): oculta el indicador al alejarse del objeto
art(escenarios): agrega tiles del baño en la realidad
audio(nivel01): agrega ambiente del pasillo y voz femenina
scene(prologo): coloca muebles y puertas en la habitación
refactor(narrativa): separa las banderas del prólogo en EstadoJuego
docs(readme): explica cómo abrir el proyecto
```

## 5. Reglas de Unity para no romper el proyecto

1. **Siempre sube el `.meta` junto con su archivo.** Sin el `.meta` se pierden las referencias en las escenas de los demás.
2. **Mueve, renombra y borra assets desde Unity**, no desde el explorador de Windows.
3. **Una escena tiene un solo dueño a la vez.** Avisa en el grupo antes de abrir y guardar una escena que no es tuya.
   Las escenas casi no se pueden fusionar en Git.
4. **Trabaja en prefabs.** El escenario de cada habitación (Grid y Tilemaps) es un prefab de arte; la escena solo lo
   coloca. Así arte y programación no tocan el mismo archivo.
5. **Para probar, usa tu escena en `Scenes/Pruebas/`** (por ejemplo `PRB_Melchor_Interaccion.unity`). No entran al build.
6. **No subas** `Library/`, `Temp/`, `Logs/`, `UserSettings/` ni compilaciones. El `.gitignore` ya los excluye.
7. **Archivos grandes:** audio, video, imágenes y PDF pasan por Git LFS automáticamente (`.gitattributes`).
   La cuota gratuita de LFS en GitHub es limitada: no subas versiones intermedias, archivos de proyecto de FL Studio
   ni exportaciones sin comprimir que no vaya a usar el juego.
8. **Antes de cada commit**, abre el proyecto y comprueba que la consola no tenga errores rojos.

## 6. Nomenclatura de archivos

| Tipo | Formato | Ejemplo |
|---|---|---|
| Escena | `<NIVEL>_<Lugar>` | `PRO_Habitacion`, `N1_Pasillo`, `N2_Sala`, `N3_Bano`, `DES_Noticiero`, `MENU_Principal` |
| Escena de pruebas | `PRB_<Nombre>_<Tema>` | `PRB_Ideny_Movimiento` |
| Script | `PascalCase`, igual que la clase | `PlayerController.cs`, `InteraccionObjeto.cs` |
| Prefab | `PF_<Nombre>` | `PF_Elaki`, `PF_PuertaBano`, `PF_EscenarioN1Pasillo` |
| Sprite / hoja | `SPR_<Categoria>_<Nombre>_<Variante>` | `SPR_PJ_Elaki_Caminar`, `SPR_OBJ_Gabinete_R` |
| Tile | `TILE_<Dimension>_<Nombre>` | `TILE_R_PisoBano`, `TILE_D3_ParedCarne` |
| Paleta de tiles | `PAL_<Dimension>_<Lugar>` | `PAL_R_Bano` |
| Animación / Animator | `ANIM_<Personaje>_<Accion>` / `AC_<Personaje>` | `ANIM_Elaki_Caminar_NE`, `AC_Elaki` |
| Material | `MAT_<Nombre>` | `MAT_SpriteIluminado` |
| Audio | `AMB_`, `SFX_`, `MUS_`, `VOZ_` + nivel + nombre | `AMB_N1_Pasillo`, `SFX_Puerta_Abrir`, `MUS_Menu`, `VOZ_N1_Ayuda` |
| Video | `VID_<Nivel>_<Nombre>` | `VID_N2_Monitor`, `VID_DES_Noticiero` |
| Datos (ScriptableObject) | `SO_<Tipo>_<Nombre>` | `SO_Dialogo_PrologoDesayuno`, `SO_Enemigo_Perro` |
| Sprite de interfaz | `UI_<Nombre>` | `UI_BotonNormal`, `UI_CajaDialogo` |

Abreviaturas de dimensión: `R` realidad, `D1`, `D2`, `D3`. Abreviaturas de nivel: `PRO`, `N1`, `N2`, `N3`, `DES`, `MENU`.
Sin espacios, acentos ni ñ en los nombres de archivo (`Bano`, no `Baño`).

## 7. Antes de `git flow feature finish`

Como `finish` fusiona directo en `develop`, cada quien revisa su propia rama antes de cerrarla:

- [ ] Traje lo último de `develop` a mi rama (paso 4 del ciclo) y resolví los conflictos ahí.
- [ ] El proyecto abre sin errores rojos en la consola.
- [ ] La escena que toqué se juega y lo que hice funciona.
- [ ] Los archivos nuevos tienen su `.meta` y sus nombres siguen la sección 6.
- [ ] Mis commits siguen la sección 4 (si no, corrígelos antes con `git commit --amend` o `git rebase -i`).
- [ ] Si mi feature toca código de otra área, avisé a quien la usa antes de cerrarla.

Arte, audio y video: si no estás segura o seguro de cómo quedó la importación en Unity, publica la rama
(`git flow feature publish`) y pide a un programador que la pruebe antes de cerrarla.
