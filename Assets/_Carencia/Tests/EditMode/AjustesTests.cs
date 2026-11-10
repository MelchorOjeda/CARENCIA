using NUnit.Framework;

public class AjustesTests
{
    [Test]
    public void SubirUnVolumenSumaDiezSinPasarDeCien()
    {
        var ajustes = new Ajustes { volumenMusica = 60 };

        ajustes.Cambiar(Ajuste.VolumenMusica, +1);
        Assert.AreEqual(70, ajustes.volumenMusica);

        ajustes.volumenMusica = 100;
        ajustes.Cambiar(Ajuste.VolumenMusica, +1);
        Assert.AreEqual(100, ajustes.volumenMusica);
    }

    [Test]
    public void BajarUnVolumenRestaDiezSinBajarDeCero()
    {
        var ajustes = new Ajustes { volumenVoces = 10 };

        ajustes.Cambiar(Ajuste.VolumenVoces, -1);
        Assert.AreEqual(0, ajustes.volumenVoces);

        ajustes.Cambiar(Ajuste.VolumenVoces, -1);
        Assert.AreEqual(0, ajustes.volumenVoces);
    }

    [Test]
    public void UnaOpcionDeSiONoDaLaVueltaEnAmbosSentidos()
    {
        var ajustes = new Ajustes { contrasteAlto = false };

        ajustes.Cambiar(Ajuste.ContrasteAlto, +1);
        Assert.IsTrue(ajustes.contrasteAlto);

        ajustes.Cambiar(Ajuste.ContrasteAlto, -1);
        Assert.IsFalse(ajustes.contrasteAlto);
    }

    [Test]
    public void ElTamanoDeTextoDaLaVueltaAlLlegarAlFinal()
    {
        var ajustes = new Ajustes { tamanoTexto = TamanoTexto.Grande };

        ajustes.Cambiar(Ajuste.TamanoTexto, +1);
        Assert.AreEqual(TamanoTexto.Pequeno, ajustes.tamanoTexto);

        ajustes.Cambiar(Ajuste.TamanoTexto, -1);
        Assert.AreEqual(TamanoTexto.Grande, ajustes.tamanoTexto);
    }

    [Test]
    public void RestablecerUnaCategoriaSoloDevuelveLosValoresDeEsaCategoria()
    {
        var ajustes = new Ajustes { volumenGeneral = 10, volumenMusica = 0, contrasteAlto = true };

        ajustes.Restablecer(Categoria.Audio);

        Assert.AreEqual(80, ajustes.volumenGeneral);
        Assert.AreEqual(60, ajustes.volumenMusica);
        Assert.IsTrue(ajustes.contrasteAlto, "Accesibilidad no debe cambiar al restablecer Audio");
    }

    [Test]
    public void HaySinGuardarSoloCuandoAlgoDifiereDeLoGuardado()
    {
        var guardados = new Ajustes();
        var enEdicion = guardados.Copia();
        Assert.IsTrue(enEdicion.IgualA(guardados));

        enEdicion.Cambiar(Ajuste.Brillo, +1);
        Assert.IsFalse(enEdicion.IgualA(guardados));

        enEdicion.Cambiar(Ajuste.Brillo, -1);
        Assert.IsTrue(enEdicion.IgualA(guardados), "Volver al valor original ya no cuenta como cambio");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("{ esto no es json")]
    public void UnArchivoDeAjustesDanadoDaLosValoresPorDefecto(string contenido)
    {
        var ajustes = Ajustes.DesdeJson(contenido);

        Assert.IsTrue(ajustes.IgualA(new Ajustes()));
    }

    [Test]
    public void LosValoresFueraDeRangoEnElArchivoSeCorrigen()
    {
        var ajustes = Ajustes.DesdeJson("{\"volumenGeneral\": 250, \"brillo\": -40, \"volumenMusica\": 55, \"tamanoTexto\": 9}");

        Assert.AreEqual(100, ajustes.volumenGeneral);
        Assert.AreEqual(0, ajustes.brillo);
        Assert.AreEqual(60, ajustes.volumenMusica, "Se redondea al paso de 10 más cercano");
        Assert.AreEqual(TamanoTexto.Mediano, ajustes.tamanoTexto);
    }

    [Test]
    public void LaResolucionRecorreLasDelMonitorYDaLaVuelta()
    {
        var disponibles = new[] { new Resolucion(1280, 720), new Resolucion(1600, 900), new Resolucion(1920, 1080) };
        var ajustes = new Ajustes { anchoPantalla = 1600, altoPantalla = 900 };

        ajustes.CambiarResolucion(+1, disponibles);
        Assert.AreEqual(new Resolucion(1920, 1080), ajustes.Resolucion);

        ajustes.CambiarResolucion(+1, disponibles);
        Assert.AreEqual(new Resolucion(1280, 720), ajustes.Resolucion);
    }

    [Test]
    public void UnaResolucionQueElMonitorNoTieneSaltaALaPrimeraDisponible()
    {
        var disponibles = new[] { new Resolucion(1280, 720), new Resolucion(1920, 1080) };
        var ajustes = new Ajustes { anchoPantalla = 3000, altoPantalla = 2000 };

        ajustes.CambiarResolucion(+1, disponibles);

        Assert.AreEqual(new Resolucion(1280, 720), ajustes.Resolucion);
    }

    [Test]
    public void LaCalidadRecorreLosNivelesDelProyectoYDaLaVuelta()
    {
        var ajustes = new Ajustes { calidad = 5 };

        ajustes.CambiarCalidad(+1, 6);
        Assert.AreEqual(0, ajustes.calidad);

        ajustes.CambiarCalidad(-1, 6);
        Assert.AreEqual(5, ajustes.calidad);
    }

    [Test]
    public void LoQueSeGuardaSeLeeIgual()
    {
        var original = new Ajustes { volumenEfectos = 30, reducirMovimiento = true, tamanoTexto = TamanoTexto.Grande };

        var leido = Ajustes.DesdeJson(original.AJson());

        Assert.IsTrue(leido.IgualA(original));
    }
}
