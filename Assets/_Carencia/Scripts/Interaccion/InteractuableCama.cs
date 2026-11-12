using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Componente interactuable para la Cama de Elaki en su habitación.
/// Soporta cambio dinámico de estado (Hecha, Destendida al levantarse en el Prólogo, Desordenada tras pesadillas)
/// y diálogos/pensamientos interiores contextuales.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InteractuableCama : MonoBehaviour
{
    public enum EstadoCama
    {
        Hecha,                // Cama tendida y ordenada
        DestendidaPrologo,    // Despertar del prólogo (sábanas abiertas, almohada marcada)
        DesordenadaPesadilla, // Tras despertar de pesadilla / mundo deformado
        BaseDesnuda           // Solo estructura y colchón
    }

    [Header("Configuración de Estado")]
    [SerializeField] private EstadoCama estadoActual = EstadoCama.DestendidaPrologo;
    [SerializeField] private string textoAccion = "Mirar cama";

    [Header("Sprites por Estado (48 PPU)")]
    [SerializeField] private Sprite spriteHecha;
    [SerializeField] private Sprite spriteDestendida;
    [SerializeField] private Sprite spriteDesordenada;
    [SerializeField] private Sprite spriteBase;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Pensamientos de Elaki por Estado")]
    [TextArea(2, 3)]
    [SerializeField] private string pensamientoDestendida = "No tengo fuerzas para levantarme... pero mamá insiste con el desayuno.";
    [TextArea(2, 3)]
    [SerializeField] private string pensamientoDesordenada = "Las sábanas están frías y húmedas de sudor... No quiero volver a cerrar los ojos.";
    [TextArea(2, 3)]
    [SerializeField] private string pensamientoHecha = "Alguien la tendió mientras no estaba... pero nadie debería haber entrado.";

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoCrujidoMadera;
    [SerializeField] private AudioClip sonidoRopaCama;

    [Header("Eventos")]
    public UnityEvent AlInteractuar;
    public UnityEvent AlIniciarCinematicaDespertar;

    private bool jugadorCerca;
    private AudioSource audioSource;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        ActualizarVisualEstado();
    }

    /// <summary>
    /// Cambia el estado de la cama y actualiza su sprite y narrativa correspondiente.
    /// </summary>
    public void SetEstado(EstadoCama nuevoEstado)
    {
        estadoActual = nuevoEstado;
        ActualizarVisualEstado();
    }

    public void ActualizarVisualEstado()
    {
        if (spriteRenderer == null) return;

        switch (estadoActual)
        {
            case EstadoCama.Hecha:
                if (spriteHecha != null) spriteRenderer.sprite = spriteHecha;
                break;
            case EstadoCama.DestendidaPrologo:
                if (spriteDestendida != null) spriteRenderer.sprite = spriteDestendida;
                break;
            case EstadoCama.DesordenadaPesadilla:
                if (spriteDesordenada != null) spriteRenderer.sprite = spriteDesordenada;
                break;
            case EstadoCama.BaseDesnuda:
                if (spriteBase != null) spriteRenderer.sprite = spriteBase;
                break;
        }
    }

    public string ObtenerPensamientoActual()
    {
        switch (estadoActual)
        {
            case EstadoCama.DestendidaPrologo:
                return pensamientoDestendida;
            case EstadoCama.DesordenadaPesadilla:
                return pensamientoDesordenada;
            case EstadoCama.Hecha:
            default:
                return pensamientoHecha;
        }
    }

    private void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Interactuar();
        }
    }

    public void Interactuar()
    {
        if (sonidoCrujidoMadera != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoCrujidoMadera);
        }

        Debug.Log($"[Cama Elaki] {textoAccion} ({estadoActual}): \"{ObtenerPensamientoActual()}\"");
        AlInteractuar?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.name.Contains("Elaki") || collision.name.Contains("Protagonista"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.name.Contains("Elaki") || collision.name.Contains("Protagonista"))
        {
            jugadorCerca = false;
        }
    }
}

