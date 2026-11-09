using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Una categoría de Opciones (Audio, Video…): subrayada en rojo cuando está activa; se elige con Q/E o con clic.
public class PestanaOpciones : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text etiqueta;
    [SerializeField] private Image subrayado;
    [SerializeField] private Color colorActiva = new Color32(0x1B, 0x1B, 0x1E, 0xFF);
    [SerializeField] private Color colorInactiva = new Color32(0x5A, 0x5A, 0x60, 0xFF);

    public event Action<PestanaOpciones> Pulsada;

    public void MostrarActiva(bool activa)
    {
        etiqueta.color = activa ? colorActiva : colorInactiva;
        subrayado.enabled = activa;
    }

    public void OnPointerClick(PointerEventData evento)
    {
        if (evento.button == PointerEventData.InputButton.Left)
            Pulsada?.Invoke(this);
    }
}
