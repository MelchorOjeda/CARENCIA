using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Progreso de la partida: banderas narrativas, daño y punto de control.
// Al morir se restaura lo que había en el último punto de control.
public static class EstadoJuego
{
    [Serializable]
    private class Datos
    {
        public string escena;
        public string entrada;
        public List<string> banderas = new List<string>();
    }

    private static readonly HashSet<string> Banderas = new HashSet<string>();
    private static List<string> banderasDelControl = new List<string>();

    public static string EscenaControl { get; private set; }
    public static string EntradaControl { get; private set; }
    public static int Golpes { get; set; }

    private static string Ruta => Path.Combine(Application.persistentDataPath, "partida.json");

    public static bool HayPartida => File.Exists(Ruta);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        Banderas.Clear();
        banderasDelControl = new List<string>();
        EscenaControl = null;
        EntradaControl = null;
        Golpes = 0;
    }

    public static void NuevaPartida()
    {
        Reiniciar();
        if (File.Exists(Ruta))
            File.Delete(Ruta);
    }

    public static bool Tiene(string bandera) => string.IsNullOrEmpty(bandera) || Banderas.Contains(bandera);

    public static void Poner(string bandera)
    {
        if (!string.IsNullOrEmpty(bandera))
            Banderas.Add(bandera);
    }

    public static void GuardarPunto(string escena, string entrada)
    {
        EscenaControl = escena;
        EntradaControl = entrada;
        banderasDelControl = new List<string>(Banderas);

        var datos = new Datos { escena = escena, entrada = entrada, banderas = banderasDelControl };
        File.WriteAllText(Ruta, JsonUtility.ToJson(datos));
    }

    public static void RestaurarPunto()
    {
        Banderas.Clear();
        foreach (var bandera in banderasDelControl)
            Banderas.Add(bandera);

        Golpes = 0;
    }

    public static bool CargarPartida()
    {
        if (!HayPartida)
            return false;

        var datos = JsonUtility.FromJson<Datos>(File.ReadAllText(Ruta));
        if (datos == null || string.IsNullOrEmpty(datos.escena))
            return false;

        EscenaControl = datos.escena;
        EntradaControl = datos.entrada;
        banderasDelControl = datos.banderas ?? new List<string>();
        RestaurarPunto();
        return true;
    }
}
