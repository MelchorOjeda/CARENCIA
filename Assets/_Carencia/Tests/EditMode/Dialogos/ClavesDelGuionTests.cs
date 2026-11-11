using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;

// Revisa el guion contra las escenas reales: ninguna línea del juego debe verse como [clave].
public class ClavesDelGuionTests
{
    private const string RutaGuion = "Assets/_Carencia/Resources/SO_Dialogo_Guion.asset";
    private const string CarpetaEscenas = "Assets/_Carencia/Scenes";

    // Con Tamaño de texto Grande caben tres renglones de unas 50 letras
    private const int LetrasPorPaginaMaximas = 150;

    // Secuencia.Tipo.Dialogo se guarda en la escena como el número 20
    private static readonly Regex PasoDialogo =
        new Regex(@"- tipo: 20\r?\n\s+valor: .*\r?\n\s+valor2: .*\r?\n\s+texto: (\S+)");

    private static GuionDialogos Guion => AssetDatabase.LoadAssetAtPath<GuionDialogos>(RutaGuion);

    [Test]
    public void ElGuionExisteEnResources()
    {
        Assert.IsNotNull(Guion, $"Falta {RutaGuion}");
    }

    [Test]
    public void CadaClaveDeLasEscenasTieneSuLinea()
    {
        var claves = new HashSet<string>(Guion.lineas.Select(linea => linea.clave));

        var faltantes = ClavesDeLasEscenas().Where(clave => !claves.Contains(clave)).ToList();

        Assert.IsEmpty(faltantes, "Claves sin línea en el guion: " + string.Join(", ", faltantes));
    }

    [Test]
    public void NingunaClaveSeRepite()
    {
        var repetidas = Guion.lineas
            .GroupBy(linea => linea.clave)
            .Where(grupo => grupo.Count() > 1)
            .Select(grupo => grupo.Key)
            .ToList();

        Assert.IsEmpty(repetidas, "Claves repetidas: " + string.Join(", ", repetidas));
    }

    [Test]
    public void CadaPaginaCabeEnLaCaja()
    {
        var largas = Guion.lineas
            .Where(linea => linea.Paginas().Any(pagina => pagina.Length > LetrasPorPaginaMaximas))
            .Select(linea => linea.clave)
            .ToList();

        Assert.IsEmpty(largas, "Separa en páginas con una línea en blanco: " + string.Join(", ", largas));
    }

    private static IEnumerable<string> ClavesDeLasEscenas()
    {
        return Directory.GetFiles(CarpetaEscenas, "*.unity", SearchOption.AllDirectories)
            .Where(ruta => !ruta.Replace('\\', '/').Contains("/Pruebas/"))
            .SelectMany(ruta => PasoDialogo.Matches(File.ReadAllText(ruta)).Cast<Match>())
            .Select(coincidencia => coincidencia.Groups[1].Value)
            .Distinct();
    }
}
