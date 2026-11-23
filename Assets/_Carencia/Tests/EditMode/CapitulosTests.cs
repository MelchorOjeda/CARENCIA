using NUnit.Framework;

public class CapitulosTests
{
    [Test]
    public void CadaEscenaDeLaHistoriaPerteneceASuCapitulo()
    {
        Assert.AreEqual(0, Capitulos.DeEscena("PRO_Casa"));
        Assert.AreEqual(1, Capitulos.DeEscena("N1_Laberinto"));
        Assert.AreEqual(1, Capitulos.DeEscena("N1_Pasillo"));
        Assert.AreEqual(1, Capitulos.DeEscena("R1_Casa"));
        Assert.AreEqual(2, Capitulos.DeEscena("N2_Casa"));
        Assert.AreEqual(2, Capitulos.DeEscena("R2_Casa"));
        Assert.AreEqual(3, Capitulos.DeEscena("N3_Casa"));
        Assert.AreEqual(4, Capitulos.DeEscena("DES_Final"));
    }

    [Test]
    public void ElMenuYLasPruebasNoSonCapitulos()
    {
        Assert.AreEqual(-1, Capitulos.DeEscena("MEN_Principal"));
        Assert.AreEqual(-1, Capitulos.DeEscena("PRB_Melchor_Casa"));
        Assert.AreEqual(-1, Capitulos.DeEscena(null));
    }

    [Test]
    public void SoloLaPrimeraEscenaEmpiezaElCapitulo()
    {
        Assert.IsTrue(Capitulos.EsInicio("N1_Laberinto"));
        Assert.IsFalse(Capitulos.EsInicio("N1_Pasillo"));
        Assert.IsFalse(Capitulos.EsInicio("R2_Casa"));
    }

    [Test]
    public void ElTituloSaleAlEntrarAlCapituloNoAlReaparecer()
    {
        Assert.IsTrue(Capitulos.MostrarTitulo("R1_Casa", "N2_Casa"));
        Assert.IsTrue(Capitulos.MostrarTitulo("MEN_Principal", "PRO_Casa"));
        Assert.IsFalse(Capitulos.MostrarTitulo("N2_Casa", "N2_Casa"));
        Assert.IsFalse(Capitulos.MostrarTitulo("N1_Laberinto", "N1_Pasillo"));
        Assert.IsFalse(Capitulos.MostrarTitulo("DES_Final", "MEN_Principal"));
    }

    [Test]
    public void ContinuarDesdeElMenuRecuerdaElCapitulo()
    {
        Assert.IsTrue(Capitulos.MostrarTitulo("MEN_Principal", "R2_Casa"));
    }

    [Test]
    public void UnCapituloBloqueadoNoDiceSuTitulo()
    {
        foreach (var capitulo in Capitulos.Todos)
            StringAssert.DoesNotContain(capitulo.Titulo, capitulo.NombreBloqueado);
    }

    [Test]
    public void RepetirSoloReemplazaLaPartidaAlLlegarMasLejos()
    {
        Assert.IsFalse(Capitulos.SuperaLaPartida(1, 2));
        Assert.IsFalse(Capitulos.SuperaLaPartida(2, 2));
        Assert.IsTrue(Capitulos.SuperaLaPartida(3, 2));
    }
}
