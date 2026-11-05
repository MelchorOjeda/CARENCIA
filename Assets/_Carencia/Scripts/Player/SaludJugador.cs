using UnityEngine;

// Daño de Elaki: no hay barra de vida, cada golpe aumenta las manchas rojas y al llegar al límite se reinicia el punto de control.
public class SaludJugador : MonoBehaviour
{
    [SerializeField] private int golpesParaMorir = 3;

    private bool muerto;

    private void Start()
    {
        PantallaJuego.Instancia.SetManchas(Intensidad());
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
        PantallaJuego.Instancia.ReiniciarDesdeControl();
    }

    private float Intensidad()
    {
        return (float)EstadoJuego.Golpes / golpesParaMorir;
    }
}
