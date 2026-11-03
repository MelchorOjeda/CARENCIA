# Medidas de la casa

Medidas de la casa en el mundo normal, tomadas de los bocetos del plano y del baño. Sirven para que el escenario
de arte (`Prefabs/Escenarios`), las colisiones y la cámara coincidan. El blockout gris de
`Scenes/Pruebas/PRB_Melchor_Casa` está armado con este mismo mapa.

## Escala

| Concepto | Valor |
|---|---|
| Rejilla | Isométrica, *Cell Size* `(1, 0.5, 1)` |
| Una celda | 50 cm de la casa real |
| Paredes | 1 celda de grosor, compartidas entre cuartos |
| Puertas | 2 celdas de ancho (1 m) |
| Casa completa | 19 × 30 celdas (9.5 × 15 m) |

## Escala en píxeles

Esta escala es una propuesta. Antes de dibujar todo se valida con piezas de prueba: un tile de suelo, un tramo de
pared, la puerta y Elaki de frente.

| Concepto | Valor |
|---|---|
| Tile de suelo | 48 × 24 px (una celda) |
| Pixels Per Unit | 48 |
| Altura | 1 m = 54 px |
| Vista del juego | 320 × 180 px, escalada ×6 a 1920 × 1080 |

**Importación en Unity:** *Pixels Per Unit* 48, *Filter Mode* Point, *Compression* None y pivote en la base del objeto
(en los pies, para los personajes).

**Cómo calcular una pieza:**

- **Mueble o pieza sobre el suelo** de `a × b` celdas y altura `h` en metros: ancho `(a + b) × 24` px y alto
  `(a + b) × 12 + h × 54` px.
- **Pieza plana en una pared**, de largo `l` y alto `h` en metros: ancho `l × 48` px y alto `l × 24 + h × 54` px.

### Personajes

Las estaturas son propuestas; el GDD solo indica que Elaki tiene 17 años.

| Personaje | Estatura | Alto del cuerpo | Lienzo sugerido |
|---|---|---|---|
| Elaki | 1.70 m | 92 px | 64 × 104 |
| Padre | 1.75 m | 95 px | 64 × 104 |
| Madre | 1.60 m | 86 px | 64 × 96 |
| Margarita | 1.55 m | 84 px | 64 × 96 |

### Casa y objetos

| Pieza | Medida real | Lienzo en píxeles |
|---|---|---|
| Pared, por cada celda de largo | 2.40 m de alto | 24 × 142 |
| Puerta | 1 × 2 m | 48 × 132 |
| Armario | 1 × 2 celdas, 2 m de alto | 72 × 144 |
| Mesa del pasillo | 1 × 2 celdas, 0.75 m de alto | 72 × 77 |
| Mesa del comedor | 3 × 2 celdas, 0.75 m de alto | 120 × 101 |
| Bañera | 3 × 2 celdas, 0.55 m de alto | 120 × 90 |
| Escusado | 1 celda, 0.75 m de alto | 48 × 65 |
| Lavabo | 1 celda, 0.85 m de alto | 48 × 70 |
| Cama | 2 × 4 celdas, 0.50 m de alto | 144 × 99 |
| Calendario | 30 × 40 cm | 14 × 29 |
| Foto grupal | 60 × 40 cm | 29 × 36 |
| Foto de boda | 50 × 60 cm | 24 × 44 |
| Ventana | 1.20 × 1 m | 58 × 83 |

El calendario y las fotos solo se reconocen en la pared. Las fechas y los textos se muestran en la vista de inspección,
con un dibujo aparte a mayor tamaño.

## Cámara

La cámara va cerca de Elaki y la sigue: *Orthographic Size* 1.875, centrada 1.25 unidades por encima de sus pies. Al
salir al pasillo se ven los detalles de la pared a unos 2 m hacia cada lado, no el pasillo completo.

**Qué paredes se ven:** la cámara solo ve las caras que miran hacia abajo de la pantalla.

- **Desde el pasillo** se ve la pared de arriba a la derecha (padres, servicio y escaleras). Calendario, fotos y mesa van
  en esa pared.
- **Las puertas de Elaki, el baño y Margarita** quedan de espaldas desde el pasillo. Se indican con el hueco en la pared y
  una marca en el suelo. El letrero NO PASAR y los stickers de la puerta de Elaki se ven desde dentro de su habitación.
- **Las paredes de abajo** de cada espacio se dibujan bajas o recortadas para no tapar a Elaki.
- **Dentro de cada cuarto** se dibujan altas las dos paredes de atrás.

## Espacios

| Espacio | Interior (celdas) | Metros | En el prólogo |
|---|---|---|---|
| Habitación de Elaki | 6 × 6 | 3 × 3 | Sí (cinemática del despertar) |
| Baño | 6 × 4 | 3 × 2 | Sí (exploración, gabinete, espejo) |
| Pasillo | 3 × 19 | 1.5 × 9.5 | Sí (une todo) |
| Sala, comedor y cocina | 17 × 9 | 8.5 × 4.5 | Sí (cinemática del desayuno) |
| Habitación de Margarita | 6 × 6 | 3 × 3 | No: un armario tapa su puerta |
| Habitación de los padres | 6 × 6 | 3 × 3 | No: puerta cerrada |
| Cuarto de servicio | 6 × 3 | 3 × 1.5 | No: puerta cerrada |
| Escaleras y entrada principal | 6 × 7 | 3 × 3.5 | No: puerta cerrada |

## Mapa por celdas

La fila 0 es el fondo del pasillo, donde está la ventana; la fila 29 es la pared de abajo de la sala.

```
           columna →
           0123456789012345678
fila  0    ########VVV########
      1    #xxxxxx#...+xxxxxx#   ← padres (puerta cerrada)
      2    #xxxxxx#A..+xxxxxx#
      3    #xxxxxx#A..#xxxxxx#   Margarita (el armario tapa su puerta)
      4    #xxxxxx#...#xxxxxx#
      5    #xxxxxx#..P#xxxxxx#
      6    #xxxxxx#..P#xxxxxx#
      7    ########...########
      8    #BBB...D...+xxxxxx#   ← baño · servicio (cerrado)
      9    #BBB...D...+xxxxxx#
     10    #.....W#...#xxxxxx#
     11    #.....L#...########
     12    ########...#xxxxxx#   escaleras (cerradas)
     13    #CC....D...#xxxxxx#   ← habitación de Elaki
     14    #CC....D...#xxxxxx#
     15    #CC....#...#xxxxxx#
     16    #CC....#...#xxxxxx#
     17    #.....E#...#xxxxxx#
     18    #.....E#...#xxxxxx#
     19    ########...###+++##   ← entrada principal (cerrada)
     20    #................K#
     21    #................K#
     22    #................K#
     23    #.......MMM......K#   sala · comedor · cocina
     24    #.......MMM......K#
     25    #T...............K#
     26    #T................#
     27    #T................#
     28    #.................#
     29    ###################
```

| Símbolo | Significado |
|---|---|
| `#` | Pared |
| `.` | Suelo |
| `D` | Puerta abierta |
| `+` | Puerta cerrada |
| `V` | Ventana |
| `x` | Cuarto cerrado, sin suelo |
| `A` | Armario que tapa la puerta de Margarita |
| `B` | Bañera |
| `W` | Escusado |
| `L` | Lavabo (el espejo y el gabinete van en la pared de encima) |
| `C` | Cama |
| `E` | Escritorio |
| `M` | Mesa del comedor |
| `P` | Mesa del pasillo con florero |
| `K` | Cocina y barra |
| `T` | Escritorio con la computadora |

**De mapa a Tilemap:** la celda del mapa (columna, fila) es la celda de Tilemap **(columna, 29 − fila)**.
Así la fila 29 queda en `y = 0` y el fondo del pasillo aparece arriba a la izquierda de la pantalla.

## Escenas

La casa se dibuja completa en el blockout, pero en el juego cada habitación es su propia escena
(ver [ESTRUCTURA.md](ESTRUCTURA.md)). Cada prefab de escenario contiene solo su habitación y el tramo de pasillo que
se ve desde ella, con las mismas medidas de esta tabla.

Las medidas son una primera propuesta: si al recorrer el blockout algún cuarto se siente demasiado grande o pequeño,
se corrige aquí primero y después en el escenario.
