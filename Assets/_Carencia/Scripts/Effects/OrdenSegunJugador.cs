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

        var celda = (Vector2Int)grid.WorldToCell(jugador.position);
        var delante = false;
        foreach (var zona in zonasDelante)
            delante |= zona.Contains(celda);
        sr.sortingOrder = delante ? ordenCuandoElakiDelante : ordenCuandoElakiDetras;
    }
}
