using System.Collections.Generic;

// Mueve el foco por una lista de opciones saltando las deshabilitadas y dando la vuelta en los extremos.
public static class Navegacion
{
    public static int Primera(IReadOnlyList<bool> habilitadas)
    {
        for (var i = 0; i < habilitadas.Count; i++)
        {
            if (habilitadas[i])
                return i;
        }

        return 0;
    }

    // Si no hay otra opción habilitada, el foco se queda donde está.
    public static int Siguiente(IReadOnlyList<bool> habilitadas, int actual, int direccion)
    {
        var candidata = actual;
        for (var paso = 0; paso < habilitadas.Count; paso++)
        {
            candidata = Ciclo.Siguiente(candidata, habilitadas.Count, direccion);
            if (habilitadas[candidata])
                return candidata;
        }

        return actual;
    }
}
