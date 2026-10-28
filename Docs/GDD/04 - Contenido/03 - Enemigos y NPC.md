# Enemigos y NPC

## Descripción de la sección

Documenta las entidades controladas por el juego y su comportamiento.

## Plantilla de enemigo

### Nombre: Criatura canina deformada

**Rol de combate:** Enemigo de persecución. Representa una amenaza física que obliga al jugador a desplazarse y evitar el contacto.

**Comportamiento:** Su aparición se activa después de que el protagonista interactúa con la computadora y observa un video perturbador.

La criatura comienza a perseguirlo por el escenario. El jugador debe desplazarse por la ruta disponible y evitar recibir suficientes ataques como para provocar su derrota.

**Vida:** No aplica. La criatura no puede ser eliminada mediante combate.

**Daño:** Provoca daño al protagonista al alcanzarlo o atacarlo. Cada impacto debe producir una señal visual, como manchas rojas en la pantalla.

**Defensas:** No aplica. El jugador no dispone de ataques que puedan dañarla.

**Velocidad:** Debe permitir una persecución desafiante, pero superable mediante el movimiento del protagonista. Su valor definitivo se establecerá mediante pruebas.

**Rango de detección:** Su persecución se activa por un evento narrativo, no por un sistema de patrullaje. 

**Ataques:** Contacto o ataque de corto alcance que provoca daño. No se han definido ataques a distancia.

**Debilidades:** No presenta vulnerabilidades de combate. La forma de superar el encuentro es escapar y alcanzar la zona segura o el evento que finaliza la persecución.

**Recompensa:** Superar la primera persecución y continuar con los acontecimientos del Nivel 2.

**Estados de IA:**

```text
Patrullar → Detectar → Perseguir → Atacar
                  ↓
                Perder objetivo → Buscar
```

```text
INACTIVO
   ↓
Evento de la computadora
   ↓
APARICIÓN
   ↓
PERSEGUIR AL JUGADOR
   ├── Jugador al alcance
   │       ↓
   │     ATACAR
   │       ├── Jugador sobrevive → PERSEGUIR
   │       └── Jugador derrotado → REINICIAR
   │
   └── Jugador alcanza la salida
           ↓
     FINALIZAR PERSECUCIÓN
```

### Nombre: Manifestación grotesca de Margarita

**Rol de combate:** Antagonista principal en su versión hostil. Enemigo de persecución que combina horror psicológico y corporal.

**Comportamiento:** Aparece después de la primera persecución, inicialmente como una figura femenina que recuerda a Margarita.

Su apariencia se transforma y revela rasgos grotescos. Posteriormente comienza a perseguir al protagonista, quien debe escapar para continuar con la historia.

**Vida:** No aplica. No puede ser derrotada mediante ataques.

**Daño:** Puede dañar al protagonista durante la persecución. Los impactos producen retroalimentación visual y pueden provocar la derrota si se alcanza el límite establecido.

**Defensas:** No aplica. No existe un sistema de combate contra esta manifestación.

**Velocidad:** Debe generar una sensación de peligro mayor que la primera persecución, sin hacer imposible la evasión. Los valores definitivos se ajustarán mediante pruebas.

**Rango de detección:** La persecución comienza mediante un evento narrativo. No se contempla patrullaje previo.

**Ataques:** Ataque por contacto. 

**Debilidades:** Puede morir con cualquier momento en el que el los enemigos lo ataquen al alcanzarlo. 

**Recompensa:** Supervivencia y activación del siguiente nivel, que permite continuar con la historia.

**Estados de IA:**

```text
INACTIVA
   ↓
Encuentro con figura femenina
   ↓
REVELACIÓN / TRANSFORMACIÓN
   ↓
PERSEGUIR AL JUGADOR
   ├── Jugador al alcance
   │       ↓
   │     ATACAR
   │       ├── Jugador sobrevive → PERSEGUIR
   │       └── Jugador derrotado → REINICIAR
   │
   └── Jugador completa la huida
           ↓
     FINALIZAR PERSECUCIÓN
           ↓
     SEGUNDO DESPERTAR
```
## Plantilla de NPC 

El juego no contempla NPC convencionales con diálogos interactivos, misiones secundarias, comercio o sistemas de acompañamiento.

Sin embargo, existen personajes controlados por el juego que participan en las cinemáticas y los eventos narrativos.

**Rol:** Madre

**Función:** Personaje narrativo que participa en las conversaciones familiares y contribuye al misterio sobre Margarita.

**Ubicación:** Comedor de la casa familiar, durante las secuencias de desayuno.

**Interacción:** Participación mediante cinemáticas. No cuenta con un sistema de conversación libre.

**Diálogos:** Niega que el protagonista haya tenido una hermana y mantiene una actitud evasiva.

**Condiciones:** Aparece durante los eventos narrativos del primer y segundo desayuno.

**Recompensas / información:** Introduce contradicciones entre los recuerdos del protagonista y las afirmaciones de su familia.


**Rol:** Padre del protagonista. 

**Función:** Personaje narrativo que refuerza la negación de la existencia de Margarita.

**Ubicación:** Comedor de la casa familiar.

**Interacción:** Participación mediante cinemáticas, sin diálogo interactivo.

**Diálogos:** Respuestas breves y evasivas ante las preguntas del protagonista.

**Condiciones:** Aparece durante las secuencias familiares establecidas por la historia.

**Recompensas / información:** Refuerza el conflicto narrativo y la incertidumbre sobre los recuerdos del protagonista.

**Rol:** Manifestación de Margarita

**Función:** Entidad narrativa que conduce al desenlace del demo.

**Ubicación:** Bañera del Nivel 3.

**Interacción:** Encuentro dirigido mediante un evento narrativo. No cuenta con un sistema de conversación libre.

**Diálogos:** Invita al protagonista a acercarse. 

Margarita: «¿Por qué tardaste tanto?»
Protagonista: «¿Margarita…?»
Margarita: «Te estaba esperando. Ven, acércate… Ya no tenemos que separarnos».
Protagonista: «Pensé que te había perdido».
Margarita: «Nunca me fui. Ven conmigo».

**Condiciones:** Se presenta cuando el protagonista alcanza el baño final.

**Recompensas / información:** Activa la transición hacia los carteles de desaparición y la noticia sobre el hallazgo del cuerpo de Margarita.

> **Navegación:** [[00 - Índice]] · ← [[02 - Niveles]] · [[04 - Objetos y Coleccionables]] →
