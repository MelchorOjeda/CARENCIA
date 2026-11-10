using NUnit.Framework;

public class FlujoMenuTests
{
    [Test]
    public void JugarSinPartidaGuardadaEmpiezaDirecto()
    {
        Assert.AreEqual(Decision.Hacerlo, FlujoMenu.AlElegirJugar(hayPartida: false));
    }

    [Test]
    public void JugarConPartidaGuardadaPreguntaAntesDeBorrarla()
    {
        Assert.AreEqual(Decision.Preguntar, FlujoMenu.AlElegirJugar(hayPartida: true));
    }

    [Test]
    public void SalirDeOpcionesSinCambiosVuelveDirecto()
    {
        Assert.AreEqual(Decision.Hacerlo, FlujoMenu.AlSalirDeOpciones(haySinGuardar: false));
    }

    [Test]
    public void SalirDeOpcionesConCambiosSinGuardarPreguntaAntes()
    {
        Assert.AreEqual(Decision.Preguntar, FlujoMenu.AlSalirDeOpciones(haySinGuardar: true));
    }

    [Test]
    public void LaPausaSeAbreSoloCuandoElJugadorPuedeMoverse()
    {
        var libre = new SituacionJuego { hayJugador = true };
        Assert.IsTrue(FlujoMenu.PuedePausar(libre));

        Assert.IsFalse(FlujoMenu.PuedePausar(new SituacionJuego()), "Sin jugador, como en el menú principal");
        Assert.IsFalse(FlujoMenu.PuedePausar(new SituacionJuego { hayJugador = true, jugadorBloqueado = true }), "Secuencia, gabinete o muerte");
        Assert.IsFalse(FlujoMenu.PuedePausar(new SituacionJuego { hayJugador = true, hayDialogo = true }), "Esc ya cierra el diálogo");
        Assert.IsFalse(FlujoMenu.PuedePausar(new SituacionJuego { hayJugador = true, cargandoEscena = true }), "Durante el fundido de carga");
    }

    [Test]
    public void UnSegundoEnterNoRepiteLaAccion()
    {
        var accion = new AccionUnica();

        Assert.IsTrue(accion.Tomar(), "El primer Enter sí cuenta");
        Assert.IsFalse(accion.Tomar(), "El segundo Enter llega mientras carga la escena y se ignora");
        Assert.IsFalse(accion.Tomar());
    }
}
