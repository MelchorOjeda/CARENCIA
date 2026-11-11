using System.Collections.Generic;
using UnityEngine;

// Perro de la familia que acompaña a Elaki en la casa normal (prólogo). Recorre el mismo camino que ella, unos pasos detrás, así
// nunca atraviesa muebles ni paredes. Si Elaki aparece lejos (entrada de escena, puerta o secuencia), el perro
// reaparece junto a ella. Choca con paredes y muebles como Elaki, pero no con ella, y no se puede usar.
[RequireComponent(typeof(Rigidbody2D))]
public class PerroAcompanante : MonoBehaviour
{
    private const int PuntosMaximos = 400;
    private const float SeparacionEntrePuntos = 0.05f;

    [Header("Dibujo")]
    [SerializeField] private SpriteRenderer visual;

    [Tooltip("De lado caminando, mirando a la derecha")]
    [SerializeField] private Sprite[] caminarLado = new Sprite[0];

    [Tooltip("De lado quieto, moviendo la cola")]
    [SerializeField] private Sprite[] esperaLado = new Sprite[0];

    [SerializeField] private Sprite[] frente = new Sprite[0];
    [SerializeField] private Sprite[] espalda = new Sprite[0];

    [Header("Seguimiento")]
    [Tooltip("Distancia, a lo largo del camino de Elaki, a la que camina detrás de ella")]
    [SerializeField] private float distanciaDetras = 0.9f;

    [Tooltip("Más rápido que Elaki corriendo (2.6) para no quedarse atrás")]
    [SerializeField] private float velocidadMaxima = 3.5f;

    [Tooltip("Si queda más lejos que esto, reaparece junto a Elaki")]
    [SerializeField] private float distanciaParaReaparecer = 3f;

    [Tooltip("Capas que el perro no puede pisar al reaparecer (paredes y muebles)")]
    [SerializeField] private LayerMask obstaculos = ~0;

    private readonly List<Vector3> camino = new List<Vector3>();
    private PlayerController jugador;
    private Rigidbody2D cuerpoElaki;
    private Rigidbody2D rb;
    private Vector2 ultimaDireccion = new Vector2(1f, -0.3f);
    private float tiempoAnimacion;
    private Vector3 posicionBaseVisual;

    private void Awake()
    {
        // Si se colocó dentro de Elaki, se movería pegado a ella y nunca caminaría ni giraría por su cuenta.
        // Se suelta a la raíz de la escena conservando su posición.
        if (transform.parent != null)
            transform.SetParent(null, true);

        // Cuerpo físico como el de Elaki: las paredes y los muebles lo detienen igual que a ella.
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        jugador = FindAnyObjectByType<PlayerController>();
        if (jugador != null)
        {
            cuerpoElaki = jugador.GetComponent<Rigidbody2D>();

            // El perro no empuja a Elaki ni ella a él.
            foreach (var propio in GetComponentsInChildren<Collider2D>())
            {
                foreach (var deElaki in jugador.GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(propio, deElaki);
            }
        }

        if (visual != null)
            posicionBaseVisual = visual.transform.localPosition;

        ReaparecerJuntoAElaki();
    }

    private void FixedUpdate()
    {
        if (jugador == null)
            return;

        // Elaki se mueve en FixedUpdate, así que su camino se registra al mismo ritmo.
        Vector3 cabeza = cuerpoElaki != null ? cuerpoElaki.position : (Vector2)jugador.transform.position;
        if (camino.Count == 0 || (cabeza - camino[camino.Count - 1]).sqrMagnitude > SeparacionEntrePuntos * SeparacionEntrePuntos)
            camino.Add(cabeza);

        if (camino.Count > PuntosMaximos)
            camino.RemoveRange(0, camino.Count - PuntosMaximos);

        var posicion = PosicionActual();
        if (Vector2.Distance(posicion, cabeza) > distanciaParaReaparecer)
        {
            ReaparecerJuntoAElaki();
            return;
        }

        var destino = (Vector2)PuntoDetras(cabeza, distanciaDetras);
        var paso = Vector2.MoveTowards(posicion, destino, velocidadMaxima * Time.fixedDeltaTime) - posicion;

        if (rb != null)
            rb.linearVelocity = paso / Time.fixedDeltaTime;
        else
            transform.position = posicion + paso;
    }

    private void Update()
    {
        if (jugador == null)
            return;

        // Si Elaki se oculta en una secuencia, el perro también.
        if (visual != null)
            visual.enabled = jugador.gameObject.activeInHierarchy;

        // La velocidad real ya descuenta lo que frenan las paredes: si choca, se queda quieto y no "camina en el aire".
        var velocidad = rb != null ? rb.linearVelocity : Vector2.zero;
        var caminando = velocidad.magnitude > 0.3f;
        if (caminando)
            ultimaDireccion = velocidad.normalized;

        Animar(caminando);
    }

    private Vector2 PosicionActual() => rb != null ? rb.position : (Vector2)transform.position;

    // Lo coloca un paso detrás de hacia donde camina Elaki; si ahí hay una pared, en la misma posición que ella.
    private void ReaparecerJuntoAElaki()
    {
        if (jugador == null)
            return;

        Vector2 posicionElaki = cuerpoElaki != null ? cuerpoElaki.position : (Vector2)jugador.transform.position;
        var direccion = jugador.EstaCaminando ? jugador.Direccion.normalized : ultimaDireccion;
        var detras = posicionElaki - direccion * distanciaDetras;

        foreach (var choque in Physics2D.OverlapCircleAll(detras, 0.15f, obstaculos))
        {
            if (!choque.isTrigger && choque.attachedRigidbody != cuerpoElaki && choque.attachedRigidbody != rb)
            {
                detras = posicionElaki;
                break;
            }
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = detras;
        }

        transform.position = detras;
        camino.Clear();
        camino.Add(detras);
        camino.Add(posicionElaki);
    }

    // Punto del camino recorrido que queda a la distancia indicada detrás de Elaki.
    private Vector3 PuntoDetras(Vector3 cabeza, float distancia)
    {
        var restante = distancia;
        var a = cabeza;
        for (var i = camino.Count - 1; i >= 0; i--)
        {
            var b = camino[i];
            var tramo = Vector3.Distance(a, b);
            if (tramo >= restante)
                return Vector3.Lerp(a, b, restante / Mathf.Max(tramo, 0.00001f));

            restante -= tramo;
            a = b;
        }

        // El camino es más corto que la distancia: se queda donde está para no encimarse a Elaki.
        return Vector3.Distance(transform.position, cabeza) < distancia ? transform.position : a;
    }

    private void Animar(bool caminando)
    {
        if (visual == null)
            return;

        tiempoAnimacion += Time.deltaTime;
        var d = ultimaDireccion;
        var vertical = Mathf.Abs(d.y) > Mathf.Abs(d.x) * 1.6f;

        Sprite[] cuadros;
        float duracionCuadro;
        if (vertical)
        {
            cuadros = d.y > 0f ? espalda : frente;
            duracionCuadro = caminando ? 0.14f : 0.45f;
            visual.flipX = false;
        }
        else
        {
            cuadros = caminando ? caminarLado : esperaLado;
            duracionCuadro = caminando ? 0.09f : 0.4f;
            visual.flipX = d.x < 0f;
        }

        if (cuadros == null || cuadros.Length == 0)
            cuadros = esperaLado;
        if (cuadros == null || cuadros.Length == 0)
            return;

        visual.sprite = cuadros[(int)(tiempoAnimacion / duracionCuadro) % cuadros.Length];

        // De frente o de espaldas no hay ciclo de patas: un leve rebote marca el paso.
        var rebote = caminando && vertical ? Mathf.Abs(Mathf.Sin(tiempoAnimacion * 14f)) * 0.03f : 0f;
        visual.transform.localPosition = posicionBaseVisual + new Vector3(0f, rebote, 0f);
    }
}
