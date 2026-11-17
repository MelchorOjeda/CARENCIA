using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// Recalcula en el editor las celdas que usan TileVariado en las escenas abiertas.
// Marca la escena como modificada: guárdala solo si es tuya (ver CONTRIBUTING, sección 5).
public static class RefrescarTilesVariados
{
    [MenuItem("Carencia/Refrescar tiles variados")]
    private static void Refrescar()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene escena = SceneManager.GetSceneAt(i);
            TileVariado.Refrescar(escena);
            EditorSceneManager.MarkSceneDirty(escena);
        }
    }
}
