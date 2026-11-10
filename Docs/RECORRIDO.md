# Recorrido jugable

Versión de prueba del juego, del menú hasta la bañera del Nivel 3. Los escenarios reutilizan la casa de `PRB_Melchor_Casa` con colores por nivel, y lo que aún no tiene arte es blockout con etiqueta. No lleva sonido, textos ni desenlace.

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
| `PRO_Casa` | Elaki despierta en su habitación y explora la casa libremente. Al acercarse a la silla del comedor aparece la E, y con ella empieza el desayuno con los papás, seguido del mareo, y aparece en el baño. Desde entonces, clic en el espejo abre la cajonera; al cerrarla aparece la silueta y luego el portal | Clic de nuevo en el espejo |
| `N1_Bano` | El mismo baño en blanco y frío, para la sensación de valle inquietante, con la cajonera en el mismo tono | E en la puerta |
| `N1_Pasillo` | Pasillo blanco de 30 tramos (240 celdas) con solo 3 puertas muy separadas (tramos 8, 19 y 30); lo demás es pared con cuadros en blanco. Al final, puerta bloqueada y ventana cubierta | Clic en el portal, que aparece cerca del final |
| `N2_Casa` | Fotos, ventana, platos y televisión. La computadora inicia el video y la persecución de la criatura; termina en el pasillo. Al fondo, Margarita se transforma y persigue a Elaki | Llegar a la marca junto a la puerta de la entrada |
| `R2_Casa` | Segundo despertar y desayuno. Periódico (obligatorio), altar del perro sobre la mesa junto al armario, y armario. Cortina en estado 2 | Acercarse a la bañera, después de leer el periódico |
| `N3_Casa` | Casa orgánica: televisión, agua, ojos, calendario, figura en la ventana y puerta del corazón | Acercarse a Margarita en la bañera; vuelve al menú |

## Reglas

- **Daño:** 3 golpes reinician desde el último punto de control. Las manchas rojas crecen con cada golpe y se borran al terminar la persecución.
- **Velocidades:** Elaki camina a 1.5 y corre a 2.6. La criatura va a 2.0 y Margarita a 2.2, así que hay que correr para escapar.
- **Puntos de control:** al entrar a cada nivel, antes y después de cada persecución. Se guardan en un archivo y los usa Continuar.

## Sistemas

| Script | Función |
|---|---|
| `Core/EstadoJuego` | Banderas, golpes, punto de control y partida guardada |
| `Core/PuntoEntrada` | Lugar de llegada a una escena |
| `UI/PantallaJuego` | Fundidos, manchas, penumbra, cuadro en blanco, barra de video y carga de escenas |
| `Narrative/Secuencia` | Pasos en orden: bloquear, cámara, activar, banderas, enemigos y escenas |
| `Narrative/ZonaEvento` | Dispara una secuencia al pisar un área |
| `Narrative/Parpadeo` | Interferencia de la televisión |
| `Enemies/EnemigoPerseguidor` | Busca a Elaki por las celdas libres y la golpea por contacto |
| `Player/SaludJugador` | Recibe golpes y dispara la muerte |
| `UI/MenuPrincipal` | Menú inicial |

## Pendiente

- **Contenido de otras áreas:** textos en los cuadros en blanco, sonidos, voz femenina y video real del monitor. Hoy la cámara se acerca al monitor y su pantalla se ve gris con una barra de avance; a la mitad se aleja y aparece la criatura.
- **Arte:** Elaki a 96 PPU, habitación, enemigos, padres, tazas verde y de carne, y dibujos de los cuadros del pasillo.
- **Desenlace:** carteles, noticiero y fin del demo.
- **Sin sentarse:** Elaki se coloca frente al monitor, pero aún no tiene animación de sentarse.
- **Sin aviso de bloqueo:** un objeto o portal que aún no se puede usar no da señal al jugador.
