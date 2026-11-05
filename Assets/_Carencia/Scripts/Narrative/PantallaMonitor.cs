using UnityEngine;

// Pantalla del monitor dentro del escenario: gris, con una barra que muestra cuánto falta del video.
// Se dibuja inclinada 2:1, igual que el monitor, y por encima de Elaki para que no la tape durante el acercamiento.
[RequireComponent(typeof(SpriteRenderer))]
public class PantallaMonitor : MonoBehaviour
{
    [SerializeField] private int ancho = 64;
    [SerializeField] private int alto = 44;
    [SerializeField] private Color gris = new Color32(150, 152, 158, 255);
    [SerializeField] private Color fondoBarra = new Color32(70, 70, 76, 255);
    [SerializeField] private Color barra = new Color32(214, 60, 52, 255);

    private Texture2D textura;
    private Color32[] pixeles;
    private int altoTextura;
    private int avanceDibujado = -1;

    private void Awake()
    {
        altoTextura = alto + (ancho - 1) / 2;
        textura = new Texture2D(ancho, altoTextura, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        pixeles = new Color32[ancho * altoTextura];

        // Pivote en la esquina de arriba a la izquierda de la pantalla
        var pivote = new Vector2(0f, 1f - ((ancho - 1) / 2f) / altoTextura);
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(textura, new Rect(0, 0, ancho, altoTextura), pivote, 96f);
        SetAvance(0f);
    }

    public void SetAvance(float avance)
    {
        var lleno = Mathf.RoundToInt(Mathf.Clamp01(avance) * (ancho - 8));
        if (lleno == avanceDibujado)
            return;

        avanceDibujado = lleno;
        var vacio = new Color32(0, 0, 0, 0);
        for (var i = 0; i < pixeles.Length; i++)
            pixeles[i] = vacio;

        for (var x = 0; x < ancho; x++)
        {
            // Cada columna sube medio píxel por cada píxel hacia la derecha
            var desplazamiento = (ancho - 1) / 2 - x / 2;
            for (var y = 0; y < alto; y++)
            {
                Color color = gris;
                var enBarra = y >= alto - 8 && y < alto - 4 && x >= 4 && x < ancho - 4;
                if (enBarra)
                    color = x - 4 < lleno ? barra : fondoBarra;

                // Las filas de la textura cuentan desde abajo
                var fila = altoTextura - 1 - (desplazamiento + y);
                pixeles[fila * ancho + x] = color;
            }
        }

        textura.SetPixels32(pixeles);
        textura.Apply();
    }
}
