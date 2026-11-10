using System;
using UnityEngine;
using UnityEngine.Tilemaps;

// Tapa con un techo todas las zonas del laberinto menos la del jugador.
// Así los cuartos vecinos se leen como bloques cerrados y no se ve lo que hay dentro.
// Si un bloque queda delante del jugador y lo tapa, se vuelve semitransparente para no perderlo de vista.
public class VisibilidadZonas : MonoBehaviour
{
    [Serializable]
    public class Zona
    {
        public string nombre;
        public RectInt[] areas;
        // Techo y fachada que cierran la zona cuando el jugador no está en ella
        public Tilemap[] techo = new Tilemap[0];
        // Muros bajos de enfrente: se ocultan mientras la zona tiene techo, porque la fachada los reemplaza
        public Tilemap[] murosBajos = new Tilemap[0];
        // Paredes altas: se transparentan junto con el techo cuando tapan al jugador
        public Tilemap[] paredes = new Tilemap[0];
    }

    // Alto del bloque en celdas de profundidad (pared de 2.69 unidades sobre celdas de 0.25) y medio ancho del jugador
    private const float AltoBloque = 11.8f;
    private const float AnchoJugador = 1.8f;

    [SerializeField] private Zona[] zonas = new Zona[0];
    [SerializeField] private float duracionFundido = 0.3f;
    [SerializeField, Range(0f, 1f)] private float alfaTapando = 0.35f;

    private Grid grid;
    private Transform jugador;
    private float[] alfaTecho;
    private float[] alfaParedes;
    private int actual = -1;

    public void Configurar(Zona[] nuevas)
    {
        zonas = nuevas;
    }

    public bool TieneTecho(string nombre)
    {
        var i = Array.FindIndex(zonas, z => z.nombre == nombre);
        return i >= 0 && alfaTecho != null && alfaTecho[i] > 0.01f;
    }

    public bool EstaTapando(string nombre)
    {
        var i = Array.FindIndex(zonas, z => z.nombre == nombre);
        return i >= 0 && alfaParedes != null && alfaParedes[i] < 0.99f;
    }

    private void Start()
    {
        grid = GetComponentInParent<Grid>();
        var controlador = FindAnyObjectByType<PlayerController>();
        jugador = controlador != null ? controlador.transform : null;
        alfaTecho = new float[zonas.Length];
        alfaParedes = new float[zonas.Length];
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
        var posicion = grid.LocalToCellInterpolated(grid.WorldToLocal(jugador.position));
        var zona = Buscar((Vector2Int)grid.WorldToCell(jugador.position));
        if (zona >= 0)
            actual = zona;

        var paso = inmediato || duracionFundido <= 0f ? 1f : Time.deltaTime / duracionFundido;
        for (var i = 0; i < zonas.Length; i++)
        {
            var tapa = i != actual && Tapa(zonas[i], posicion);
            var metaParedes = tapa ? alfaTapando : 1f;
            var metaTecho = i == actual ? 0f : metaParedes;
            if (!inmediato && Mathf.Approximately(alfaTecho[i], metaTecho) && Mathf.Approximately(alfaParedes[i], metaParedes))
                continue;

            alfaTecho[i] = Mathf.MoveTowards(alfaTecho[i], metaTecho, paso);
            alfaParedes[i] = Mathf.MoveTowards(alfaParedes[i], metaParedes, paso);
            foreach (var capa in zonas[i].techo)
                Fundir(capa, alfaTecho[i]);
            foreach (var capa in zonas[i].paredes)
                Fundir(capa, alfaParedes[i]);
            foreach (var capa in zonas[i].murosBajos)
                Fundir(capa, i == actual ? 1f - alfaTecho[i] : 0f);
        }
    }

    // Una celda de la zona tapa al jugador si está delante de él y su bloque alcanza a cubrir su figura en pantalla
    private static bool Tapa(Zona zona, Vector3 jugador)
    {
        var suma = jugador.x + jugador.y;
        var resta = jugador.x - jugador.y;
        foreach (var area in zona.areas)
        {
            foreach (var c in area.allPositionsWithin)
            {
                // Se mide desde el centro de la celda
                var delante = suma - (c.x + c.y + 1f);
                var lado = resta - (c.x - c.y);
                if (delante > 0f && delante < AltoBloque && Mathf.Abs(lado) < AnchoJugador)
                    return true;
            }
        }

        return false;
    }

    private static void Fundir(Tilemap capa, float valor)
    {
        var color = capa.color;
        color.a = valor;
        capa.color = color;
        capa.GetComponent<TilemapRenderer>().enabled = valor > 0.01f;
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
