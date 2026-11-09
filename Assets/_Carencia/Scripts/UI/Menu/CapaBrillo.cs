using UnityEngine;
using UnityEngine.UI;

// Capa encima de todo que aclara u oscurece la imagen según el brillo de Opciones (50 = sin cambio).
// Vive entre escenas, así el brillo se respeta en el menú y en cada nivel.
public class CapaBrillo : MonoBehaviour
{
    // El proyecto mezcla en espacio de color lineal: una capa blanca pesa mucho más de lo que parece y una negra
    // mucho menos. Con estos topes, brillo 0 deja la imagen a la mitad y 100 aclara las sombras sin lavarla.
    private const float OscuridadMaxima = 0.78f;
    private const float ClaridadMaxima = 0.04f;

    private static CapaBrillo instancia;
    private Image capa;

    public static CapaBrillo Instancia
    {
        get
        {
            if (instancia == null)
                Crear();

            return instancia;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        instancia = null;
    }

    private static void Crear()
    {
        var objeto = new GameObject("CapaBrillo");
        DontDestroyOnLoad(objeto);
        var canvas = objeto.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;

        var imagen = new GameObject("Capa", typeof(RectTransform));
        imagen.transform.SetParent(objeto.transform, false);
        var rect = (RectTransform)imagen.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;

        instancia = objeto.AddComponent<CapaBrillo>();
        instancia.capa = imagen.AddComponent<Image>();
        instancia.capa.raycastTarget = false;
    }

    public void Aplicar(int brillo)
    {
        var diferencia = (brillo - 50) / 50f;
        capa.color = diferencia < 0f
            ? new Color(0f, 0f, 0f, -diferencia * OscuridadMaxima)
            : new Color(1f, 1f, 1f, diferencia * ClaridadMaxima);
    }
}
