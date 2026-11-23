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

    // Repitiendo un capítulo con una partida guardada: los puntos de control se quedan en memoria y no la reemplazan
    public static bool Repeticion { get; private set; }

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
        Repeticion = false;
    }

    public static void NuevaPartida()
    {
        Reiniciar();
        if (File.Exists(Ruta))
            File.Delete(Ruta);
    }

    // La historia terminó: la partida guardada ya no tiene a dónde continuar
    public static void TerminarPartida()
    {
        if (File.Exists(Ruta))
            File.Delete(Ruta);
        EscenaControl = null;
        EntradaControl = null;
    }

    // Empieza un capítulo elegido en el menú con las banderas que tenía el jugador al llegar a él
    public static void IniciarCapitulo(IEnumerable<string> banderas, bool repeticion)
    {
        Reiniciar();
        foreach (var bandera in banderas)
            Poner(bandera);
        banderasDelControl = new List<string>(Banderas);
        Repeticion = repeticion;
    }

    // Al llegar más lejos que la partida guardada, lo que sigue ya cuenta como la partida
    public static void TerminarRepeticion() => Repeticion = false;

    public static List<string> CopiaBanderas() => new List<string>(Banderas);

    public static bool Tiene(string bandera) => string.IsNullOrEmpty(bandera) || Banderas.Contains(bandera);

    public static void Poner(string bandera)
    {
        if (!string.IsNullOrEmpty(bandera))
            Banderas.Add(bandera);
    }

    public static void Quitar(string bandera)
    {
        if (!string.IsNullOrEmpty(bandera))
            Banderas.Remove(bandera);
    }

    public static void GuardarPunto(string escena, string entrada)
    {
        EscenaControl = escena;
        EntradaControl = entrada;
        banderasDelControl = new List<string>(Banderas);
        if (Repeticion)
            return;

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
        var datos = LeerDatos();
        if (datos == null)
            return false;

        Repeticion = false;
        EscenaControl = datos.escena;
        EntradaControl = datos.entrada;
        banderasDelControl = datos.banderas ?? new List<string>();
        RestaurarPunto();
        return true;
    }

    // Lo que el menú muestra de la partida guardada; null si no hay o si el archivo está dañado.
    public static (string escena, DateTime fecha)? LeerPartidaGuardada()
    {
        var datos = LeerDatos();
        if (datos == null)
            return null;

        return (datos.escena, File.GetLastWriteTime(Ruta));
    }

    // Un archivo dañado o a medio escribir cuenta como si no hubiera partida, en vez de romper Continuar.
    private static Datos LeerDatos()
    {
        if (!HayPartida)
            return null;

        try
        {
            var datos = JsonUtility.FromJson<Datos>(File.ReadAllText(Ruta));
            return datos == null || string.IsNullOrEmpty(datos.escena) ? null : datos;
        }
        catch (Exception error) when (error is ArgumentException || error is IOException)
        {
            return null;
        }
    }
}
