using UnityEngine;

// Ejecuta una secuencia si Elaki se queda quieta dentro del área durante cierto tiempo.
// Se usa frente al espejo del prólogo: si el jugador no abre el gabinete, se abre solo.
[RequireComponent(typeof(Collider2D))]
public class EsperaQuieto : MonoBehaviour
{
    // Alejarse menos de esto del punto donde se detuvo no cuenta: el perro o un mueble pueden empujarla un poco
    private const float Tolerancia = 0.15f;

    [SerializeField] private Secuencia secuencia;
    [SerializeField] private float segundos = 10f;

    private Collider2D area;
    private PlayerController jugador;
    private Vector3 ultimaPosicion;
    private float quieto;
    private UnityEngine.InputSystem.InputAction mover;

    public void Configurar(Secuencia nuevaSecuencia, float nuevosSegundos)
    {
        secuencia = nuevaSecuencia;
        segundos = nuevosSegundos;
    }

    private void Awake()
    {
        area = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (jugador == null)
            jugador = FindAnyObjectByType<PlayerController>();

        // Se mira la posición de Elaki directamente: no depende de los eventos de entrada al área
        if (jugador == null || jugador.MovimientoBloqueado || !area.OverlapPoint(jugador.transform.position))
        {
            quieto = 0f;
            return;
        }

        // Quieto es no estar caminando: si el perro la empuja un poco, sigue contando
        if (mover == null)
            mover = UnityEngine.InputSystem.InputSystem.actions.FindAction("Player/Move");
        var caminando = mover != null && mover.ReadValue<Vector2>().sqrMagnitude > 0.01f;
        var posicion = jugador.transform.position;
        if (!caminando && (quieto == 0f || Vector2.Distance(posicion, ultimaPosicion) < 0.6f))
        {
            if (quieto == 0f)
                ultimaPosicion = posicion;
            quieto += Time.deltaTime;
        }
        else
        {
            quieto = 0f;
        }

        if (quieto >= segundos)
        {
            quieto = 0f;
            secuencia.Ejecutar();
        }
    }
}
