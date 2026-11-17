using TMPro;
using UnityEngine;
using UnityEngine.Video;

// Subtítulos sobre una cinemática, sincronizados con el tiempo del video. Cada línea es una clave del guion
// (SO_Dialogo_Guion) y se muestra con el nombre de quien habla.
public class SubtitulosVideo : MonoBehaviour
{
    [SerializeField] private VideoPlayer video;
    [SerializeField] private TextMeshProUGUI subtitulo;
    [Tooltip("Claves del guion, en orden")]
    [SerializeField] private string[] claves = new string[0];
    [Tooltip("Inicio y fin de cada línea en segundos del video (x = inicio, y = fin)")]
    [SerializeField] private Vector2[] tiempos = new Vector2[0];

    private string[] lineas;

    public void Configurar(VideoPlayer nuevoVideo, TextMeshProUGUI texto, string[] nuevasClaves, Vector2[] nuevosTiempos)
    {
        video = nuevoVideo;
        subtitulo = texto;
        claves = nuevasClaves;
        tiempos = nuevosTiempos;
    }

    private void Awake()
    {
        var guion = Resources.Load<GuionDialogos>(GuionDialogos.RutaEnResources);
        lineas = new string[claves.Length];
        for (var i = 0; i < claves.Length; i++)
        {
            var linea = guion != null ? guion.LineaDe(claves[i]) : null;
            lineas[i] = linea == null ? claves[i]
                : string.IsNullOrEmpty(linea.quien) ? linea.texto
                : "<color=#A39C8C>" + linea.quien + ":</color>  " + linea.texto;
        }
    }

    private void OnEnable() => Mostrar("");

    private void Update()
    {
        var momento = video != null && video.isPlaying ? video.time : -1d;
        var texto = "";
        for (var i = 0; i < tiempos.Length && i < lineas.Length; i++)
            if (momento >= tiempos[i].x && momento <= tiempos[i].y)
                texto = lineas[i];
        Mostrar(texto);
    }

    private void Mostrar(string texto)
    {
        if (subtitulo == null)
            return;
        if (subtitulo.text != texto)
            subtitulo.text = texto;
        if (subtitulo.transform.parent != null)
            subtitulo.transform.parent.gameObject.SetActive(texto.Length > 0);
    }
}
