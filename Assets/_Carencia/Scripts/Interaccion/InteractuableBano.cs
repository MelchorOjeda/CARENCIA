using UnityEngine;
using UnityEngine.Events;

// Componente interactuable para los objetos del baño (espejo, inodoro, cortina, toallero).
// El toallero soporta 4 toallas y evoluciona visual y narrativamente según la dimensión o nivel:
// En la casa normal (Prólogo) tienen 4 colores vivos; en los niveles alterados se vuelven grises menos una (la de Margarita).
[RequireComponent(typeof(Collider2D))]
public class InteractuableBano : MonoBehaviour
{
    public enum TipoInteractuable
    {
        EspejoGabinete,
        Inodoro,
        CortinaDucha,
        Toallero4Toallas,
        Personalizado
    }

    public enum EstadoDimension
    {
        PrologoRealidad,   // 4 colores vivos
        Nivel1_Ruptura,    // 2 toallas grises, 2 con color
        Nivel2_Deformado,  // Todas grises menos una (Margarita)
        Nivel3_Final       // Baño alterado
    }

    [Header("Configuración")]
    [SerializeField] private TipoInteractuable tipo = TipoInteractuable.EspejoGabinete;
    [SerializeField] private EstadoDimension estadoActual = EstadoDimension.PrologoRealidad;
    [SerializeField] private string textoAccion = "Inspeccionar";
    [TextArea(2, 4)]
    [SerializeField] private string pensamientoElaki;

    [Header("Sprites de Evolución del Toallero")]
    [SerializeField] private Sprite toalleroPrologo;          // 4 toallas con color (Verde, Naranja, Azul, Amarillo)
    [SerializeField] private Sprite toalleroNivel1;           // Padres grises, Elaki y Margarita con color
    [SerializeField] private Sprite toalleroNivel2Margarita;  // Todas grises menos la de Margarita
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoInteraccion;

    [Header("Eventos")]
    public UnityEvent AlInteractuar;

    private bool jugadorCerca;
    private AudioSource audioSource;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && sonidoInteraccion != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        ActualizarEstadoVisual();
    }

    public void ConfigurarEstado(EstadoDimension nuevoEstado)
    {
        estadoActual = nuevoEstado;
        ActualizarEstadoVisual();
    }

    private void ActualizarEstadoVisual()
    {
        if (tipo == TipoInteractuable.Toallero4Toallas && spriteRenderer != null)
        {
            switch (estadoActual)
            {
                case EstadoDimension.PrologoRealidad:
                    if (toalleroPrologo != null) spriteRenderer.sprite = toalleroPrologo;
                    textoAccion = "Mirar toallero";
                    pensamientoElaki = "Cuatro toallas colgadas: verde para papá, naranja para mamá, azul para mí... y la amarilla de Margarita. Si ella nunca existió, ¿por qué su toalla sigue colgada aquí?";
                    break;
                case EstadoDimension.Nivel1_Ruptura:
                    if (toalleroNivel1 != null) spriteRenderer.sprite = toalleroNivel1;
                    textoAccion = "Examinar toallero";
                    pensamientoElaki = "Las toallas de papá y mamá han perdido todo su color, parecen viejas y grises. Pero la mía y la de Margarita siguen intactas.";
                    break;
                case EstadoDimension.Nivel2_Deformado:
                case EstadoDimension.Nivel3_Final:
                    if (toalleroNivel2Margarita != null) spriteRenderer.sprite = toalleroNivel2Margarita;
                    textoAccion = "Examinar toallas desteñidas";
                    pensamientoElaki = "Todas están grises y muertas... menos una. La toalla amarilla de Margarita sigue teniendo color en este baño deformado. Ella es real.";
                    break;
            }
        }
        else if (string.IsNullOrEmpty(pensamientoElaki))
        {
            switch (tipo)
            {
                case TipoInteractuable.Inodoro:
                    textoAccion = "Tirar de la palanca";
                    pensamientoElaki = "El mecanismo de la cisterna hace un ruido sordo... todo sigue igual de silencioso después.";
                    break;
                case TipoInteractuable.CortinaDucha:
                    textoAccion = "Examinar cortina";
                    pensamientoElaki = "La cortina está corrida. Huele a humedad y a jabón seco, pero la bañera está vacía.";
                    break;
                case TipoInteractuable.EspejoGabinete:
                    textoAccion = "Abrir gabinete";
                    pensamientoElaki = "El espejo del baño... detrás está el gabinete con las medicinas.";
                    break;
            }
        }
    }

    private void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            EjecutarInteraccion();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            jugadorCerca = false;
        }
    }

    private void OnMouseDown()
    {
        if (jugadorCerca)
        {
            EjecutarInteraccion();
        }
    }

    public void EjecutarInteraccion()
    {
        if (sonidoInteraccion != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoInteraccion);
        }

        AlInteractuar?.Invoke();

        if (tipo == TipoInteractuable.EspejoGabinete)
        {
            if (GabineteInspeccionUI.Instancia != null)
            {
                GabineteInspeccionUI.Instancia.AbrirGabinete();
            }
        }
        else
        {
            Debug.Log($"[Elaki]: {pensamientoElaki}");
        }
    }
}
