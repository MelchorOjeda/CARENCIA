using UnityEngine;

// Alterna el color de un sprite de forma irregular: televisión con interferencias o luces inestables.
[RequireComponent(typeof(SpriteRenderer))]
public class Parpadeo : MonoBehaviour
{
    [SerializeField] private Color colorA = Color.white;
    [SerializeField] private Color colorB = Color.gray;
    [SerializeField] private float rapidez = 8f;

    private SpriteRenderer figura;

    public void Configurar(Color a, Color b, float nuevaRapidez)
    {
        colorA = a;
        colorB = b;
        rapidez = nuevaRapidez;
    }

    private void Awake()
    {
        figura = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Opciones → Accesibilidad → Reducir destellos deja el color fijo en vez de alternarlo
        if (AjustesJuego.EnPantalla.reducirDestellos)
        {
            figura.color = colorA;
            return;
        }

        var valor = Mathf.PerlinNoise(Time.time * rapidez, transform.position.x);
        figura.color = valor > 0.5f ? colorA : colorB;
    }
}
