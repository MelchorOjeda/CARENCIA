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
        const int pestanas = 4;

        Assert.AreEqual(3, Ciclo.Siguiente(0, pestanas, -1), "Q en Audio va a Accesibilidad");
        Assert.AreEqual(0, Ciclo.Siguiente(3, pestanas, +1), "E en Accesibilidad va a Audio");
    }
}
