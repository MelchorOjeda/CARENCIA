# Sistemas del Juego

## Descripción de la sección

Los sistemas son conjuntos de reglas y componentes que trabajan juntos: combate, inventario, IA, diálogo, progresión, economía, misiones, etc.

## Inventario de sistemas

| Sistema                  | Propósito                                                                                                                                                | Dependencias                                     | Estado   |
| ------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ | -------- |
| Movimiento del personaje | Desplazamiento mediante WASD o flechas direccionales.                                                                                                    | Colisiones, animaciones y estados del personaje. | Definido |
| Interacción              | Detectar objetos cercanos y permitir examinarlos o activarlos mediante una opción de interacción                                                         | Proximidad, interfaz y eventos.                  | Definido |
| Cámara isométrica        | Realizar acercamientos a objetos que requieran observación para obtener información y recuperar la vista habitual, en el área donde se ubique en jugador | Movimiento, escenarios y eventos narrativos.     | Definido |
| Secuencias               | Coordinar eventos jugables, semijugables y cinemáticos.                                                                                                  | Movimiento, cámara, animaciones, video y audio.  | Definido |
| Video                    | Reproducir automáticamente contenido en el monitor y mostrar su progreso.                                                                                | Interacción, cámara, interfaz y audio.           | Definido |
| Persecución              | Controlar el seguimiento, comportamiento y ataques de los enemigos.                                                                                      | Movimiento, navegación, colisiones y daño.       | Definido |
| Daño visual              | Representar el deterioro del personaje mediante una mancha rojas progresivas alrededor de la pantalla.                                                   | Enemigos, colisiones, interfaz y secuencias.     | Definido |
| Iluminación              | Modificar la visibilidad y reforzar la distorsión de los escenarios.                                                                                     | Escenarios, cámara y eventos.                    | Definido |
| Audio                    | Reproducir sonidos ambientales, murmullos y sonido ambiental de fondo.                                                                                   | Eventos, escenarios y enemigos.                  | Definido |
| Progreso del juego       | Gestionar el reinicio de fases o puntos de control.                                                                                                      | Progresión, estados del personaje y escenas.     | Definido |

## Plantilla de sistema

### Sistema 01
### Nombre: Sistema del movimiento del personaje

**Objetivo:** Permitir que el jugador controle directamente el desplazamiento del protagonista dentro de los escenarios 2D isométricos mediante las teclas WASD o flechas direccionales, tanto durante la exploración como durante las persecuciones.

**Entradas:** 
- Teclas W, A, S, D o flechas direccionales.
- Clic izquierdo sobre una posición transitable del escenario.
- Estado actual del protagonista.
- Colisiones y límites del escenario.

**Procesamiento / reglas:** 
- El personaje se desplaza de acuerdo con la dirección indicada por el jugador.
- El movimiento debe adaptarse a la perspectiva isométrica.
- Las paredes, muebles y demás obstáculos sólidos limitan el desplazamiento.
- El jugador debe acercarse manualmente a los objetos para poder interactuar con ellos.
- Hacer clic sobre un objeto no provoca desplazamiento automático.
- Durante determinadas secuencias narrativas, el movimiento se bloquea temporalmente.
- Durante las persecuciones, el jugador conserva el control para escapar y esquivar enemigos.
- La velocidad de movimiento debe permitir un desplazamiento fluido y preciso.

**Salidas:** 
- Actualización de la posición del protagonista.
- Animaciones de movimiento.

**Estados posibles:**
- Inactivo.
- Caminando.
- Corriendo.
- Bloqueado por evento narrativo.

**Dependencias:** Colisiones, animaciones, escenarios y estados del personaje.

**Interacciones:** Se relaciona con el sistema de "Interacción", ya que la posición del protagonista determina qué objetos pueden inspeccionarse. También proporciona información a la cámara y a los enemigos durante las persecuciones.

**Datos que necesita:** Posición actual, dirección de entrada, límites del escenario y estado de control.

**Datos que genera:** Nueva posición, dirección de desplazamiento y estado de movimiento.

**Riesgos:**
- Atascamiento en obstáculos.
- Controles bloqueados después de una secuencia narrativa.

**Criterios de aceptación:**
- El protagonista puede desplazarse mediante WASD y flechas direccionales.
- El movimiento corresponde correctamente a la perspectiva isométrica.
- El personaje no atraviesa obstáculos sólidos.
- El jugador puede acercarse manualmente a los objetos interactivos.
- El movimiento se bloquea y restaura correctamente durante eventos narrativos.
- El protagonista puede desplazarse y evadir enemigos durante las persecuciones.

### Sistema 02
### Nombre: Interacción

**Objetivo:** Permitir que el jugador inspeccione, examine o active objetos del escenario cuando el protagonista se encuentre a una distancia adecuada, mostrando una opción de interacción únicamente cuando se cumplan las condiciones necesarias.

**Entradas:** 
- Posición del protagonista.
- Proximidad a objetos interactivos.
- Clic sobre la opción de interacción o tecla indicada.
- Estado del objeto.
- Estado narrativo actual.

**Procesamiento / reglas:** 
- Cada objeto interactivo dispone de un área de detección.
- Cuando el protagonista entra en dicha área, aparece una opción visual para interactuar, por ejemplo, "E - Inspeccionar".
- La opción desaparece cuando el personaje se aleja del objeto.
- El jugador debe activar manualmente la interacción.
- La acción ejecutada depende del objeto: inspeccionar, abrir, activar, leer o iniciar un evento.
- Al inspeccionar un objeto, puede mostrarse un texto, diálogo, imagen ampliada o información relevante.
- Algunos objetos pueden activar secuencias narrativas especiales, como el monitor.
- La interacción no provoca desplazamiento automático del personaje.

**Salidas:** 
- Visualización de información del objeto.
- Inicio de diálogos, videos o secuencias narrativas.

**Estados posibles:**
- Fuera de alcance.
- Disponible para interactuar.
- Interacción activa.
- Interacción completada.
- Bloqueada.

**Dependencias:** Movimiento, detección de proximidad, interfaz y secuencias narrativas.

**Interacciones:** Recibe la posición del protagonista desde el sistema de movimiento. Se comunica con la interfaz para mostrar la opción de inspección y con el sistema de secuencias para activar eventos cuando corresponda.

**Datos que necesita:** Identificador del objeto, posición, distancia de interacción, acción disponible, estado del objeto y estado del protagonista.

**Datos que genera:** Objeto detectado, disponibilidad de interacción, acción ejecutada y evento activado.

**Riesgos:**
- Opciones de interacción que aparecen a distancias incorrectas.
- Dificultad para seleccionar objetos cercanos entre sí.
- Indicadores que permanecen visibles después de alejarse.
- Interacciones activadas durante persecuciones.
- Activación repetida de eventos narrativos.

**Criterios de aceptación:**
- La opción de interacción aparece cuando el protagonista está suficientemente cerca.
- El indicador desaparece al alejarse.
- El jugador puede inspeccionar objetos mediante la acción indicada.
- Las interacciones no desplazan automáticamente al protagonista.
- Cada objeto ejecuta la acción que le corresponde.
- Los eventos especiales se activan correctamente.
- Las interacciones se bloquean cuando el estado del juego lo requiere.

### Sistema 03
### Nombre: Cámara isométrica

**Objetivo:** Mostrar los escenarios desde una perspectiva isométrica y adaptar el encuadre durante la exploración y los eventos narrativos, especialmente durante la reproducción del video en el monitor.

**Entradas:** 
- Posición del protagonista.
- Evento narrativo activo.
- Objetivo de enfoque.
- Parámetros de acercamiento y transición.

**Procesamiento / reglas:** 
- Durante la exploración, la cámara se mantiene fija al escenario conservando la perspectiva isométrica.
- Debe permitir visualizar el espacio completo en el que el personaje se está desplazando e interactuando.
- Al interactuar con el monitor o televisión u otro elemento, que ayude a la trama, la cámara realiza un acercamiento gradual.
- El acercamiento debe permitir observar el video sin cambiar a una perspectiva en primera persona.
- Conforme avanza el video, la cámara puede alejarse progresivamente para revelar una presencia en el escenario.
- Al finalizar la secuencia, recupera su encuadre habitual.
- Los cambios de cámara no deben interferir con el movimiento durante las persecuciones.

**Salidas:** 
- Visualización del escenario mediante un encuadre isométrico fijo.
- Acercamientos y alejamientos de cámara.
- Enfoque de objetos o acontecimientos narrativos.
- Transiciones suaves entre encuadres.
- Restauración de la vista habitual.

**Estados posibles:**
- Acercamiento.
- Enfoque narrativo.
- Alejamiento.
- Retorno al encuadre habitual.

**Dependencias:** 
Sistema de movimiento, interacción y secuencias narrativas .

**Interacciones:** 
- Movimiento: permite mantener al protagonista dentro del área visible del escenario, sin que la cámara siga constantemente su desplazamiento.
- **Interacción:** cuando el jugador activa un objeto especial, como el monitor, puede solicitarse un cambio de cámara mediante una secuencia.
- **Secuencias:** controla cuándo se ejecutan los acercamientos, alejamientos y cambios de enfoque.
- **Video:** sincroniza los cambios de encuadre con la reproducción del contenido audiovisual.
- **Iluminación:** complementa los cambios de cámara para ocultar o revelar elementos durante eventos de terror.

**Datos que necesita:** 
- Posición del protagonista.
- Posición del objetivo de enfoque.
- Nivel de zoom.
- Velocidad y duración de las transiciones.
- Estado de la secuencia narrativa.
- Límites del escenario.

**Datos que genera:** 
- Posición actual de la cámara.
- Nivel de acercamiento.
- Estado del encuadre.

**Riesgos:**
- Que el contenido del monitor no sea suficientemente visible.
- Movimientos bruscos que afecten la experiencia.
- Revelar enemigos antes del momento narrativo previsto.
- Perder de vista al protagonista durante las persecuciones.
- Que la cámara no regrese al seguimiento normal después de una secuencia.

**Criterios de aceptación:**
- La cámara mantiene la perspectiva isométrica durante la exploración.
- Mantiene un encuadre fijo durante la exploración, permitiendo visualizar el escenario y los desplazamientos del protagonista.
- Al interactuar con el monitor, realiza el acercamiento correspondiente.
- Durante el video, ejecuta el alejamiento progresivo para revelar la presencia de la criatura.
- Los movimientos de cámara son suaves y están sincronizados con los eventos narrativos.
- El protagonista siempre permanece visible.
- Al finalizar las secuencias, la cámara recupera su funcionamiento habitual.

### Sistema 04
### Nombre: Secuencias

**Objetivo:** Coordinar los acontecimientos narrativos que modifican temporalmente el control del protagonista, la cámara, las animaciones y el entorno.

**Entradas:** 
- Interacciones con objetos.
- Progreso del jugador.
- Finalización de videos o animaciones.
- Eventos de enemigos y muerte.

**Procesamiento / reglas:** 
- Cada secuencia comienza cuando se cumple su condición de activación.
- Algunas secuencias bloquean temporalmente el movimiento.
- Durante los eventos semijugables, el jugador inicia una acción, pero el sistema controla su desarrollo.
- Las secuencias pueden activar diálogos, animaciones, sonidos, efectos y cambios de cámara.
- En el encuentro con la hermana, el protagonista debe observar su aparición y transformación antes de que comience la persecución.
- Al finalizar una secuencia, se habilitan únicamente los controles correspondientes al siguiente estado.
- Las secuencias no deben ejecutarse simultáneamente si sus acciones son incompatibles.

**Salidas:** 
- Bloqueo o restauración de controles.
- Cambios de cámara.
- Activación de enemigos.
- Transiciones entre estados y escenarios.

**Estados posibles:**
- Inactiva.
- Preparando.
- En ejecución.
- Esperando finalización.
- Completada.

**Dependencias:** Movimiento, cámara, animaciones, audio, video y progresión.

**Interacciones:** Coordina los sistemas principales durante eventos como la reproducción del monitor, la aparición de la hermana, la deformación del escenario y el despertar.

**Datos que necesita:** Identificador del evento, condiciones de activación, duración, acciones programadas y estado del jugador.

**Datos que genera:** Estado de la secuencia y eventos completados.

**Riesgos:**
- Animaciones desincronizadas.
- Transiciones incorrectas entre escenas.

**Criterios de aceptación:**
- Las secuencias se activan en el momento establecido.
- Los controles se bloquean cuando corresponde.
- Los eventos audiovisuales se ejecutan en el orden correcto.
- El jugador recupera el control cuando comienza una persecución o al finalizar la reproducción de un video.
### Sistema 05
### Nombre: Video

**Objetivo:** Reproducir automáticamente un video narrativo cuando el protagonista interactúa con el monitor, utilizando el contenido audiovisual para generar tensión y preparar la aparición de una amenaza.

**Entradas:** 
- Interacción con el monitor.
- Archivo de video.
- Archivo o pista de audio.
- Estado de la secuencia narrativa.

**Procesamiento / reglas:** 
- El jugador debe interactuar voluntariamente con el monitor.
- Al hacerlo, el protagonista se coloca automáticamente frente a él y se sienta.
- La reproducción comienza sin requerir otra acción.
- El movimiento permanece bloqueado durante la reproducción.
- La cámara se acerca al monitor.
- Conforme avanza el video, la cámara se aleja gradualmente y permite distinguir una presencia silenciosa en el escenario.
- Al finalizar el evento, se activa la reacción de la amenaza y se habilita la huida.
- El jugador no puede cancelar manualmente la reproducción durante esta secuencia.

**Salidas:** 
- Reproducción audiovisual.
- Activación de eventos de cámara.
- Activación de la amenaza y transición a persecución

**Estados posibles:**
- Inactivo.
- Preparando reproducción.
- Reproduciendo.
- Completado.

**Dependencias:** Interacción, cámara, secuencias, interfaz y audio.

**Interacciones:** Se relaciona con la cámara para controlar el acercamiento, con las secuencias para bloquear el movimiento y para activar la amenaza al finalizar el evento.

**Datos que necesita:** Archivo de video, duración y estado de reproducción.

**Datos que genera:** Porcentaje de reproducción, estado del video y señal de finalización.

**Riesgos:**
- Problemas de compatibilidad del formato de video.
- Desincronización de audio e imagen.
- Video poco visible desde la cámara isométrica.
- Fallos al desbloquear el movimiento.
- Activación incorrecta de la persecución.

**Criterios de aceptación:**
- El video inicia automáticamente al interactuar con el monitor.
- El protagonista permanece sentado durante la reproducción.
- La barra de progreso refleja el avance del video.
- La cámara ejecuta los acercamientos y alejamientos.
- La amenaza aparece en el momento narrativo previsto.
- La secuencia termina permitiendo al jugador escapar.

### Sistema 06
### Nombre: Persecución

**Objetivo:** Controlar el comportamiento de los enemigos durante los encuentros de peligro, permitiendo que persigan al protagonista y reaccionen a su posición.

**Entradas:** 
- Activación del evento de persecución.
- Posición del protagonista.
- Posición del enemigo.
- Obstáculos y límites del escenario.

**Procesamiento / reglas:** 
- Los enemigos permanecen inactivos hasta que se activa su evento correspondiente.
- Al comenzar una persecución, el enemigo identifica la posición del protagonista y se desplaza hacia él.
- El seguimiento debe respetar los obstáculos del escenario.
- La persecución de la primera criatura termina cuando el protagonista alcanza la zona establecida del pasillo.
- Durante el encuentro posterior, la hermana es la amenaza principal y persigue directamente al protagonista.
- Los gusanos de su secuencia funcionan como elementos ambientales y no deben causar daño directo.
- La hermana puede utilizar diferentes animaciones de ataque si los recursos artísticos y técnicos lo permiten.

**Salidas:** 
- Movimiento del enemigo.
- Animaciones de persecución.
- Eventos de daño.
- Finalización o cambio de estado de la persecución.

**Estados posibles:**
- Inactivo.
- Aparición.
- Alerta.
- Persiguiendo.
- Atacando.
- Finalizando persecución.

**Dependencias:** Movimiento, navegación, colisiones, animaciones, secuencias y daño. 

**Interacciones:** Utiliza la posición generada por el sistema de movimiento, recibe activaciones del sistema de secuencias y comunica los ataques al sistema de daño.

**Datos que necesita:** Posiciones del enemigo y protagonista, velocidad, distancia de ataque, ruta disponible, estado narrativo y condiciones de finalización.

**Datos que genera:** Estado del enemigo, posición actualizada, ataques ejecutados y eventos de persecución.

**Riesgos:**
- Enemigos atascados en obstáculos.
- Ataques demasiado frecuentes.
- Persecuciones imposibles de superar.
- Movimientos rígidos que reduzcan la sensación de amenaza.
- Activación prematura o tardía de enemigos.

**Criterios de aceptación:**
- Los enemigos comienzan a perseguir únicamente cuando corresponde.
- Pueden seguir al protagonista dentro del escenario.
- La primera persecución concluye al alcanzar el pasillo.
- Los gusanos no dañan al jugador.
- La dificultad permite reaccionar y esquivar sin eliminar la tensión.

### Sistema 07
### Nombre: Daño visual

**Objetivo:** Representar el deterioro del protagonista durante las persecuciones mediante una superposición rojiza progresiva, comunicando el peligro sin depender de una barra de vida tradicional.

**Entradas:** 
- Eventos de ataque de enemigos.
- Colisiones válidas.
- Cantidad de daño recibido.
- Estado actual del protagonista.


**Procesamiento / reglas:** 
- Cuando un enemigo ejecuta un ataque válido, se registra el daño recibido.
- Cada golpe incrementa la intensidad de las manchas rojas en la pantalla.
- El efecto visual se acumula progresivamente.
- Conforme aumenta el daño, la visibilidad del escenario disminuye.
- Al alcanzar el límite establecido, se activa el estado de "muerte", en el cual se reiniciará el juego en el nivel que perdió..

**Salidas:** 
- Actualización de daño acumulado.
- Superposición rojiza en pantalla.
- Señal de muerte.
- Activación de transición o reinicio correspondiente.

**Estados posibles:**
- Sin daño.
- Daño leve.
- Daño moderado.
- Daño crítico.
- Muerte (reinicio de nivel)

**Dependencias:** persecución, colisiones, interfaz y secuencias.

**Interacciones:** Recibe los ataques de los enemigos, actualiza los efectos visuales y comunica al sistema de secuencias cuando debe comenzar el evento de reinicio de nivel.

**Datos que necesita:** Daño acumulado, límite de daño, intensidad visual correspondiente y estado del protagonista.

**Datos que genera:** Nuevo valor de daño, evento de "muerte".

**Riesgos:**
- Muerte activada incorrectamente.
- Falta de claridad sobre el estado del protagonista.

**Criterios de aceptación:**
- Los ataques válidos incrementan el daño.
- La pantalla muestra manchas rojas progresivas.
- Los efectos reflejan correctamente el estado del personaje.
- Al alcanzar el límite de daño se activa la muerte.
- La transición posterior ocurre sin errores.
- El efecto mantiene la tensión sin perjudicar excesivamente la jugabilidad.

### Sistema 08
### Nombre: Iluminación

**Objetivo:** Controlar la visibilidad de los escenarios para reforzar la atmósfera de terror psicológico y comunicar las alteraciones de la realidad.

**Entradas:** 
- Nivel actual.
- Posición del protagonista.
- Estado narrativo.
- Eventos de transformación.
- Configuración de iluminación del escenario.

**Procesamiento / reglas:** 
- En los escenarios de aparente normalidad, la iluminación permite observar el entorno con claridad.
- Durante la primera fase liminal, el escenario puede mantenerse relativamente visible para destacar su vacío y amplitud.
- En las fases deformadas se introduce una penumbra que limita parcialmente la visibilidad.
- La zona cercana al protagonista debe conservar suficiente iluminación para permitir el desplazamiento.
- La penumbra no debe funcionar como un círculo de linterna demasiado marcado.
- Determinados eventos pueden modificar la iluminación para revelar u ocultar elementos.
- Durante el video del monitor, la iluminación contribuye a ocultar inicialmente la presencia de la criatura.

**Salidas:** 
- Cambios en la visibilidad del escenario.
- Efectos de penumbra.
- Revelación progresiva de elementos.
- Variaciones de ambiente.

**Estados posibles:**
- Iluminación normal.
- Iluminación liminal.
- Penumbra.
- Iluminación de evento.
- Transición.

**Dependencias:** Escenarios, cámara y secuencias.

**Interacciones:** Responde a la progresión narrativa y trabaja con la cámara para controlar qué elementos puede observar el jugador.

**Datos que necesita:** Nivel, intensidad de iluminación, posición del protagonista, zonas iluminadas y evento activo.
**Datos que genera:** Estado de iluminación, intensidad aplicada y cambios de visibilidad.

**Riesgos:**
- Oscuridad excesiva.
- Pérdida de visibilidad de objetos importantes.

**Criterios de aceptación:**
- Los escenarios normales y deformados presentan diferencias visuales reconocibles.
- El protagonista conserva visibilidad suficiente para desplazarse.
- La penumbra genera tensión sin impedir la exploración.
- Los cambios de iluminación se sincronizan con los eventos.
- Los enemigos no se revelan antes del momento previsto.

### Sistema 09
### Nombre: Audio

**Objetivo:** Generar tensión psicológica mediante sonidos ambientales, murmullos, efectos de persecución y señales auditivas que acompañen los acontecimientos del juego.

**Entradas:** 
- Nivel actual.
- Posición del protagonista.
- Eventos narrativos.
- Estado de los enemigos.
- Reproducción del video.

**Procesamiento / reglas:** 
- Cada escenario reproduce los sonidos ambientales correspondientes.
- Durante la exploración pueden aparecer murmullos y ruidos inesperados.
- El video del monitor reproduce su audio sincronizado con la imagen.
- El audio posicional o diferenciado entre canales izquierdo y derecho se considera una mejora opcional, sujeta a pruebas técnicas.
- Los efectos deben equilibrarse para que no oculten diálogos o sonidos importantes.

**Salidas:** 
- Sonidos ambientales.
- Voces y diálogos.
- Efectos de terror.
- Audio de video.
- Señales auditivas de peligro.

**Estados posibles:**
- Ambiente normal.
- Ambiente liminal.
- Tensión.
- Evento narrativo.
- Persecución.
- Silencio narrativo.

**Dependencias:** Escenarios, secuencias, video e IA de persecución.

**Interacciones:** Recibe información de eventos narrativos, cambios de escenario y activación de enemigos para reproducir los sonidos correspondientes.

**Datos que necesita:** Archivos de audio, volumen, duración, evento asociado, estado narrativo y, cuando corresponda, posición de la fuente sonora.

**Datos que genera:** Estado de reproducción y finalización de sonidos.

**Riesgos:**
- Efectos demasiado fuertes o repetitivos.
- Desincronización con animaciones y videos.
- Sonidos que se reproducen en momentos incorrectos.

**Criterios de aceptación:**
- Los sonidos se reproducen durante los eventos correctos.
- El audio del video está sincronizado.
- Los murmullos y efectos ambientales refuerzan la tensión.
- Los sonidos no impiden comprender los diálogos.
- El juego funciona correctamente incluso si se descartan los efectos de audio posicional avanzados.
### Sistema 10
### Nombre: Progreso del juego

**Objetivo:** Gestionar el progreso del jugador y permitir el reinicio desde un estado válido cuando ocurre una muerte o interrupción, evitando la repetición innecesaria de contenido.

**Entradas:** 
- Progreso narrativo.
- Nivel o fase actual.
- Evento de muerte.
- Solicitud de reinicio.

**Procesamiento / reglas:** 
- El sistema debe identificar la fase y los eventos completados.
- En caso de muerte, el juego ejecuta la consecuencia narrativa o el reinicio correspondiente.
- Al reiniciar, los sistemas de movimiento, enemigos, daño, cámara y secuencias deben recuperar un estado válido.
- La persistencia del progreso entre sesiones y la ubicación definitiva de los puntos de control.

**Salidas:** 
- Registro de progreso.
- Restauración de fase.
- Reinicio del protagonista.
- Recuperación de estados de los sistemas.

**Estados posibles:**
- Sin progreso registrado.
- Guardando.
- Restaurando.
- Reiniciando fase.

**Dependencias:** Progresión, escenarios, estados del personaje y secuencias.

**Interacciones:** Recibe información de la progresión y coordina la restauración del movimiento, cámara, enemigos, daño y eventos narrativos.

**Datos que necesita:** Nivel actual, punto de control, eventos completados, posición de reinicio y estados necesarios para restaurar la fase.

**Datos que genera:** Registro de progreso, estado restaurado y confirmación de reinicio.

**Riesgos:**
- Reiniciar en una posición incorrecta.
- Repetir eventos que deberían permanecer completados.

**Criterios de aceptación:**
- El jugador se reinicia desde el punto establecido cuando no logra pasar de nivel. 
- El personaje aparece en una posición válida.
- Los sistemas recuperan sus estados correspondientes.
- Las secuencias narrativas obligatorias se ejecutan correctamente.
- El reinicio no provoca bloqueos ni errores de progresión.
## Relaciones entre sistemas 

Describe qué sistemas dependen de otros y qué información intercambian.

| Sistema                      | Sistemas de los que depende o con los que se relaciona                           | Información que intercambian                                                                                                                                              |
| ---------------------------- | -------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Movimiento del personaje** | Interacción, cámara isométrica, secuencias y persecución.                        | Proporciona la posición, dirección y estado de movimiento del protagonista. Recibe órdenes para bloquear o restaurar los controles durante eventos narrativos.            |
| **Interacción**              | Movimiento del personaje, secuencias y video.                                    | Recibe la posición del protagonista para detectar objetos cercanos. Comunica qué objeto se seleccionó y qué acción o evento debe activarse.                               |
| **Cámara isométrica**        | Interacción, secuencias, video e iluminación.                                    | Recibe eventos que requieren modificar el encuadre. Intercambia parámetros de acercamiento, alejamiento, enfoque y restauración de la vista habitual.                     |
| **Secuencias**               | Interacción, movimiento, cámara, video, persecución, audio y progreso del juego. | Recibe eventos activados y comunica cuándo bloquear controles, reproducir animaciones, modificar la cámara, activar enemigos o finalizar una secuencia.                   |
| **Video**                    | Interacción, secuencias, cámara y audio.                                         | Recibe la orden de reproducción y comunica su estado, avance y finalización para sincronizar los cambios de cámara, sonido y eventos narrativos.                          |
| **Persecución**              | Movimiento del personaje, secuencias y daño visual.                              | Recibe la posición del protagonista y la orden de iniciar o terminar la persecución. Comunica ataques y colisiones válidas al sistema de daño.                            |
| **Daño visual**              | Persecución, secuencias y progreso del juego.                                    | Recibe los ataques del enemigo, actualiza el daño acumulado y comunica cuándo debe activarse la muerte y el reinicio del nivel.                                           |
| **Iluminación**              | Cámara isométrica y secuencias.                                                  | Recibe información sobre el escenario y los eventos narrativos para modificar la intensidad de luz, la penumbra y la visibilidad de determinados elementos.               |
| **Audio**                    | Secuencias, video, persecución y progreso del juego.                             | Recibe información sobre eventos, reproducción de video, apariciones y persecuciones para activar los sonidos correspondientes.                                           |
| **Progreso del juego**       | Secuencias, movimiento, persecución y daño visual.                               | Recibe información del nivel actual, eventos completados y muerte del protagonista. Comunica las condiciones necesarias para reiniciar el nivel y restaurar los sistemas. |

> **Navegación:** [[00 - Índice]] · ← [[02 - Mecánicas]] · [[04 - Controles]] →
