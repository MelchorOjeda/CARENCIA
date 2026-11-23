using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Modo historia: qué capítulos ha alcanzado el jugador y con qué estado narrativo llegó a cada uno.
// Se guarda aparte de la partida (historia.json): empezar una historia nueva o repetir un capítulo no borra los desbloqueos.
public static class ProgresoHistoria
{
    [Serializable]
    private class Datos
    {
        public int alcanzado = -1;
        public bool terminada;
        public List<Llegada> llegadas = new List<Llegada>();
    }

    // Banderas que había al entrar al capítulo por primera vez: repetirlo arranca igual
    [Serializable]
    private class Llegada
    {
        public int capitulo;
        public List<string> banderas = new List<string>();
    }

    private static Datos datos;
    private static string escenaAnterior;
    // La escena que se carga empieza un capítulo desde cero (nueva historia o capítulo elegido), no desde un punto de control
    private static bool inicioLimpio;
    private static int capituloDeLaPartida = -1;

    private static string Ruta => Path.Combine(Application.persistentDataPath, "historia.json");

    public static int Alcanzado => Leer().alcanzado;

    // Ya vio el desenlace alguna vez (lo usa el aviso de Continuar)
    public static bool Terminada => Leer().terminada;

    public static void MarcarTerminada()
    {
        var d = Leer();
        if (d.terminada)
            return;
        d.terminada = true;
        Guardar(d);
    }

    public static bool Desbloqueado(int capitulo) => capitulo >= 0 && capitulo <= Alcanzado;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        datos = null;
        escenaAnterior = null;
        inicioLimpio = false;
        capituloDeLaPartida = -1;
        SceneManager.sceneLoaded -= AlCargarEscena;
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    // Nueva historia: borra la partida guardada, pero los capítulos alcanzados se quedan
    public static void EmpezarHistoria()
    {
        EstadoJuego.NuevaPartida();
        var d = Leer();
        if (d.terminada)
        {
            d.terminada = false;
            Guardar(d);
        }
        inicioLimpio = true;
    }

    // Prepara el estado narrativo del capítulo (sus banderas de llegada) antes de cargar su primera escena.
    // Si hay una partida guardada, el capítulo se juega como repetición y no la reemplaza.
    public static Capitulo EmpezarCapitulo(int indice)
    {
        var partida = EstadoJuego.LeerPartidaGuardada();
        capituloDeLaPartida = partida.HasValue ? Capitulos.DeEscena(partida.Value.escena) : -1;
        EstadoJuego.IniciarCapitulo(BanderasDeLlegada(indice), partida.HasValue);
        inicioLimpio = true;
        return Capitulos.Todos[indice];
    }

    private static List<string> BanderasDeLlegada(int capitulo)
    {
        foreach (var llegada in Leer().llegadas)
            if (llegada.capitulo == capitulo)
                return new List<string>(llegada.banderas);

        return new List<string>();
    }

    // Al cargar cada escena: desbloquea el capítulo, guarda con qué banderas se llegó y muestra el título
    private static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        var anterior = escenaAnterior;
        escenaAnterior = escena.name;
        var capitulo = Capitulos.DeEscena(escena.name);
        if (capitulo < 0)
            return;

        // Se llega "limpio" desde la escena anterior de la historia o desde Nueva historia / Capítulos.
        // Continuar desde el menú trae banderas de mitad de capítulo: desbloquea, pero no se guarda como llegada
        var desdeLaHistoria = Capitulos.DeEscena(anterior) >= 0 && anterior != escena.name;
        var limpio = inicioLimpio || desdeLaHistoria;
        inicioLimpio = false;
        Alcanzar(capitulo, Capitulos.EsInicio(escena.name) && limpio);

        // Si jugando una repetición el jugador avanza más allá de su partida, ese avance ya es su partida.
        // Elegir directo un capítulo posterior en el menú no cuenta: eso sigue siendo repetir
        if (EstadoJuego.Repeticion && desdeLaHistoria && Capitulos.SuperaLaPartida(capitulo, capituloDeLaPartida))
            EstadoJuego.TerminarRepeticion();

        if (Capitulos.MostrarTitulo(anterior, escena.name))
            TituloCapitulo.Mostrar(Capitulos.Todos[capitulo], escena.name);
    }

    private static void Alcanzar(int capitulo, bool guardarLlegada)
    {
        var d = Leer();
        var cambio = false;
        if (capitulo > d.alcanzado)
        {
            d.alcanzado = capitulo;
            cambio = true;
        }

        if (guardarLlegada && !d.llegadas.Exists(l => l.capitulo == capitulo))
        {
            d.llegadas.Add(new Llegada { capitulo = capitulo, banderas = EstadoJuego.CopiaBanderas() });
            cambio = true;
        }

        if (cambio)
            Guardar(d);
    }

    private static Datos Leer()
    {
        if (datos != null)
            return datos;

        datos = new Datos();
        try
        {
            if (File.Exists(Ruta))
                datos = JsonUtility.FromJson<Datos>(File.ReadAllText(Ruta)) ?? new Datos();
        }
        catch (Exception error) when (error is ArgumentException || error is IOException)
        {
            datos = new Datos();
        }

        datos.llegadas ??= new List<Llegada>();

        // Partidas de antes del Modo historia: lo que ya se jugó cuenta como alcanzado
        var partida = EstadoJuego.LeerPartidaGuardada();
        if (partida.HasValue)
            datos.alcanzado = Mathf.Max(datos.alcanzado, Capitulos.DeEscena(partida.Value.escena));

        return datos;
    }

    private static void Guardar(Datos d)
    {
        try
        {
            File.WriteAllText(Ruta, JsonUtility.ToJson(d));
        }
        catch (IOException error)
        {
            Debug.LogWarning("No se pudo guardar el progreso de capítulos: " + error.Message);
        }
    }
}
