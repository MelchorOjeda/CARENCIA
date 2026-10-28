# UX y Flujo del Jugador

## Descripción de la sección

UX (experiencia de usuario) describe qué tan comprensible, cómoda y coherente resulta la interacción con el videojuego.

## Flujo principal

```text
Inicio → Menú → Nueva partida → Tutorial → Juego → Pausa → Juego → Final
```

```text
INICIO DEL JUEGO
       ↓
MENÚ PRINCIPAL
       ├── Jugar / Iniciar nueva partida
       ├── Continuar (retomar progreso guardado)
       ├── Opciones
       └── Salir
       ↓
JUGAR
       ↓
CINEMÁTICAS INICIALES
Despertar y primer desayuno
       ↓
PRÓLOGO — TUTORIAL INTEGRADO
Explorar el baño
Aprender movimiento e interacción
Inspeccionar el gabinete
       ↓
PRIMER PORTAL
       ↓
NIVEL 1 — EXPLORACIÓN
Reconocer alteraciones
Seguir pistas ambientales
       ↓
SEGUNDO PORTAL
       ↓
NIVEL 2 — EXPLORACIÓN Y PELIGRO
Investigar la casa
Interactuar con la computadora
       ↓
PRIMERA PERSECUCIÓN
Escapar de la criatura canina
       ↓
SEGUNDA PERSECUCIÓN
Escapar de Margarita deformada
       ↓
SEGUNDO DESPERTAR
Explorar la casa real
       ↓
TERCER PORTAL
       ↓
NIVEL 3 — HORROR Y REVELACIÓN
Explorar la casa orgánica
Encontrar a Margarita en la bañera
       ↓
DESENLACE
Carteles de desaparición
Noticiero sobre Margarita
       ↓
FIN DEL DEMO
       ↓
MENÚ PRINCIPAL
```

```text
JUEGO / CINEMÁTICA / PERSECUCIÓN
             ↓
         PRESIONAR ESC
             ↓
         MENÚ DE PAUSA
             ├── Reanudar
             ├── Opciones
             └── Volver al menú principal
```

```text
PERSECUCIÓN
     ↓
EL ENEMIGO ALCANZA AL JUGADOR
     ↓
RECIBE DAÑO
     ├── Puede continuar → Sigue escapando
     │
     └── Alcanza el límite de daño
                    ↓
                 DERROTA
                    ↓
          REINICIAR DESDE
          PUNTO DE CONTROL
```
## Primeros minutos

Documenta exactamente qué descubre y aprende el jugador.

Los primeros minutos deben enseñar las mecánicas principales sin interrumpir innecesariamente la experiencia narrativa.
El tutorial se integra directamente en el prólogo, utilizando el baño familiar como primer espacio de exploración.

## Retroalimentación

Para cada acción importante define:

**Entrada → Respuesta visual → Respuesta sonora → Resultado**

|Entrada|Respuesta visual|Respuesta sonora|Resultado|
|---|---|---|---|
|Presionar WASD o flechas|El personaje se desplaza y reproduce su animación de movimiento.|Pasos acordes con el escenario.|Movimiento del protagonista.|
|Acercarse a un objeto interactivo|Aparece una indicación discreta de interacción.|Sin sonido obligatorio.|El jugador reconoce que puede interactuar.|
|Presionar E frente a un objeto|Se activa la animación, vista o texto correspondiente.|Sonido relacionado con el objeto, cuando corresponda.|Interacción ejecutada.|
|Seleccionar un objeto del gabinete|Se destaca o muestra información del elemento seleccionado.|Sonido sutil de selección, si se implementa.|Presentación de un pensamiento o descripción.|
|Activar un portal|Se reproduce la distorsión o transición visual.|Efecto sonoro asociado al portal.|Cambio de dimensión o escena.|
|Acercarse a una amenaza|Se muestra la aparición o movimiento del enemigo.|Sonido de aparición y aumento de tensión.|Inicio o desarrollo de una persecución.|
|Recibir daño|Aparecen manchas rojas o un efecto visual de impacto.|Sonido de daño.|El protagonista acumula daño.|
|Alcanzar el límite de daño|Se muestra la transición de derrota.|Sonido de derrota o interrupción del ambiente.|Reinicio desde el punto de control.|
|Completar una persecución|El jugador alcanza la salida o se activa el siguiente evento.|Disminuye o cambia el sonido de peligro.|Continúa la historia.|
|Presionar Esc|Aparece el menú de pausa.|Sonido discreto de apertura, si se implementa.|La experiencia queda pausada.|
|Reanudar la partida|Desaparece el menú de pausa.|Regresa el audio de la escena.|Continúa la experiencia.|
|Activar una cinemática|Cambia la presentación visual al evento correspondiente.|Diálogos, música o efectos de la escena.|Avance narrativo.|
## Fricción intencional

¿Qué dificultad forma parte del diseño y qué dificultad es simplemente mala experiencia de usuario?

La dificultad intencional consiste en explorar escenarios desconocidos, identificar alteraciones en la casa, interpretar pistas y escapar de los enemigos durante las persecuciones. Estas dificultades buscan generar miedo, tensión e incertidumbre.

Por otro lado, un  mala experiencia de usuario ocurre cuando el jugador no puede avanzar debido a controles confusos, objetos interactivos difíciles de identificar, rutas de escape poco claras, errores de colisión o enemigos imposibles de evitar.
## Descubrimiento

#### ¿Cómo sabe el jugador qué hacer?

El videojuego utiliza objetivos contextuales, elementos ambientales y acontecimientos narrativos.
Durante el prólogo se introducen indicaciones breves para enseñar los controles. Posteriormente, el jugador debe reconocer los elementos relevantes mediante su ubicación, apariencia y relación con la historia.

#### ¿Cómo sabe dónde ir?

La orientación se apoya en la distribución de los escenarios y en señales visuales o auditivas.
Por ejemplo, el pasillo del Nivel 1 conduce al siguiente acontecimiento narrativo, mientras que las persecuciones deben contar con una ruta de escape reconocible.
Los espacios bloqueados y las transiciones deben comunicar claramente cuándo existe o no un camino disponible.

#### ¿Cómo sabe qué puede interactuar?

Cuando el protagonista se encuentre suficientemente cerca de un objeto interactivo, aparecerá una indicación visual discreta, como la tecla E.
En las vistas fijas de inspección, el mouse permitirá seleccionar los elementos disponibles.
Los objetos exclusivamente decorativos no deben mostrar indicadores de interacción.

#### ¿Cómo sabe qué ocurrió?

Las acciones importantes producen respuestas visuales, sonoras o narrativas.

Por ejemplo:

Al inspeccionar un medicamento, aparece un pensamiento del protagonista.
Al activar un portal, el escenario cambia mediante una transición.
Al reproducir el video de la computadora, se desencadena la aparición de la criatura canina.
Al recibir daño, aparecen efectos visuales que indican el impacto.
Al finalizar una persecución, cambia el estado de peligro y continúa la historia.

#### ¿Cómo sabe por qué ganó o perdió?

En el juego, el éxito consiste en completar los objetivos narrativos, superar los obstáculos y sobrevivir a las amenazas.

Durante una persecución, el jugador debe reconocer cuándo recibe daño y cuándo alcanza una zona segura.

Si pierde, la pantalla de derrota debe comunicar que el protagonista fue alcanzado y permitir reiniciar desde el punto de control correspondiente.

Al completar el demo, la secuencia del noticiero y la pantalla final deben comunicar que el recorrido ha concluido.

> **Navegación:** [[00 - Índice]] · ← [[05 - Audio y Música]] · [[02 - UI y Menús]] →
