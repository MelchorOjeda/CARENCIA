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
    public void UnSegundoEnterNoRepiteLaAccion()
    {
        var accion = new AccionUnica();

        Assert.IsTrue(accion.Tomar(), "El primer Enter sí cuenta");
        Assert.IsFalse(accion.Tomar(), "El segundo Enter llega mientras carga la escena y se ignora");
        Assert.IsFalse(accion.Tomar());
    }
}
