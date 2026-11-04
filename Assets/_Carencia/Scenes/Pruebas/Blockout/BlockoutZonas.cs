using System;
using UnityEngine;

// Solo para el blockout: muestra los muros del espacio donde está Elaki, como lo hará cada escena real.
// Si Elaki no está en ningún espacio, se conservan los muros anteriores.
public class BlockoutZonas : MonoBehaviour
{
    [Serializable]
    public class Zona
    {
        public string nombre;
        public RectInt celdas;
        public GameObject muros;
    }

    [SerializeField] private Grid grid;
    [SerializeField] private Transform objetivo;
    [SerializeField] private Zona[] zonas;

    private Zona actual;

    public void Configurar(Grid nuevoGrid, Transform nuevoObjetivo, Zona[] nuevasZonas)
    {
        grid = nuevoGrid;
        objetivo = nuevoObjetivo;
        zonas = nuevasZonas;
    }

    private void Update()
    {
        var celda = grid.WorldToCell(new Vector3(objetivo.position.x, objetivo.position.y, 0f));

        foreach (var zona in zonas)
        {
            if (zona != actual && zona.celdas.Contains(new Vector2Int(celda.x, celda.y)))
            {
                Activar(zona);
                return;
            }
        }
    }

    private void Activar(Zona nueva)
    {
        actual = nueva;

        foreach (var zona in zonas)
            zona.muros.SetActive(zona == nueva);
    }
}
