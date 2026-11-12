using UnityEngine;

// Recorre los cuadros de una hoja de sprites: en bucle (las moscas sobre un plato)
// o una sola vez, quedándose en el último cuadro (la cortina de una bañera al correrse).
[RequireComponent(typeof(SpriteRenderer))]
public class AnimarSprites : MonoBehaviour
{
    [SerializeField] private Sprite[] cuadros = new Sprite[0];
    [SerializeField] private float cuadrosPorSegundo = 12f;
    [SerializeField] private bool bucle = true;

    private SpriteRenderer sr;
    private float desfase;

    public void Configurar(Sprite[] nuevosCuadros, float velocidad, bool enBucle = true)
    {
        cuadros = nuevosCuadros;
        cuadrosPorSegundo = velocidad;
        bucle = enBucle;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // En bucle cada uno empieza en un punto distinto; una sola vez, empieza al aparecer
        desfase = bucle ? Random.value * 10f : -Time.time;
    }

    private void Update()
    {
        if (cuadros.Length == 0)
            return;

        var avance = (int)((Time.time + desfase) * cuadrosPorSegundo);
        var indice = bucle ? avance % cuadros.Length : Mathf.Min(avance, cuadros.Length - 1);
        sr.sprite = cuadros[indice];
    }
}
