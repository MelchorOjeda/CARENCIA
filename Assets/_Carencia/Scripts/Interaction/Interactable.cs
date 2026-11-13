using UnityEngine;
using UnityEngine.Events;

// Objeto con el que Elaki puede interactuar al acercarse y pulsar E.
// Necesita un Collider2D marcado como Trigger que define su área de detección.
[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    [Tooltip("Texto de la acción que muestra el indicador, por ejemplo Inspeccionar o Abrir")]
    [SerializeField] private string accion = "Inspeccionar";

    [Tooltip("Indicador visual que se activa mientras Elaki está en el área")]
    [SerializeField] private GameObject indicador;

    [Tooltip("Los eventos narrativos de una sola ejecución no se repiten")]
    [SerializeField] private bool unaSolaVez;

    [SerializeField] private bool bloqueado;

    [Tooltip("Solo se usa con clic: E no lo activa y no muestra indicador")]
    [SerializeField] private bool soloConClic;

    [Tooltip("Parte del dibujo donde se puede hacer clic. Si está vacío, sirve cualquiera de sus áreas")]
    [SerializeField] private Collider2D zonaClic;

    [SerializeField] private UnityEvent alInteractuar;

    public UnityEvent AlInteractuar => alInteractuar;
    public bool SoloConClic => soloConClic;
    public string Accion => accion;
    public GameObject Indicador => indicador;
    public bool Completado { get; private set; }

    public bool Bloqueado
    {
        get => bloqueado;
        set
        {
            bloqueado = value;
            if (bloqueado)
                MostrarIndicador(false);
        }
    }

    public bool Disponible => isActiveAndEnabled && !bloqueado && !(unaSolaVez && Completado);

    private void Awake()
    {
        MostrarIndicador(false);
    }

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public bool AceptaClicEn(Vector2 punto)
    {
        return zonaClic == null || zonaClic.OverlapPoint(punto);
    }

    public void MostrarIndicador(bool visible)
    {
        if (indicador != null)
            indicador.SetActive(visible && Disponible);
    }

    public void Interactuar()
    {
        if (!Disponible)
            return;

        Completado = true;
        alInteractuar.Invoke();

        if (!Disponible)
            MostrarIndicador(false);
    }
}
