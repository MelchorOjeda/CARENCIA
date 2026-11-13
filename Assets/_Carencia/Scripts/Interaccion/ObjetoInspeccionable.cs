using System;
using UnityEngine;
using UnityEngine.Events;

// Representa un elemento interactivo dentro de la vista de inspección (como una caja de medicina, rastrillo, etc.)
// Puede colocarse en un GameObject 2D con Collider2D (usando OnMouseDown / OnMouseEnter) o con EventTrigger / UI.
public class ObjetoInspeccionable : MonoBehaviour
{
    [Header("Datos del Objeto")]
    [SerializeField] private string idObjeto = "sertralina";
    [SerializeField] private string nombreVisible = "Sertralina 50 mg";
    [TextArea(2, 5)]
    [SerializeField] private string pensamientoElaki = "Sertralina... Dice mi nombre en la receta de la caja. 'Tomar una cada mañana'. Pero hace días que siento que no me hace nada.";

    [Header("Comportamiento")]
    [SerializeField] private bool obligatorioParaAvanzar = true;
    [SerializeField] private GameObject indicadorResaltado; // Opcional: Sprite de contorno o highlight

    [Header("Eventos")]
    public UnityEvent AlInspeccionar;

    public string Id => idObjeto;
    public string Nombre => nombreVisible;
    public string Pensamiento => pensamientoElaki;
    public bool Obligatorio => obligatorioParaAvanzar;
    public bool HaSidoInspeccionado { get; private set; }

    private void Start()
    {
        if (indicadorResaltado != null)
            indicadorResaltado.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (indicadorResaltado != null)
            indicadorResaltado.SetActive(true);

        GabineteInspeccionUI.Instancia?.MostrarNombreHover(nombreVisible);
    }

    private void OnMouseExit()
    {
        if (indicadorResaltado != null)
            indicadorResaltado.SetActive(false);

        GabineteInspeccionUI.Instancia?.OcultarNombreHover();
    }

    private void OnMouseDown()
    {
        Examinar();
    }

    public void Examinar()
    {
        HaSidoInspeccionado = true;
        AlInspeccionar?.Invoke();
        GabineteInspeccionUI.Instancia?.RegistrarInspeccion(this);
    }
}

