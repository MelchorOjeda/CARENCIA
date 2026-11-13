using UnityEngine;

// Muestra un cuarto aislado (el cuarto de lavado de la casa real, al que se entra por una puerta que lleva de un salto)
// solo mientras Elaki está dentro: así no se asoma por la orilla de la pantalla desde otras habitaciones.
public class MostrarSoloDentro : MonoBehaviour
{
    [SerializeField] private Renderer[] dibujos = new Renderer[0];
    [SerializeField] private Vector2[] area = new Vector2[0];

    private Transform jugador;
    private bool visible = true;

    public void Configurar(Renderer[] nuevosDibujos, Vector2[] nuevaArea)
    {
        dibujos = nuevosDibujos;
        area = nuevaArea;
    }

    private void Update()
    {
        if (jugador == null)
        {
            var control = FindAnyObjectByType<PlayerController>();
            jugador = control != null ? control.transform : null;
        }

        var dentro = jugador != null && Contiene(jugador.position);
        if (dentro == visible)
            return;

        visible = dentro;
        foreach (var dibujo in dibujos)
            if (dibujo != null)
                dibujo.enabled = dentro;
    }

    private bool Contiene(Vector2 punto)
    {
        var dentro = false;
        for (int i = 0, j = area.Length - 1; i < area.Length; j = i++)
        {
            if ((area[i].y > punto.y) != (area[j].y > punto.y) &&
                punto.x < (area[j].x - area[i].x) * (punto.y - area[i].y) / (area[j].y - area[i].y) + area[i].x)
                dentro = !dentro;
        }

        return dentro;
    }
}
