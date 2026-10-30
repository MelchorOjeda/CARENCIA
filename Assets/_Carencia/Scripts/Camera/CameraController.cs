using System.Collections;
using UnityEngine;

// Cámara isométrica ortográfica. En exploración mantiene un encuadre fijo de la habitación;
// si la habitación no cabe en pantalla (por ejemplo el pasillo) sigue a Elaki dentro de los límites.
// Las secuencias narrativas usan Enfocar y VolverAlEncuadre para acercarse y alejarse con suavidad.
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Encuadre habitual")]
    [SerializeField] private float tamanoHabitual = 5f;

    [Tooltip("Desactivado: la cámara se queda donde está colocada en la escena")]
    [SerializeField] private bool seguirObjetivo;

    [SerializeField] private Transform objetivo;

    [Tooltip("Collider que marca el área visible de la habitación; la cámara no muestra nada fuera de él")]
    [SerializeField] private Collider2D limites;

    [Tooltip("Segundos que tarda en alcanzar a Elaki; 0 la mueve sin retraso")]
    [SerializeField] private float suavizado = 0.15f;

    private Camera camara;
    private Vector3 posicionFija;
    private Vector3 velocidad;
    private Coroutine transicion;

    public bool EnSecuencia { get; private set; }

    private void Awake()
    {
        camara = GetComponent<Camera>();
        camara.orthographic = true;
        camara.orthographicSize = tamanoHabitual;
        posicionFija = transform.position;
    }

    private void LateUpdate()
    {
        if (EnSecuencia || !seguirObjetivo)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, PosicionHabitual(), ref velocidad, suavizado);
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
    }

    private Vector3 PosicionHabitual()
    {
        if (!seguirObjetivo || objetivo == null)
            return posicionFija;

        return ConProfundidad(LimitarALaHabitacion(objetivo.position));
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
