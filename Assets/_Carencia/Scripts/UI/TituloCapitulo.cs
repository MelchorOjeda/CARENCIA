using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Título breve al empezar un capítulo, con las mismas piezas de la pausa: el título con temblor rojo y azul, la etiqueta
// con su barra roja y la sombra inferior. No bloquea nada: espera a que termine la cinemática de entrada
// (Elaki ya se puede mover, sin diálogo ni negro), aparece unos segundos y se va. Si ese momento no llega, no sale.
// En un capítulo que es solo cinemática (el desenlace) sale al empezar, encima del video.
public class TituloCapitulo : MonoBehaviour
{
    private const float EsperaMaxima = 20f;
    private const float Entrada = 0.9f;
    private const float Visible = 3f;
    private const float Salida = 1.1f;
    private const string Plantilla = "PF_MenuPausa";

    private static TituloCapitulo instancia;

    private CanvasGroup grupo;
    private TMP_Text numero;
    private TMP_Text roja;
    private TMP_Text azul;
    private TMP_Text hueso;
    private Coroutine rutina;
    private string escenaDelTitulo;
    private bool alEmpezar;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        instancia = null;
    }

    public static void Mostrar(Capitulo capitulo, string escena)
    {
        if (instancia == null && !Crear())
            return;

        if (instancia.rutina != null)
            instancia.StopCoroutine(instancia.rutina);
        instancia.escenaDelTitulo = escena;
        instancia.alEmpezar = capitulo.SoloCinematica;
        instancia.gameObject.SetActive(true);
        instancia.numero.text = capitulo.Numero.ToUpperInvariant();
        instancia.roja.text = "<alpha=#8C>" + capitulo.Titulo;
        instancia.azul.text = "<alpha=#38>" + capitulo.Titulo;
        instancia.hueso.text = capitulo.Titulo;
        instancia.rutina = instancia.StartCoroutine(instancia.Rutina());
    }

    // Lienzo propio que sobrevive a las escenas, por encima de los fundidos y de los videos (orden 120 en el desenlace).
    // Copia del menú de pausa el escalado y las piezas, para que se vea igual que el resto de la interfaz
    private static bool Crear()
    {
        var pausa = Resources.Load<GameObject>(Plantilla);
        var piezas = pausa != null ? pausa.transform.Find("Contenido/PantallaPausa") : null;
        if (piezas == null)
            return false;

        var objeto = new GameObject("TituloCapitulo", typeof(RectTransform));
        DontDestroyOnLoad(objeto);
        var lienzo = objeto.AddComponent<Canvas>();
        lienzo.renderMode = RenderMode.ScreenSpaceOverlay;
        lienzo.sortingOrder = 130;
        var escala = objeto.AddComponent<CanvasScaler>();
        var escalaPausa = pausa.GetComponent<CanvasScaler>();
        escala.uiScaleMode = escalaPausa.uiScaleMode;
        escala.referenceResolution = escalaPausa.referenceResolution;
        escala.matchWidthOrHeight = escalaPausa.matchWidthOrHeight;

        instancia = objeto.AddComponent<TituloCapitulo>();
        instancia.grupo = objeto.AddComponent<CanvasGroup>();
        instancia.grupo.alpha = 0f;
        instancia.grupo.blocksRaycasts = false;
        instancia.grupo.interactable = false;

        // Sombra de abajo, para que el texto se lea sobre pisos claros
        Instantiate(piezas.Find("SombraInferior").gameObject, objeto.transform, false).name = "SombraInferior";

        // Etiqueta "CAPÍTULO 1" con la barra roja, como el lugar en la pausa
        var etiqueta = (RectTransform)Instantiate(piezas.Find("Progreso").gameObject, objeto.transform, false).transform;
        etiqueta.name = "Etiqueta";
        etiqueta.anchorMin = etiqueta.anchorMax = Vector2.zero;
        etiqueta.pivot = new Vector2(0f, 1f);
        etiqueta.anchoredPosition = new Vector2(150f, 300f);
        Destroy(etiqueta.Find("UltimoGuardado").gameObject);
        var barra = (RectTransform)etiqueta.Find("Barra");
        barra.sizeDelta = new Vector2(barra.sizeDelta.x, 40f);
        barra.anchoredPosition = new Vector2(barra.anchoredPosition.x, -21f);
        instancia.numero = etiqueta.Find("Lugar").GetComponent<TMP_Text>();

        // El título con sus tres copias que tiemblan
        var titulo = (RectTransform)Instantiate(piezas.Find("Titulo").gameObject, objeto.transform, false).transform;
        titulo.name = "Titulo";
        titulo.anchorMin = titulo.anchorMax = Vector2.zero;
        titulo.pivot = new Vector2(0f, 1f);
        titulo.anchoredPosition = new Vector2(135f, 250f);
        titulo.sizeDelta = new Vector2(1700f, 150f);
        foreach (var copia in titulo.GetComponentsInChildren<TMP_Text>(true))
        {
            copia.fontSize = 110f;
            copia.textWrappingMode = TextWrappingModes.NoWrap;
            ((RectTransform)copia.transform).sizeDelta = titulo.sizeDelta;
        }
        instancia.roja = titulo.Find("Roja").GetComponent<TMP_Text>();
        instancia.azul = titulo.Find("Azul").GetComponent<TMP_Text>();
        instancia.hueso = titulo.Find("Hueso").GetComponent<TMP_Text>();

        objeto.SetActive(false);
        return true;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    // Si la escena cambia antes de que termine, el título de la anterior no pasa a la nueva
    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (rutina == null || escena.name == escenaDelTitulo)
            return;

        StopCoroutine(rutina);
        rutina = null;
        grupo.alpha = 0f;
        gameObject.SetActive(false);
    }

    private IEnumerator Rutina()
    {
        grupo.alpha = 0f;

        var pantalla = PantallaJuego.Instancia;
        PlayerController jugador = null;
        var libre = alEmpezar;
        // Al cargar, las secuencias de la escena todavía no arrancan: la escena debe seguir libre un rato seguido
        // (si no, el título saldría justo antes de que empiece la cinemática de entrada)
        var libreDesde = -1f;
        for (var t = 0f; !libre && t < EsperaMaxima; t += Time.unscaledDeltaTime)
        {
            yield return null;
            if (jugador == null && !pantalla.Cargando)
                jugador = FindAnyObjectByType<PlayerController>();
            var ahora = !pantalla.Cargando && pantalla.Negro < 0.05f && !pantalla.PantallaNegraVisible && !pantalla.CuadroVisible
                && jugador != null && !jugador.MovimientoBloqueado;
            if (!ahora)
                libreDesde = -1f;
            else if (libreDesde < 0f)
                libreDesde = t;
            else if (t - libreDesde >= 0.5f)
                libre = true;
        }

        if (!libre)
        {
            rutina = null;
            gameObject.SetActive(false);
            yield break;
        }

        for (var t = 0f; t < Entrada; t += Time.unscaledDeltaTime)
        {
            grupo.alpha = t / Entrada;
            yield return null;
        }
        grupo.alpha = 1f;
        yield return new WaitForSecondsRealtime(Visible);
        for (var t = 0f; t < Salida; t += Time.unscaledDeltaTime)
        {
            grupo.alpha = 1f - t / Salida;
            yield return null;
        }

        grupo.alpha = 0f;
        rutina = null;
        gameObject.SetActive(false);
    }
}
