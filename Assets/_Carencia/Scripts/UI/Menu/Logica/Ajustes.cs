using System;
using UnityEngine;

// Las pestañas de Opciones.
public enum Categoria
{
    Audio,
    Video,
    Controles,
}

// Cada opción que el jugador puede cambiar en Opciones.
public enum Ajuste
{
    VolumenGeneral,
    VolumenMusica,
    VolumenEfectos,
    VolumenVoces,
    Brillo,
}

// Valores de Opciones. Los deslizadores van de 0 a 100 en pasos de 10.
[Serializable]
public class Ajustes
{
    private const int PasoDeslizador = 10;
    private const int MaximoDeslizador = 100;

    public int volumenGeneral = 80;
    public int volumenMusica = 60;
    public int volumenEfectos = 90;
    public int volumenVoces = 100;

    public int brillo = 50;

    public void Cambiar(Ajuste ajuste, int direccion)
    {
        switch (ajuste)
        {
            case Ajuste.VolumenGeneral: volumenGeneral = Deslizar(volumenGeneral, direccion); break;
            case Ajuste.VolumenMusica: volumenMusica = Deslizar(volumenMusica, direccion); break;
            case Ajuste.VolumenEfectos: volumenEfectos = Deslizar(volumenEfectos, direccion); break;
            case Ajuste.VolumenVoces: volumenVoces = Deslizar(volumenVoces, direccion); break;
            case Ajuste.Brillo: brillo = Deslizar(brillo, direccion); break;
        }
    }

    // Solo la pestaña indicada vuelve a sus valores de fábrica; las demás se quedan como están.
    public void Restablecer(Categoria categoria)
    {
        var fabrica = new Ajustes();
        switch (categoria)
        {
            case Categoria.Audio:
                volumenGeneral = fabrica.volumenGeneral;
                volumenMusica = fabrica.volumenMusica;
                volumenEfectos = fabrica.volumenEfectos;
                volumenVoces = fabrica.volumenVoces;
                break;
            case Categoria.Video:
                brillo = fabrica.brillo;
                break;
        }
    }

    public Ajustes Copia() => (Ajustes)MemberwiseClone();

    public string AJson() => JsonUtility.ToJson(this, true);

    // El archivo puede faltar, estar dañado o editado a mano: nunca debe dejar el juego con valores imposibles.
    // Los campos de versiones anteriores (resolución, accesibilidad…) se ignoran al leer y desaparecen al guardar.
    public static Ajustes DesdeJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Ajustes();

        try
        {
            var leidos = JsonUtility.FromJson<Ajustes>(json) ?? new Ajustes();
            leidos.Corregir();
            return leidos;
        }
        catch (ArgumentException)
        {
            return new Ajustes();
        }
    }

    private void Corregir()
    {
        volumenGeneral = AlPasoMasCercano(volumenGeneral);
        volumenMusica = AlPasoMasCercano(volumenMusica);
        volumenEfectos = AlPasoMasCercano(volumenEfectos);
        volumenVoces = AlPasoMasCercano(volumenVoces);
        brillo = AlPasoMasCercano(brillo);
    }

    private static int AlPasoMasCercano(int valor)
    {
        var redondeado = (int)Math.Round(valor / (double)PasoDeslizador, MidpointRounding.AwayFromZero) * PasoDeslizador;
        return Math.Clamp(redondeado, 0, MaximoDeslizador);
    }

    // Comparar el JSON incluye cualquier campo que se agregue después sin tener que acordarse de compararlo aquí.
    public bool IgualA(Ajustes otros) => JsonUtility.ToJson(this) == JsonUtility.ToJson(otros);

    private static int Deslizar(int valor, int direccion) =>
        Math.Clamp(valor + Math.Sign(direccion) * PasoDeslizador, 0, MaximoDeslizador);
}
