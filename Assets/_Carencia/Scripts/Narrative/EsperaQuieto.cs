using UnityEngine;

// Ejecuta una secuencia si Elaki se queda quieta dentro del área durante cierto tiempo.
// Se usa frente al espejo del prólogo: si el jugador no abre el gabinete, se abre solo.
[RequireComponent(typeof(Collider2D))]
public class EsperaQuieto : MonoBehaviour
{
    [SerializeField] private Secuencia secuencia;
    [SerializeField] private float segundos = 10f;

    private PlayerController jugador;
    private Vector3 ultimaPosicion;
    private float quieto;
    private int contactos;

    public void Configurar(Secuencia nuevaSecuencia, float nuevosSegundos)
    {
        secuencia = nuevaSecuencia;
        segundos = nuevosSegundos;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.GetComponentInParent<PlayerController>() != null)
            contactos++;
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.GetComponentInParent<PlayerController>() != null && contactos > 0)
        {
            contactos--;
            quieto = 0f;
        }
    }

    private void Update()
    {
        if (jugador == null)
            jugador = FindAnyObjectByType<PlayerController>();

        if (contactos == 0 || jugador == null || jugador.MovimientoBloqueado)
        {
            quieto = 0f;
            return;
        }

        var posicion = jugador.transform.position;
        quieto = (posicion - ultimaPosicion).sqrMagnitude < 0.0001f ? quieto + Time.deltaTime : 0f;
        ultimaPosicion = posicion;

        if (quieto >= segundos)
        {
            quieto = 0f;
            secuencia.Ejecutar();
        }
    }
}
