using UnityEngine;

// Penumbra propia de un nivel: en el Nivel 3 solo queda una luz pequeña alrededor de Elaki y todo lo demás casi no se ve.
// Cambia la forma de la penumbra al empezar la escena; su intensidad la sigue poniendo la secuencia del ambiente.
public class PenumbraNivel : MonoBehaviour
{
    [SerializeField] private float centro = 0.07f;
    [SerializeField] private float borde = 0.34f;
    [SerializeField] private Color color = new Color(0.01f, 0f, 0.01f);

    public void Configurar(float nuevoCentro, float nuevoBorde, Color nuevoColor)
    {
        centro = nuevoCentro;
        borde = nuevoBorde;
        color = nuevoColor;
    }

    private void Start()
    {
        if (PantallaJuego.Instancia != null)
            PantallaJuego.Instancia.ConfigurarPenumbra(centro, borde, color);
    }

    // La pantalla sobrevive al cambio de escena: al salir del nivel vuelve la penumbra de siempre
    private void OnDestroy()
    {
        if (PantallaJuego.Instancia != null)
        {
            PantallaJuego.Instancia.RestaurarPenumbra();
            PantallaJuego.Instancia.SetMirada(0f);
        }
    }
}
