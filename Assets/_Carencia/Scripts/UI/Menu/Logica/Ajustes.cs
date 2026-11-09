using System;
using System.Collections.Generic;
using UnityEngine;

// Las pestañas de Opciones.
public enum Categoria
{
    Audio,
    Video,
    Controles,
    Accesibilidad,
}

// Cada opción que el jugador puede cambiar en Opciones.
public enum Ajuste
{
    VolumenGeneral,
    VolumenMusica,
    VolumenEfectos,
    VolumenVoces,
    PantallaCompleta,
    Brillo,
    TamanoTexto,
    Subtitulos,
    ContrasteAlto,
    ReducirDestellos,
    ReducirMovimiento,
    IndicadoresSonido,
}

public enum TamanoTexto
{
    Pequeno,
    Mediano,
    Grande,
}

// Valores de Opciones. Los deslizadores van de 0 a 100 en pasos de 10; las listas dan la vuelta al llegar al final.
[Serializable]
public class Ajustes
{
    private const int PasoDeslizador = 10;
    private const int MaximoDeslizador = 100;

    public int volumenGeneral = 80;
    public int volumenMusica = 60;
    public int volumenEfectos = 90;
    public int volumenVoces = 100;

    // 0 y -1 significan "la del monitor" y "la del proyecto": se resuelven al aplicar, porque dependen de cada equipo.
    public int anchoPantalla;
    public int altoPantalla;
    public int calidad = -1;
    public bool pantallaCompleta = true;
    public int brillo = 50;

    public TamanoTexto tamanoTexto = TamanoTexto.Mediano;
    public bool subtitulos = true;
    public bool contrasteAlto;
    public bool reducirDestellos;
    public bool reducirMovimiento;
    public bool indicadoresSonido = true;

    public void Cambiar(Ajuste ajuste, int direccion)
    {
        switch (ajuste)
        {
            case Ajuste.VolumenGeneral: volumenGeneral = Deslizar(volumenGeneral, direccion); break;
            case Ajuste.VolumenMusica: volumenMusica = Deslizar(volumenMusica, direccion); break;
            case Ajuste.VolumenEfectos: volumenEfectos = Deslizar(volumenEfectos, direccion); break;
            case Ajuste.VolumenVoces: volumenVoces = Deslizar(volumenVoces, direccion); break;
            case Ajuste.PantallaCompleta: pantallaCompleta = !pantallaCompleta; break;
            case Ajuste.Brillo: brillo = Deslizar(brillo, direccion); break;
            case Ajuste.TamanoTexto: tamanoTexto = (TamanoTexto)Ciclo.Siguiente((int)tamanoTexto, 3, direccion); break;
            case Ajuste.Subtitulos: subtitulos = !subtitulos; break;
            case Ajuste.ContrasteAlto: contrasteAlto = !contrasteAlto; break;
            case Ajuste.ReducirDestellos: reducirDestellos = !reducirDestellos; break;
            case Ajuste.ReducirMovimiento: reducirMovimiento = !reducirMovimiento; break;
            case Ajuste.IndicadoresSonido: indicadoresSonido = !indicadoresSonido; break;
        }
    }

    public Resolucion Resolucion => new Resolucion(anchoPantalla, altoPantalla);

    // Una resolución que el monitor no tiene (otro monitor, archivo editado) empieza por la primera de la lista.
    public void CambiarResolucion(int direccion, IReadOnlyList<Resolucion> disponibles)
    {
        if (disponibles.Count == 0)
            return;

        var actual = IndiceDe(Resolucion, disponibles);
        var siguiente = actual < 0 ? 0 : Ciclo.Siguiente(actual, disponibles.Count, direccion);
        anchoPantalla = disponibles[siguiente].ancho;
        altoPantalla = disponibles[siguiente].alto;
    }

    public void CambiarCalidad(int direccion, int cantidadNiveles)
    {
        calidad = Ciclo.Siguiente(calidad, cantidadNiveles, direccion);
    }

    private static int IndiceDe(Resolucion buscada, IReadOnlyList<Resolucion> lista)
    {
        for (var i = 0; i < lista.Count; i++)
        {
            if (lista[i].Equals(buscada))
                return i;
        }

        return -1;
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
                anchoPantalla = fabrica.anchoPantalla;
                altoPantalla = fabrica.altoPantalla;
                calidad = fabrica.calidad;
                pantallaCompleta = fabrica.pantallaCompleta;
                brillo = fabrica.brillo;
                break;
            case Categoria.Accesibilidad:
                tamanoTexto = fabrica.tamanoTexto;
                subtitulos = fabrica.subtitulos;
                contrasteAlto = fabrica.contrasteAlto;
                reducirDestellos = fabrica.reducirDestellos;
                reducirMovimiento = fabrica.reducirMovimiento;
                indicadoresSonido = fabrica.indicadoresSonido;
                break;
        }
    }

    public Ajustes Copia() => (Ajustes)MemberwiseClone();

    public string AJson() => JsonUtility.ToJson(this, true);

    // El archivo puede faltar, estar dañado o editado a mano: nunca debe dejar el juego con valores imposibles.
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

        if (!Enum.IsDefined(typeof(TamanoTexto), tamanoTexto))
            tamanoTexto = TamanoTexto.Mediano;
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
