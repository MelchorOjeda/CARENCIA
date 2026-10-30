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

Con *Cell Size* `(1, 0.5, 1)`, un tile de suelo mide 2:1 en píxeles. El tamaño en píxeles (por ejemplo 32 × 16 a
32 PPU) lo decide Arte; lo único que importa es que un tile ocupe exactamente una celda.

## Espacios

| Espacio | Interior (celdas) | Metros | En el prólogo |
|---|---|---|---|
| Habitación de Elaki | 6 × 6 | 3 × 3 | Sí (cinemática del despertar) |
| Baño | 6 × 4 | 3 × 2 | Sí (exploración, gabinete, espejo) |
| Pasillo | 3 × 19 | 1.5 × 9.5 | Sí (une todo) |
| Sala, comedor y cocina | 17 × 9 | 8.5 × 4.5 | Sí (cinemática del desayuno) |
| Habitación de Margarita | 6 × 6 | 3 × 3 | No: su puerta no se ve en el mundo normal |
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
      2    #xxxxxx#...+xxxxxx#
      3    #xxxxxx#...#xxxxxx#   Margarita (puerta oculta en el mundo normal)
      4    #xxxxxx#...#xxxxxx#
      5    #xxxxxx#..P#xxxxxx#
      6    #xxxxxx#..P#xxxxxx#
      7    ########...########
      8    #BBBB.WD...+xxxxxx#   ← baño · servicio (cerrado)
      9    #BBBB..D...+xxxxxx#
     10    #.....L#...#xxxxxx#
     11    #......#...########
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
Así la fila 29 queda en `y = 0` y el fondo del pasillo aparece arriba a la derecha de la pantalla.

## Escenas

La casa se dibuja completa en el blockout, pero en el juego cada habitación es su propia escena
(ver [ESTRUCTURA.md](ESTRUCTURA.md)). Cada prefab de escenario contiene solo su habitación y el tramo de pasillo que
se ve desde ella, con las mismas medidas de esta tabla.

Las medidas son una primera propuesta: si al recorrer el blockout algún cuarto se siente demasiado grande o pequeño,
se corrige aquí primero y después en el escenario.
