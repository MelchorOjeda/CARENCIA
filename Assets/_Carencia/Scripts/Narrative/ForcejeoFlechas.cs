using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Algo bajo la cama agarra a Elaki (Nivel 3). Para soltarse hay que presionar las flechas en el orden que aparecen,
// antes de que se acabe el tiempo, mientras suena la música de suspenso.
// Un error o quedarse sin tiempo la lastima (la pantalla se pone más roja) y empieza otra secuencia.
// Se libera al completar todas las rondas; si los golpes llegan al límite, muere como siempre.
public class ForcejeoFlechas : MonoBehaviour
{
    // 0 izquierda, 1 arriba, 2 derecha, 3 abajo
    private static readonly float[] Giros = { 90f, 0f, -90f, 180f };
    private const int MaximoFlechas = 9;

    [SerializeField] private int rondas = 2;
    [SerializeField] private int flechasPrimeraRonda = 5;
    [SerializeField] private float segundosPorFlecha = 1.15f;
    [SerializeField] private AudioSource musica;
    [SerializeField] private AudioSource error;
    [SerializeField] private SpriteRenderer manos;
    [SerializeField] private Sprite[] cuadrosManos = new Sprite[0];

    private Canvas capa;
    private Image[] flechas;
    private Sprite flecha;
    private Text aviso;
    private Image barra;
    private readonly List<int> secuencia = new List<int>();

    public bool Liberada { get; private set; }

    // Flechas acertadas de la secuencia actual
    public int Acierto { get; private set; }

    public void Configurar(AudioSource nuevaMusica, AudioSource nuevoError, SpriteRenderer nuevasManos, Sprite[] cuadros)
    {
        musica = nuevaMusica;
        error = nuevoError;
        manos = nuevasManos;
        cuadrosManos = cuadros;
    }

    public IEnumerator Jugar()
    {
        var jugador = FindAnyObjectByType<PlayerController>();
        var salud = jugador.GetComponent<SaludJugador>();
        jugador.MovimientoBloqueado = true;
        Liberada = false;
        CrearCapa();
        capa.gameObject.SetActive(true);
        if (musica != null)
            musica.Play();
        if (manos != null)
            manos.gameObject.SetActive(true);

        var ronda = 0;
        while (ronda < rondas)
        {
            if (salud != null && salud.Muerto)
                yield break;

            var largo = flechasPrimeraRonda + ronda * 2;
            secuencia.Clear();
            for (var i = 0; i < largo; i++)
                secuencia.Add(Random.Range(0, 4));

            var acierto = 0;
            Acierto = 0;
            var limite = largo * segundosPorFlecha;
            var tiempo = 0f;
            aviso.text = ronda == 0 ? "¡Suéltame!" : "¡Más fuerte!";
            var fallo = false;
            while (acierto < largo)
            {
                tiempo += Time.deltaTime;
                Dibujar(acierto, 1f - tiempo / limite);
                AnimarManos(tiempo, acierto, largo);
                if (tiempo >= limite)
                {
                    fallo = true;
                    break;
                }

                var tecla = Leida();
                if (tecla >= 0)
                {
                    if (tecla == secuencia[acierto])
                    {
                        acierto++;
                        Acierto = acierto;
                    }
                    else
                    {
                        fallo = true;
                        break;
                    }
                }

                yield return null;
            }

            if (fallo)
            {
                // Tira de ella: golpe, pantalla más roja y otra secuencia de la misma ronda
                if (error != null)
                    error.Play();
                salud?.Golpear();
                aviso.text = "¡Me jala!";
                Dibujar(-1, 0f);
                yield return new WaitForSeconds(0.8f);
                continue;
            }

            ronda++;
            yield return new WaitForSeconds(0.25f);
        }

        Liberada = true;
        capa.gameObject.SetActive(false);
        if (musica != null)
            musica.Stop();
        if (manos != null && cuadrosManos.Length > 0)
        {
            // Las manos se esconden otra vez bajo la cama
            for (var i = cuadrosManos.Length - 1; i >= 0; i--)
            {
                manos.sprite = cuadrosManos[i];
                yield return new WaitForSeconds(0.08f);
            }

            manos.gameObject.SetActive(false);
        }

        jugador.MovimientoBloqueado = false;
    }

    // Flechas del teclado, o WASD con la misma dirección
    private static int Leida()
    {
        var k = Keyboard.current;
        if (k == null)
            return -1;
        if (k.leftArrowKey.wasPressedThisFrame || k.aKey.wasPressedThisFrame) return 0;
        if (k.upArrowKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame) return 1;
        if (k.rightArrowKey.wasPressedThisFrame || k.dKey.wasPressedThisFrame) return 2;
        if (k.downArrowKey.wasPressedThisFrame || k.sKey.wasPressedThisFrame) return 3;
        return -1;
    }

    // Las ya presionadas se ven apagadas, la que sigue en rojo y más grande, el resto en blanco
    private void Dibujar(int acierto, float restante)
    {
        for (var i = 0; i < flechas.Length; i++)
        {
            var imagen = flechas[i];
            imagen.enabled = i < secuencia.Count;
            if (!imagen.enabled)
                continue;

            imagen.rectTransform.localRotation = Quaternion.Euler(0f, 0f, Giros[secuencia[i]]);
            var actual = i == acierto;
            imagen.rectTransform.localScale = Vector3.one * (actual ? 1.35f : 1f);
            imagen.color = acierto < 0 ? new Color32(122, 16, 16, 255)
                : i < acierto ? new Color32(70, 56, 56, 255)
                : actual ? new Color32(255, 58, 42, 255)
                : new Color32(240, 230, 220, 255);
            imagen.rectTransform.anchoredPosition = new Vector2((i - (secuencia.Count - 1) / 2f) * 30f, 0f);
        }

        barra.fillAmount = Mathf.Clamp01(restante);
    }

    // Flecha de pixel art (apunta hacia arriba): punta triangular y tallo
    private static Sprite CrearFlecha()
    {
        const int lado = 16;
        var textura = new Texture2D(lado, lado, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        for (var y = 0; y < lado; y++)
            for (var x = 0; x < lado; x++)
            {
                var desdeArriba = lado - 1 - y;
                var punta = desdeArriba < 8 && Mathf.Abs(x - 7.5f) <= desdeArriba + 0.5f;
                var tallo = desdeArriba >= 8 && desdeArriba < 15 && x >= 5 && x <= 10;
                textura.SetPixel(x, y, punta || tallo ? Color.white : Color.clear);
            }
        textura.Apply();
        return Sprite.Create(textura, new Rect(0, 0, lado, lado), new Vector2(0.5f, 0.5f), 16f);
    }

    private void AnimarManos(float tiempo, int acierto, int largo)
    {
        if (manos == null || cuadrosManos.Length == 0)
            return;
        // Aprietan y aflojan; mientras más aciertos, menos agarran
        var fuerza = 1f - (float)acierto / largo;
        var indice = Mathf.Clamp(Mathf.RoundToInt(fuerza * (cuadrosManos.Length - 1) + Mathf.Sin(tiempo * 14f) * 0.6f), 0, cuadrosManos.Length - 1);
        manos.sprite = cuadrosManos[indice];
    }

    // Para las pruebas: la secuencia que se está mostrando
    public int[] SecuenciaActual => secuencia.ToArray();

    private void CrearCapa()
    {
        if (capa != null)
            return;

        var objeto = new GameObject("Forcejeo");
        objeto.transform.SetParent(transform, false);
        capa = objeto.AddComponent<Canvas>();
        capa.renderMode = RenderMode.ScreenSpaceOverlay;
        capa.sortingOrder = 102;
        var escala = objeto.AddComponent<CanvasScaler>();
        escala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escala.referenceResolution = new Vector2(640f, 360f);
        escala.matchWidthOrHeight = 0.5f;
        var fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var fondo = new GameObject("Fondo", typeof(RectTransform));
        fondo.transform.SetParent(objeto.transform, false);
        var rectFondo = (RectTransform)fondo.transform;
        rectFondo.anchorMin = new Vector2(0.1f, 0.06f);
        rectFondo.anchorMax = new Vector2(0.9f, 0.32f);
        rectFondo.offsetMin = rectFondo.offsetMax = Vector2.zero;
        fondo.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.82f);

        aviso = Texto(fondo.transform, fuente, 16, new Vector2(0f, 0.68f), new Vector2(1f, 1f), new Color32(230, 200, 190, 255));
        var fila = new GameObject("Flechas", typeof(RectTransform));
        fila.transform.SetParent(fondo.transform, false);
        var rectFila = (RectTransform)fila.transform;
        rectFila.anchorMin = new Vector2(0f, 0.2f);
        rectFila.anchorMax = new Vector2(1f, 0.7f);
        rectFila.offsetMin = rectFila.offsetMax = Vector2.zero;
        flecha = CrearFlecha();
        flechas = new Image[MaximoFlechas];
        for (var i = 0; i < MaximoFlechas; i++)
        {
            var objeto2 = new GameObject("Flecha" + i, typeof(RectTransform));
            objeto2.transform.SetParent(fila.transform, false);
            var imagen = objeto2.AddComponent<Image>();
            imagen.sprite = flecha;
            imagen.raycastTarget = false;
            imagen.rectTransform.sizeDelta = new Vector2(22f, 22f);
            flechas[i] = imagen;
        }

        var barraFondo = new GameObject("Tiempo", typeof(RectTransform));
        barraFondo.transform.SetParent(fondo.transform, false);
        var rectBarra = (RectTransform)barraFondo.transform;
        rectBarra.anchorMin = new Vector2(0.08f, 0.08f);
        rectBarra.anchorMax = new Vector2(0.92f, 0.15f);
        rectBarra.offsetMin = rectBarra.offsetMax = Vector2.zero;
        barra = barraFondo.AddComponent<Image>();
        barra.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        barra.type = Image.Type.Filled;
        barra.fillMethod = Image.FillMethod.Horizontal;
        barra.color = new Color32(150, 24, 24, 255);
        capa.gameObject.SetActive(false);
    }

    private static Text Texto(Transform padre, Font fuente, int tamano, Vector2 min, Vector2 max, Color color)
    {
        var objeto = new GameObject("Texto", typeof(RectTransform));
        objeto.transform.SetParent(padre, false);
        var rect = (RectTransform)objeto.transform;
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        var texto = objeto.AddComponent<Text>();
        texto.font = fuente;
        texto.fontSize = tamano;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = color;
        texto.supportRichText = true;
        texto.raycastTarget = false;
        return texto;
    }
}
