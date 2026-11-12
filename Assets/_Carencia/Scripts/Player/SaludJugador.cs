using System.Collections;
using UnityEngine;

// Daño de Elaki: no hay barra de vida; lo único que la muestra es qué tan roja está la pantalla.
// Cada golpe aumenta las manchas rojas (con un destello) y al llegar al límite se reinicia el punto de control.
// Con regeneración (Nivel 3), si pasa un rato sin recibir daño las manchas se van borrando poco a poco.
public class SaludJugador : MonoBehaviour
{
    [SerializeField] private int golpesParaMorir = 3;

    [Tooltip("Segundos de la animación de muerte antes de reiniciar")]
    [SerializeField] private float duracionMuerte = 2.4f;

    [Header("Regeneración")]
    [SerializeField] private bool regenerar;
    [Tooltip("Segundos sin daño antes de empezar a sanar")]
    [SerializeField] private float esperaRegeneracion = 4f;
    [Tooltip("Golpes que sana por segundo una vez que empieza")]
    [SerializeField] private float golpesPorSegundo = 0.2f;

    private bool muerto;
    // Daño que todavía no completa un golpe (el ojo de la ventana lastima poco a poco)
    private float parcial;
    private float ultimoDano = -100f;
    private float ultimoGolpe = -100f;
    // Tras un golpe, un instante sin recibir otro: si varias cosas la tocan a la vez cuenta como uno
    private const float Invulnerable = 0.8f;
    private float destello;

    // Escena a la que lleva la muerte (formato escena|entrada). Vacío: se reinicia el punto de control
    private string destinoAlMorir;

    public bool Muerto => muerto;

    // La muerte frente a "ella" no es una derrota: es la transición al siguiente despertar
    public void LlevarAlMorir(string destino)
    {
        destinoAlMorir = destino;
    }

    public void Configurar(bool conRegeneracion, float espera, float ritmo)
    {
        regenerar = conRegeneracion;
        esperaRegeneracion = espera;
        golpesPorSegundo = ritmo;
    }

    private void Start()
    {
        Pintar();
    }

    // Un solo golpe basta: "ella" siempre alcanza a Elaki (no la frena el instante sin daño tras otro golpe)
    public void Matar()
    {
        if (muerto)
            return;

        EstadoJuego.Golpes = golpesParaMorir - 1;
        ultimoGolpe = -100f;
        Golpear();
    }

    private void Update()
    {
        if (muerto)
            return;

        if (regenerar && Time.time - ultimoDano > esperaRegeneracion && (EstadoJuego.Golpes > 0 || parcial > 0f))
        {
            parcial -= golpesPorSegundo * Time.deltaTime;
            while (parcial < 0f && EstadoJuego.Golpes > 0)
            {
                EstadoJuego.Golpes--;
                parcial += 1f;
            }

            parcial = Mathf.Max(0f, parcial);
        }

        destello = Mathf.MoveTowards(destello, 0f, Time.deltaTime * 1.6f);
        Pintar();
    }

    // Borra todo el daño, también el que no completaba un golpe
    public void Curar()
    {
        EstadoJuego.Golpes = 0;
        parcial = 0f;
        destello = 0f;
        Pintar();
    }

    public void Golpear()
    {
        if (muerto || Time.time - ultimoGolpe < Invulnerable)
            return;

        ultimoGolpe = Time.time;
        ultimoDano = Time.time;
        destello = 0.35f;
        EstadoJuego.Golpes++;
        Pintar();
        Revisar();
    }

    // Daño continuo, en fracciones de golpe (por ejemplo, mientras el ojo la ve moverse)
    public void Herir(float cantidad)
    {
        if (muerto || cantidad <= 0f)
            return;

        ultimoDano = Time.time;
        destello = Mathf.Max(destello, 0.12f);
        parcial += cantidad;
        while (parcial >= 1f)
        {
            parcial -= 1f;
            EstadoJuego.Golpes++;
        }

        Pintar();
        Revisar();
    }

    private void Revisar()
    {
        if (EstadoJuego.Golpes < golpesParaMorir)
            return;

        muerto = true;
        GetComponent<PlayerController>().MovimientoBloqueado = true;
        StartCoroutine(Morir());
    }

    private void Pintar()
    {
        if (PantallaJuego.Instancia != null)
            PantallaJuego.Instancia.SetManchas(Intensidad() + destello);
    }

    private IEnumerator Morir()
    {
        var animador = GetComponent<AnimadorJugador>();
        if (animador != null)
        {
            animador.Morir();
            yield return new WaitForSeconds(duracionMuerte);
        }

        if (string.IsNullOrEmpty(destinoAlMorir))
        {
            PantallaJuego.Instancia.ReiniciarDesdeControl();
            yield break;
        }

        yield return PantallaJuego.Instancia.Oscurecer(2.4f);
        EstadoJuego.Golpes = 0;
        PantallaJuego.Instancia.SetManchas(0f);
        var partes = destinoAlMorir.Split('|');
        PantallaJuego.Instancia.CargarEscena(partes[0], partes.Length > 1 ? partes[1] : null);
    }

    private float Intensidad()
    {
        return (EstadoJuego.Golpes + parcial) / golpesParaMorir;
    }
}
