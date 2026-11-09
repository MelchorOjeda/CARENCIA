using System;

// Tamaño de la imagen en píxeles, sin la frecuencia del monitor: dos modos de 60 y 144 Hz cuentan como la misma opción.
public readonly struct Resolucion : IEquatable<Resolucion>
{
    public readonly int ancho;
    public readonly int alto;

    public Resolucion(int ancho, int alto)
    {
        this.ancho = ancho;
        this.alto = alto;
    }

    public bool Equals(Resolucion otra) => ancho == otra.ancho && alto == otra.alto;

    public override bool Equals(object otra) => otra is Resolucion resolucion && Equals(resolucion);

    public override int GetHashCode() => HashCode.Combine(ancho, alto);

    public override string ToString() => $"{ancho} × {alto}";
}
