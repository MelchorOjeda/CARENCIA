using UnityEngine;

// Para muebles grandes en forma de L (la barra de la cocina del Nivel 3): un solo punto de orden no sirve
// para los dos lados. Si Elaki está en una de las zonas de enfrente, el mueble se dibuja detrás de ella;
// si está del otro lado, delante.
[RequireComponent(typeof(SpriteRenderer))]
public class OrdenSegunJugador : MonoBehaviour
{
    [SerializeField] private RectInt[] zonasDelante = new RectInt[0];
    [SerializeField] private int ordenCuandoElakiDelante = -1;
    [SerializeField] private int ordenCuandoElakiDetras = 1;
    [Tooltip("Opcional: borde frontal del mueble en el mundo (puntos de izquierda a derecha). Si se usa, Elaki queda detrás " +
             "solo cuando sus pies están más arriba que ese borde y dentro de su largo; fuera de él siempre queda delante")]
    [SerializeField] private Vector2[] bordeFrontal = new Vector2[0];

    private SpriteRenderer sr;
    private Grid grid;
    private Transform jugador;

    public void Configurar(RectInt[] zonas)
    {
        zonasDelante = zonas;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        grid = GetComponentInParent<Grid>();
    }

    private void LateUpdate()
    {
        if (jugador == null)
        {
            var control = FindAnyObjectByType<PlayerController>();
            if (control == null || grid == null)
                return;
            jugador = control.transform;
        }

        bool delante;
        if (bordeFrontal != null && bordeFrontal.Length >= 2)
        {
            delante = true;
            Vector2 pies = jugador.position;
            for (var i = 0; i < bordeFrontal.Length - 1; i++)
            {
                var a = bordeFrontal[i];
                var b = bordeFrontal[i + 1];
                if (pies.x < Mathf.Min(a.x, b.x) || pies.x > Mathf.Max(a.x, b.x))
                    continue;
                var y = Mathf.Lerp(a.y, b.y, Mathf.InverseLerp(a.x, b.x, pies.x));
                delante = pies.y < y;
                break;
            }
        }
        else
        {
            var celda = (Vector2Int)grid.WorldToCell(jugador.position);
            delante = false;
            foreach (var zona in zonasDelante)
                delante |= zona.Contains(celda);
        }
        sr.sortingOrder = delante ? ordenCuandoElakiDelante : ordenCuandoElakiDetras;
    }
}
