using UnityEngine;

// Recorre en bucle los cuadros de una hoja de sprites (por ejemplo, las moscas volando sobre un plato).
[RequireComponent(typeof(SpriteRenderer))]
public class AnimarSprites : MonoBehaviour
{
    [SerializeField] private Sprite[] cuadros = new Sprite[0];
    [SerializeField] private float cuadrosPorSegundo = 12f;

    private SpriteRenderer sr;
    private float desfase;

    public void Configurar(Sprite[] nuevosCuadros, float velocidad)
    {
        cuadros = nuevosCuadros;
        cuadrosPorSegundo = velocidad;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // Cada mosca empieza en un punto distinto para que no se muevan al mismo tiempo
        desfase = Random.value * 10f;
    }

    private void Update()
    {
        if (cuadros.Length == 0)
            return;

        var indice = (int)((Time.time + desfase) * cuadrosPorSegundo) % cuadros.Length;
        sr.sprite = cuadros[indice];
    }
}
