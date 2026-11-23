using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Menú inicial: "pulsa cualquier tecla", luego Jugar, Continuar, Ajustes y Salir.
// Continuar aparece tachado si no hay partida guardada (o si el archivo está dañado).
// Lo que borra la partida o cierra el juego pregunta antes; una vez elegido, el menú deja de responder.
public class MenuPrincipal : MonoBehaviour
{
    private const int Jugar = 0;
    private const int Continuar = 1;
    private const int Opciones = 2;
    private const int Salir = 3;

    [SerializeField] private string primeraEscena = "PRO_Casa";
    [SerializeField] private GameObject pantallaInicio;
    [SerializeField] private GameObject pantallaMenu;
    [SerializeField] private ListaOpciones lista;
    [SerializeField] private OpcionMenu opcionContinuar;
    [SerializeField] private TMP_Text detalleCartel;
    [SerializeField] private PantallaOpciones pantallaOpciones;
    [SerializeField] private NotaConfirmacion nota;
    [SerializeField] private Image vineta;
    [SerializeField] private Sprite vinetaMenu;
    [SerializeField] private Sprite vinetaOpciones;

    // Bandera que deja el desenlace: al llegar con ella se muestra primero el fin del demo
    private const string BanderaFin = "desenlace";

    private readonly AccionUnica salida = new AccionUnica();
    private GameObject pantallaFin;
    private (string escena, DateTime fecha)? partida;

    private void Awake()
    {
        lista.Elegida += Elegir;
#if UNITY_WEBGL && !UNITY_EDITOR
        // En el navegador no se puede cerrar el juego: se quitan Salir y la ayuda de Esc
        var salir = pantallaMenu.transform.Find("Opciones/Salir");
        if (salir != null)
        {
            salir.GetComponent<OpcionMenu>()?.SetHabilitada(false);
            salir.gameObject.SetActive(false);
        }
        pantallaMenu.transform.Find("Ayuda/Tecla Esc")?.gameObject.SetActive(false);
#endif
    }

    private static bool PuedeSalir =>
#if UNITY_WEBGL && !UNITY_EDITOR
        false;
#else
        true;
#endif

    private void Start()
    {
        PantallaJuego.Instancia.Fundir(0f, 0.3f);
        PantallaJuego.Instancia.SetPenumbra(0f);
        PantallaJuego.Instancia.SetManchas(0f);

        LeerPartida();
        pantallaOpciones.gameObject.SetActive(false);
        nota.gameObject.SetActive(false);
        if (EstadoJuego.Tiene(BanderaFin))
        {
            EstadoJuego.Quitar(BanderaFin);
            MostrarFin();
        }
        else
        {
            MostrarInicio();
        }
    }

    private void Update()
    {
        if (pantallaFin != null)
        {
            if (ControlesMenu.Pulso(ControlesMenu.Confirmar))
                VolverDelFin();
            return;
        }

        if (pantallaInicio.activeSelf && ControlesMenu.Pulso(ControlesMenu.CualquierTecla))
            MostrarMenu(Jugar);
        else if (PuedeSalir && pantallaMenu.activeSelf && lista.Activa && !nota.Abierta && ControlesMenu.Pulso(ControlesMenu.Volver))
            PreguntarSalir();
    }

    private void MostrarInicio()
    {
        pantallaInicio.SetActive(true);
        pantallaMenu.SetActive(false);
    }

    // Fin del demo con el mismo diseño del menú: título, "Fin del demo", el cartel y una sola opción para volver
    private void MostrarFin()
    {
        pantallaInicio.SetActive(false);
        pantallaMenu.SetActive(false);
        vineta.sprite = vinetaMenu;

        // La copia se hace con el menú apagado: así ninguno de sus scripts arranca antes de ajustarla
        pantallaFin = Instantiate(pantallaMenu, pantallaMenu.transform.parent);
        pantallaFin.name = "PantallaFin";
        pantallaFin.transform.SetSiblingIndex(pantallaMenu.transform.GetSiblingIndex() + 1);
        var raiz = pantallaFin.transform;

        var frase = raiz.Find("Frase").GetComponent<TMP_Text>();
        var gracias = Instantiate(frase.gameObject, raiz).GetComponent<TMP_Text>();
        gracias.name = "Gracias";
        gracias.text = "Gracias por jugar.";
        gracias.fontSize = 24f;
        ((RectTransform)gracias.transform).anchoredPosition = ((RectTransform)frase.transform).anchoredPosition + new Vector2(0f, -66f);
        frase.text = "Fin del demo";
        frase.fontSize = 48f;
        frase.color = new Color32(0xEC, 0xE6, 0xD8, 0xFF);
        ((RectTransform)frase.transform).sizeDelta = new Vector2(900f, 64f);

        var opciones = raiz.Find("Opciones");
        DestroyImmediate(opciones.GetComponent<ListaOpciones>());
        ((RectTransform)opciones).anchoredPosition += new Vector2(0f, -90f);
        foreach (var nombre in new[] { "Continuar", "Ajustes", "Salir" })
            DestroyImmediate(opciones.Find(nombre).gameObject);
        var volver = opciones.Find("Jugar").GetComponent<OpcionMenu>();
        volver.CambiarTexto("Volver al menú principal");
        ((RectTransform)volver.transform).sizeDelta = new Vector2(1100f, 100f);
        foreach (var texto in volver.GetComponentsInChildren<TMP_Text>(true))
            ((RectTransform)texto.transform).sizeDelta = new Vector2(1050f, 96f);
        volver.Pulsada += _ => VolverDelFin();

        var ayuda = raiz.Find("Ayuda");
        foreach (Transform tecla in ayuda)
            if (!tecla.name.Contains("Enter"))
                tecla.gameObject.SetActive(false);

        raiz.Find("Cartel/Detalle").GetComponent<TMP_Text>().text = "Nadie sabe quién es.";

        pantallaFin.SetActive(true);
        volver.MostrarEnfoque(true);
        ControlesMenu.Bloquear();
    }

    private void VolverDelFin()
    {
        Destroy(pantallaFin);
        pantallaFin = null;
        MostrarMenu(Jugar);
    }

    private void MostrarMenu(int enfocada)
    {
        pantallaInicio.SetActive(false);
        pantallaMenu.SetActive(true);
        vineta.sprite = vinetaMenu;
        lista.Enfocar(enfocada);
        ControlesMenu.Bloquear();
    }

    private void Elegir(int opcion)
    {
        switch (opcion)
        {
            case Jugar: PedirJugar(); break;
            case Continuar: ContinuarPartida(); break;
            case Opciones: AbrirOpciones(); break;
            case Salir: PreguntarSalir(); break;
        }
    }

    private void PedirJugar()
    {
        if (FlujoMenu.AlElegirJugar(partida.HasValue) == Decision.Hacerlo)
        {
            EmpezarPartida();
            return;
        }

        nota.Preguntar(
            "¿Empezar desde el principio?",
            $"Se borrará tu partida guardada ({TextosPartida.NombreDe(partida.Value.escena)}).",
            "Sí, empezar de nuevo",
            EmpezarPartida);
    }

    private void EmpezarPartida()
    {
        if (!salida.Tomar())
            return;

        lista.Activa = false;
        EstadoJuego.NuevaPartida();
        PantallaJuego.Instancia.CargarEscena(primeraEscena, null);
    }

    private void ContinuarPartida()
    {
        // El archivo pudo borrarse o dañarse después de abrir el menú: Continuar se tacha y no pasa nada más
        if (!EstadoJuego.CargarPartida())
        {
            LeerPartida();
            lista.EnfocarPrimera();
            return;
        }

        if (!salida.Tomar())
            return;

        lista.Activa = false;
        PantallaJuego.Instancia.CargarEscena(EstadoJuego.EscenaControl, EstadoJuego.EntradaControl);
    }

    private void AbrirOpciones()
    {
        pantallaMenu.SetActive(false);
        vineta.sprite = vinetaOpciones;
        pantallaOpciones.Abrir(() => MostrarMenu(Opciones));
    }

    private void PreguntarSalir()
    {
        nota.Preguntar(
            "¿Salir del juego?",
            partida.HasValue ? "Tu progreso está guardado." : "Todavía no has empezado una partida.",
            "Sí, salir",
            SalirDelJuego);
    }

    private void SalirDelJuego()
    {
        if (!salida.Tomar())
            return;

        lista.Activa = false;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LeerPartida()
    {
        partida = EstadoJuego.LeerPartidaGuardada();
        opcionContinuar.SetHabilitada(partida.HasValue);
        detalleCartel.text = partida.HasValue
            ? $"Último rastro: {TextosPartida.NombreDe(partida.Value.escena)}\n{TextosPartida.HaceCuanto(partida.Value.fecha)}"
            : "Nadie ha empezado a buscar.";
    }
}
