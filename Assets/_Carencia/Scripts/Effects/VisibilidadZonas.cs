using System;
using UnityEngine;
using UnityEngine.Tilemaps;

// Tapa con un techo todas las zonas del laberinto menos la del jugador.
// Así los cuartos vecinos se leen como bloques cerrados y no se ve lo que hay dentro.
// Los techos nunca se transparentan: si un bloque tapa al jugador, se dibuja solo su silueta encima.
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
        // Paredes altas de la zona; se usan para saber qué bloques pueden tapar al jugador
        public Tilemap[] paredes = new Tilemap[0];
    }

    // Alto del bloque en celdas de profundidad (pared de 2.69 unidades sobre celdas de 0.25) y medio ancho del jugador
    private const float AltoBloque = 11.8f;
    private const float AnchoJugador = 1.8f;

    [SerializeField] private Zona[] zonas = new Zona[0];
    [SerializeField] private float duracionFundido = 0.3f;
    [SerializeField] private Color colorSilueta = new Color(0.12f, 0.12f, 0.18f, 0.55f);

    private Grid grid;
    private Transform jugador;
    private float[] alfaTecho;
    private bool[] tapando;
    private int actual = -1;
    private SpriteRenderer figura;
    private SpriteRenderer silueta;

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
        return i >= 0 && tapando != null && tapando[i];
    }

    public bool SiluetaVisible => silueta != null && silueta.enabled;

    private void Start()
    {
        grid = GetComponentInParent<Grid>();
        var controlador = FindAnyObjectByType<PlayerController>();
        jugador = controlador != null ? controlador.transform : null;
        alfaTecho = new float[zonas.Length];
        tapando = new bool[zonas.Length];
        CrearSilueta();
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
        var alguno = false;
        for (var i = 0; i < zonas.Length; i++)
        {
            tapando[i] = i != actual && Tapa(zonas[i], posicion);
            alguno |= tapando[i];
            var metaTecho = i == actual ? 0f : 1f;
            if (!inmediato && Mathf.Approximately(alfaTecho[i], metaTecho))
                continue;

            alfaTecho[i] = Mathf.MoveTowards(alfaTecho[i], metaTecho, paso);
            foreach (var capa in zonas[i].techo)
                Fundir(capa, alfaTecho[i]);
            foreach (var capa in zonas[i].murosBajos)
                Fundir(capa, i == actual ? 1f - alfaTecho[i] : 0f);
        }

        if (silueta != null)
            silueta.enabled = alguno;
    }

    // Copia oscura y translúcida de Elaki, dibujada por encima de los techos.
    // Va fuera de Elaki: si fuera su hija, se ordenaría con ella y quedaría debajo del techo.
    private void CrearSilueta()
    {
        if (jugador == null)
            return;

        // El dibujo de Elaki es el más grande de sus SpriteRenderer
        foreach (var sr in jugador.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (sr.sprite != null && (figura == null || sr.sprite.rect.height > figura.sprite.rect.height))
                figura = sr;
        }

        if (figura == null)
            return;

        var objeto = new GameObject("SiluetaJugador");
        objeto.transform.SetParent(transform, false);
        silueta = objeto.AddComponent<SpriteRenderer>();
        silueta.sharedMaterial = figura.sharedMaterial;
        silueta.color = colorSilueta;
        silueta.sortingLayerName = "Mundo";
        silueta.sortingOrder = 200;
        silueta.enabled = false;
    }

    private void LateUpdate()
    {
        if (silueta == null || !silueta.enabled)
            return;

        silueta.sprite = figura.sprite;
        silueta.flipX = figura.flipX;
        silueta.transform.SetPositionAndRotation(figura.transform.position, figura.transform.rotation);
        silueta.transform.localScale = figura.transform.lossyScale;
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
