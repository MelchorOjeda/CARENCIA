using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;

// Cada frasco del gabinete busca su texto por su nombre: si alguien renombra un objeto, esta prueba lo avisa.
public class TextosGabineteTests
{
    private const string RutaGuion = "Assets/_Carencia/Resources/SO_Dialogo_Guion.asset";
    private const string CarpetaEscenas = "Assets/_Carencia/Scenes";

    // GUID de ObjetoGabinete.cs; cada componente apunta a su GameObject por fileID
    private const string GuidObjetoGabinete = "66724b6989f3c1849b7b81dd5609d2bb";

    private static readonly Regex Documento = new Regex(@"--- !u!\d+ &(-?\d+)[^\n]*\n(.*?)(?=\n--- !u!|\z)", RegexOptions.Singleline);
    private static readonly Regex GameObjectDelComponente = new Regex(@"m_GameObject: \{fileID: (-?\d+)\}");
    private static readonly Regex Nombre = new Regex(@"m_Name: (.+)");

    [Test]
    public void LaClaveDeUnFrascoEsSuNombreConElPrefijoDelGabinete()
    {
        Assert.AreEqual("P5_Risperidona", GuionDialogos.ClaveDeGabinete("Risperidona"));
    }

    [Test]
    public void CadaObjetoDelGabineteTieneSuLineaEnElGuion()
    {
        var guion = AssetDatabase.LoadAssetAtPath<GuionDialogos>(RutaGuion);
        var claves = new HashSet<string>(guion.lineas.Select(linea => linea.clave));

        var faltantes = NombresDeObjetosDelGabinete()
            .Select(GuionDialogos.ClaveDeGabinete)
            .Where(clave => !claves.Contains(clave))
            .ToList();

        Assert.IsEmpty(faltantes, "Objetos del gabinete sin línea en el guion: " + string.Join(", ", faltantes));
    }

    private static IEnumerable<string> NombresDeObjetosDelGabinete()
    {
        return Directory.GetFiles(CarpetaEscenas, "*.unity", SearchOption.AllDirectories)
            .Where(ruta => !ruta.Replace('\\', '/').Contains("/Pruebas/"))
            .SelectMany(ruta => NombresEnEscena(File.ReadAllText(ruta)))
            .Distinct();
    }

    private static IEnumerable<string> NombresEnEscena(string escena)
    {
        var documentos = Documento.Matches(escena).Cast<Match>()
            .ToDictionary(documento => documento.Groups[1].Value, documento => documento.Groups[2].Value);

        return documentos.Values
            .Where(documento => documento.Contains(GuidObjetoGabinete))
            .Select(componente => documentos[GameObjectDelComponente.Match(componente).Groups[1].Value])
            .Select(gameObject => Nombre.Match(gameObject).Groups[1].Value.Trim());
    }
}
