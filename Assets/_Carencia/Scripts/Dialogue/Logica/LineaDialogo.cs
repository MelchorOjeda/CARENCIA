using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Una línea del guion: quién la dice y qué dice. La clave es la misma que usa el paso Dialogo de las secuencias.
[Serializable]
public class LineaDialogo
{
    private static readonly string[] SeparadorDePaginas = { "\n\n" };

    [Tooltip("La misma clave que el paso Dialogo de la secuencia, por ejemplo P2_elaki_1")]
    public string clave;

    [Tooltip("Nombre que se muestra arriba del texto. Vacío para textos sin voz, como el periódico")]
    public string quien;

    [Tooltip("Una línea en blanco separa páginas. Vacío si el guion dice \"Sin texto\": la secuencia se la salta")]
    [TextArea(2, 6)]
    public string texto;

    [Tooltip("Lo que Elaki piensa y no dice: se muestra en cursiva")]
    public bool pensamiento;

    public bool TieneTexto => Paginas().Any(pagina => pagina.Length > 0);

    // Siempre hay al menos una página, aunque esté vacía, para que la caja pueda mostrarse
    public IReadOnlyList<string> Paginas()
    {
        var paginas = (texto ?? "")
            .Replace("\r\n", "\n")
            .Split(SeparadorDePaginas, StringSplitOptions.None)
            .Select(pagina => pagina.Trim())
            .Where(pagina => pagina.Length > 0)
            .ToList();

        return paginas.Count > 0 ? paginas : new List<string> { "" };
    }
}
