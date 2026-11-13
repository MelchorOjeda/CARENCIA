using System;
using NUnit.Framework;

public class AjustesTests
{
    [Test]
    public void LasPestanasDeOpcionesSonAudioVideoYControles()
    {
        var pestanas = (Categoria[])Enum.GetValues(typeof(Categoria));

        CollectionAssert.AreEqual(new[] { Categoria.Audio, Categoria.Video, Categoria.Controles }, pestanas);
    }

    [Test]
    public void LoUnicoQueSeAjustaSonLosVolumenesYElBrillo()
    {
        var ajustables = (Ajuste[])Enum.GetValues(typeof(Ajuste));

        CollectionAssert.AreEqual(
            new[] { Ajuste.VolumenGeneral, Ajuste.VolumenMusica, Ajuste.VolumenEfectos, Ajuste.VolumenVoces, Ajuste.Brillo },
            ajustables);
    }

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
    public void RestablecerUnaCategoriaSoloDevuelveLosValoresDeEsaCategoria()
    {
        var ajustes = new Ajustes { volumenGeneral = 10, volumenMusica = 0, brillo = 90 };

        ajustes.Restablecer(Categoria.Audio);

        Assert.AreEqual(80, ajustes.volumenGeneral);
        Assert.AreEqual(60, ajustes.volumenMusica);
        Assert.AreEqual(90, ajustes.brillo, "Video no debe cambiar al restablecer Audio");
    }

    [Test]
    public void RestablecerVideoDevuelveElBrilloDeFabricaSinTocarElAudio()
    {
        var ajustes = new Ajustes { brillo = 90, volumenEfectos = 30 };

        ajustes.Restablecer(Categoria.Video);

        Assert.AreEqual(50, ajustes.brillo);
        Assert.AreEqual(30, ajustes.volumenEfectos);
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
        var ajustes = Ajustes.DesdeJson("{\"volumenGeneral\": 250, \"brillo\": -40, \"volumenMusica\": 55}");

        Assert.AreEqual(100, ajustes.volumenGeneral);
        Assert.AreEqual(0, ajustes.brillo);
        Assert.AreEqual(60, ajustes.volumenMusica, "Se redondea al paso de 10 más cercano");
    }

    // Los jugadores que ya guardaron con la versión anterior tienen estos campos en su ajustes.json
    [Test]
    public void UnArchivoDeLaVersionAnteriorConservaAudioYBrilloYOlvidaLoDemas()
    {
        const string archivoAnterior = "{\"volumenGeneral\": 30, \"brillo\": 70, \"anchoPantalla\": 1280, \"altoPantalla\": 720, " +
                                       "\"calidad\": 2, \"pantallaCompleta\": false, \"tamanoTexto\": 2, \"contrasteAlto\": true, " +
                                       "\"reducirDestellos\": true, \"reducirMovimiento\": true}";

        var ajustes = Ajustes.DesdeJson(archivoAnterior);

        Assert.IsTrue(ajustes.IgualA(new Ajustes { volumenGeneral = 30, brillo = 70 }));
        StringAssert.DoesNotContain("pantallaCompleta", ajustes.AJson());
        StringAssert.DoesNotContain("contrasteAlto", ajustes.AJson());
    }

    [Test]
    public void LoQueSeGuardaSeLeeIgual()
    {
        var original = new Ajustes { volumenEfectos = 30, brillo = 20 };

        var leido = Ajustes.DesdeJson(original.AJson());

        Assert.IsTrue(leido.IgualA(original));
    }
}
