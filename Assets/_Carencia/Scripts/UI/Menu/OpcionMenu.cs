using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Una opción elegible de un menú o de una nota: muestra el foco (rasguño, color y sombras) y avisa del mouse.
// Deshabilitada se ve tachada y transparente, y no responde al mouse.
public class OpcionMenu : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private const float AlfaDeshabilitada = 0.38f;

    [SerializeField] private TMP_Text etiqueta;
    [SerializeField] private Image rasguno;
    [Tooltip("Copias roja y azul detrás del texto enfocado; pueden quedar vacías")]
    [SerializeField] private GameObject sombras;
    [SerializeField] private bool subrayarAlEnfocar;
    [SerializeField] private Color colorEnfocada = new Color32(0xEC, 0xE6, 0xD8, 0xFF);
    [SerializeField] private Color colorNormal = new Color(0.541f, 0.608f, 0.659f, 0.8f);

    public event Action<OpcionMenu> Apuntada;
    public event Action<OpcionMenu> Pulsada;

    public bool Habilitada { get; private set; } = true;

    public void CambiarTexto(string texto)
    {
        etiqueta.text = texto;
        if (sombras == null)
            return;

        foreach (var copia in sombras.GetComponentsInChildren<TMP_Text>(true))
            copia.text = texto;
    }

    public void SetHabilitada(bool habilitada)
    {
        Habilitada = habilitada;
        etiqueta.fontStyle = habilitada ? FontStyles.Normal : FontStyles.Strikethrough;
        MostrarEnfoque(false);
    }

    public void MostrarEnfoque(bool enfocada)
    {
        rasguno.enabled = enfocada;
        if (sombras != null)
            sombras.SetActive(enfocada);

        var color = enfocada ? colorEnfocada : colorNormal;
        if (!Habilitada)
            color.a *= AlfaDeshabilitada;

        etiqueta.color = color;
        if (subrayarAlEnfocar)
            etiqueta.fontStyle = enfocada ? FontStyles.Underline : FontStyles.Normal;
    }

    public void OnPointerEnter(PointerEventData evento)
    {
        if (Habilitada)
            Apuntada?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData evento)
    {
        if (Habilitada && evento.button == PointerEventData.InputButton.Left)
            Pulsada?.Invoke(this);
    }
}
