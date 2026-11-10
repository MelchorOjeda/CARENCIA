using System;
using System.Linq;
using UnityEngine;

// Lista vertical de opciones: ↑↓ mueve el foco saltando las deshabilitadas, Enter elige, el mouse enfoca y elige.
// Mientras hay una nota abierta, o después de elegir algo que carga otra escena, no responde.
public class ListaOpciones : MonoBehaviour
{
    [SerializeField] private OpcionMenu[] opciones;
    [SerializeField] private NotaConfirmacion nota;

    private int enfocada;

    public event Action<int> Elegida;

    public bool Activa { get; set; } = true;

    private bool Responde => Activa && (nota == null || !nota.Abierta);

    private void Awake()
    {
        foreach (var opcion in opciones)
        {
            opcion.Apuntada += AlApuntar;
            opcion.Pulsada += AlPulsar;
        }
    }

    private void Update()
    {
        if (!Responde)
            return;

        var direccion = ControlesMenu.Vertical();
        if (direccion != 0)
            Enfocar(Navegacion.Siguiente(Habilitadas(), enfocada, direccion));
        else if (ControlesMenu.Pulso(ControlesMenu.Confirmar))
            Elegir();
    }

    public void EnfocarPrimera()
    {
        Enfocar(Navegacion.Primera(Habilitadas()));
    }

    public void Enfocar(int indice)
    {
        enfocada = opciones[indice].Habilitada ? indice : Navegacion.Primera(Habilitadas());
        for (var i = 0; i < opciones.Length; i++)
            opciones[i].MostrarEnfoque(i == enfocada);
    }

    private void Elegir()
    {
        if (opciones[enfocada].Habilitada)
            Elegida?.Invoke(enfocada);
    }

    private void AlApuntar(OpcionMenu opcion)
    {
        if (Responde)
            Enfocar(Array.IndexOf(opciones, opcion));
    }

    private void AlPulsar(OpcionMenu opcion)
    {
        if (!Responde)
            return;

        Enfocar(Array.IndexOf(opciones, opcion));
        Elegir();
    }

    private bool[] Habilitadas() => opciones.Select(opcion => opcion.Habilitada).ToArray();
}
