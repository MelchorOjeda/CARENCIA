using UnityEngine;
using UnityEngine.InputSystem;

// Mueve a Elaki con WASD o flechas según las direcciones de la pantalla.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float velocidad = 1.5f;

    [Tooltip("Velocidad con Shift, pensada para escapar de las persecuciones")]
    [SerializeField] private float velocidadCarrera = 2.6f;

    // Las cinemáticas, transiciones y vistas de inspección lo activan para detener al jugador
    [SerializeField] private bool movimientoBloqueado;

    private Rigidbody2D rb;
    private InputAction accionMover;
    private InputAction accionCorrer;
    private Vector2 direccion;

    public Vector2 Direccion => direccion;
    public bool EstaCaminando => direccion != Vector2.zero;
    public bool EstaCorriendo => EstaCaminando && accionCorrer.IsPressed();

    public bool MovimientoBloqueado
    {
        get => movimientoBloqueado;
        set => movimientoBloqueado = value;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        accionMover = InputSystem.actions.FindAction("Player/Move", true);
        accionCorrer = InputSystem.actions.FindAction("Player/Sprint", true);
    }

    private void Update()
    {
        if (movimientoBloqueado)
        {
            direccion = Vector2.zero;
            return;
        }

        // Limitar la magnitud a 1 hace que la diagonal tenga la misma velocidad que el movimiento recto
        direccion = Vector2.ClampMagnitude(accionMover.ReadValue<Vector2>(), 1f);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direccion * (EstaCorriendo ? velocidadCarrera : velocidad);
    }

    // Lleva a Elaki a un punto sin recorrer el camino: entradas de escena y secuencias
    public void Colocar(Vector2 posicion)
    {
        direccion = Vector2.zero;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.zero;
        rb.position = posicion;
        transform.position = posicion;
    }
}
