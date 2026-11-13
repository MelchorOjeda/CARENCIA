using System;
using System.IO;
using UnityEngine;

// Une los valores de Opciones con el juego: los lee del disco al arrancar, los guarda y los aplica.
// Mientras el jugador edita, el audio y el brillo se ven al momento.
// Resolución, modo de pantalla y calidad no se eligen en el menú: los fija la configuración del proyecto.
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
        Previsualizar(guardados);
    }

    // Vuelve a lo guardado, por ejemplo al salir de Opciones sin guardar
    public static void Descartar()
    {
        Previsualizar(Guardados);
    }

    private static Ajustes Leer()
    {
        return File.Exists(Ruta) ? Ajustes.DesdeJson(LeerTexto()) : new Ajustes();
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
}
