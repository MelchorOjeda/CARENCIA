using UnityEngine;
using UnityEngine.InputSystem;

// Silla en la que Elaki se sienta: se oculta su dibujo y el de la silla y aparece Elaki sentada
// (la animación de sentarse una vez y luego la de esperar sentada en bucle; ambas traen la silla dibujada).
// Libre: se sienta con E y se levanta con E otra vez o al caminar. Desde una secuencia (el desayuno),
// se levanta sola cuando la secuencia la lleva a otro lugar.
public class SillaSentable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer silla;
    [SerializeField] private SpriteRenderer sentada;
    [SerializeField] private Sprite[] sentarse = new Sprite[0];
    [SerializeField] private Sprite[] espera = new Sprite[0];
    [SerializeField] private float cuadrosSentarse = 10f;
    [SerializeField] private float cuadrosEspera = 3f;
    [Tooltip("Punto del piso donde Elaki queda sentada. Vacío: la posición de la silla (su punto de orden puede estar más adelante)")]
    [SerializeField] private Vector2 asiento;
    [SerializeField] private bool conAsiento;

    private PlayerController jugador;
    private SpriteRenderer[] dibujosJugador;
    private bool[] estabanVisibles;
    private InputAction mover;
    private bool ocupada;
    private bool porSecuencia;
    private float inicio;
    private Vector2 dondeEstaba;
    private Rigidbody2D cuerpo;
    private RigidbodyType2D tipoCuerpo;
    private bool contactosCompletos;

    public bool Ocupada => ocupada;

    private Vector2 Asiento => conAsiento ? asiento : (Vector2)transform.position;

    public void FijarAsiento(Vector2 punto)
    {
        asiento = punto;
        conAsiento = true;
    }

    public void Configurar(SpriteRenderer nuevaSilla, SpriteRenderer nuevaSentada, Sprite[] cuadrosAlSentarse, Sprite[] cuadrosSentada)
    {
        silla = nuevaSilla;
        sentada = nuevaSentada;
        sentarse = cuadrosAlSentarse;
        espera = cuadrosSentada;
    }

    // La silla más cercana a un punto (la del desayuno cuando una secuencia sienta a Elaki)
    public static SillaSentable MasCercana(Vector2 punto, float distanciaMaxima)
    {
        SillaSentable mejor = null;
        var mejorDistancia = distanciaMaxima;
        foreach (var candidata in FindObjectsByType<SillaSentable>(FindObjectsSortMode.None))
        {
            var distancia = Vector2.Distance(punto, candidata.Asiento);
            if (distancia <= mejorDistancia)
            {
                mejor = candidata;
                mejorDistancia = distancia;
            }
        }

        return mejor;
    }

    public static SillaSentable LaOcupada()
    {
        foreach (var candidata in FindObjectsByType<SillaSentable>(FindObjectsSortMode.None))
            if (candidata.ocupada)
                return candidata;
        return null;
    }

    private void Awake()
    {
        if (sentada != null)
            sentada.enabled = false;
        mover = InputSystem.actions != null ? InputSystem.actions.FindAction("Player/Move") : null;
    }

    // La E de la silla: sentarse o, si ya está sentada aquí, levantarse
    public void Alternar()
    {
        if (ocupada)
            Levantar();
        else
            Sentar(false);
    }

    public void Sentar(bool desdeSecuencia)
    {
        if (ocupada)
            return;

        jugador = FindAnyObjectByType<PlayerController>();
        if (jugador == null)
            return;

        ocupada = true;
        porSecuencia = desdeSecuencia;
        inicio = Time.time;
        dondeEstaba = jugador.transform.position;
        // La silla está junto a la mesa (con colisión): cinemática, la colisión no la saca de la silla,
        // pero sigue detectando lo que tiene cerca para revisarlo sentada
        cuerpo = jugador.GetComponent<Rigidbody2D>();
        if (cuerpo != null)
        {
            tipoCuerpo = cuerpo.bodyType;
            contactosCompletos = cuerpo.useFullKinematicContacts;
            cuerpo.linearVelocity = Vector2.zero;
            cuerpo.bodyType = RigidbodyType2D.Kinematic;
            cuerpo.useFullKinematicContacts = true;
        }
        jugador.Colocar(Asiento);
        if (!desdeSecuencia)
            jugador.MovimientoBloqueado = true;

        dibujosJugador = jugador.GetComponentsInChildren<SpriteRenderer>();
        estabanVisibles = new bool[dibujosJugador.Length];
        for (var i = 0; i < dibujosJugador.Length; i++)
        {
            estabanVisibles[i] = dibujosJugador[i].enabled;
            dibujosJugador[i].enabled = false;
        }

        if (silla != null)
            silla.enabled = false;
        if (sentada != null)
        {
            sentada.enabled = true;
            sentada.sprite = sentarse.Length > 0 ? sentarse[0] : (espera.Length > 0 ? espera[0] : sentada.sprite);
        }
    }

    public void Levantar()
    {
        if (!ocupada)
            return;

        ocupada = false;
        if (cuerpo != null)
        {
            cuerpo.bodyType = tipoCuerpo;
            cuerpo.useFullKinematicContacts = contactosCompletos;
        }
        if (dibujosJugador != null)
            for (var i = 0; i < dibujosJugador.Length; i++)
                if (dibujosJugador[i] != null)
                    dibujosJugador[i].enabled = estabanVisibles[i];

        if (silla != null)
            silla.enabled = true;
        if (sentada != null)
            sentada.enabled = false;
        // Sentada por su cuenta: vuelve a donde estaba parada, fuera de la silla
        if (!porSecuencia && jugador != null)
        {
            jugador.Colocar(dondeEstaba);
            jugador.MovimientoBloqueado = false;
        }
    }

    private void Update()
    {
        if (!ocupada || jugador == null)
            return;

        // Sentarse una vez y luego esperar sentada en bucle
        var t = Time.time - inicio;
        var cuadro = (int)(t * cuadrosSentarse);
        if (cuadro < sentarse.Length)
            sentada.sprite = sentarse[cuadro];
        else if (espera.Length > 0)
            sentada.sprite = espera[(int)((t - sentarse.Length / cuadrosSentarse) * cuadrosEspera) % espera.Length];

        // Una secuencia la llevó a otro lado (al levantarse del desayuno): se levanta sola
        if (Vector2.Distance(jugador.transform.position, Asiento) > 0.3f)
        {
            Levantar();
            return;
        }

        // Sentada por su cuenta: caminar la levanta (tras terminar de sentarse)
        if (!porSecuencia && mover != null && t > sentarse.Length / cuadrosSentarse && mover.ReadValue<Vector2>().sqrMagnitude > 0.25f)
            Levantar();
    }
}
