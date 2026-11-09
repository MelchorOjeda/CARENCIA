using System;
using System.Linq;
using TMPro;
using UnityEngine;

// Nota de papel que pregunta antes de una acción que no se puede deshacer.
// El foco empieza en "No", así un Enter de más nunca borra ni sale de nada. Esc equivale a "No".
public class NotaConfirmacion : MonoBehaviour
{
    private const int No = 0;
    private const int Si = 1;

    [SerializeField] private TMP_Text pregunta;
    [SerializeField] private TMP_Text detalle;
    [SerializeField] private OpcionMenu opcionNo;
    [SerializeField] private OpcionMenu opcionSi;
    [Tooltip("Barras de ayuda de las pantallas de abajo: se esconden para que no se encimen con la de la nota")]
    [SerializeField] private GameObject[] ocultarMientrasPregunta;

    private bool[] estabanVisibles;
    private Action alAceptar;
    private Action alCancelar;
    private int enfocada;

    public bool Abierta => gameObject.activeSelf;

    private void Awake()
    {
        opcionNo.Apuntada += _ => Enfocar(No);
        opcionSi.Apuntada += _ => Enfocar(Si);
        opcionNo.Pulsada += _ => Responder(No);
        opcionSi.Pulsada += _ => Responder(Si);
    }

    public void Preguntar(string textoPregunta, string textoDetalle, string textoSi, Action siAcepta, Action siCancela = null)
    {
        pregunta.text = textoPregunta;
        detalle.text = textoDetalle;
        opcionSi.CambiarTexto(textoSi);
        alAceptar = siAcepta;
        alCancelar = siCancela;

        OcultarLoDeAbajo();
        gameObject.SetActive(true);
        Enfocar(No);
        ControlesMenu.Bloquear();
    }

    private void Update()
    {
        if (!ControlesMenu.Listo)
            return;

        if (ControlesMenu.Horizontal() != 0 || ControlesMenu.Vertical() != 0)
            Enfocar(enfocada == No ? Si : No);
        else if (ControlesMenu.Pulso(ControlesMenu.Confirmar))
            Responder(enfocada);
        else if (ControlesMenu.Pulso(ControlesMenu.Volver))
            Responder(No);
    }

    private void Enfocar(int opcion)
    {
        enfocada = opcion;
        opcionNo.MostrarEnfoque(opcion == No);
        opcionSi.MostrarEnfoque(opcion == Si);
    }

    // Se cierra antes de avisar, por si la respuesta abre otra nota
    private void Responder(int opcion)
    {
        gameObject.SetActive(false);
        RestaurarLoDeAbajo();
        ControlesMenu.Bloquear();
        var respuesta = opcion == Si ? alAceptar : alCancelar;
        respuesta?.Invoke();
    }

    private void OcultarLoDeAbajo()
    {
        if (Abierta)
            return;

        estabanVisibles = ocultarMientrasPregunta.Select(objeto => objeto.activeSelf).ToArray();
        foreach (var objeto in ocultarMientrasPregunta)
            objeto.SetActive(false);
    }

    private void RestaurarLoDeAbajo()
    {
        for (var i = 0; i < ocultarMientrasPregunta.Length; i++)
            ocultarMientrasPregunta[i].SetActive(estabanVisibles[i]);
    }
}
