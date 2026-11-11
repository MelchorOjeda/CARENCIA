using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Pausa del juego con Esc: Reanudar, Ajustes y Volver al menú principal.
// Se crea sola al arrancar y vive entre escenas, así ninguna escena de juego tiene que incluirla.
// Detiene el tiempo y desactiva al jugador (movimiento y la E) hasta reanudar.
public class MenuPausa : MonoBehaviour
{
    private const string Prefab = "PF_MenuPausa";
    private const int Reanudar = 0;
    private const int Opciones = 1;
    private const int VolverAlMenu = 2;

    private static MenuPausa instancia;

    [SerializeField] private string escenaMenu = "MEN_Principal";
    [SerializeField] private GameObject contenido;
    [SerializeField] private GameObject pantallaPausa;
    [SerializeField] private ListaOpciones lista;
    [SerializeField] private TMP_Text lugar;
    [SerializeField] private TMP_Text ultimoGuardado;
    [SerializeField] private PantallaOpciones pantallaOpciones;
    [SerializeField] private NotaConfirmacion nota;

    private PlayerController jugador;
    private InteractionDetector detector;
    private bool jugadorEstabaBloqueado;
    private bool podiaPausarAntes;
    private float escalaDeTiempo = 1f;
    private AccionUnica salida;

    public bool Abierta => contenido.activeSelf;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        instancia = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Crear()
    {
        if (instancia != null)
            return;

        instancia = Instantiate(Resources.Load<MenuPausa>(Prefab));
        instancia.name = Prefab;
        DontDestroyOnLoad(instancia.gameObject);
    }

    private void Awake()
    {
        lista.Elegida += Elegir;
        contenido.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void Update()
    {
        if (Abierta)
        {
            if (pantallaPausa.activeSelf && !nota.Abierta && ControlesMenu.Pulso(ControlesMenu.Volver))
                Cerrar(true);
            return;
        }

        // El Esc que cierra un diálogo no debe abrir también la pausa: se exige poder pausar desde el fotograma anterior
        var puedePausar = FlujoMenu.PuedePausar(LeerSituacion());
        if (puedePausar && podiaPausarAntes && ControlesMenu.Pulso(ControlesMenu.Volver))
            Abrir();

        podiaPausarAntes = puedePausar;
    }

    private SituacionJuego LeerSituacion()
    {
        if (jugador == null)
            jugador = FindAnyObjectByType<PlayerController>();

        var pantalla = PantallaJuego.Instancia;
        return new SituacionJuego
        {
            hayJugador = jugador != null,
            jugadorBloqueado = jugador != null && jugador.MovimientoBloqueado,
            hayDialogo = pantalla.CuadroVisible,
            cargandoEscena = pantalla.Cargando,
        };
    }

    private void Abrir()
    {
        escalaDeTiempo = Time.timeScale;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        DetenerJugador();

        salida = new AccionUnica();
        contenido.SetActive(true);
        MostrarPausa(Reanudar);
    }

    private void MostrarPausa(int enfocada)
    {
        pantallaPausa.SetActive(true);
        MostrarProgreso();
        lista.Activa = true;
        lista.Enfocar(enfocada);
        ControlesMenu.Bloquear();
    }

    private void Elegir(int opcion)
    {
        switch (opcion)
        {
            case Reanudar: Cerrar(true); break;
            case Opciones: AbrirOpciones(); break;
            case VolverAlMenu: PreguntarVolver(); break;
        }
    }

    private void AbrirOpciones()
    {
        pantallaPausa.SetActive(false);
        pantallaOpciones.Abrir(() => MostrarPausa(Opciones), "Pausa  ›  Ajustes");
    }

    private void PreguntarVolver()
    {
        nota.Preguntar(
            "¿Volver al menú principal?",
            "Se perderá lo jugado desde el último punto de control.",
            "Sí, volver",
            Volver);
    }

    private void Volver()
    {
        if (!salida.Tomar())
            return;

        lista.Activa = false;
        Cerrar(false);
        PantallaJuego.Instancia.CargarEscena(escenaMenu, null);
    }

    // Al volver al menú el jugador se queda quieto: la escena está por cambiar
    private void Cerrar(bool devolverControl)
    {
        contenido.SetActive(false);
        Time.timeScale = escalaDeTiempo;
        AudioListener.pause = false;
        ControlesMenu.Bloquear();
        if (devolverControl)
            DevolverJugador();
    }

    private void DetenerJugador()
    {
        jugadorEstabaBloqueado = jugador.MovimientoBloqueado;
        jugador.MovimientoBloqueado = true;
        detector = jugador.GetComponentInChildren<InteractionDetector>();
        if (detector != null)
            detector.enabled = false;
    }

    private void DevolverJugador()
    {
        if (jugador != null)
            jugador.MovimientoBloqueado = jugadorEstabaBloqueado;
        if (detector != null)
            detector.enabled = true;
    }

    private void MostrarProgreso()
    {
        lugar.text = TextosPartida.NombreDe(SceneManager.GetActiveScene().name).ToUpperInvariant();
        var partida = EstadoJuego.LeerPartidaGuardada();
        ultimoGuardado.text = partida.HasValue
            ? $"Último punto de control: {TextosPartida.HaceCuanto(partida.Value.fecha)}"
            : "Todavía sin punto de control";
    }

    // Si la escena cambia con la pausa abierta, el tiempo vuelve a correr y se olvida al jugador de la escena anterior
    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (Abierta)
            Cerrar(false);

        jugador = null;
        podiaPausarAntes = false;
    }
}
