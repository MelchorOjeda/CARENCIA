using NUnit.Framework;

// Lo que el jugador ve y hace con una línea de diálogo: el texto se escribe letra por letra
// y cada pulsación (Enter, clic o E) completa la página o avanza.
public class LineaEnPantallaTests
{
    private const float DiezLetrasPorSegundo = 10f;

    private static LineaEnPantalla Mostrar(string texto, float letrasPorSegundo = DiezLetrasPorSegundo)
    {
        var linea = new LineaDialogo { clave = "prueba", quien = "Elaki", texto = texto };
        return new LineaEnPantalla(linea, letrasPorSegundo);
    }

    [Test]
    public void AlAbrirNoSeVeNingunaLetra()
    {
        var linea = Mostrar("Cuatro.");

        Assert.AreEqual(0, linea.LetrasVisibles);
        Assert.IsTrue(linea.Escribiendo);
    }

    [Test]
    public void LasLetrasAparecenSegunElTiempo()
    {
        var linea = Mostrar("Cuatro sillas.");

        linea.Avanzar(0.5f);

        Assert.AreEqual(5, linea.LetrasVisibles);
    }

    [Test]
    public void UnFotogramaMuyLargoNoSePasaDelFinal()
    {
        var linea = Mostrar("Cuatro.");

        linea.Avanzar(60f);

        Assert.AreEqual(7, linea.LetrasVisibles);
        Assert.IsFalse(linea.Escribiendo);
    }

    [Test]
    public void SinVelocidadElTextoSeVeCompletoDeGolpe()
    {
        var linea = Mostrar("Cuatro.", letrasPorSegundo: 0f);

        Assert.AreEqual(7, linea.LetrasVisibles);
        Assert.IsFalse(linea.Escribiendo);
    }

    [Test]
    public void PulsarMientrasEscribeCompletaLaPaginaSinCerrar()
    {
        var linea = Mostrar("Está cerrada. Mejor así.");
        linea.Avanzar(0.3f);

        linea.Pulsar();

        Assert.AreEqual("Está cerrada. Mejor así.".Length, linea.LetrasVisibles);
        Assert.IsFalse(linea.Terminada);
    }

    [Test]
    public void PulsarConLaPaginaCompletaCierraLaLinea()
    {
        var linea = Mostrar("Bien.");
        linea.Avanzar(60f);

        linea.Pulsar();

        Assert.IsTrue(linea.Terminada);
    }

    [Test]
    public void DosPulsacionesSeguidasCompletanYDespuesCierran()
    {
        var linea = Mostrar("Ya pasé por aquí.");

        linea.Pulsar();
        Assert.IsFalse(linea.Terminada);

        linea.Pulsar();
        Assert.IsTrue(linea.Terminada);
    }

    [Test]
    public void UnaLineaEnBlancoSeparaPaginas()
    {
        var linea = Mostrar("Más familias buscan a sus desaparecidos\n\nEl registro sigue creciendo.");

        Assert.AreEqual("Más familias buscan a sus desaparecidos", linea.Pagina);
    }

    [Test]
    public void PulsarConLaPaginaCompletaPasaALaSiguienteDesdeCero()
    {
        var linea = Mostrar("Primera.\n\nSegunda.");
        linea.Avanzar(60f);

        linea.Pulsar();

        Assert.AreEqual("Segunda.", linea.Pagina);
        Assert.AreEqual(0, linea.LetrasVisibles);
        Assert.IsFalse(linea.Terminada);
    }

    [Test]
    public void LaLineaTerminaAlPulsarEnLaUltimaPagina()
    {
        var linea = Mostrar("Primera.\n\nSegunda.");

        linea.Pulsar();
        linea.Pulsar();
        linea.Pulsar();
        linea.Pulsar();

        Assert.IsTrue(linea.Terminada);
    }

    [Test]
    public void LosRenglonesVaciosDeMasNoCreanPaginasVacias()
    {
        var linea = Mostrar("\n\nPrimera.\n\n\n\n  \n\nSegunda.\n\n");
        linea.Avanzar(60f);
        linea.Pulsar();

        Assert.AreEqual("Segunda.", linea.Pagina);
    }

    [Test]
    public void UnTextoVacioMuestraLaCajaVaciaYCierraConUnaPulsacion()
    {
        var linea = Mostrar("");

        Assert.AreEqual("", linea.Pagina);
        Assert.IsFalse(linea.Escribiendo);
        Assert.IsFalse(linea.Terminada);

        linea.Pulsar();
        Assert.IsTrue(linea.Terminada);
    }

    [Test]
    public void PulsarDespuesDeTerminarNoHaceNada()
    {
        var linea = Mostrar("Bien.");
        linea.Pulsar();
        linea.Pulsar();

        linea.Pulsar();
        linea.Avanzar(1f);

        Assert.IsTrue(linea.Terminada);
        Assert.AreEqual("Bien.", linea.Pagina);
    }

    [Test]
    public void ConservaQuienHablaYSiEsPensamiento()
    {
        var datos = new LineaDialogo { quien = "Mamá", texto = "Hijo…", pensamiento = true };

        var linea = new LineaEnPantalla(datos, DiezLetrasPorSegundo);

        Assert.AreEqual("Mamá", linea.Quien);
        Assert.IsTrue(linea.EsPensamiento);
    }

    [Test]
    public void SoloLasLineasConTextoSeMuestran()
    {
        Assert.IsTrue(new LineaDialogo { texto = "Cuatro." }.TieneTexto);
        Assert.IsFalse(new LineaDialogo { texto = "" }.TieneTexto);
        Assert.IsFalse(new LineaDialogo { texto = "  \n\n " }.TieneTexto);
        Assert.IsFalse(new LineaDialogo().TieneTexto);
    }
}
