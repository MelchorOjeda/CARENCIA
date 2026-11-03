using UnityEngine;
using UnityEngine.InputSystem;

// Mueve a Elaki con WASD o flechas según las direcciones de la pantalla.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float velocidad = 1.5f;

    // Las cinemáticas, transiciones y vistas de inspección lo activan para detener al jugador
    [SerializeField] private bool movimientoBloqueado;

    private Rigidbody2D rb;
    private InputAction accionMover;
    private Vector2 direccion;

    public bool MovimientoBloqueado
    {
        get => movimientoBloqueado;
        set => movimientoBloqueado = value;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        accionMover = InputSystem.actions.FindAction("Player/Move", true);
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
        rb.linearVelocity = direccion * velocidad;
    }
}
