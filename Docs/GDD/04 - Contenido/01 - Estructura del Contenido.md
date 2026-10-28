# Estructura del Contenido

## Descripción de la sección

Organiza el contenido del videojuego en unidades manejables: mundos, capítulos, zonas, niveles, misiones y desafíos.

## Estructura

```text
Juego
├── Mundo / Capítulo 1
│   ├── Nivel
│   ├── Nivel
│   └── Jefe / Evento
├── Mundo / Capítulo 2
│   └── ...
└── Final
```

```text
Juego
PRÓLOGO — LA REALIDAD
│   ├── Despertar en la habitación
│   ├── Primer desayuno con los padres
│   ├── Transición al baño
│   ├── Exploración del baño real
│   ├── Inspección obligatoria del gabinete
│   └── Aparición del portal en el espejo
│
├── NIVEL 1 — LA PRIMERA RUPTURA
│   ├── Baño ligeramente distorsionado
│   ├── Exploración del pasillo prolongado
│   ├── Observación de alteraciones del entorno
│   ├── Voz femenina misteriosa
│   └── Transición a la siguiente dimensión
│
├── NIVEL 2 — LA DEFORMACIÓN
│   ├── Pasillo con fotografías alteradas
│   ├── Ventana con paisaje liminal
│   ├── Sala y comedor distorsionados
│   ├── Interacción con la computadora
│   ├── Persecución de la criatura canina
│   ├── Encuentro con la manifestación de Margarita
│   ├── Segunda persecución
│   ├── Despertar en la habitación real
│   ├── Segundo desayuno y periódico
│   ├── Exploración del altar del perro
│   └── Regreso al baño y nuevo portal
│
├── NIVEL 3 — LA REALIDAD SE ROMPE
│   ├── Exploración de la casa orgánica
│   ├── Sala y televisión distorsionadas
│   ├── Pasillo con alteraciones corporales
│   ├── Encuentro visual opcional en la ventana
│   ├── Calendario del 31 de octubre
│   ├── Puerta del baño con corazón deformado
│   ├── Bañera con la cortina abierta
│   └── Encuentro final con Margarita
│
└── DESENLACE DEL DEMO
    ├── Transición a través de la bañera
    ├── Carteles de personas desaparecidas
    ├── Noticia del hallazgo de Margarita
    └── Fin del demo
```
## Cantidad estimada

| Tipo     |                                                                                                                                        Cantidad |
| -------- | ----------------------------------------------------------------------------------------------------------------------------------------------: |
| Mundos   |                                                                                 1 entorno principal: la casa familiar y sus distintas versiones |
| Niveles  |                                                                                                                  3 niveles jugables + 1 prólogo |
| Misiones |                                                                                           4 objetivos principales, uno por cada sección jugable |
| Enemigos |                                                               2 amenazas de persecución: criatura canina y manifestación deformada de Margarita |
| Jefes    |                                                                                                          no se contemplan combates contra jefes |
| NPC      |                                                                                                                                           0 NPC |
| Objetos  | Al menos 8 objetos principales: gabinete, medicamentos y productos personales, toallas, espejo, computadora, periódico, calendario y televisión |

## Dependencias

¿Qué contenido debe existir antes de otro?

| Contenido                 | Dependencia                                                                                    |
| ------------------------- | ---------------------------------------------------------------------------------------------- |
| Prólogo                   | Es el inicio del videojuego. No requiere contenido previo.                                     |
| Nivel 1                   | Completar la exploración del baño real, inspeccionar el gabinete y atravesar el primer portal. |
| Nivel 2                   | Recorrer el Nivel 1 y alcanzar el siguiente portal.                                            |
| Persecuciones del Nivel 2 | Activar el evento de la computadora y sobrevivir a las amenazas para continuar.                |
| Nivel 3                   | Completar los acontecimientos del Nivel 2 y atravesar el siguiente portal.                     |
| Desenlace                 | Llegar a la bañera del Nivel 3 y completar el encuentro con "Margarita".                       |
> **Navegación:** [[00 - Índice]] · ← [[04 - Diálogos y Narrativa]] · [[02 - Niveles]] →
