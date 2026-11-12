using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Objeto de una vista fija (gabinete, refrigerador): se resalta al pasar el mouse y se selecciona con clic.
public class ObjetoDeVista : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private VistaDeObjetos vista;
    [SerializeField] private Image figura;
    [SerializeField] private GameObject contorno;

    [Tooltip("Se lanza al seleccionar el objeto, por ejemplo para mostrar su descripción")]
    [SerializeField] private UnityEvent alSeleccionar;

    private bool bajoElMouse;
    private bool seleccionado;

    public UnityEvent AlSeleccionar => alSeleccionar;

    public bool Seleccionado
    {
        get => seleccionado;
        set
        {
            seleccionado = value;
            ActualizarContorno();
        }
    }

    private void Awake()
    {
        // Solo la parte visible del dibujo responde al mouse
        figura.alphaHitTestMinimumThreshold = 0.5f;
    }

    private void OnEnable()
    {
        bajoElMouse = false;
        ActualizarContorno();
    }

    public void OnPointerEnter(PointerEventData datos)
    {
        bajoElMouse = true;
        ActualizarContorno();
    }

    public void OnPointerExit(PointerEventData datos)
    {
        bajoElMouse = false;
        ActualizarContorno();
    }

    public void OnPointerClick(PointerEventData datos)
    {
        if (datos.button != PointerEventData.InputButton.Left)
            return;

        vista.Seleccionar(this);
        alSeleccionar.Invoke();
    }

    private void ActualizarContorno()
    {
        contorno.SetActive(bajoElMouse || seleccionado);
    }
}
