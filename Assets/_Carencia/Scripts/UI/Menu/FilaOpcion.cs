using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Un renglón de la hoja de Opciones: nombre a la izquierda; a la derecha un deslizador o un valor.
// Con el mouse, un clic a la izquierda de la marca (o del centro del valor) baja y a la derecha sube.
public class FilaOpcion : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private TMP_Text etiqueta;
    [SerializeField] private TMP_Text valor;
    [SerializeField] private Image rasguno;
    [SerializeField] private Image fondoEnfoque;
    [SerializeField] private RectTransform deslizador;
    [SerializeField] private RectTransform relleno;
    [SerializeField] private RectTransform marca;

    public event Action<FilaOpcion> Apuntada;
    public event Action<FilaOpcion, int> Ajustada;

    public void Mostrar(string nombre, string textoValor, int? porcentaje)
    {
        etiqueta.text = nombre;
        valor.text = textoValor;
        deslizador.gameObject.SetActive(porcentaje.HasValue);
        if (porcentaje.HasValue)
            MoverMarca(porcentaje.Value / 100f);
    }

    public void MostrarEnfoque(bool enfocada)
    {
        rasguno.enabled = enfocada;
        fondoEnfoque.enabled = enfocada;
    }

    public void OnPointerEnter(PointerEventData evento)
    {
        Apuntada?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData evento)
    {
        if (evento.button != PointerEventData.InputButton.Left)
            return;

        var referencia = deslizador.gameObject.activeSelf ? marca : (RectTransform)valor.transform;
        var centro = RectTransformUtility.WorldToScreenPoint(evento.pressEventCamera, referencia.position).x;
        Ajustada?.Invoke(this, evento.position.x < centro ? -1 : +1);
    }

    private void MoverMarca(float fraccion)
    {
        var ancho = deslizador.rect.width;
        relleno.sizeDelta = new Vector2(ancho * fraccion, relleno.sizeDelta.y);
        marca.anchoredPosition = new Vector2(ancho * fraccion, marca.anchoredPosition.y);
    }
}
