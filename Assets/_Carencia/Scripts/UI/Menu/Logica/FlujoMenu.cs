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

    // Esc pausa cuando no tiene otro dueño: las vistas (periódico, tele) se cierran con Esc y las transiciones bloquean.
    // Durante un diálogo sí pausa (el diálogo avanza con Enter o clic), aunque la secuencia tenga quieta a Elaki
    public static bool PuedePausar(SituacionJuego situacion) =>
        situacion.hayJugador && !situacion.cargandoEscena && !situacion.hayVista
        && (situacion.hayDialogo || !situacion.jugadorBloqueado);
}

// Lo que hay en pantalla en el momento de pulsar Esc
public struct SituacionJuego
{
    public bool hayJugador;
    public bool jugadorBloqueado;
    public bool hayDialogo;
    public bool cargandoEscena;
    public bool hayVista;
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
