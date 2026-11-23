using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Al terminar un build WebGL:
//  - copia los mp4 de _Carencia/Videos a <build>/StreamingAssets/Videos, porque en el
//    navegador los videos solo se pueden reproducir por URL (ver VideoWeb.cs);
//  - activa en index.html el guardado automático de Application.persistentDataPath,
//    para que la partida y los ajustes sigan ahí al volver a abrir la página.
// No toca la carpeta Assets ni afecta a los builds de Windows.
public class VideosParaWeb : IPostprocessBuildWithReport
{
    private const string Origen = "Assets/_Carencia/Videos";

    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
            return;

        var salida = report.summary.outputPath;
        var destino = Path.Combine(salida, "StreamingAssets", VideoWeb.Carpeta);
        Directory.CreateDirectory(destino);
        foreach (var mp4 in Directory.GetFiles(Origen, "*.mp4"))
            File.Copy(mp4, Path.Combine(destino, Path.GetFileName(mp4)), true);

        var index = Path.Combine(salida, "index.html");
        if (File.Exists(index))
        {
            var html = File.ReadAllText(index);
            html = html.Replace("// config.autoSyncPersistentDataPath = true;", "config.autoSyncPersistentDataPath = true;");
            File.WriteAllText(index, html);
        }

        Debug.Log($"[VideosParaWeb] Videos copiados a {destino}");
    }
}
