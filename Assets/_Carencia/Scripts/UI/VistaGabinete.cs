using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Vista fija de la cajonera del baño. Se abre desde un Interactable y se cierra con E.
// Mientras está abierta, Elaki no se mueve y el escenario no responde.
public class VistaGabinete : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform cuadroDialogo;
    [SerializeField] private PlayerController jugador;
    [SerializeField] private InteractionDetector detector;

    [Tooltip("Distancia vertical del cuadro al centro de la pantalla, en píxeles de la vista")]
    [SerializeField] private float alturaCuadro = 136f;

    [Tooltip("Se lanza al cerrar la vista, por ejemplo para habilitar el espejo")]
    [SerializeField] private UnityEvent alCerrar;

    private InputAction accionInteractuar;
    private ObjetoGabinete seleccionado;
    private int fotogramaApertura;

    public bool Abierta { get; private set; }
    public UnityEvent AlCerrar => alCerrar;

    private void Awake()
    {
        accionInteractuar = InputSystem.actions.FindAction("Player/Interact", true);
        panel.SetActive(false);
        cuadroDialogo.gameObject.SetActive(false);
    }

    // Cambia el color de toda la vista, para las versiones alteradas de la cajonera
    public void SetTinte(Color tinte)
    {
        foreach (var imagen in panel.GetComponentsInChildren<UnityEngine.UI.Image>(true))
            imagen.color = tinte;
    }

    private void Update()
    {
        // La misma pulsación que abre la vista no debe cerrarla
        if (Abierta && Time.frameCount > fotogramaApertura && accionInteractuar.WasPressedThisFrame())
            Cerrar();
    }

    public void Abrir()
    {
        if (Abierta)
            return;

        Abierta = true;
        fotogramaApertura = Time.frameCount;
        jugador.MovimientoBloqueado = true;
        detector.InteraccionBloqueada = true;
        panel.SetActive(true);
    }

    public void Cerrar()
    {
        if (!Abierta)
            return;

        Abierta = false;
        Seleccionar(null);
        panel.SetActive(false);
        jugador.MovimientoBloqueado = false;
        StartCoroutine(LiberarInteraccion());
        alCerrar.Invoke();
    }

    public void Seleccionar(ObjetoGabinete objeto)
    {
        if (seleccionado != null)
            seleccionado.Seleccionado = false;

        seleccionado = objeto;
        cuadroDialogo.gameObject.SetActive(objeto != null);

        if (objeto == null)
            return;

        objeto.Seleccionado = true;

        // El cuadro se coloca en la mitad contraria para no tapar el objeto
        var abajo = objeto.transform.localPosition.y >= 0f;
        cuadroDialogo.anchoredPosition = new Vector2(0f, abajo ? -alturaCuadro : alturaCuadro);
    }

    // La pulsación que cierra la vista no debe volver a abrirla
    private IEnumerator LiberarInteraccion()
    {
        yield return null;
        detector.InteraccionBloqueada = false;
    }
}
