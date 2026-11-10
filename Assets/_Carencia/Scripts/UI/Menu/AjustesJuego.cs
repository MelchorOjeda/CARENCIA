using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Une los valores de Opciones con el juego: los lee del disco al arrancar, los guarda y los aplica.
// Mientras el jugador edita, audio, brillo y accesibilidad se ven al momento; el video solo cambia al guardar,
// porque cambiar de resolución en cada flecha haría parpadear la pantalla.
public static class AjustesJuego
{
    private static Ajustes guardados;
    private static Ajustes enPantalla;

    public static event Action<Ajustes> Cambiaron;

    // Lo que está guardado en disco
    public static Ajustes Guardados => guardados ??= Leer();

    // Lo que se está viendo ahora: igual a Guardados, salvo mientras el jugador prueba cambios en Opciones
    public static Ajustes EnPantalla => enPantalla ??= Guardados.Copia();

    private static string Ruta => Path.Combine(Application.persistentDataPath, "ajustes.json");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        guardados = null;
        enPantalla = null;
        Cambiaron = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AplicarAlArrancar()
    {
        AplicarVideo(Guardados);
        Previsualizar(Guardados);
    }

    public static void Previsualizar(Ajustes ajustes)
    {
        enPantalla = ajustes.Copia();
        AudioListener.volume = ajustes.volumenGeneral / 100f;
        CapaBrillo.Instancia.Aplicar(ajustes.brillo);
        Cambiaron?.Invoke(enPantalla);
    }

    public static void Guardar(Ajustes ajustes)
    {
        guardados = ajustes.Copia();
        File.WriteAllText(Ruta, guardados.AJson());
        AplicarVideo(guardados);
        Previsualizar(guardados);
    }

    // Vuelve a lo guardado, por ejemplo al salir de Opciones sin guardar
    public static void Descartar()
    {
        Previsualizar(Guardados);
    }

    // Convierte "la del monitor" y "la del proyecto" en valores concretos para poder mostrarlos y recorrerlos
    public static void CompletarVideo(Ajustes ajustes)
    {
        if (ajustes.anchoPantalla <= 0 || ajustes.altoPantalla <= 0)
        {
            var monitor = Screen.currentResolution;
            ajustes.anchoPantalla = monitor.width;
            ajustes.altoPantalla = monitor.height;
        }

        if (ajustes.calidad < 0 || ajustes.calidad >= QualitySettings.count)
            ajustes.calidad = QualitySettings.GetQualityLevel();
    }

    // El monitor repite cada tamaño con varias frecuencias; para el jugador cuenta solo el tamaño
    public static IReadOnlyList<Resolucion> ResolucionesDisponibles()
    {
        return Screen.resolutions
            .Select(modo => new Resolucion(modo.width, modo.height))
            .Distinct()
            .OrderBy(resolucion => resolucion.ancho)
            .ThenBy(resolucion => resolucion.alto)
            .ToList();
    }

    private static Ajustes Leer()
    {
        var ajustes = File.Exists(Ruta) ? Ajustes.DesdeJson(LeerTexto()) : new Ajustes();
        CompletarVideo(ajustes);
        return ajustes;
    }

    private static string LeerTexto()
    {
        try
        {
            return File.ReadAllText(Ruta);
        }
        catch (IOException)
        {
            return null;
        }
    }

    private static void AplicarVideo(Ajustes ajustes)
    {
        QualitySettings.SetQualityLevel(ajustes.calidad, true);

        // En el Editor la ventana Game no cambia de resolución; solo aplica en el juego compilado
        if (Application.isEditor)
            return;

        var modo = ajustes.pantallaCompleta ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(ajustes.anchoPantalla, ajustes.altoPantalla, modo);
    }
}
