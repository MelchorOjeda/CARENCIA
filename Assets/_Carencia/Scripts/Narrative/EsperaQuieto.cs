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

        var posicion = jugador.transform.position;
        if (quieto > 0f && Vector2.Distance(posicion, ultimaPosicion) < Tolerancia)
        {
            quieto += Time.deltaTime;
        }
        else
        {
            // Empieza a contar desde aquí
            ultimaPosicion = posicion;
            quieto = Time.deltaTime;
        }

        if (quieto >= segundos)
        {
            quieto = 0f;
            secuencia.Ejecutar();
        }
    }
}
