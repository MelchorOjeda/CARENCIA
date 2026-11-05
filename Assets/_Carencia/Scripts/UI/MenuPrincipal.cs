using UnityEngine;
using UnityEngine.UI;

// Menú inicial: nueva partida, continuar desde el último punto de control y salir.
public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private string primeraEscena = "PRO_Casa";
    [SerializeField] private Button botonContinuar;

    public void Configurar(string escena, Button continuar)
    {
        primeraEscena = escena;
        botonContinuar = continuar;
    }

    private void Start()
    {
        PantallaJuego.Instancia.Fundir(0f, 0.3f);
        PantallaJuego.Instancia.SetPenumbra(0f);
        PantallaJuego.Instancia.SetManchas(0f);
        botonContinuar.interactable = EstadoJuego.HayPartida;
    }

    public void NuevaPartida()
    {
        EstadoJuego.NuevaPartida();
        PantallaJuego.Instancia.CargarEscena(primeraEscena, null);
    }

    public void Continuar()
    {
        if (EstadoJuego.CargarPartida())
            PantallaJuego.Instancia.CargarEscena(EstadoJuego.EscenaControl, EstadoJuego.EntradaControl);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
