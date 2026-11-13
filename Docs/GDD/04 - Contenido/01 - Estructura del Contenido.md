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
PRÓLOGO — LA NEGACIÓN
│   ├── Despertar en la habitación
│   ├── Primer desayuno con los padres
│   ├── Mareo y llegada al baño real
│   ├── Inspección obligatoria del gabinete
│   └── Portal en el espejo
│
├── NIVEL 1 — LA PRIMERA RUPTURA
│   ├── Baño del otro lado
│   ├── Pasaje detrás de la cortina y calendario
│   ├── Versión liminal del cuarto de Elaki
│   ├── Espacios del otro lado de la casa
│   ├── Pasillo con la voz de Margarita
│   ├── Desmayo
│   ├── Segundo despertar
│   └── Segundo desayuno
│
├── NIVEL 2 — LA DEFORMACIÓN
│   ├── Baño real (segunda visita) y portal
│   ├── Baño abandonado y calendario
│   ├── Pasillo con la fotografía familiar
│   ├── Ventana con la barda
│   ├── Sala y cocina en penumbra
│   ├── Video de la computadora
│   ├── Persecución del perro de la familia
│   ├── Encuentro con la Manifestación de Margarita
│   ├── Tercer despertar
│   ├── Tercer desayuno
│   └── Altar del perro y alacena
│
├── NIVEL 3 — LA REALIDAD SE ROMPE
│   ├── Baño real (tercera visita) y portal
│   ├── Baño de carne
│   ├── Pasillo de carne
│   ├── Sala y cocina de carne; anuncio en la televisión
│   ├── Los ojos guían al baño
│   ├── Ventana con el hombre armado (opcional)
│   └── Margarita en la tina
│
└── DESENLACE
    ├── Imágenes alternadas de la tina
    ├── Carteles de búsqueda
    ├── Transmisión sobre el cuerpo de Margarita
    ├── Pantalla de texto sobre el joven ahogado
    └── Escena del edificio
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
