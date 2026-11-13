# Audio y Música

## Descripción de la sección

Define la identidad sonora y cómo el audio comunica información al jugador.

La identidad sonora es **horror liminal y psicológico**. El juego arranca con sonidos domésticos reconocibles (ventilador, refrigerador, desayuno) que se deforman nivel a nivel: primero se vuelven extraños, luego deformados y al final orgánicos. El audio le avisa al jugador **qué tan lejos está de la realidad**, **cuándo está en peligro** y **dónde hay algo importante** (portales, voces, objetos).

Todo el audio se produce y masteriza en **FL Studio**. Se exporta en **MP3 (44.1 kHz, 320 kbps)** para el juego.

## Música

**Estilo:**
Dark ambient y drone para la exploración. Música de persecución rítmica y disonante, al estilo del horror de supervivencia. La música no tiene melodías alegres. Se apoya en texturas, clústeres disonantes y pulsos graves.

**Instrumentación:**
- **Drones y pads:** sintetizadores en FL Studio (Sytrus, Harmor, 3x Osc) con detune lento y armónicos inestables.
- **Cuerdas disonantes:** clústeres en Re (Re, Re#, La, La#) con sierras filtradas o cuerdas de FLEX y vibrato leve.
- **Percusión:** bombo grave sintetizado, toms graves y ticks metálicos, programados en el Channel Rack.
- **Bajo:** ostinato de sierra doble filtrada, con compuerta rítmica.
- **Texturas:** ruido filtrado, estática y risers de ruido.
- **Procesamiento:** Fruity Reeverb 2 / Fruity Convolver para espacios grandes, Gross Beat para tartamudeos y glitches, Fruity Parametric EQ 2, Fruity Limiter en el máster.

**Temas principales:**

| Tema | Uso | Duración | Carácter |
|---|---|---|---|
| Fondo musical inquietante | Menú principal | Loop de 20 s | Drone tenso que anticipa el tono del juego |
| Música de persecución 1 | Nivel 2: criatura canina | Loop de ~60 s, 140 BPM | Bombo insistente, bajo ostinato, clúster de cuerdas |
| Música de persecución 2 | Nivel 2: Margarita deformada | Loop de ~55 s, 165 BPM | Versión intensificada: más percusión, riser y armonía más densa |
| Música de revelación | Desenlace: carteles | 30–60 s, una sola vez | Revelación lenta, pads que se abren |
| Música de cierre | Cinemática final | 45–90 s, una sola vez | Resolución melancólica |
| Fondo de derrota | Pantalla de derrota | Loop de 15–30 s | Drone grave y apagado |
| Fondo de fin del demo | Pantalla final | Loop de 30–60 s | Ambiente tranquilo pero inquietante |

**Música dinámica:** Sí
- En la exploración no hay música. Solo suena el ambiente de cada nivel, que funciona como "música".
- Cuando empieza una persecución, el ambiente baja con un fade de ~1 s y entra la música de persecución en loop hasta escapar.
- La persecución 2 es la versión intensificada de la persecución 1. Mantiene la misma tonalidad (Re) para reforzar la escalada.
- Si el jugador escapa, la música sale con un fade de 2–3 s y vuelve el ambiente. Si lo atrapan, la música se corta en seco y suena el sonido de captura.
- Los loops están cortados en compases completos para que la música se repita sin saltos.

## Efectos de sonido

| Evento | Sonido | Propósito |
|---|---|---|
| Navegar o confirmar en menús | Selección y confirmación de botones (0.2–0.8 s) | Retroalimentación de la interfaz |
| Abrir pausa | Sonido de pausa; el ambiente se atenúa | Indicar que el juego se detuvo |
| Caminar | Pasos según la superficie (0.2–0.6 s por paso) | Presencia del jugador y sensación del espacio |
| Abrir o cerrar puertas | Rechinido y golpe de cierre (0.5–3 s) | Retroalimentación de la interacción |
| Interactuar con objetos | Toques, roces y manipulación (0.2–1.5 s) | Confirmar la acción |
| Avanzar diálogo | Sonido corto de texto (0.1–0.3 s) | Ritmo de lectura |
| Prólogo: desayuno | Cubiertos, plato, taza, cuchara y murmullo de los padres | Normalidad cotidiana (punto de partida) |
| Prólogo: gabinete | Abrir o cerrar, frascos, pastillas | Interacción y exploración del baño |
| Espejo / portales | Zumbido de portal en loop + tintineo | Señalar el objetivo y la transición |
| Activación del tercer portal | Subida tonal + impacto grave (6 s) | Momento clave del Nivel 2 |
| Nivel 1: voz misteriosa | Voz femenina distante (2–8 s) | Guiar y generar intriga |
| Nivel 1: distorsión | Glitch de nombre | Romper la realidad |
| Nivel 2: casa deformada | Crujidos, alteraciones del entorno | Tensión y sensación de que la casa "está viva" |
| Nivel 2: computadora | Encendido, mouse, teclado, reproducir video y estática | Retroalimentación diegética |
| Nivel 2: criatura canina | Sonido del enemigo | Avisar de la amenaza y de su posición |
| Nivel 2: Margarita deformada | Vocalización distorsionada y grito | Amenaza principal, impacto emocional |
| Nivel 2: periódico / altar | Hojear y desdoblar papel, tomar o dejar objeto, destello | Señalar la pista narrativa |
| Nivel 3: casa orgánica | Pulsaciones, chapoteos húmedos, latidos | Ambiente perturbador y corporal |
| Nivel 3: pasillo | Goteo, llanto distante | Guiar al jugador hacia el evento |
| Nivel 3: calendario | Sonido de descubrimiento | Marcar la pista importante |
| Nivel 3: puerta del baño | Latido del corazón deformado | Indicar cercanía al clímax |
| Atravesar la bañera | Distorsión de transición | Paso al desenlace |
| Captura / derrota | Golpe y corte de la persecución | Retroalimentación del fallo |

Los efectos con varias variantes (cubiertos, crujidos, pasos) se reproducen al azar, con una variación leve de tono (±5 %), para que no suenen repetitivos.

## Audio espacial

¿Se utilizará audio posicional? **Sí.**

- **3D (posicional):** ventilador, refrigerador, vecino con el pájaro, zumbidos de los portales, crujidos, goteo, llanto, voz femenina, criatura canina, Margarita, televisión, computadora y latido del corazón deformado. Estos sonidos sirven para orientar al jugador: puede ubicar el portal, la amenaza o la pista por el oído.
- **2D (no posicional):** música, ambientes de base de cada nivel, interfaz, pasos del jugador, voz de narración del noticiero y transiciones entre dimensiones.
- **Atenuación:** logarítmica, con un radio mínimo de 1–2 m y un máximo de 10–20 m según el objeto. Los portales y el latido tienen un radio mayor porque funcionan como guía.
- **Reverberación por zona:** baño corto y brillante, pasillo largo y lejano, casa orgánica oscura y húmeda.

## Voz

**Narración:** No hay narrador. La única voz tipo narración es el **audio del noticiero** del desenlace.

**Diálogos hablados:** Limitados. Los diálogos principales se muestran **en texto** con sonido de avance. Hay voz para:
- **Los padres en el desayuno:** murmullo de fondo, sin palabras entendibles.
- **La voz femenina distante:** vocalizaciones lejanas.
- **Margarita:** voz en el encuentro final. En su forma deformada, vocalizaciones distorsionadas.
- **El noticiero.**

Las líneas con palabras se graban con voz real y se procesan en FL Studio: Pitcher/NewTone para cambiar el tono, Vocodex o distorsión para la versión deformada, y reverberación según la escena.

**Idiomas:** Español (voz y texto). Los textos se preparan para traducirse más adelante. Las vocalizaciones sin palabras no necesitan traducción.

## Mezcla

**Prioridad (de mayor a menor):**
1. Eventos de peligro: criatura, Margarita, captura.
2. Voz y diálogo: Margarita, voz femenina, noticiero.
3. Señales de objetivo: portales, tintineos, descubrimientos.
4. Música de persecución y cinemáticas.
5. Efectos de interacción y pasos.
6. Ambientes de nivel.
7. Interfaz.

**Volumen relativo de referencia (en el bus máster):**

| Bus | Nivel |
|---|---|
| Voz / diálogo | 0 dB (referencia) |
| Enemigos y eventos de peligro | −2 dB |
| Música de persecución | −6 dB |
| Señales de objetivo (portales) | −8 dB |
| Efectos de interacción y pasos | −10 dB |
| Ambientes de nivel | −16 a −20 dB |
| Interfaz | −12 dB |

**Comportamiento ante eventos importantes:**
- **Persecución:** el ambiente baja 8–10 dB y entra la música de persecución. Al terminar, todo vuelve en 2–3 s.
- **Voz o diálogo:** ducking de 6 dB sobre la música y el ambiente mientras se escucha la voz.
- **Pausa:** el ambiente del nivel se atenúa con un filtro de paso bajo, y se mantiene solo la interfaz.
- **Captura:** corte inmediato de la música y sonido de derrota a volumen completo.
- **Transiciones entre dimensiones:** fade cruzado de 1–2 s entre el ambiente que sale y el que entra, cubierto por el sonido de distorsión.
- **Límite de voces:** máximo ~24 sonidos simultáneos. Los de menor prioridad (ambiente, pasos lejanos) se descartan primero.

**Máster:** todo el audio se exporta desde FL Studio en MP3, con picos máximos de −1 dBFS. Los volúmenes finales se ajustan por bus dentro del juego.

> **Navegación:** [[00 - Índice]] · ← [[04 - Interfaz Visual]] · [[01 - UX y Flujo del Jugador]] →
