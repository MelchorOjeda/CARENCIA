# Recorrido jugable

Versión de prueba del juego completo, del menú al desenlace, en el orden del guion unificado v1 (prólogo, Nivel 1, segundo despertar, Nivel 2, tercer despertar, Nivel 3 y desenlace). Los escenarios reutilizan la casa de `PRB_Melchor_Casa` con colores por nivel; los objetos de la colección de arte se usan en su estado `normal`, `sucio_oscuro` o `agobiante` según el nivel, y lo que aún no tiene arte es blockout con etiqueta. No lleva sonido. Las líneas del guion aparecen como cuadros en blanco con su clave: ver [DIALOGOS.md](DIALOGOS.md).

## Cómo jugarlo

Abre `Scenes/Menu/MEN_Principal` y dale Play. También puede abrirse cualquier escena del recorrido directamente.

| Acción | Control |
|---|---|
| Caminar | WASD o flechas |
| Correr | Shift |
| Cruzar puertas y cerrar vistas o cuadros | E |
| Usar objetos que se iluminan | Clic, con Elaki cerca |
| Ver el interior del refrigerador | E junto al refrigerador, en la casa real; E otra vez para cerrar |
| Pausa (Reanudar, Ajustes, Volver al menú principal) | Esc; Esc otra vez para reanudar |

La pausa solo se abre cuando Elaki puede moverse: durante una secuencia, una vista, un diálogo o un fundido de carga,
Esc conserva su uso de siempre. Con la pausa abierta el tiempo se detiene y la E no interactúa con nada. Volver al menú
pregunta antes, porque se pierde lo jugado desde el último punto de control.

### Menú y Ajustes

| Acción | Control |
|---|---|
| Pasar de la pantalla de inicio al menú | Cualquier tecla o clic |
| Elegir una opción | ↑ ↓ o W S; el mouse enfoca al pasar por encima |
| Confirmar | Enter o clic |
| Volver, cancelar una nota o salir del juego desde el menú | Esc |
| Ajustar una opción | ← → o A D; clic a la izquierda o a la derecha de la marca |
| Cambiar de pestaña en Ajustes | Q y E, o clic en la pestaña |
| Guardar las opciones | G |
| Restablecer la pestaña actual | R |

Reglas del menú:

- **Continuar** aparece tachado y no se puede elegir si no hay partida guardada o si `partida.json` está dañado.
- **Jugar** con una partida guardada pregunta antes de borrarla; sin partida, empieza directo.
- Las notas de confirmación empiezan en **No**, y Esc equivale a No.
- En **Ajustes**, audio, brillo y accesibilidad se ven al momento; resolución, modo de pantalla y calidad se aplican al
  guardar. Con cambios sin guardar aparece el sello **Sin guardar**, y Esc pregunta antes de descartarlos.
- Los ajustes se guardan en `ajustes.json`, junto a `partida.json` (`Application.persistentDataPath`). Restablecer no
  toca la partida.
- **Subtítulos** e **Indicadores de sonido** se guardan, pero aún no hacen nada: el juego todavía no tiene voces ni
  sonidos. Los volúmenes de Música, Efectos y Voces se aplican a cada `AudioSource` que lleve `VolumenPorCategoria`.

## Escenas

| Escena | Contenido | Cómo se sale |
|---|---|---|
| `MEN_Principal` | Pantalla de inicio, menú (Jugar, Continuar, Ajustes y Salir) y Ajustes, según el Figma del menú | Jugar o Continuar |
| `PRO_Casa` | P1 despertar y llamado de mamá. Al acercarse al comedor empieza el desayuno: sentado, se revisan el periódico (opcional) y el lugar vacío, que dispara la plática y el mareo. Los padres ya están sentados (el padre siempre detrás del periódico). En el baño real (cortina cerrada, cuatro toallas) se revisan con E lavabo, toallas y cortina; E en el espejo abre el gabinete, o se abre solo si Elaki se queda quieta 10 s frente a él. Al cerrarlo se abre el portal y la silueta lo cruza | E en el espejo: espiral |
| `N1_Laberinto` | Baño del otro lado en blanco: toallas, gabinete con símbolos, puerta que no abre. La cortina se abre con clic y deja ver el pasaje con el calendario del 31. Cuarto liminal con cama y cuadros; luego sala (sillón pegado a la pared), cocina, comedor (cuatro sillas sin mesa), pared de frente y cuarto de lavado (aviso en la tercera vuelta). Techos en las zonas donde no está Elaki | Salida pasando el cuarto de lavado |
| `N1_Pasillo` | Pasillo blanco largo; la salida queda cerrada detrás. Al entrar se oye la voz y la visión se oscurece más con cada paso hacia la puerta del fondo | Llegar a la puerta: "Ayuda." y corte a negro |
| `R1_Casa` | Segundo despertar, segundo desayuno (periódico opcional) y decisión de volver al baño. Baño real: cortina en estado 1, tres toallas, gabinete con E y el portal ya abierto | E en el espejo |
| `N2_Casa` | Llega al baño abandonado (bañera, toallas y calendario sucios). Pasillo con la foto de los cuatro y la ventana a la barda, que luego queda tapiada. Sala y cocina en penumbra con cuatro platos, TV con estática y refrigerador podrido. Al sentarse en la computadora se guarda; el video dura 50 s mientras la penumbra cede y el perro se acerca, y luego persigue. Si alcanza a Elaki, vuelve casi al final del video. En el pasillo, "ella" está al fondo: el perro revienta, ella gira y siempre alcanza a Elaki | Tercer golpe de "ella": espiral, sin pantalla de derrota |
| `R2_Casa` | Tercer despertar y desayuno con la madre gris: se levanta al revisar los cuatro obligatorios (lugar vacío, madre, comida y periódico). Altar del perro con sollozo, alacena, y aviso si intenta ir a la cocina. Baño real: cortina en estado 2, cuatro toallas, gabinete sin parches y portal abierto | E en el espejo |
| `N3_Casa` | Toda la casa es carne y está a oscuras: solo hay una luz pequeña alrededor de Elaki, que camina lento y no puede correr. No hay barra de vida: cada daño pone la pantalla más roja (tres seguidos y reaparece en el último punto de control) y se va limpiando si pasa un rato sin daño. Empieza en el baño de carne; al salir, la luz parpadea, se va y Elaki aparece en la cocina (más amplia, sellada del pasillo). En la cocina hay bocas en el piso que muerden y apéndices que salen de las paredes; la puerta al cuarto está cubierta de carne. Al abrir el refrigerador las carnes (un ave cruda, un pez lleno de cuerpos, una rana pálida y un feto con dientes) salen y la persiguen hasta que sale de la cocina; la membrana de la puerta se abre. En su cuarto deforme, algo bajo la cama la agarra: se suelta presionando las flechas en el orden que aparecen, con música de suspenso (cada error lastima). Al salir al pasillo, un ojo enorme en la ventana del fondo se abre y se cierra: con el ojo abierto la pantalla se oscurece y, si Elaki se mueve, se lastima poco a poco; quieta, no. El piso del pasillo es blando y la frena. Junto a la puerta del baño el ojo se cierra y los ojos de la pared lloran mirando al baño. En el baño, la cortina está abierta por completo y Margarita aparece en el remolino de la tina (aquí irá la cinemática de Yare). El sillón con el anuncio queda como opcional antes de abrir el refrigerador | Clic en la tina: fundido a negro y desenlace |
| `DES_Final` | Glitch alternado, carteles, noticiero, pantalla de texto y edificio en Navidad, como marcadores hasta que llegue el arte | Vuelve al menú |

## Reglas

- **Daño:** solo se puede morir con el perro y con "ella". Con el perro, 3 golpes reinician desde la computadora; con "ella", el tercer golpe lleva al tercer despertar. Las manchas rojas crecen con cada golpe.
- **Velocidades:** Elaki camina a 1.5 y corre a 2.6. El perro va a 2.0, así que se le puede escapar; "ella" va a 2.75 y siempre alcanza a Elaki.
- **Puntos de control:** al entrar a cada nivel, antes y después de cada persecución. Se guardan en un archivo y los usa Continuar.
- **Perro de la familia:** en el prólogo (`PRO_Casa`) aparece después del desayuno, y en el segundo despertar (`R1_Casa`) está antes y después del desayuno hasta que Elaki cruza al Nivel 2. En ambos acompaña a Elaki unos pasos detrás, por el mismo camino que ella recorrió, así que no atraviesa paredes ni muebles. Si Elaki aparece lejos (al entrar a la escena, al cruzar una puerta o en una secuencia), el perro reaparece junto a ella. No choca ni se puede usar. El constructor coloca `Prefabs/Personajes/PF_PerroAcompanante` en esas escenas.

## Sistemas

| Script | Función |
|---|---|
| `Core/EstadoJuego` | Banderas, golpes, punto de control y partida guardada |
| `Core/PuntoEntrada` | Lugar de llegada a una escena |
| `UI/PantallaJuego` | Fundidos, manchas, penumbra, cuadro en blanco, barra de video y carga de escenas |
| `Narrative/Secuencia` | Pasos en orden: bloquear, cámara, activar, banderas, enemigos y escenas |
| `Narrative/ZonaEvento` | Dispara una secuencia al pisar un área |
| `Narrative/Parpadeo` | Interferencia de la televisión |
| `Narrative/OscurecerAlAcercarse` | La visión se oscurece al acercarse a un punto (desmayo del Nivel 1) |
| `Narrative/EsperaQuieto` | Ejecuta una secuencia si Elaki se queda quieta en un área (gabinete que se abre solo) |
| `Narrative/ContarEntradas` | Ejecuta una secuencia a la n-ésima entrada a un área (tercera vuelta al lavado) |
| `Enemies/EnemigoPerseguidor` | Busca a Elaki por las celdas libres y la golpea por contacto |
| `Player/SaludJugador` | Recibe golpes y dispara la muerte |
| `Player/PerroAcompanante` | El perro sigue el camino de Elaki y reaparece junto a ella si queda lejos |
| `UI/MenuPrincipal` | Menú inicial |

## Pendiente

- **Contenido de otras áreas:** textos de las claves de [DIALOGOS.md](DIALOGOS.md), sonidos, voz femenina, video del monitor y anuncio de la TV.
- **Arte:** Elaki a 96 PPU, habitación, enemigos, padres, tazas verde y de carne, y dibujos de los cuadros del pasillo.
- **Arte del guion que aún es marcador:** vista de la barda (mujeres y hombre armado), carteles, noticiero, edificio, objetos de carne del Nivel 3, agua de la tina y el reflejo de Margarita.
- **Aviso de contenido:** va antes del menú, que es del equipo de UI.
- **Sin sentarse:** Elaki se coloca frente al monitor, pero aún no tiene animación de sentarse.
- **Sin aviso de bloqueo:** un objeto o portal que aún no se puede usar no da señal al jugador.
