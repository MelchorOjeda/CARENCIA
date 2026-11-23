using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PortalesGame;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// Antes de un build WebGL anota en Resources/CatalogoVideosWeb.asset cada video de las escenas del
// build (VideoPlayer, FleshWall, VideoEnPantalla): en web los VideoClip llegan vacíos y VideoWeb
// necesita saber qué mp4 reproducir y cuánto dura.
// Con la plataforma WebGL activa los VideoClip ni siquiera cargan en el editor (su importación web se
// reduce a nada para no inflar la descarga), así que el video de cada componente se lee del archivo de
// la escena y su duración y tamaño, del importador del mp4. Abre las escenas solo para leerlas.
public class CatalogoVideosWebBuild : IPreprocessBuildWithReport
{
    private const string Ruta = "Assets/Resources/" + CatalogoVideosWeb.Recurso + ".asset";
    private static readonly Regex Referencia = new Regex(@"^\s*(?:m_VideoClip|clip): \{fileID: \d+, guid: ([0-9a-f]{32})", RegexOptions.Multiline);

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.WebGL)
            Generar();
    }

    [MenuItem("CARENCIA/Generar catálogo de videos web")]
    public static void Generar()
    {
        var catalogo = AssetDatabase.LoadAssetAtPath<CatalogoVideosWeb>(Ruta);
        if (catalogo == null)
        {
            catalogo = ScriptableObject.CreateInstance<CatalogoVideosWeb>();
            AssetDatabase.CreateAsset(catalogo, Ruta);
        }
        catalogo.entradas.Clear();

        foreach (var ajuste in EditorBuildSettings.scenes.Where(s => s.enabled))
        {
            var texto = File.ReadAllText(ajuste.path);
            var escena = SceneManager.GetSceneByPath(ajuste.path);
            var abierta = escena.IsValid() && escena.isLoaded;
            if (!abierta)
                escena = EditorSceneManager.OpenScene(ajuste.path, OpenSceneMode.Additive);

            foreach (var raiz in escena.GetRootGameObjects())
            {
                foreach (var video in raiz.GetComponentsInChildren<VideoPlayer>(true))
                    Anotar(catalogo, ajuste.path, texto, video);
                foreach (var pared in raiz.GetComponentsInChildren<FleshWall>(true))
                    Anotar(catalogo, ajuste.path, texto, pared);
                foreach (var enPantalla in raiz.GetComponentsInChildren<VideoEnPantalla>(true))
                    Anotar(catalogo, ajuste.path, texto, enPantalla);
            }

            if (!abierta)
                EditorSceneManager.CloseScene(escena, true);
        }

        EditorUtility.SetDirty(catalogo);
        AssetDatabase.SaveAssets();
        Debug.Log($"[CatalogoVideosWeb] {catalogo.entradas.Count} videos anotados");
    }

    private static void Anotar(CatalogoVideosWeb catalogo, string escena, string texto, Component componente)
    {
        var mp4 = VideoDe(texto, componente);
        if (mp4 == null)
            return;

        var ruta = CatalogoVideosWeb.RutaDe(componente.transform);
        if (catalogo.entradas.Any(e => e.escena == escena && e.ruta == ruta))
            return;

        var importador = (VideoClipImporter)AssetImporter.GetAtPath(mp4);
        catalogo.entradas.Add(new CatalogoVideosWeb.Entrada
        {
            escena = escena,
            ruta = ruta,
            nombre = Path.GetFileNameWithoutExtension(mp4),
            duracion = importador.frameRate > 0 ? (float)(importador.frameCount / importador.frameRate) : 0f,
            ancho = importador.GetResizeWidth(VideoResizeMode.OriginalSize),
            alto = importador.GetResizeHeight(VideoResizeMode.OriginalSize),
        });
    }

    // Ruta del mp4 que el componente tiene asignado en el archivo de la escena, o null si no tiene.
    private static string VideoDe(string texto, Component componente)
    {
        var id = GlobalObjectId.GetGlobalObjectIdSlow(componente).targetObjectId;
        var inicio = texto.IndexOf(" &" + id + "\n", System.StringComparison.Ordinal);
        if (inicio < 0)
            inicio = texto.IndexOf(" &" + id + "\r\n", System.StringComparison.Ordinal);
        if (inicio < 0)
            return null;

        var fin = texto.IndexOf("\n--- ", inicio, System.StringComparison.Ordinal);
        var bloque = fin < 0 ? texto.Substring(inicio) : texto.Substring(inicio, fin - inicio);
        var m = Referencia.Match(bloque);
        if (!m.Success)
            return null;

        var ruta = AssetDatabase.GUIDToAssetPath(m.Groups[1].Value);
        return ruta.EndsWith(".mp4", System.StringComparison.OrdinalIgnoreCase) ? ruta : null;
    }
}
