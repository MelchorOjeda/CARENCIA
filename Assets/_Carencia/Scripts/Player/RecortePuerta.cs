using System;
using UnityEngine;

// Oculta la parte de Elaki que ya cruzó una puerta en una pared de atrás a la derecha.
// Las coordenadas de celda son continuas: la celda 11 va de 11.0 a 12.0.
public class RecortePuerta : MonoBehaviour
{
    [Serializable]
    public class Puerta
    {
        [Tooltip("Celda X de la pared; su borde visible es este valor")]
        public float planoX;
        public float desdeY;
        public float hastaY;
    }

    [SerializeField] private Grid grid;
    [SerializeField] private SpriteMask mascara;
    [SerializeField] private Puerta[] puertas;

    // Frente a una pared, los pies de Elaki no pasan de esta distancia al plano
    private const float InicioAntesDelPlano = 1f;

    // A esta distancia ya cruzó por completo y se ve la vista del cuarto
    private const float FinDespuesDelPlano = 1f;

    // El marco ocupa este tramo de la puerta: el corte nunca llega más allá
    private const float AnchoMarco = 0.17f;

    private const float LadoMascara = 8f;

    public void Configurar(Grid nuevoGrid, SpriteMask nuevaMascara, Puerta[] nuevasPuertas)
    {
        grid = nuevoGrid;
        mascara = nuevaMascara;
        puertas = nuevasPuertas;
    }

    private void LateUpdate()
    {
        var pies = grid.LocalToCellInterpolated(grid.WorldToLocal(transform.position));

        foreach (var puerta in puertas)
        {
            if (pies.y < puerta.desdeY || pies.y >= puerta.hastaY)
                continue;
            if (pies.x <= puerta.planoX - InicioAntesDelPlano || pies.x >= puerta.planoX + FinDespuesDelPlano)
                continue;

            ColocarMascara(puerta, pies.y);
            return;
        }

        mascara.enabled = false;
    }

    private void ColocarMascara(Puerta puerta, float piesY)
    {
        var corteEnY = Mathf.Max(piesY, puerta.desdeY + AnchoMarco);
        var corte = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3(puerta.planoX, corteEnY, 0f)));

        mascara.enabled = true;
        mascara.transform.position = new Vector3(corte.x + LadoMascara / 2f, transform.position.y, transform.position.z);
    }
}
