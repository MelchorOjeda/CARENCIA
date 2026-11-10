using System;
using System.Collections.Generic;

// Cómo se le muestra al jugador dónde va y cuándo se guardó: lo usan el cartel del menú y la pausa.
public static class TextosPartida
{
    private static readonly Dictionary<string, string> NombresDeEscena = new Dictionary<string, string>
    {
        { "PRO_Casa", "Prólogo — La casa" },
        { "N1_Laberinto", "Nivel 1 — El laberinto" },
        { "N1_Pasillo", "Nivel 1 — El pasillo" },
        { "N2_Casa", "Nivel 2 — La casa" },
        { "R2_Casa", "Nivel 2 — El despertar" },
        { "N3_Casa", "Nivel 3 — La casa" },
    };

    // Una escena nueva que aún no está en la tabla se muestra con su nombre de archivo
    public static string NombreDe(string escena) =>
        NombresDeEscena.TryGetValue(escena, out var nombre) ? nombre : escena;

    public static string HaceCuanto(DateTime fecha)
    {
        var tiempo = DateTime.Now - fecha;
        if (tiempo.TotalMinutes < 1)
            return "hace un momento";
        if (tiempo.TotalHours < 1)
            return $"hace {(int)tiempo.TotalMinutes} min";
        if (tiempo.TotalDays < 1)
            return $"hace {(int)tiempo.TotalHours} h";

        return (int)tiempo.TotalDays == 1 ? "hace 1 día" : $"hace {(int)tiempo.TotalDays} días";
    }
}
