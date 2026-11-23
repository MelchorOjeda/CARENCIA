// Capítulos del Modo historia: qué escenas forman cada uno y por dónde empieza.
// Sigue el flujo real de las escenas; si cambia el orden de la historia, se cambia aquí.
public sealed class Capitulo
{
    public readonly string Numero;
    public readonly string Titulo;
    // Punto de entrada al cargar la primera escena (el mismo que usa la escena anterior); null: donde empieza la escena
    public readonly string Entrada;
    public readonly string[] Escenas;
    // Capítulo que es solo cinemática (el jugador nunca toma el control): el título sale en cuanto empieza
    public readonly bool SoloCinematica;

    public Capitulo(string numero, string titulo, string entrada, params string[] escenas)
        : this(numero, titulo, entrada, false, escenas)
    {
    }

    public Capitulo(string numero, string titulo, string entrada, bool soloCinematica, params string[] escenas)
    {
        SoloCinematica = soloCinematica;
        Numero = numero;
        Titulo = titulo;
        Entrada = entrada;
        Escenas = escenas;
    }

    public string Nombre => Numero + " · " + Titulo;

    public string PrimeraEscena => Escenas[0];

    // Lo que se ve de un capítulo bloqueado: solo su número, sin título ni detalles
    public string NombreBloqueado => Numero + " · ???";
}

public static class Capitulos
{
    public static readonly Capitulo[] Todos =
    {
        new Capitulo("Prólogo", "La negación", null, "PRO_Casa"),
        new Capitulo("Nivel 1", "La primera ruptura", "inicio", "N1_Laberinto", "N1_Pasillo", "R1_Casa"),
        new Capitulo("Nivel 2", "La deformación", "inicio", "N2_Casa", "R2_Casa"),
        new Capitulo("Nivel 3", "La realidad se rompe", "inicio", "N3_Casa"),
        new Capitulo("Desenlace", "La revelación", "bano", true, "DES_Final"),
    };

    // Índice del capítulo al que pertenece la escena; -1 si no es de la historia (el menú, las pruebas)
    public static int DeEscena(string escena)
    {
        if (string.IsNullOrEmpty(escena))
            return -1;

        for (var i = 0; i < Todos.Length; i++)
            foreach (var nombre in Todos[i].Escenas)
                if (nombre == escena)
                    return i;

        return -1;
    }

    public static bool EsInicio(string escena)
    {
        var capitulo = DeEscena(escena);
        return capitulo >= 0 && Todos[capitulo].PrimeraEscena == escena;
    }

    // El título se muestra al entrar a un capítulo: al llegar a su primera escena desde otra, o al venir del menú.
    // Reaparecer en la misma escena (morir, reiniciar) no lo vuelve a mostrar.
    public static bool MostrarTitulo(string anterior, string actual)
    {
        if (DeEscena(actual) < 0 || anterior == actual)
            return false;

        return EsInicio(actual) || DeEscena(anterior) < 0;
    }

    // Repetir un capítulo no toca la partida guardada, salvo que el jugador llegue más lejos que ella
    public static bool SuperaLaPartida(int capituloActual, int capituloDeLaPartida) =>
        capituloActual > capituloDeLaPartida;
}
