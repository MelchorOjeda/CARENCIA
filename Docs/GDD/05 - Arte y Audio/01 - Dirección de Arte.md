# Dirección de Arte

## Descripción de la sección

Define la identidad visual del videojuego y las reglas que deben seguir los recursos gráficos.

## Identidad visual

**Estilo:** Pixel art isométrico 2D. Los personajes se modelan en 3D (Blender), se animan con Mixamo y se convierten en sprites de 8 direcciones con sombreado *cel* de 3 tonos, paleta cerrada y contorno de 1 px. Después se retocan a mano en Aseprite (rostros, pelo, siluetas y colores). Los escenarios y objetos se dibujan directamente en pixel art sobre una cuadrícula isométrica 2:1.

**Referencias:** Las imágenes de [[02 - Referencias e Inspiración]] (REF-01 a REF-12), las láminas y referencias de cada personaje en `Sprites_Game/Personajes/<personaje>/referencia_personaje/`, ciclos de caminata isométrica en pixel art (`Sprites_Game/Referencias/`) y referencias de anatomía, proporción y peinado para construir los modelos.

**Sensación buscada:** Un hogar mexicano cotidiano que se vuelve incómodo poco a poco. La casa real se ve cálida y limpia; sus versiones dimensionales se ensucian, se oscurecen y se deforman. Las criaturas deben producir rechazo físico (horror corporal) sin perder rasgos que las relacionen con lo familiar: Margarita y el perro de la casa.

**Nivel de realismo:** Estilizado. Proporciones humanas creíbles (el protagonista mide unas 6.5 cabezas) y detalle reducido a lo que se lee a tamaño de sprite. El horror se comunica con la silueta, el color y la pose, no con detalle fino.

**Formas predominantes:**

- **Casa real:** formas rectas y ordenadas, azulejos y muebles de madera con bordes limpios.
- **Elaki:** silueta sencilla y compacta, ropa de casa (playera, pantalón y pantuflas).
- **Criaturas:** formas alargadas, huesudas y angulosas: brazos demasiado largos, garras, costillas marcadas, posturas encorvadas y a cuatro patas.
- **Símbolo recurrente:** la espiral (vientre de la Manifestación de Margarita y portal).

**Iluminación:** Luz única fija, desde arriba a la izquierda de la cámara, igual para todas las direcciones del personaje. La cámara y la luz no se mueven: el personaje gira en pasos de 45°, así el sombreado es coherente en las 8 direcciones. Sin sombras proyectadas en el sprite; la iluminación de ambiente (penumbra, linternas, luces 2D) se resuelve en Unity.

**Materiales:** Colores planos por material, sin texturas finas ni reflejos. Cada material tiene 3 tonos (oscuro, medio, claro). La suciedad, la sangre y las manchas se pintan como zonas de color propias, no como texturas, para que se lean a tamaño de sprite.

## Paleta de color

**Colores principales:**

| Elemento | Tonos (oscuro → claro) | Uso |
|---|---|---|
| Piel de Elaki | `#7b5c4d` `#a07f6a` `#bd9e87` | Piel morena clara |
| Playera de Elaki | `#b4b3b5` `#e6e6e4` `#f7f7f4` | Lo más claro del personaje; lo separa del fondo |
| Mangas, pantalón y pelo | `#0b0b0e` `#161616` `#1a1f18` `#383a32` | Negros fríos |
| Piso de azulejo (casa real) | `#8c7366` `#ae9989` `#d0baac` `#e8d7c0` `#fffbf4` | Beige cálido |
| Madera de muebles | `#502725` `#6e4033` `#89553d` `#a06b4b` `#ce9769` | Café rojizo |

**Colores secundarios:**

| Elemento | Tonos | Uso |
|---|---|---|
| Pantuflas de Elaki | `#4e3257` `#6f4a78` `#8e6a97` | Morado; único color saturado del protagonista |
| Piel de la Manifestación de Margarita | `#433f4e` `#6c5c5b` `#8f7a71` (celdas `#24222d`) | Morena con subtono azul, enfermiza |
| Vestido de la Manifestación | `#1a181c` `#2b272d` `#403a3f` | Negro desteñido |
| Piel de la criatura canina | `#777166` `#5f5a50` `#413d37` | Gris sucio |
| Pelo de la criatura canina | `#6f6454` `#3e3731` | Beige grisáceo |
| Mugre | `#443c33` `#29231e` | Manos, pies y boca de las criaturas |
| Azulejo alterno | variante lila del azulejo | Versión alterada del piso |

**Colores para información jugable:**

| Color | Significado |
|---|---|
| Rojo vino `#350b0b` (y `#4a0c0f` / `#2a090b`) | Sangre y peligro. Es el único rojo de los sprites; se reserva para criaturas y daño. |
| Rojo en pantalla | Daño recibido (manchas rojas progresivas, ver [[03 - Sistemas]]). |
| Ojos claros `#b8968c` en cara oscura | Señal de que una manifestación mira a Elaki. |
| Contorno general `#08080b` | Separa a los personajes del escenario. |
| Contorno de piel `#3d2a22` (Elaki) y `#2e2a26` (criatura canina) | Mantiene la piel legible sin usar negro puro. |

## Reglas visuales

1. **Perspectiva isométrica 2:1 en todo.** Cámara ortográfica con 30° de elevación y 45° de acimut. Las piezas del piso avanzan +32, +16 px por bloque.
2. **Una sola escala para todos los personajes:** unos 70 px por metro. Elaki mide 112 px (1.60 m). Ningún personaje se reescala en Unity.
3. **8 direcciones por animación** (N, NE, E, SE, S, SW, W, NW) con los pies anclados a la misma línea de suelo.
4. **Paleta cerrada por personaje.** Cada personaje tiene su archivo `.gpl` y todo render se ajusta a él. Colores nuevos solo si se agregan a la paleta.
5. **Sombreado de 3 tonos, sin antialiasing ni semitransparencias.** Píxeles duros, contorno exterior de 1 px y sin píxeles sueltos.
6. **El rojo solo significa peligro.** No se usa en objetos decorativos de la casa real.
7. **La deformación es progresiva** (ver [[02 - Mundo]]): la casa real es cálida; Nivel 1 cambia colores y objetos; Nivel 2 oscurece y ensucia; Nivel 3 lleva el horror corporal a paredes y objetos.
8. **Las criaturas conservan rasgos reconocibles** de lo que fueron: la Manifestación conserva el pelo largo y la silueta de Margarita; la criatura canina, el pelaje y el hocico del perro de la familia.
9. **La silueta manda.** Si un detalle (dientes, gusanos, punto del ojo) no se lee a tamaño real, se simplifica o se quita.
10. **No se pisan versiones.** Cada cambio importante se guarda con un sufijo nuevo (`_v2`, `_tela3_moreno`…) y se conserva la versión anterior.

## Referencias

Añade enlaces o imágenes dentro de la carpeta del proyecto.

- [[02 - Referencias e Inspiración]]: REF-01 a REF-12 (escenas, baño, fotografía familiar, criatura canina, Manifestación de Margarita, calendario y tina).
- `Sprites_Game/Personajes/Main_Character/referencia_personaje/`: ficha técnica, vistas y paleta original del protagonista.
- `Sprites_Game/Personajes/Hermana/referencia_personaje/`: cuerpo demacrado, forma del cuerpo, vestido y falda rota, gusanos, espiral, textura de piel y rostro.
- `Sprites_Game/Personajes/Perro humanoide/referencia_personaje/`: cuerpo, rostro, sonrisa y pelaje.
- `Sprites_Game/Referencias/`: ciclos de caminata isométrica en 8 direcciones, maniquí isométrico de Mixamo y referencias de peinado y anatomía.


> **Navegación:** [[00 - Índice]] · ← [[05 - Misiones y Desafíos]] · [[02 - Personajes y Animación]] →
