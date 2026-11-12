using System.Collections;
using UnityEngine;

// Daño de Elaki: no hay barra de vida, cada golpe aumenta las manchas rojas y al llegar al límite se reinicia el punto de control.
public class SaludJugador : MonoBehaviour
{
    [SerializeField] private int golpesParaMorir = 3;

    [Tooltip("Segundos de la animación de muerte antes de reiniciar")]
    [SerializeField] private float duracionMuerte = 2.4f;

    private bool muerto;

    // Escena a la que lleva la muerte (formato escena|entrada). Vacío: se reinicia el punto de control
    private string destinoAlMorir;

    // La muerte frente a "ella" no es una derrota: es la transición al siguiente despertar
    public void LlevarAlMorir(string destino)
    {
        destinoAlMorir = destino;
    }

    private void Start()
    {
        PantallaJuego.Instancia.SetManchas(Intensidad());
    }

    // Un solo golpe basta: "ella" siempre alcanza a Elaki
    public void Matar()
    {
        if (muerto)
            return;

        EstadoJuego.Golpes = golpesParaMorir - 1;
        Golpear();
    }

    public void Golpear()
    {
        if (muerto)
            return;

        EstadoJuego.Golpes++;
        PantallaJuego.Instancia.SetManchas(Intensidad());

        if (EstadoJuego.Golpes < golpesParaMorir)
            return;

        muerto = true;
        GetComponent<PlayerController>().MovimientoBloqueado = true;
        StartCoroutine(Morir());
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
        return (float)EstadoJuego.Golpes / golpesParaMorir;
    }
}
