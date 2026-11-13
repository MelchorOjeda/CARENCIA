using UnityEngine;

// Una voz que se repite cada cierto tiempo, con bastante espacio entre una y otra: muy bajita desde lejos
// y cada vez más fuerte mientras Elaki se acerca a su origen (el "ayuda" del pasillo del Nivel 1).
[RequireComponent(typeof(AudioSource))]
public class VozQueSeAcerca : MonoBehaviour
{
    [SerializeField] private Transform origen;
    [SerializeField] private float distanciaLejos = 110f;
    [SerializeField] private float distanciaCerca = 4f;
    [SerializeField] private float volumenLejos = 0.03f;
    [SerializeField] private float volumenCerca = 0.55f;
    [SerializeField] private float esperaMinima = 8f;
    [SerializeField] private float esperaMaxima = 12f;

    private AudioSource fuente;
    private Transform jugador;
    private float siguiente;

    public void Configurar(Transform nuevoOrigen, float lejos, float cerca)
    {
        origen = nuevoOrigen;
        distanciaLejos = lejos;
        distanciaCerca = cerca;
    }

    public void ConfigurarVolumen(float nuevoLejos, float nuevoCerca, float nuevaEsperaMinima, float nuevaEsperaMaxima)
    {
        volumenLejos = nuevoLejos;
        volumenCerca = nuevoCerca;
        esperaMinima = nuevaEsperaMinima;
        esperaMaxima = nuevaEsperaMaxima;
    }

    private void Awake()
    {
        fuente = GetComponent<AudioSource>();
        fuente.loop = false;
        fuente.playOnAwake = false;
    }

    private void OnEnable()
    {
        // La primera se oye poco después de entrar al pasillo
        siguiente = Time.time + 2.5f;
    }

    private void Update()
    {
        if (jugador == null)
        {
            var controlador = FindAnyObjectByType<PlayerController>();
            jugador = controlador != null ? controlador.transform : null;
        }

        if (jugador == null || origen == null || Time.time < siguiente || fuente.clip == null)
            return;

        var distancia = Vector2.Distance(jugador.position, origen.position);
        var cercania = Mathf.InverseLerp(distanciaLejos, distanciaCerca, distancia);
        fuente.PlayOneShot(fuente.clip, Mathf.Lerp(volumenLejos, volumenCerca, cercania * cercania));
        siguiente = Time.time + Random.Range(esperaMinima, esperaMaxima);
    }
}
