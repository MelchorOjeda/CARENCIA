using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;

// Cada objeto de una vista (gabinete, refrigerador) busca su texto con el prefijo de su vista más su nombre:
// si alguien renombra un objeto o falta su línea en el guion, esta prueba lo avisa.
public class TextosDeVistasTests
{
    private const string RutaGuion = "Assets/_Carencia/Resources/SO_Dialogo_Guion.asset";
    private static readonly string[] CarpetasConVistas = { "Assets/_Carencia/Scenes", "Assets/_Carencia/Prefabs" };

    // GUID de ObjetoDeVista.cs; cada componente apunta a su GameObject y a su vista por fileID
    private const string GuidObjetoDeVista = "66724b6989f3c1849b7b81dd5609d2bb";

    private static readonly Regex Documento = new Regex(@"--- !u!\d+ &(-?\d+)[^\n]*\n(.*?)(?=\n--- !u!|\z)", RegexOptions.Singleline);
    private static readonly Regex GameObjectDelComponente = new Regex(@"m_GameObject: \{fileID: (-?\d+)\}");
    private static readonly Regex VistaDelObjeto = new Regex(@"\bvista: \{fileID: (-?\d+)\}");
    private static readonly Regex PrefijoDeLaVista = new Regex(@"prefijoClave: (.+)");
    private static readonly Regex Nombre = new Regex(@"m_Name: (.+)");

    [Test]
    public void LaClaveDeUnObjetoEsElPrefijoDeSuVistaMasSuNombre()
    {
        Assert.AreEqual("REF_Frijoles", GuionDialogos.ClaveDeObjeto("REF_", "Frijoles"));
    }

    [Test]
    public void LosObjetosDelGabineteUsanElPrefijoDelGuionP5()
    {
        Assert.AreEqual("P5_Risperidona", GuionDialogos.ClaveDeObjeto(GuionDialogos.PrefijoGabinete, "Risperidona"));
    }

    [Test]
    public void CadaObjetoDeUnaVistaTieneSuLineaEnElGuion()
    {
        var guion = AssetDatabase.LoadAssetAtPath<GuionDialogos>(RutaGuion);
        var claves = new HashSet<string>(guion.lineas.Select(linea => linea.clave));

        var faltantes = ClavesDeTodasLasVistas().Where(clave => !claves.Contains(clave)).ToList();

        Assert.IsEmpty(faltantes, "Objetos de vista sin línea en el guion: " + string.Join(", ", faltantes));
    }

    private static IEnumerable<string> ClavesDeTodasLasVistas()
    {
        return CarpetasConVistas
            .SelectMany(carpeta => Directory.GetFiles(carpeta, "*.*", SearchOption.AllDirectories))
            .Where(ruta => ruta.EndsWith(".unity") || ruta.EndsWith(".prefab"))
            .Where(ruta => !ruta.Replace('\\', '/').Contains("/Pruebas/"))
            .SelectMany(ruta => ClavesEnArchivo(File.ReadAllText(ruta)))
            .Distinct();
    }

    private static IEnumerable<string> ClavesEnArchivo(string yaml)
    {
        var documentos = Documento.Matches(yaml).Cast<Match>()
            .ToDictionary(documento => documento.Groups[1].Value, documento => documento.Groups[2].Value);

        return documentos.Values
            .Where(documento => documento.Contains(GuidObjetoDeVista))
            .Select(componente => ClaveDelObjeto(componente, documentos));
    }

    private static string ClaveDelObjeto(string componente, IReadOnlyDictionary<string, string> documentos)
    {
        var gameObject = documentos[GameObjectDelComponente.Match(componente).Groups[1].Value];
        var nombre = Nombre.Match(gameObject).Groups[1].Value.Trim();
        return GuionDialogos.ClaveDeObjeto(PrefijoDeSuVista(componente, documentos), nombre);
    }

    // Las escenas guardadas antes de que existiera el campo no lo tienen: usan el prefijo del gabinete
    private static string PrefijoDeSuVista(string componente, IReadOnlyDictionary<string, string> documentos)
    {
        var vista = documentos[VistaDelObjeto.Match(componente).Groups[1].Value];
        var prefijo = PrefijoDeLaVista.Match(vista);
        return prefijo.Success ? prefijo.Groups[1].Value.Trim() : GuionDialogos.PrefijoGabinete;
    }
}
