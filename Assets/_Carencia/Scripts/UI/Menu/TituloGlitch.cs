using UnityEngine;

// Título con tres copias desfasadas (roja, azul y hueso) que tiemblan de vez en cuando.
public class TituloGlitch : MonoBehaviour
{
    [SerializeField] private RectTransform capaRoja;
    [SerializeField] private RectTransform capaAzul;
    [SerializeField] private Vector2 desfaseRojo = new Vector2(12f, 0f);
    [SerializeField] private Vector2 desfaseAzul = Vector2.zero;
    [SerializeField] private float temblorMaximo = 6f;

    private float siguienteTemblor;

    private void OnEnable()
    {
        Colocar(Vector2.zero, Vector2.zero);
    }

    private void Update()
    {
        // Tiempo sin escala: el título también tiembla con el juego en pausa
        if (Time.unscaledTime < siguienteTemblor)
            return;

        var esSacudida = Random.value < 0.15f;
        var fuerza = esSacudida ? temblorMaximo : temblorMaximo * 0.3f;
        Colocar(Random.insideUnitCircle * fuerza, Random.insideUnitCircle * fuerza * 0.5f);
        siguienteTemblor = Time.unscaledTime + (esSacudida ? 0.06f : Random.Range(0.12f, 0.35f));
    }

    private void Colocar(Vector2 temblorRojo, Vector2 temblorAzul)
    {
        capaRoja.anchoredPosition = desfaseRojo + new Vector2(temblorRojo.x, temblorRojo.y * 0.2f);
        capaAzul.anchoredPosition = desfaseAzul + new Vector2(temblorAzul.x, temblorAzul.y * 0.2f);
    }
}
