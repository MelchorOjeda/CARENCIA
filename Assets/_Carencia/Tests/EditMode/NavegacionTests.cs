using NUnit.Framework;

public class NavegacionTests
{
    // Jugar, Continuar (deshabilitado: no hay partida), Opciones, Salir
    private static readonly bool[] MenuSinPartida = { true, false, true, true };

    [Test]
    public void BajarDesdeJugarSaltaContinuarDeshabilitado()
    {
        Assert.AreEqual(2, Navegacion.Siguiente(MenuSinPartida, 0, +1));
    }

    [Test]
    public void SubirDesdeOpcionesSaltaContinuarDeshabilitado()
    {
        Assert.AreEqual(0, Navegacion.Siguiente(MenuSinPartida, 2, -1));
    }

    [Test]
    public void LaListaDaLaVueltaEnAmbosExtremos()
    {
        Assert.AreEqual(0, Navegacion.Siguiente(MenuSinPartida, 3, +1));
        Assert.AreEqual(3, Navegacion.Siguiente(MenuSinPartida, 0, -1));
    }

    [Test]
    public void ElFocoInicialEsLaPrimeraOpcionHabilitada()
    {
        Assert.AreEqual(0, Navegacion.Primera(MenuSinPartida));
        Assert.AreEqual(1, Navegacion.Primera(new[] { false, true, true }));
    }

    [Test]
    public void SiSoloHayUnaOpcionHabilitadaElFocoSeQuedaEnElla()
    {
        var soloSalir = new[] { false, false, false, true };

        Assert.AreEqual(3, Navegacion.Siguiente(soloSalir, 3, +1));
        Assert.AreEqual(3, Navegacion.Siguiente(soloSalir, 3, -1));
    }

    [Test]
    public void LasPestanasDanLaVueltaConQyE()
    {
        var pestanas = System.Enum.GetValues(typeof(Categoria)).Length;

        Assert.AreEqual((int)Categoria.Controles, Ciclo.Siguiente((int)Categoria.Audio, pestanas, -1), "Q en Audio va a Controles");
        Assert.AreEqual((int)Categoria.Audio, Ciclo.Siguiente((int)Categoria.Controles, pestanas, +1), "E en Controles va a Audio");
    }
}
