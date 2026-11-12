using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Capa de pantalla común a todas las escenas: fundidos, manchas de daño, penumbra, caja de diálogo y barra de video.
// También carga escenas y coloca a Elaki en su punto de entrada. Se crea sola la primera vez que se pide.
public class PantallaJuego : MonoBehaviour
{
    private static PantallaJuego instancia;
    private static string entradaPendiente;

    private Image fundido;
    private Image manchas;
    private Image amenaza;
    private Image penumbra;
    private CajaDialogo caja;
    private GameObject marcador;
    private Text textoMarcador;
    private Image espiral;
    private GameObject vistaImagen;
    private Image imagenVista;
    private Image paredVista;
    private Image pisoVista;
    private GameObject video;
    private Image barraVideo;
    private Image imagenVideo;
    private Coroutine rutinaFundido;
    private bool cargando;

    public static PantallaJuego Instancia
    {
        get
        {
            if (instancia == null)
                Crear();

            return instancia;
        }
    }

    public CajaDialogo Dialogo => caja;

    public bool CuadroVisible => caja.Visible;

    // Durante el fundido de una carga no se puede pausar
    public bool Cargando => cargando;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        instancia = null;
        entradaPendiente = null;
    }

    private static void Crear()
    {
        var objeto = new GameObject("PantallaJuego");
        DontDestroyOnLoad(objeto);
        instancia = objeto.AddComponent<PantallaJuego>();
        instancia.Construir();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    // Efectos de las secuencias (una vez) y música de fondo (en bucle); la música se corta al cambiar de escena
    private AudioSource fuenteEfectos;
    private AudioSource fuenteMusica;

    public void ReproducirEfecto(AudioClip clip, float volumen = 1f)
    {
        if (clip != null)
            fuenteEfectos.PlayOneShot(clip, volumen);
    }

    public void ReproducirMusica(AudioClip clip)
    {
        if (clip == null)
        {
            fuenteMusica.Stop();
            fuenteMusica.clip = null;
            return;
        }

        if (fuenteMusica.clip == clip && fuenteMusica.isPlaying)
            return;

        fuenteMusica.clip = clip;
        fuenteMusica.Play();
    }

    private AudioSource FuenteDeAudio(string nombre, CategoriaSonido categoria, bool enBucle)
    {
        var objeto = new GameObject(nombre);
        objeto.transform.SetParent(transform, false);
        var fuente = objeto.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.loop = enBucle;
        objeto.AddComponent<VolumenPorCategoria>().Categoria = categoria;
        return fuente;
    }

    private void Construir()
    {
        fuenteEfectos = FuenteDeAudio("Efectos", CategoriaSonido.Efectos, false);
        fuenteMusica = FuenteDeAudio("Musica", CategoriaSonido.Musica, true);
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var escala = gameObject.AddComponent<CanvasScaler>();
        escala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escala.referenceResolution = new Vector2(640f, 360f);
        escala.matchWidthOrHeight = 0.5f;

        penumbra = Capa("Penumbra", Vineta(new Color(0.02f, 0.01f, 0.03f), 0.25f));
        SetAlfa(penumbra, 0f);
        amenaza = Capa("Amenaza", Vineta(new Color(0.5f, 0.02f, 0.04f), 0f));
        SetAlfa(amenaza, 0f);
        manchas = Capa("Manchas", Manchas());
        SetAlfa(manchas, 0f);

        // Marcador de una imagen o video que aún no tiene arte: fondo oscuro con una descripción
        marcador = new GameObject("Marcador", typeof(RectTransform));
        marcador.transform.SetParent(transform, false);
        var rectMarcador = (RectTransform)marcador.transform;
        rectMarcador.anchorMin = Vector2.zero;
        rectMarcador.anchorMax = Vector2.one;
        rectMarcador.offsetMin = rectMarcador.offsetMax = Vector2.zero;
        marcador.AddComponent<Image>().color = new Color32(12, 10, 12, 255);
        textoMarcador = Texto(marcador.transform, 14, TextAnchor.MiddleCenter, new Color32(226, 220, 210, 255));
        marcador.SetActive(false);

        // Vista fija de un objeto, por ejemplo el interior del refrigerador
        vistaImagen = new GameObject("VistaImagen", typeof(RectTransform));
        vistaImagen.transform.SetParent(transform, false);
        var rectVista = (RectTransform)vistaImagen.transform;
        rectVista.anchorMin = Vector2.zero;
        rectVista.anchorMax = Vector2.one;
        rectVista.offsetMin = rectVista.offsetMax = Vector2.zero;
        // La pared y el piso continúan la imagen hasta los bordes, como si Elaki estuviera frente al objeto
        paredVista = vistaImagen.AddComponent<Image>();
        var objetoPiso = new GameObject("Piso", typeof(RectTransform));
        objetoPiso.transform.SetParent(vistaImagen.transform, false);
        var rectPiso = (RectTransform)objetoPiso.transform;
        rectPiso.anchorMin = Vector2.zero;
        rectPiso.anchorMax = new Vector2(1f, 0.5f);
        rectPiso.offsetMin = Vector2.zero;
        pisoVista = objetoPiso.AddComponent<Image>();
        pisoVista.raycastTarget = false;
        var objetoImagen = new GameObject("Imagen", typeof(RectTransform));
        objetoImagen.transform.SetParent(vistaImagen.transform, false);
        imagenVista = objetoImagen.AddComponent<Image>();
        imagenVista.raycastTarget = false;
        vistaImagen.SetActive(false);

        video = new GameObject("Video", typeof(RectTransform));
        video.transform.SetParent(transform, false);
        var rectVideo = (RectTransform)video.transform;
        rectVideo.anchorMin = rectVideo.anchorMax = new Vector2(0.5f, 0.5f);
        rectVideo.sizeDelta = new Vector2(300f, 170f);
        rectVideo.anchoredPosition = new Vector2(0f, 20f);
        imagenVideo = video.AddComponent<Image>();
        imagenVideo.color = new Color32(20, 28, 32, 255);

        var fondoBarra = new GameObject("FondoBarra", typeof(RectTransform));
        fondoBarra.transform.SetParent(video.transform, false);
        var rectFondo = (RectTransform)fondoBarra.transform;
        rectFondo.anchorMin = new Vector2(0f, 0f);
        rectFondo.anchorMax = new Vector2(1f, 0f);
        rectFondo.pivot = new Vector2(0.5f, 0f);
        rectFondo.sizeDelta = new Vector2(-20f, 6f);
        rectFondo.anchoredPosition = new Vector2(0f, 8f);
        fondoBarra.AddComponent<Image>().color = new Color32(70, 70, 76, 255);

        var barra = new GameObject("Barra", typeof(RectTransform));
        barra.transform.SetParent(fondoBarra.transform, false);
        var rectBarra = (RectTransform)barra.transform;
        rectBarra.anchorMin = Vector2.zero;
        rectBarra.anchorMax = Vector2.one;
        rectBarra.offsetMin = rectBarra.offsetMax = Vector2.zero;
        barraVideo = barra.AddComponent<Image>();
        barraVideo.color = new Color32(214, 60, 52, 255);
        barraVideo.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        barraVideo.type = Image.Type.Filled;
        barraVideo.fillMethod = Image.FillMethod.Horizontal;
        video.SetActive(false);

        // Encima de marcadores, vistas y video, para que una línea se lea sobre ellos
        caja = CajaDialogo.Crear(transform);

        espiral = Capa("Espiral", Espiral());
        SetAlfa(espiral, 0f);

        fundido = Capa("Fundido", null);
        fundido.color = new Color(0f, 0f, 0f, 0f);

        // La caja de diálogo sube sobre espiral y fundido: lo que se dice a oscuras (el "Ayuda." del desmayo) se sigue leyendo
        caja.transform.SetAsLastSibling();
    }

    private Image Capa(string nombre, Sprite sprite)
    {
        var objeto = new GameObject(nombre, typeof(RectTransform));
        objeto.transform.SetParent(transform, false);
        var rect = (RectTransform)objeto.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        var imagen = objeto.AddComponent<Image>();
        imagen.sprite = sprite;
        imagen.raycastTarget = false;
        return imagen;
    }

    private static Text Texto(Transform padre, int tamano, TextAnchor ancla, Color color)
    {
        var objeto = new GameObject("Texto", typeof(RectTransform));
        objeto.transform.SetParent(padre, false);
        var rect = (RectTransform)objeto.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(12f, 6f);
        rect.offsetMax = new Vector2(-12f, -6f);
        var texto = objeto.AddComponent<Text>();
        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.fontSize = tamano;
        texto.alignment = ancla;
        texto.color = color;
        texto.raycastTarget = false;
        return texto;
    }

    // Espiral oscura que gira hacia el centro; se usa en los portales, el mareo y la muerte frente a "ella"
    private static Sprite Espiral()
    {
        const int lado = 160;
        var textura = new Texture2D(lado, lado, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        for (var y = 0; y < lado; y++)
        {
            for (var x = 0; x < lado; x++)
            {
                var dx = (x + 0.5f) / lado * 2f - 1f;
                var dy = (y + 0.5f) / lado * 2f - 1f;
                var radio = Mathf.Sqrt(dx * dx + dy * dy);
                var angulo = Mathf.Atan2(dy, dx);
                var brazo = Mathf.Sin(angulo * 3f + radio * 16f) * 0.5f + 0.5f;
                var alfa = Mathf.Clamp01(radio * 1.4f) * Mathf.SmoothStep(0.25f, 0.75f, brazo);
                textura.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Max(alfa, Mathf.InverseLerp(0.9f, 1.3f, radio))));
            }
        }

        textura.filterMode = FilterMode.Point;
        textura.Apply();
        return Sprite.Create(textura, new Rect(0, 0, lado, lado), new Vector2(0.5f, 0.5f));
    }

    private static void SetAlfa(Image imagen, float alfa)
    {
        var color = imagen.color;
        color.a = alfa;
        imagen.color = color;
    }

    // Oscurece hacia los bordes sin dibujar un círculo marcado
    private static Sprite Vineta(Color color, float centro)
    {
        const int ancho = 128;
        const int alto = 72;
        var textura = new Texture2D(ancho, alto, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        for (var y = 0; y < alto; y++)
        {
            for (var x = 0; x < ancho; x++)
            {
                var dx = (x + 0.5f) / ancho * 2f - 1f;
                var dy = (y + 0.5f) / alto * 2f - 1f;
                var distancia = Mathf.Sqrt(dx * dx + dy * dy) / 1.414f;
                var alfa = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(centro, 1f, distancia));
                textura.SetPixel(x, y, new Color(color.r, color.g, color.b, alfa));
            }
        }

        textura.Apply();
        return Sprite.Create(textura, new Rect(0, 0, ancho, alto), new Vector2(0.5f, 0.5f));
    }

    // Manchas rojas irregulares que entran desde los bordes
    private static Sprite Manchas()
    {
        const int ancho = 160;
        const int alto = 90;
        var textura = new Texture2D(ancho, alto, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        for (var y = 0; y < alto; y++)
        {
            for (var x = 0; x < ancho; x++)
            {
                var dx = (x + 0.5f) / ancho * 2f - 1f;
                var dy = (y + 0.5f) / alto * 2f - 1f;
                var borde = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
                var ruido = Mathf.PerlinNoise(x * 0.09f + 3.1f, y * 0.11f + 7.7f);
                var valor = borde * 0.75f + ruido * 0.45f;
                var alfa = Mathf.InverseLerp(0.62f, 0.95f, valor);
                alfa = Mathf.Round(alfa * 4f) / 4f;
                textura.SetPixel(x, y, new Color(0.45f, 0.02f, 0.03f, alfa * 0.92f));
            }
        }

        textura.filterMode = FilterMode.Point;
        textura.Apply();
        return Sprite.Create(textura, new Rect(0, 0, ancho, alto), new Vector2(0.5f, 0.5f));
    }

    // Tinte rojo de los bordes cuando un enemigo se acerca; lo controla AmenazaCercana
    public void SetAmenaza(float intensidad)
    {
        SetAlfa(amenaza, Mathf.Clamp01(intensidad));
    }

    // Para quien solo quiere limpiar un efecto: no crea la pantalla si ya no existe (al cerrar el juego)
    public static bool Existe => instancia != null;

    public void SetManchas(float intensidad)
    {
        SetAlfa(manchas, Mathf.Clamp01(intensidad));
    }

    public float Penumbra => penumbra.color.a;

    public void SetPenumbra(float intensidad)
    {
        SetAlfa(penumbra, Mathf.Clamp01(intensidad));
    }

    public void MostrarImagen(Sprite sprite)
    {
        vistaImagen.SetActive(sprite != null);
        if (sprite == null)
            return;

        imagenVista.sprite = sprite;
        var rect = (RectTransform)imagenVista.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
        rect.anchoredPosition = Vector2.zero;
        ContinuarFondo(sprite);
    }

    // Toma el color de la pared (arriba) y del piso (abajo) en el borde izquierdo de la imagen
    private void ContinuarFondo(Sprite sprite)
    {
        var textura = sprite.texture;
        var area = sprite.rect;
        var rectPiso = (RectTransform)pisoVista.transform;

        if (!textura.isReadable)
        {
            paredVista.color = new Color32(46, 40, 44, 255);
            pisoVista.color = new Color32(34, 30, 32, 255);
            rectPiso.offsetMax = new Vector2(0f, -area.height / 2f);
            return;
        }

        var x = (int)area.x + 1;
        var pared = textura.GetPixel(x, (int)(area.y + area.height) - 2);
        var piso = textura.GetPixel(x, (int)area.y + 1);

        // El piso termina donde el borde deja de tener su color
        var alturaPiso = 1;
        while (alturaPiso < area.height / 2f && Parecido(textura.GetPixel(x, (int)area.y + alturaPiso), piso))
            alturaPiso++;

        paredVista.color = new Color(pared.r, pared.g, pared.b, 1f);
        pisoVista.color = new Color(piso.r, piso.g, piso.b, 1f);
        rectPiso.offsetMax = new Vector2(0f, -area.height / 2f + alturaPiso);
    }

    private static bool Parecido(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b) < 0.12f;
    }

    public void MostrarMarcador(string texto)
    {
        marcador.SetActive(!string.IsNullOrEmpty(texto));
        textoMarcador.text = texto ?? "";
    }

    // La espiral cubre la pantalla girando y termina en negro
    // Negra en el mareo y la muerte; morada al cruzar de una dimensión a otra
    private static readonly Color EspiralNegra = new Color(0.03f, 0.01f, 0.04f, 0f);
    private static readonly Color EspiralMorada = new Color(0.4f, 0.14f, 0.55f, 0f);

    public Coroutine Espiral(float duracion, bool morada = false)
    {
        espiral.color = morada ? EspiralMorada : EspiralNegra;
        return StartCoroutine(RutinaEspiral(duracion));
    }

    private IEnumerator RutinaEspiral(float duracion)
    {
        var rect = (RectTransform)espiral.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        for (var t = 0f; t < duracion; t += Time.unscaledDeltaTime)
        {
            var avance = t / duracion;
            var lado = Mathf.Lerp(1400f, 520f, avance);
            rect.sizeDelta = new Vector2(lado, lado);
            rect.localRotation = Quaternion.Euler(0f, 0f, -avance * 540f);
            SetAlfa(espiral, Mathf.Clamp01(avance * 2.5f));
            SetAlfa(fundido, Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.55f, 1f, avance)));
            yield return null;
        }

        SetAlfa(fundido, 1f);
        SetAlfa(espiral, 0f);
    }

    public void MostrarVideo(bool visible)
    {
        video.SetActive(visible);
        barraVideo.fillAmount = 0f;
    }

    public void SetAvanceVideo(float avance)
    {
        barraVideo.fillAmount = Mathf.Clamp01(avance);

        // Interferencia provisional mientras llega el video definitivo
        var ruido = Mathf.PerlinNoise(Time.time * 9f, 0.3f);
        imagenVideo.color = Color.Lerp(new Color32(20, 28, 32, 255), new Color32(92, 40, 46, 255), ruido * avance);
    }

    // Fija el negro de pantalla al instante (para oscurecer poco a poco desde otro componente).
    // Si hay un fundido en curso no lo toca: una secuencia puede estar esperando a que termine
    public void SetFundido(float alfa)
    {
        if (rutinaFundido != null)
            return;

        SetAlfa(fundido, Mathf.Clamp01(alfa));
    }

    public Coroutine Fundir(float alfa, float duracion)
    {
        if (rutinaFundido != null)
            StopCoroutine(rutinaFundido);

        rutinaFundido = StartCoroutine(RutinaFundido(alfa, duracion));
        return rutinaFundido;
    }

    private IEnumerator RutinaFundido(float destino, float duracion)
    {
        var inicio = fundido.color.a;
        for (var t = 0f; t < duracion; t += Time.unscaledDeltaTime)
        {
            SetAlfa(fundido, Mathf.Lerp(inicio, destino, t / duracion));
            yield return null;
        }

        SetAlfa(fundido, destino);
        rutinaFundido = null;
    }

    public void CargarEscena(string escena, string entrada)
    {
        if (!cargando)
            StartCoroutine(RutinaCarga(escena, entrada, false));
    }

    public void ReiniciarDesdeControl()
    {
        if (!cargando)
            StartCoroutine(RutinaCarga(EstadoJuego.EscenaControl, EstadoJuego.EntradaControl, true));
    }

    private IEnumerator RutinaCarga(string escena, string entrada, bool restaurar)
    {
        cargando = true;
        yield return Fundir(1f, 0.6f);

        if (restaurar)
        {
            EstadoJuego.RestaurarPunto();
            SetManchas(0f);
        }

        // Sin punto de control todavía: repite la escena actual
        if (string.IsNullOrEmpty(escena))
            escena = SceneManager.GetActiveScene().name;

        entradaPendiente = entrada;
        caja.Ocultar();
        MostrarVideo(false);
        MostrarImagen(null);
        MostrarMarcador(null);
        SceneManager.LoadScene(escena);
        yield return null;
        cargando = false;
        Fundir(0f, 0.6f);
    }

    private static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (instancia != null && instancia.amenaza != null)
            instancia.SetAmenaza(0f);

        // Una línea que abrió la escena anterior justo antes de irse se queda sin quien la cierre:
        // se oculta aquí, antes del Start de la escena nueva, para que sus diálogos no esperen turno para siempre
        if (instancia != null && instancia.caja != null && instancia.caja.Visible)
            instancia.caja.Ocultar();
        // La música de una persecución no sigue en la escena siguiente
        if (instancia != null && instancia.fuenteMusica != null)
            instancia.ReproducirMusica(null);

        if (string.IsNullOrEmpty(entradaPendiente))
            return;

        var punto = PuntoEntrada.Buscar(entradaPendiente);
        var jugador = FindAnyObjectByType<PlayerController>();
        entradaPendiente = null;

        if (punto != null && jugador != null)
            jugador.Colocar(punto.transform.position);
    }
}
