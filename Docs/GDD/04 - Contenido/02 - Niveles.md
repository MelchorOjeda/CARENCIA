# Niveles

## Descripción de la sección

Cada nivel debe documentar su propósito jugable, estructura, dificultad, contenido y condiciones de entrada/salida.

## Plantilla de nivel

### Prólogo — La realidad

**Objetivo principal:** Explorar el baño de la casa familiar, inspeccionar el gabinete y descubrir el portal que aparece en el espejo.

**Objetivos secundarios:**

- Reconocer los elementos del baño para identificar sus alteraciones en niveles posteriores.
- Inspeccionar objetos cotidianos y conocer los pensamientos del protagonista.
- Familiarizarse con el movimiento y el sistema de interacción.

**Duración estimada:** 2–3 minutos de exploración jugable, sin contar las cinemáticas iniciales.

**Dificultad:** Baja. Introducción a los controles y a la exploración.

**Mecánicas introducidas:**
- Movimiento del personaje mediante WASD o flechas.
- Interacción con objetos mediante la tecla E.
- Inspección de objetos mediante una vista fija en primera persona y selección con el mouse.
- Presentación de pensamientos y textos.
- Activación de eventos narrativos.
- Interacción con portales.

**Mecánicas utilizadas:** Exploración, observación, inspección de objetos e interacción con el entorno.

**Enemigos / obstáculos:** No existen enemigos. El principal obstáculo es descubrir qué interacción permite avanzar.

**Recompensas:** Información sobre el protagonista, primeras pistas narrativas y acceso a la primera dimensión.

**Puntos de guardado:** Inicio de la exploración del baño. Se recomienda registrar un punto de control al completar el prólogo.

**Condición de entrada:** Iniciar una nueva partida y completar las cinemáticas del despertar y el primer desayuno.

**Condición de salida:** Inspeccionar el gabinete y atravesar el portal del espejo.

**Eventos narrativos:**

1. El protagonista despierta en su habitación.
2. Durante el desayuno pregunta por Margarita, pero sus padres niegan su existencia.
3. Experimenta un mareo y se traslada al baño.
4. El jugador explora el baño real e inspecciona los objetos.
5. Al revisar el gabinete, descubre medicamentos y productos personales.
6. Después de cerrar el gabinete, observa una silueta femenina o alteración en el espejo.
7. Se manifiesta el primer portal y el protagonista lo atraviesa.

**Mapa / diagrama:**

```text
CINEMÁTICA: HABITACIÓN
          ↓
CINEMÁTICA: COMEDOR
          ↓
BAÑO REAL
    ├── Inodoro
    ├── Toallas
    ├── Productos de cuidado personal
    └── Gabinete (obligatorio)
          ↓
ALTERACIÓN DEL ESPEJO
          ↓
PRIMER PORTAL
          ↓
NIVEL 1
```

### Nivel 1 — La primera ruptura

**Objetivo principal:** Explorar la primera versión alterada de la casa y encontrar el portal que permite continuar la búsqueda de Margarita.

**Objetivos secundarios:**

- Comparar el baño con su versión original.
- Identificar cambios en las toallas, el inodoro y otros elementos.
- Observar el comportamiento de la cortina de la ducha.
- Investigar el pasillo y seguir las señales auditivas.

**Duración estimada:** ==🟣Pendiente de validar mediante pruebas de recorrido.==

**Dificultad:** Baja. El desafío se concentra en la exploración y la identificación de cambios.

**Mecánicas introducidas:**

- Comparación visual entre versiones de un mismo escenario.
- Exploración de espacios distorsionados.
- Seguimiento de pistas auditivas.
- Activación de eventos ambientales.

**Mecánicas utilizadas:** Movimiento, interacción, inspección, exploración y uso de portales.

**Enemigos / obstáculos:** No existen persecuciones. Los obstáculos incluyen un pasillo anormalmente largo, una ventana cubierta con tablas y una puerta bloqueada. Las alteraciones visuales pueden dificultar la orientación.

**Recompensas:** Nuevas pistas sobre las dimensiones y acceso a la siguiente versión de la casa.

**Puntos de guardado:** Entrada al nivel y punto de control al alcanzar la siguiente transición, sujetos a la implementación definitiva.

**Condición de entrada:** Atravesar el portal del espejo durante el prólogo.

**Condición de salida:** Recorrer el pasillo, alcanzar el evento narrativo final y atravesar el siguiente portal.

**Eventos narrativos:**

1. El protagonista aparece en un baño similar al anterior.
2. Descubre cambios en los colores y la apariencia de los objetos.
3. La cortina de la ducha presenta su primera alteración.
4. Observa diferencias en los productos y las etiquetas del gabinete.
5. Sale al pasillo, cuya longitud resulta anormal.
6. Encuentra una ventana cubierta y una puerta que no permite el paso.
7. Escucha una voz femenina de origen desconocido.
8. Sigue las señales hasta encontrar el siguiente portal.

**Mapa / diagrama:**

```text
BAÑO ALTERADO
    ├── Toallas modificadas
    ├── Inodoro amarillento
    ├── Gabinete distorsionado
    └── Cortina de ducha (estado 1)
          ↓
PASILLO PROLONGADO
    ├── Ventana cubierta
    ├── Puerta bloqueada
    └── Voz femenina
          ↓
SEGUNDO PORTAL
          ↓
NIVEL 2
```

### Nivel 2 — La deformación

**Objetivo principal:** Investigar la nueva dimensión de la casa, sobrevivir a las persecuciones y encontrar la forma de continuar hacia el siguiente nivel.

**Objetivos secundarios:**

- Examinar las fotografías familiares alteradas.
- Observar el paisaje de la ventana.
- Investigar las anomalías del comedor y la televisión.
- Interactuar con la computadora.
- Descubrir nuevas pistas después del segundo despertar.
- Revisar el periódico y los cambios en las habitaciones.

**Duración estimada:** ==🟣Pendiente de validar mediante pruebas, especialmente por la duración de las persecuciones.==

**Dificultad:** Media-alta. Es el primer nivel que incorpora amenazas directas y condiciones de derrota.

**Mecánicas introducidas:**

- Persecuciones y evasión de enemigos.
- Sistema de daño y efectos visuales al recibir ataques.
- Derrota y reinicio desde un punto de control.
- Activación de amenazas mediante eventos narrativos.

**Mecánicas utilizadas:** Movimiento, exploración, interacción, inspección, activación de eventos, evasión y transición entre escenarios.

**Enemigos / obstáculos:**

- Criatura canina deformada: persigue al protagonista después del evento de la computadora.
- Manifestación grotesca de Margarita: desencadena una segunda persecución.
- Pasillos y espacios que limitan las rutas de escape.
- Alteraciones ambientales que dificultan reconocer el entorno.

Las amenazas deben evitarse. No se contempla un sistema de combate.

**Recompensas:** Supervivencia, avance narrativo, nuevas pistas sobre la familia y acceso a la dimensión final del demo.

**Puntos de guardado:** Se recomienda establecer puntos de control antes de cada persecución y después de superarlas, para evitar repetir secuencias extensas.

**Condición de entrada:** Atravesar el portal al final del Nivel 1.

**Condición de salida:** Sobrevivir a ambas persecuciones, completar los acontecimientos del segundo despertar y atravesar el portal del baño.

**Eventos narrativos:**

1. El protagonista llega a un pasillo con una apariencia visual diferente.
2. Encuentra fotografías familiares alteradas, incluyendo una en la que aparece un perro.
3. Observa un paisaje extraño desde la ventana.
4. En el comedor descubre cuatro platos con comida deteriorada y una televisión con interferencias.
5. Interactúa con una computadora que reproduce un video perturbador.
6. Aparece la criatura canina y comienza la primera persecución.
7. Después de escapar, se encuentra con una figura que recuerda a Margarita.
8. La figura revela una apariencia grotesca y comienza la segunda persecución.
9. El protagonista despierta nuevamente en su habitación.
10. Durante el segundo desayuno observa a su madre con una apariencia cansada.
11. Encuentra un periódico del 20 de noviembre que informa sobre el aumento de las desapariciones.
12. Explora el altar del perro y el espacio donde recuerda que estaba la habitación de Margarita, ahora ocupado por un armario.
13. Regresa al baño y encuentra la cortina de la ducha en su segundo estado de alteración.
14. Atraviesa el siguiente portal.

**Mapa / diagrama:**

```text
PASILLO ALTERADO
    ├── Fotografías familiares
    └── Ventana con paisaje liminal
          ↓
SALA / COMEDOR
    ├── Cuatro platos deteriorados
    └── Televisión con interferencias
          ↓
COMPUTADORA
          ↓
VIDEO PERTURBADOR
          ↓
PERSECUCIÓN 1: CRIATURA CANINA
          ↓
ENCUENTRO CON MARGARITA DEFORMADA
          ↓
PERSECUCIÓN 2: MANIFESTACIÓN
          ↓
DESPERTAR EN LA REALIDAD
          ↓
SEGUNDO DESAYUNO
          ↓
EXPLORACIÓN DE LA CASA
    ├── Periódico
    ├── Altar del perro
    └── Armario
          ↓
BAÑO / CORTINA (ESTADO 2)
          ↓
TERCER PORTAL
          ↓
NIVEL 3
```

### Nivel 3 — La realidad se rompe

**Objetivo principal:** Recorrer la versión más deformada de la casa, encontrar a Margarita en la bañera y alcanzar la revelación final.

**Objetivos secundarios:**

- Explorar las alteraciones orgánicas del entorno.
- Observar los cambios de la televisión.
- Identificar los elementos relacionados con el 31 de octubre.
- Investigar los sonidos y las apariciones del pasillo.
- Reconocer la transformación definitiva del baño.

**Duración estimada:** ==🟣Pendiente de validar mediante pruebas de recorrido.==

**Dificultad:** Media. El nivel se centra en la tensión ambiental, la exploración y los eventos narrativos.

**Mecánicas introducidas:**

- Interacciones con elementos de horror corporal.
- Eventos ambientales de mayor intensidad.
- Secuencia narrativa final vinculada con la bañera.

**Mecánicas utilizadas:** Movimiento, exploración, interacción, observación, activación de eventos y transición mediante portal.

**Enemigos / obstáculos:** El entorno presenta paredes con apariencia orgánica, ojos, estructuras corporales y elementos perturbadores. No se ha definido una persecución adicional obligatoria para este nivel.

**Recompensas:** Revelación narrativa sobre Margarita y conclusión del demo.

**Puntos de guardado:** Entrada al nivel. Se recomienda un punto de control previo al encuentro final.

**Condición de entrada:** Completar el Nivel 2 y atravesar el portal del baño.

**Condición de salida:** Llegar a la bañera, completar el encuentro con la manifestación de Margarita y activar la secuencia del desenlace.

**Eventos narrativos:**

1. El protagonista aparece en una versión de la casa con paredes y estructuras orgánicas.
2. Observa alteraciones en la televisión y elementos visuales perturbadores.
3. Recorre un pasillo con agua, ojos y sonidos de llanto.
4. Puede observar una figura humana en una ventana, si este evento se conserva en la versión definitiva.
5. Encuentra referencias al 31 de octubre.
6. Llega a una puerta del baño asociada con la imagen de un corazón deformado.
7. Descubre que la cortina de la bañera está completamente abierta.
8. Observa una manifestación de Margarita dentro de la bañera.
9. La figura lo invita a acercarse y se activa la transición final.
10. Se muestran carteles de personas desaparecidas.
11. Una noticia televisiva revela que el cuerpo de Margarita fue encontrado en un lote baldío.
12. Finaliza el demo.

**Mapa / diagrama:**

```text
CASA ORGÁNICA
    ├── Paredes deformadas
    └── Televisión alterada
          ↓
PASILLO
    ├── Agua y ojos
    ├── Sonidos de llanto
    ├── Ventana (evento opcional)
    └── Calendario del 31 de octubre
          ↓
PUERTA DEL BAÑO
    └── Corazón deformado
          ↓
BAÑO FINAL
    ├── Cortina completamente abierta
    └── Margarita en la bañera
          ↓
TRANSICIÓN FINAL
          ↓
CARTELES DE DESAPARICIÓN
          ↓
NOTICIERO
          ↓
FIN DEL DEMO
```
## Flujo del nivel

```text
Inicio → Zona A → Desafío → Zona B → Recompensa → Final
```

```text
INICIO
   ↓
PRÓLOGO
Cinemáticas familiares
   ↓
Exploración e inspección del baño
   ↓
Portal del espejo
   ↓
NIVEL 1
Primera dimensión alterada
   ↓
Exploración y voz femenina
   ↓
Segundo portal
   ↓
NIVEL 2
Exploración de la casa deformada
   ↓
Evento de la computadora
   ↓
Persecución de la criatura canina
   ↓
Persecución de Margarita deformada
   ↓
Segundo despertar y exploración
   ↓
Tercer portal
   ↓
NIVEL 3
Exploración de la casa orgánica
   ↓
Encuentro en la bañera
   ↓
REVELACIÓN FINAL
Carteles y noticiero
   ↓
FIN DEL DEMO
```
## Criterios de calidad

- El objetivo es comprensible.
- Las reglas se pueden aprender.
- El desafío corresponde al nivel de progresión.
- Existe retroalimentación suficiente.
- El jugador entiende por qué tuvo éxito o fracaso.


> **Navegación:** [[00 - Índice]] · ← [[01 - Estructura del Contenido]] · [[03 - Enemigos y NPC]] →
