using NUnit.Framework;
using UnityEngine;

// Cómo se encuentra el texto de cada clave, incluida una clave que el equipo de diálogos aún no escribe.
public class GuionDialogosTests
{
    private GuionDialogos guion;

    [SetUp]
    public void Crear()
    {
        guion = ScriptableObject.CreateInstance<GuionDialogos>();
        guion.lineas.Add(new LineaDialogo { clave = "P1_mama", quien = "Mamá", texto = "Elaki, vamos a comer." });
    }

    [TearDown]
    public void Destruir()
    {
        Object.DestroyImmediate(guion);
    }

    [Test]
    public void EncuentraLaLineaPorSuClave()
    {
        var linea = guion.LineaDe("P1_mama");

        Assert.AreEqual("Mamá", linea.quien);
        Assert.AreEqual("Elaki, vamos a comer.", linea.texto);
    }

    [Test]
    public void UnaClaveSinTextoSeMuestraEntreCorchetesParaQueSeNote()
    {
        var linea = guion.LineaDe("N9_inexistente");

        Assert.AreEqual("", linea.quien);
        Assert.AreEqual("[N9_inexistente]", linea.texto);
        Assert.IsTrue(linea.TieneTexto);
    }

    [Test]
    public void LaVelocidadNuncaEsNegativa()
    {
        Assert.GreaterOrEqual(guion.LetrasPorSegundo, 0f);
    }
}
