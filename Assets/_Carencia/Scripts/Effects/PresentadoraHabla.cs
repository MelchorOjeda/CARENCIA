using UnityEngine;
using UnityEngine.UI;

// Noticiero de una sola imagen que parece en vivo: la presentadora mueve la boca al ritmo de su voz,
// parpadea de vez en cuando y la toma se acerca despacio. Empieza al activarse su objeto.
public class PresentadoraHabla : MonoBehaviour
{
    [SerializeField] private Image imagen;
    [Tooltip("Ojos abiertos: boca cerrada, entreabierta y abierta")]
    [SerializeField] private Sprite[] ojosAbiertos = new Sprite[3];
    [Tooltip("Parpadeo: boca cerrada, entreabierta y abierta")]
    [SerializeField] private Sprite[] parpadeo = new Sprite[3];
    [SerializeField] private AudioSource voz;
    [SerializeField] private RectTransform toma;
    [SerializeField] private float acercamiento = 0.08f;
    [SerializeField] private float duracion = 11f;

    [Header("Subtítulos")]
    [SerializeField] private TMPro.TextMeshProUGUI subtitulo;
    [SerializeField] private string[] lineas = new string[0];
    [Tooltip("Inicio y fin de cada línea en segundos del audio (x = inicio, y = fin)")]
    [SerializeField] private Vector2[] tiempos = new Vector2[0];

    private readonly float[] muestras = new float[256];
    private float inicio;
    private float siguienteParpadeo;
    private float finParpadeo;
    private int boca;

    public float Duracion => duracion;

    public void ConfigurarSubtitulos(TMPro.TextMeshProUGUI texto, string[] nuevasLineas, Vector2[] nuevosTiempos)
    {
        subtitulo = texto;
        lineas = nuevasLineas;
        tiempos = nuevosTiempos;
    }

    public void Configurar(Image nuevaImagen, Sprite[] abiertos, Sprite[] cerrados, AudioSource nuevaVoz, RectTransform nuevaToma, float nuevaDuracion)
    {
        imagen = nuevaImagen;
        ojosAbiertos = abiertos;
        parpadeo = cerrados;
        voz = nuevaVoz;
        toma = nuevaToma;
        duracion = nuevaDuracion;
    }

    private void OnEnable()
    {
        inicio = Time.time;
        siguienteParpadeo = Time.time + Random.Range(1.5f, 3.5f);
        if (voz != null)
            voz.Play();
    }

    private void Update()
    {
        // Boca según el volumen de la voz en este momento
        var nivel = 0f;
        if (voz != null && voz.isPlaying)
        {
            voz.GetOutputData(muestras, 0);
            for (var i = 0; i < muestras.Length; i++)
                nivel += muestras[i] * muestras[i];
            nivel = Mathf.Sqrt(nivel / muestras.Length) / Mathf.Max(0.05f, voz.volume);
        }

        var deseada = nivel > 0.12f ? 2 : nivel > 0.04f ? 1 : 0;
        // No cambia de boca en cada cuadro: así se ve como el habla de un dibujo, no como un parpadeo
        if (deseada != boca && Time.frameCount % 3 == 0)
            boca = deseada;

        if (Time.time >= siguienteParpadeo)
        {
            finParpadeo = Time.time + 0.13f;
            siguienteParpadeo = Time.time + Random.Range(2.5f, 5f);
        }

        var cuadros = Time.time < finParpadeo ? parpadeo : ojosAbiertos;
        if (imagen != null && cuadros != null && boca < cuadros.Length && cuadros[boca] != null)
            imagen.sprite = cuadros[boca];

        // Subtítulo de lo que dice, sincronizado con el audio
        if (subtitulo != null)
        {
            var momento = voz != null && voz.isPlaying ? voz.time : -1f;
            var texto = "";
            for (var i = 0; i < tiempos.Length && i < lineas.Length; i++)
                if (momento >= tiempos[i].x && momento <= tiempos[i].y)
                    texto = lineas[i];
            if (subtitulo.text != texto)
                subtitulo.text = texto;
            if (subtitulo.transform.parent != null)
                subtitulo.transform.parent.gameObject.SetActive(texto.Length > 0);
        }

        if (toma != null)
        {
            var k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((Time.time - inicio) / Mathf.Max(0.1f, duracion)));
            toma.localScale = Vector3.one * (1f + acercamiento * k);
        }
    }
}
