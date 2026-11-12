using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Vista fija de un mueble con objetos (el gabinete del baño, el refrigerador). Se abre desde una Secuencia
// y se cierra con E o Esc. Mientras está abierta, Elaki no se mueve y el escenario no responde.
// Al seleccionar un objeto, su pensamiento sale en la caja de diálogo (clave = prefijo de la vista + nombre del objeto).
public class VistaDeObjetos : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [Tooltip("Cuadro anterior de la vista: queda oculto porque el texto ahora sale en la caja de diálogo. Vacío en vistas nuevas")]
    [SerializeField] private RectTransform cuadroDialogo;

    [Tooltip("Va antes del nombre de cada objeto para formar su clave en el guion: P5_ en el gabinete, REF_ en el refrigerador")]
    [SerializeField] private string prefijoClave = GuionDialogos.PrefijoGabinete;

    [SerializeField] private PlayerController jugador;
    [SerializeField] private InteractionDetector detector;

    [Tooltip("Apagado en el gabinete del Nivel 1: las etiquetas tienen símbolos que Elaki no puede leer")]
    [SerializeField] private bool mostrarTextos = true;

    [Tooltip("Se lanza al cerrar la vista, por ejemplo para habilitar el espejo")]
    [SerializeField] private UnityEvent alCerrar;

    private InputAction accionInteractuar;
    private InputAction accionCancelar;
    private ObjetoDeVista seleccionado;
    private Coroutine textoSeleccionado;
    private int fotogramaApertura;

    public bool Abierta { get; private set; }
    public UnityEvent AlCerrar => alCerrar;

    private void Awake()
    {
        accionInteractuar = InputSystem.actions.FindAction("Player/Interact", true);
        accionCancelar = InputSystem.actions.FindAction("UI/Cancel", true);
        panel.SetActive(false);
        if (cuadroDialogo != null)
            cuadroDialogo.gameObject.SetActive(false);
    }

    // Cambia el color de toda la vista, para las versiones alteradas del mueble
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

    public void Seleccionar(ObjetoDeVista objeto)
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

    private void MostrarTexto(ObjetoDeVista objeto)
    {
        if (!mostrarTextos)
            return;

        var caja = PantallaJuego.Instancia.Dialogo;
        var linea = caja.LineaDe(GuionDialogos.ClaveDeObjeto(prefijoClave, objeto.name));
        if (!linea.TieneTexto)
            return;

        textoSeleccionado = StartCoroutine(caja.Mostrar(linea));
    }

    // La pulsación que cierra la vista no debe volver a abrirla
    private IEnumerator LiberarInteraccion()
    {
        yield return null;
        detector.InteraccionBloqueada = false;
    }
}
