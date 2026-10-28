# Reglas y Estados del Juego

## Descripción de la sección

Define las reglas globales y los estados que puede atravesar el videojuego.

## Estados principales

- **Menú principal** 
Es el estado inicial del videojuego. Permite al jugador iniciar la experiencia y acceder a las opciones generales disponibles.
Mientras se encuentra en este estado, el protagonista no puede desplazarse ni interactuar con los escenarios.

- **Carga**
Estado temporal utilizado para preparar las escenas y los elementos necesarios antes de comenzar o continuar la partida.
Durante la carga, el jugador no puede controlar al protagonista. Una vez que la escena está preparada, el juego pasa automáticamente al estado correspondiente.

- **Jugando**
Es el estado principal de la experiencia. El jugador puede desplazarse mediante WASD o flechas direccionales, explorar los escenarios e interactuar con objetos cercanos mediante la tecla E.

Dentro de este estado pueden presentarse diferentes situaciones:
- **Exploración:** el protagonista se desplaza libremente por las zonas accesibles.
- **Interacción:** el jugador inspecciona objetos o activa acontecimientos del escenario.
- **Secuencia narrativa:** se ejecuta un acontecimiento que puede bloquear temporalmente el movimiento del protagonista y modificar el encuadre de la cámara.
- **Persecución:** un enemigo persigue al protagonista, quien debe escapar y evitar recibir daño.

- **Pausa**
Estado que se activa al presionar la tecla Esc en cualquier momento de la partida.

Mientras el juego está pausado:

- Se detiene el movimiento del protagonista.
- Se detiene el desplazamiento y comportamiento de los enemigos.
- Se suspenden las animaciones y acontecimientos que dependen del tiempo de juego.
- Se pausa la reproducción de videos y el audio asociado.
- No avanzan las persecuciones ni las secuencias narrativas.

Al reanudar, todos los sistemas continúan desde el momento en que fueron pausados.
La pausa también debe funcionar durante videos, encuentros narrativos y persecuciones.

- **Victoria**
Se alcanza cuando el jugador cumple las condiciones necesarias para completar un nivel o finalizar el videojuego.
En los niveles intermedios, la victoria permite activar la transición hacia el siguiente escenario.
Por ejemplo, sobrevivir a la persecución de la hermana y alcanzar el final del Nivel 2 permite avanzar al Nivel 3.
La victoria final ocurre únicamente cuando se completan los acontecimientos establecidos para el desenlace de la historia.

- **Derrota**
Se produce cuando el protagonista recibe suficiente daño durante una persecución y alcanza su límite de vida.

- **Game Over**
Representa el resultado de una derrota por muerte del protagonista.

Desde este estado se permite reiniciar la partida desde el punto de control establecido para recuperar la oportunidad de superar el desafío.

- **Créditos**
Aparecerán los créditos en una sección mostrada en el menú principal.

## Transiciones

| Estado origen                 | Evento                              | Estado destino                      | Condición                                                            |
| ----------------------------- | ----------------------------------- | ----------------------------------- | -------------------------------------------------------------------- |
| Menú principal                | Iniciar partida                     | Carga                               | El jugador selecciona la opción para comenzar.                       |
| Carga                         | Escena preparada                    | Jugando                             | Todos los elementos necesarios están listos.                         |
| Jugando                       | Presionar Esc                       | Pausa                               | La partida está activa, incluso durante una secuencia o persecución. |
| Pausa                         | Reanudar partida                    | Jugando                             | El jugador selecciona continuar.                                     |
| Jugando                       | Interactuar con un objeto narrativo | Jugando (secuencia narrativa)       | Se cumplen las condiciones de interacción del objeto.                |
| Jugando (secuencia narrativa) | Finalizar acontecimiento            | Jugando (exploración o persecución) | La secuencia concluye y corresponde devolver el control al jugador.  |
| Jugando                       | Completar un nivel                  | Victoria                            | Se cumple el objetivo narrativo o de supervivencia correspondiente.  |
| Victoria                      | Avanzar al siguiente nivel          | Carga                               | Existe un nivel posterior en la progresión.                          |
| Jugando                       | Vida agotada                        | Derrota                             | El protagonista recibe daño suficiente para morir.                   |
| Derrota                       | Finalizar respuesta de muerte       | Game Over                           | Se confirma la derrota del protagonista.                             |
| Game Over                     | Reiniciar                           | Carga                               | Se selecciona reiniciar desde el punto de control establecido.       |
| Jugando                       | Completar el desenlace              | Victoria                            | Se cumplen las condiciones del final del videojuego.                 |
| Victoria                      | Finalizar secuencia de cierre       | Créditos                            | Se ha completado el videojuego.                                      |
| Créditos                      | Finalizar o regresar                | Menú principal                      | Termina la presentación de créditos o se selecciona regresar.        |
## Reglas globales

1. **Movimiento:** el protagonista se controla mediante WASD o flechas direccionales. No se utiliza desplazamiento automático al hacer clic sobre el escenario.
2. **Interacción:** el jugador debe acercarse a un objeto interactivo y presionar E para inspeccionarlo o activar su función.
3. **Progresión lineal:** los niveles se completan en el orden establecido por la historia. No se permite acceder a niveles posteriores sin cumplir sus condiciones de avance.
4. **Activación de acontecimientos:** determinados eventos requieren que el protagonista alcance una zona específica o interactúe con un objeto concreto.
5. **Control durante secuencias:** algunas escenas narrativas bloquean temporalmente el movimiento del protagonista. El control se devuelve cuando el acontecimiento correspondiente finaliza.
6. **Cámara:** durante la exploración, la cámara mantiene una perspectiva isométrica fija respecto al escenario. En determinadas secuencias puede modificar temporalmente su encuadre y posteriormente regresar a su posición habitual.
7. **Persecuciones:** durante un encuentro con enemigos, el jugador debe desplazarse para escapar y evitar los ataques.
8. **Daño:** los ataques enemigos disminuyen la vida del protagonista y aumentan progresivamente las manchas rojizas alrededor de la pantalla.
9. **Muerte:** cuando el protagonista pierde toda su vida, se produce una derrota. El jugador debe reiniciar desde el punto de control correspondiente.
10. **Supervivencia obligatoria:** para completar el Nivel 2, el jugador debe sobrevivir a la persecución de la hermana. Morir durante ese encuentro no permite avanzar al Nivel 3.
11. **Pausa universal:** la tecla Esc permite pausar la partida incluso durante videos, persecuciones y secuencias narrativas. Todos los sistemas afectados deben detenerse de forma sincronizada.
12. **Recursos:** no existe inventario, economía, recolección de materiales ni mejoras de atributos. Los objetos del escenario funcionan principalmente como elementos interactivos o narrativos.
13. **Finalización:** el videojuego termina únicamente cuando se completa el desenlace establecido para el Nivel 3.
## Casos especiales

Documenta excepciones importantes para evitar interpretaciones diferentes durante la implementación.

**1. Pausa durante una secuencia narrativa**
Si el jugador presiona Esc mientras se reproduce una secuencia, esta debe detenerse junto con sus animaciones, movimientos de cámara y audio.
Al reanudar, la secuencia continúa desde el mismo momento, sin reiniciarse ni saltarse acontecimientos.

**2. Pausa durante una persecución**
Si el jugador pausa mientras un enemigo lo persigue, tanto el protagonista como el enemigo deben permanecer detenidos.
Al reanudar, la persecución continúa desde las posiciones y condiciones anteriores a la pausa.

**3. Interacción con el monitor**
Al interactuar con el monitor del Nivel 2, el protagonista se sienta y pierde temporalmente el control de movimiento.
El video comienza automáticamente y la cámara realiza los cambios de encuadre establecidos. Al finalizar la secuencia, el jugador recupera el control y comienza la persecución.
No debe permitirse que el protagonista se desplace mientras la secuencia mantiene bloqueado su movimiento.

**4. Interacción fuera de alcance**
Si el protagonista se encuentra demasiado lejos de un objeto interactivo, no puede activarlo.
El jugador debe acercarse hasta la distancia permitida para que aparezca la opción de interacción y pueda utilizar la tecla E.

**5. Daño durante las persecuciones**
El daño se representa visualmente mediante manchas rojizas progresivas.
Cuando se alcanza el límite de vida, se activa la derrota.

**6. Supervivencia durante la persecución de la hermana**
El jugador debe sobrevivir a la persecución y alcanzar el punto que marca el final del Nivel 2.
Si lo consigue, se activa la transición al Nivel 3.
Si muere, se considera una derrota y debe reiniciar desde el punto de control establecido.

**7. Transición entre niveles**
Al completar un nivel, el juego debe finalizar los acontecimientos activos, cargar la escena correspondiente y colocar al protagonista en la posición inicial definida para la siguiente etapa.
Durante este proceso no deben permanecer activos enemigos, interacciones o secuencias del nivel anterior.

**8. Activación repetida de acontecimientos**

Los acontecimientos narrativos que deben ocurrir una sola vez no pueden volver a activarse mientras se están ejecutando.
Esto evita reproducciones simultáneas de videos, duplicación de enemigos o transiciones repetidas entre niveles.



> **Navegación:** [[00 - Índice]] · ← [[06 - Economía y Recursos]] · [[01 - Premisa y Trama]] →
