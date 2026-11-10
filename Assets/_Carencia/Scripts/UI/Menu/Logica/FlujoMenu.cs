// Qué hace el menú cuando una acción puede costarle algo al jugador: hacerlo de inmediato o preguntar antes.
public enum Decision
{
    Hacerlo,
    Preguntar,
}

public static class FlujoMenu
{
    // Empezar de nuevo borra la partida guardada, así que solo se pregunta cuando hay una.
    public static Decision AlElegirJugar(bool hayPartida) => hayPartida ? Decision.Preguntar : Decision.Hacerlo;

    public static Decision AlSalirDeOpciones(bool haySinGuardar) => haySinGuardar ? Decision.Preguntar : Decision.Hacerlo;

    // Esc solo pausa cuando no tiene otro dueño: los diálogos, las vistas y las transiciones ya usan Esc o bloquean al jugador
    public static bool PuedePausar(SituacionJuego situacion) =>
        situacion.hayJugador && !situacion.jugadorBloqueado && !situacion.hayDialogo && !situacion.cargandoEscena;
}

// Lo que hay en pantalla en el momento de pulsar Esc
public struct SituacionJuego
{
    public bool hayJugador;
    public bool jugadorBloqueado;
    public bool hayDialogo;
    public bool cargandoEscena;
}

// Deja pasar una sola vez: tras elegir Jugar, Continuar o Salir, los Enter y clics repetidos no vuelven a cargar nada.
public class AccionUnica
{
    private bool tomada;

    public bool Tomar()
    {
        if (tomada)
            return false;

        tomada = true;
        return true;
    }
}
