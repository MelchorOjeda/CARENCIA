using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// Tile que alterna entre su sprite limpio y variantes desgastadas.
// La elección depende solo de la posición de la celda: es aleatoria pero siempre igual.
[CreateAssetMenu(menuName = "Carencia/Tile variado", fileName = "TILE_Variado")]
public class TileVariado : Tile
{
    [Tooltip("Variantes desgastadas. El sprite principal (arriba) es la versión limpia.")]
    public Sprite[] variantes;

    [Tooltip("Peso del sprite limpio frente a cada variante.")]
    public float pesoLimpio = 3f;

    [Tooltip("Un peso por variante, en el mismo orden.")]
    public float[] pesosVariantes = { 1f, 1f };

    public override void GetTileData(Vector3Int posicion, ITilemap tilemap, ref TileData datos)
    {
        base.GetTileData(posicion, tilemap, ref datos);
        datos.sprite = Elegir(posicion);
    }

    private Sprite Elegir(Vector3Int p)
    {
        if (variantes == null || variantes.Length == 0)
            return sprite;

        float total = pesoLimpio;
        for (int i = 0; i < variantes.Length; i++)
            total += Peso(i);

        float r = Hash(p) * total;
        if (r < pesoLimpio)
            return sprite;
        r -= pesoLimpio;
        for (int i = 0; i < variantes.Length; i++)
        {
            r -= Peso(i);
            if (r < 0f && variantes[i] != null)
                return variantes[i];
        }
        return sprite;
    }

    private float Peso(int i) =>
        pesosVariantes != null && i < pesosVariantes.Length ? Mathf.Max(0f, pesosVariantes[i]) : 1f;

    // Número estable en [0,1) a partir de la celda.
    private static float Hash(Vector3Int p)
    {
        unchecked
        {
            uint h = (uint)p.x * 73856093u ^ (uint)p.y * 19349663u ^ (uint)p.z * 83492791u;
            h ^= h >> 13;
            h *= 0x5bd1e995u;
            h ^= h >> 15;
            return (h & 0xFFFFFF) / 16777216f;
        }
    }

    // Las escenas guardan el sprite de cada celda; al cargar se recalculan las que usan este tile.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Registrar()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
        SceneManager.sceneLoaded += AlCargarEscena;
        for (int i = 0; i < SceneManager.sceneCount; i++)
            Refrescar(SceneManager.GetSceneAt(i));
    }

    private static void AlCargarEscena(Scene escena, LoadSceneMode modo) => Refrescar(escena);

    public static void Refrescar(Scene escena)
    {
        if (!escena.isLoaded)
            return;
        foreach (GameObject raiz in escena.GetRootGameObjects())
            foreach (Tilemap mapa in raiz.GetComponentsInChildren<Tilemap>(true))
                if (Usa(mapa))
                    mapa.RefreshAllTiles();
    }

    private static bool Usa(Tilemap mapa)
    {
        var usados = new TileBase[mapa.GetUsedTilesCount()];
        mapa.GetUsedTilesNonAlloc(usados);
        foreach (TileBase t in usados)
            if (t is TileVariado)
                return true;
        return false;
    }
}
