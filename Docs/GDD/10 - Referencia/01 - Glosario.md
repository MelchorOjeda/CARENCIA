# Glosario

## Descripción de la sección

Centraliza términos técnicos, de diseño y propios del universo del videojuego.

### Términos generales y técnicos

| Término                | Definición                                                                                                                   |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| GDD                    | Documento de Diseño de Videojuego.                                                                                           |
| Mecánica               | Regla o interacción mediante la cual el jugador puede actuar.                                                                |
| Sistema                | Conjunto de componentes y reglas que trabajan juntos.                                                                        |
| Unity                  | Motor de videojuegos utilizado para desarrollar Carencia.                                                                    |
| URP 2D                 | Universal Render Pipeline de Unity en su modalidad 2D, utilizado para el renderizado y la iluminación del juego.             |
| Escena                 | Archivo de Unity que contiene un tramo del juego, como el menú principal, el prólogo o un nivel.                             |
| Prefab                 | Objeto de Unity reutilizable que conserva sus componentes y configuración.                                                   |
| Sprite                 | Imagen 2D que representa un personaje, un objeto o una parte del escenario.                                                  |
| Pixel art              | Estilo gráfico basado en imágenes construidas píxel por píxel.                                                               |
| Perspectiva isométrica | Vista en la que el escenario se observa en diagonal desde arriba, mostrando dos paredes y el piso de cada espacio.           |
| Build                  | Versión ejecutable del juego generada desde Unity, en este caso para Windows.                                                |
| Punto de control       | Momento del recorrido en el que el juego guarda el progreso y desde el que se reinicia tras una derrota.                     |

### Términos de diseño

| Término | Definición |
| --- | --- |
| Terror psicológico | Género centrado en la tensión, la duda y la incomodidad, más que en el combate. |
| Horror corporal | Recurso que genera incomodidad mediante deformaciones, carne, órganos y alteraciones del cuerpo. |
| Casa real | Versión cotidiana de la casa de Elaki, donde vive con sus padres. |
| Otro lado | Versión alterada de la casa a la que Elaki accede al cruzar el espejo del baño. Cada nivel presenta una versión distinta. |
| Espacio liminal | Lugar reconocible pero vacío, amplio o inconsistente, que genera extrañeza. |
| Portal | Paso que se abre en el espejo del baño y conecta la casa real con el otro lado. |
| Espiral | Efecto visual que acompaña los portales y las transiciones entre la casa real y el otro lado. |
| Vista de inspección | Vista fija en primera persona para examinar un objeto, como el gabinete del baño. |
| Secuencia | Acontecimiento narrativo que coordina cámara, animaciones, sonidos y control del jugador. |
| Despertar | Momento en que Elaki aparece en su cuarto real después de un evento del otro lado. |
| Persecución | Encuentro en el que un enemigo sigue a Elaki y el jugador debe huir. |
| Daño visual | Manchas rojas que crecen en la pantalla con cada golpe recibido, en lugar de una barra de vida. |
| Desenlace | Secuencia final sin control del jugador que revela lo ocurrido a Margarita y a Elaki. |

### Términos del universo

| Término | Definición |
| --- | --- |
| Elaki | Protagonista de Carencia. Joven de 17 años que busca a su hermana. |
| Margarita | Hermana de Elaki, cuya existencia niegan sus padres. |
| Manifestación de Margarita | Versión deformada de Margarita que aparece al fondo del pasillo del Nivel 2. |
| Criatura canina | Manifestación del perro de la familia que persigue a Elaki en el Nivel 2. |
| Gabinete | Botiquín detrás del espejo del baño. Su revisión abre el primer portal. |
| Cortina de la bañera | Elemento del baño que se abre progresivamente a lo largo de la historia, desde cerrada en el prólogo hasta abierta por completo en el Nivel 3. |
| Calendario del 31 de octubre | Calendario que aparece junto a la puerta del baño del otro lado y marca la fecha de la desaparición de Margarita. |
| Altar del perro | Altar con la foto del perro de la familia, que aparece en el pasillo real después del Nivel 2. |
| Alacena | Mueble del pasillo real que ocupa el lugar de la puerta del cuarto de Margarita. |

## Convenciones de nombres

Documenta aquí términos que el equipo debe escribir siempre de la misma manera.

| Término | Forma correcta | Uso |
| --- | --- | --- |
| Protagonista | Elaki | Siempre por su nombre. No se usan alternativas como "el hermano" en los documentos de diseño. |
| Hermana | Margarita | Cuando se refiere a ella como persona o a su aparición en la tina. |
| Hermana deformada | Manifestación de Margarita | Cuando se refiere a su versión deformada del Nivel 2. |
| Enemigo del Nivel 2 | Criatura canina | En documentos de diseño. En textos narrativos puede llamarse "el perro de la familia". |
| Estructura | Prólogo, Nivel 1, Nivel 2, Nivel 3 y Desenlace | Con mayúscula inicial y número arábigo. |
| Espacios | Casa real y otro lado | Para distinguir la casa cotidiana de sus versiones alteradas. |
| Fecha clave | 31 de octubre | Siempre con el número y el mes en minúscula. |

| Recurso | Convención | Ejemplo |
| --- | --- | --- |
| Escenas | Prefijo del tramo y nombre del espacio | `MENU_Principal`, `PRO_Casa`, `N1_Casa`, `N2_Casa`, `N3_Casa`, `DES_Creditos` |
| Sprites | Prefijo `SPR_`, tipo, nombre y versión de la casa | `SPR_ESC_BanoHabitacion_D1` |
| Scripts y clases | PascalCase | `PlayerController` |
| Variables privadas | camelCase con prefijo `_` | `_velocidadMovimiento` |


> **Navegación:** [[00 - Índice]] · ← [[04 - Criterios de Aceptación]] · [[02 - Referencias e Inspiración]] →
