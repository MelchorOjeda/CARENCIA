using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Cámara isométrica ortográfica. En exploración va cerca de Elaki y la sigue, sin salirse de los límites;
// así en el pasillo solo se ve la puerta de donde salió y un poco más.
// Las secuencias narrativas usan Enfocar y VolverAlEncuadre para acercarse y alejarse con suavidad.
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Encuadre habitual")]
    [Tooltip("1.875 equivale a una vista de 320 x 180 px con 48 PPU")]
    [SerializeField] private float tamanoHabitual = 1.875f;

    [Tooltip("Desactivado: la cámara se queda donde está colocada en la escena")]
    [SerializeField] private bool seguirObjetivo = true;

    [SerializeField] private Transform objetivo;

    [Tooltip("Unidades por encima de los pies del objetivo, para que quepa la pared de atrás")]
    [SerializeField] private float alturaSobreObjetivo = 1.25f;

    [Tooltip("Collider que marca el área visible de la habitación; la cámara no muestra nada fuera de él")]
    [SerializeField] private Collider2D limites;

    [Tooltip("Segundos que tarda en alcanzar a Elaki; 0 la mueve sin retraso")]
    [SerializeField] private float suavizado = 0.15f;

    private const float DistanciaDeSalto = 2f;

    private Camera camara;
    private PixelPerfectCamera pixelPerfect;
    private Vector3 posicionFija;
    private Vector3 velocidad;
    private Coroutine transicion;

    public bool EnSecuencia { get; private set; }

    private void Awake()
    {
        camara = GetComponent<Camera>();
        pixelPerfect = GetComponent<PixelPerfectCamera>();
        camara.orthographic = true;
        camara.orthographicSize = tamanoHabitual;
        posicionFija = transform.position;
    }

    private void Start()
    {
        // Al cargar la escena empieza ya sobre Elaki, sin recorrer la habitación
        transform.position = PosicionHabitual();
    }

    private void LateUpdate()
    {
        if (EnSecuencia || !seguirObjetivo)
            return;

        var destino = PosicionHabitual();

        // Si Elaki cambió de espacio, la cámara salta en lugar de recorrer la distancia
        if ((destino - transform.position).sqrMagnitude > DistanciaDeSalto * DistanciaDeSalto)
        {
            transform.position = destino;
            velocidad = Vector3.zero;
            return;
        }

        transform.position = Vector3.SmoothDamp(transform.position, destino, ref velocidad, suavizado);
    }

    // Acerca la cámara a un punto, por ejemplo el monitor, hasta el tamaño indicado.
    // Un tamaño mayor que el actual aleja la cámara: sirve para revelar algo poco a poco.
    public void Enfocar(Transform punto, float tamano, float duracion)
    {
        IniciarTransicion(() => punto.position, tamano, duracion, true);
    }

    public void VolverAlEncuadre(float duracion)
    {
        IniciarTransicion(PosicionHabitual, tamanoHabitual, duracion, false);
    }

    private void IniciarTransicion(System.Func<Vector3> destino, float tamano, float duracion, bool quedarseEnSecuencia)
    {
        if (transicion != null)
            StopCoroutine(transicion);

        transicion = StartCoroutine(Transicion(destino, tamano, duracion, quedarseEnSecuencia));
    }

    private IEnumerator Transicion(System.Func<Vector3> destino, float tamano, float duracion, bool quedarseEnSecuencia)
    {
        EnSecuencia = true;

        // El Pixel Perfect Camera fija el tamaño y anularía el acercamiento
        if (pixelPerfect != null)
            pixelPerfect.enabled = false;

        var posicionInicial = transform.position;
        var tamanoInicial = camara.orthographicSize;

        for (var t = 0f; t < duracion; t += Time.deltaTime)
        {
            var avance = Mathf.SmoothStep(0f, 1f, t / duracion);
            transform.position = Vector3.Lerp(posicionInicial, ConProfundidad(destino()), avance);
            camara.orthographicSize = Mathf.Lerp(tamanoInicial, tamano, avance);
            yield return null;
        }

        transform.position = ConProfundidad(destino());
        camara.orthographicSize = tamano;
        velocidad = Vector3.zero;
        transicion = null;
        EnSecuencia = quedarseEnSecuencia;

        if (pixelPerfect != null && !quedarseEnSecuencia)
            pixelPerfect.enabled = true;
    }

    private Vector3 PosicionHabitual()
    {
        if (!seguirObjetivo || objetivo == null)
            return posicionFija;

        return ConProfundidad(LimitarALaHabitacion(objetivo.position + Vector3.up * alturaSobreObjetivo));
    }

    private Vector3 LimitarALaHabitacion(Vector3 posicion)
    {
        if (limites == null)
            return posicion;

        var area = limites.bounds;
        var medioAlto = tamanoHabitual;
        var medioAncho = tamanoHabitual * camara.aspect;

        posicion.x = area.size.x > medioAncho * 2f
            ? Mathf.Clamp(posicion.x, area.min.x + medioAncho, area.max.x - medioAncho)
            : area.center.x;
        posicion.y = area.size.y > medioAlto * 2f
            ? Mathf.Clamp(posicion.y, area.min.y + medioAlto, area.max.y - medioAlto)
            : area.center.y;

        return posicion;
    }

    private Vector3 ConProfundidad(Vector3 posicion)
    {
        posicion.z = posicionFija.z;
        return posicion;
    }
}
