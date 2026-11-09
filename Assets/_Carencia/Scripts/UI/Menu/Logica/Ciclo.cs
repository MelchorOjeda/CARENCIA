// Recorre una lista que da la vuelta: después del último sigue el primero y antes del primero, el último.
public static class Ciclo
{
    public static int Siguiente(int actual, int cantidad, int direccion)
    {
        if (cantidad <= 0)
            return 0;

        var paso = direccion > 0 ? 1 : -1;
        return ((actual + paso) % cantidad + cantidad) % cantidad;
    }
}
