using System;
using UnityEngine;
using UnityEngine.Tilemaps;

// Tapa con un techo todas las zonas del laberinto menos la del jugador.
// Así los cuartos vecinos se leen como bloques cerrados y no se ve lo que hay dentro.
public class VisibilidadZonas : MonoBehaviour
{
    [Serializable]
    public class Zona
    {
        public string nombre;
        public RectInt[] areas;
        // Techo y fachada que cierran la zona cuando el jugador no está en ella
        public Tilemap[] techo = new Tilemap[0];
    }

    [SerializeField] private Zona[] zonas = new Zona[0];
    [SerializeField] private float duracionFundido = 0.3f;

    private Grid grid;
    private Transform jugador;
    private float[] alfa;
    private int actual = -1;

    public void Configurar(Zona[] nuevas)
    {
        zonas = nuevas;
    }

    public bool TieneTecho(string nombre)
    {
        var i = Array.FindIndex(zonas, z => z.nombre == nombre);
        return i >= 0 && alfa != null && alfa[i] > 0.01f;
    }

    private void Start()
    {
        grid = GetComponentInParent<Grid>();
        var controlador = FindAnyObjectByType<PlayerController>();
        jugador = controlador != null ? controlador.transform : null;
        alfa = new float[zonas.Length];
        Actualizar(true);
    }

    private void Update()
    {
        Actualizar(false);
    }

    private void Actualizar(bool inmediato)
    {
        if (jugador == null || grid == null)
            return;

        // Entre dos zonas se conserva la última, para que el techo no parpadee en las puertas
        var zona = Buscar((Vector2Int)grid.WorldToCell(jugador.position));
        if (zona >= 0)
            actual = zona;

        var paso = inmediato || duracionFundido <= 0f ? 1f : Time.deltaTime / duracionFundido;
        for (var i = 0; i < zonas.Length; i++)
        {
            var meta = i == actual ? 0f : 1f;
            if (Mathf.Approximately(alfa[i], meta) && !inmediato)
                continue;

            alfa[i] = Mathf.MoveTowards(alfa[i], meta, paso);
            foreach (var capa in zonas[i].techo)
            {
                var color = capa.color;
                color.a = alfa[i];
                capa.color = color;
                capa.GetComponent<TilemapRenderer>().enabled = alfa[i] > 0.01f;
            }
        }
    }

    private int Buscar(Vector2Int celda)
    {
        for (var i = 0; i < zonas.Length; i++)
        {
            foreach (var area in zonas[i].areas)
            {
                if (area.Contains(celda))
                    return i;
            }
        }

        return -1;
    }
}
