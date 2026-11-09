using TMPro;
using UnityEngine;

// Aplica a un texto el tamaño de texto y el contraste alto de Opciones → Accesibilidad.
[RequireComponent(typeof(TMP_Text))]
public class TextoAccesible : MonoBehaviour
{
    [Tooltip("Color con Contraste alto. Los textos secundarios (grises) lo suben a hueso; los principales lo dejan igual")]
    [SerializeField] private Color colorContrasteAlto = new Color32(0xEC, 0xE6, 0xD8, 0xFF);
    [SerializeField] private bool cambiaConContraste;

    private TMP_Text texto;
    private float tamanoOriginal;
    private Color colorOriginal;

    private void Awake()
    {
        texto = GetComponent<TMP_Text>();
        tamanoOriginal = texto.fontSize;
        colorOriginal = texto.color;
    }

    private void OnEnable()
    {
        AjustesJuego.Cambiaron += Aplicar;
        Aplicar(AjustesJuego.EnPantalla);
    }

    private void OnDisable()
    {
        AjustesJuego.Cambiaron -= Aplicar;
    }

    // Otros scripts cambian el color según el foco; este lo toma como nuevo color base
    public void CambiarColorBase(Color color)
    {
        colorOriginal = color;
        Aplicar(AjustesJuego.EnPantalla);
    }

    private void Aplicar(Ajustes ajustes)
    {
        texto.fontSize = tamanoOriginal * Escala(ajustes.tamanoTexto);
        texto.color = cambiaConContraste && ajustes.contrasteAlto ? colorContrasteAlto : colorOriginal;
    }

    private static float Escala(TamanoTexto tamano)
    {
        switch (tamano)
        {
            case TamanoTexto.Pequeno: return 0.85f;
            case TamanoTexto.Grande: return 1.2f;
            default: return 1f;
        }
    }
}
