using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Vista fija de la cajonera del baño. Se abre desde un Interactable y se cierra con E o Esc.
// Mientras está abierta, Elaki no se mueve y el escenario no responde.
// Al seleccionar un objeto, su pensamiento sale en la caja de diálogo (clave P5_ + nombre del objeto).
public class VistaGabinete : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [Tooltip("Cuadro anterior de la vista: queda oculto porque el texto ahora sale en la caja de diálogo")]
    [SerializeField] private RectTransform cuadroDialogo;

    [SerializeField] private PlayerController jugador;
    [SerializeField] private InteractionDetector detector;

    [Tooltip("Apagado en el Nivel 1: las etiquetas tienen símbolos que Elaki no puede leer")]
    [SerializeField] private bool mostrarTextos = true;

    [Tooltip("Se lanza al cerrar la vista, por ejemplo para habilitar el espejo")]
    [SerializeField] private UnityEvent alCerrar;

    private InputAction accionInteractuar;
    private InputAction accionCancelar;
    private ObjetoGabinete seleccionado;
    private Coroutine textoSeleccionado;
    private int fotogramaApertura;

    public bool Abierta { get; private set; }
    public UnityEvent AlCerrar => alCerrar;

    private void Awake()
    {
        accionInteractuar = InputSystem.actions.FindAction("Player/Interact", true);
        accionCancelar = InputSystem.actions.FindAction("UI/Cancel", true);
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
        if (Abierta && Time.frameCount > fotogramaApertura && (accionInteractuar.WasPressedThisFrame() || accionCancelar.WasPressedThisFrame()))
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
        OcultarTexto();

        if (objeto == null)
            return;

        objeto.Seleccionado = true;
        MostrarTexto(objeto);
    }

    private void OcultarTexto()
    {
        if (textoSeleccionado == null)
            return;

        StopCoroutine(textoSeleccionado);
        textoSeleccionado = null;
        PantallaJuego.Instancia.Dialogo.Ocultar();
    }

    private void MostrarTexto(ObjetoGabinete objeto)
    {
        if (!mostrarTextos)
            return;

        var caja = PantallaJuego.Instancia.Dialogo;
        var linea = caja.LineaDe(GuionDialogos.ClaveDeGabinete(objeto.name));
        if (!linea.TieneTexto)
            return;

        // La caja va en la mitad contraria para no tapar el objeto
        var arriba = objeto.transform.localPosition.y < 0f;
        textoSeleccionado = StartCoroutine(caja.Mostrar(linea, arriba));
    }

    // La pulsación que cierra la vista no debe volver a abrirla
    private IEnumerator LiberarInteraccion()
    {
        yield return null;
        detector.InteraccionBloqueada = false;
    }
}
