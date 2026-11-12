using UnityEngine;

// Boca en el piso de carne (Nivel 3): respira cerrada; cuando Elaki pasa cerca se abre enseñando los dientes
// y muerde. Si Elaki sigue encima cuando se cierra, la lastima. Se puede esquivar pasando de largo rápido o rodeándola.
[RequireComponent(typeof(SpriteRenderer))]
public class BocaCarne : MonoBehaviour
{
    private enum Estado { Esperando, Abriendo, Mordida, Descanso }

    // 0 cerrada, 1 entreabierta, 2 abierta con dientes, 3 mordida
    [SerializeField] private Sprite[] cuadros = new Sprite[4];
    [SerializeField] private float radio = 0.42f;
    [SerializeField] private float alcanceAviso = 0.9f;
    [SerializeField] private float aviso = 0.55f;
    [SerializeField] private float descanso = 1.3f;
    [SerializeField] private AudioSource sonidoMordida;

    private SpriteRenderer sr;
    private Transform jugador;
    private SaludJugador salud;
    private Estado estado;
    private float tiempo;
    private float desfase;

    public void Configurar(Sprite[] nuevosCuadros, AudioSource mordida)
    {
        cuadros = nuevosCuadros;
        sonidoMordida = mordida;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        desfase = Random.value * 5f;
    }

    private void Start()
    {
        var control = FindAnyObjectByType<PlayerController>();
        if (control == null)
            return;
        jugador = control.transform;
        salud = control.GetComponent<SaludJugador>();
    }

    private void Update()
    {
        if (jugador == null)
            return;

        tiempo += Time.deltaTime;
        var distancia = Distancia();
        switch (estado)
        {
            case Estado.Esperando:
                // Respira: de vez en cuando se entreabre sola
                Poner(Mathf.Repeat(Time.time + desfase, 2.6f) < 0.25f ? 1 : 0);
                if (distancia < alcanceAviso)
                    Cambiar(Estado.Abriendo);
                break;
            case Estado.Abriendo:
                // Tiembla entre entreabierta y abierta mientras decide
                Poner(Mathf.Repeat(tiempo, 0.16f) < 0.08f ? 1 : 2);
                if (tiempo >= aviso)
                {
                    Cambiar(Estado.Mordida);
                    if (sonidoMordida != null)
                        sonidoMordida.Play();
                    if (distancia < radio && salud != null)
                        salud.Golpear();
                }
                break;
            case Estado.Mordida:
                Poner(3);
                if (tiempo >= 0.35f)
                    Cambiar(Estado.Descanso);
                break;
            case Estado.Descanso:
                Poner(0);
                if (tiempo >= descanso)
                    Cambiar(Estado.Esperando);
                break;
        }
    }

    // En el piso isométrico una distancia vertical cuenta doble
    private float Distancia()
    {
        var d = jugador.position - transform.position;
        return new Vector2(d.x, d.y * 2f).magnitude;
    }

    private void Cambiar(Estado nuevo)
    {
        estado = nuevo;
        tiempo = 0f;
    }

    private void Poner(int cuadro)
    {
        if (cuadro < cuadros.Length && cuadros[cuadro] != null)
            sr.sprite = cuadros[cuadro];
    }
}
