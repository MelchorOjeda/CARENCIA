using UnityEngine;

// Apéndice que sale de la pared de carne (Nivel 3): descansa dentro, asoma temblando y latiguea a lo largo del piso.
// Su ritmo es fijo (siempre el mismo ciclo, desfasado entre apéndices) para que se pueda aprender: hay que esperar
// a que se esconda para pasar. Antes de salir, una sombra roja en el piso marca por dónde va a pasar.
// Mientras está fuera, si toca los pies de Elaki la lastima y la empuja hacia el lado del que venía.
[RequireComponent(typeof(SpriteRenderer))]
public class ApendiceCarne : MonoBehaviour
{
    private enum Estado { Dentro, Asoma, Sale, Fuera, Regresa }

    // 8 cuadros: 0 dentro, 1-2 asoma, 3-5 sale de golpe, 6-7 fuera ondulando
    [SerializeField] private Sprite[] cuadros = new Sprite[8];
    // Qué tanto del largo alcanza el dibujo en cada cuadro (medido en los cuadros: punta en x 108, 100, 80, 53, 26, 4, 3, 3)
    private static readonly float[] Extension = { 0f, 0.15f, 0.33f, 0.57f, 0.81f, 1f, 1f, 1f };
    // Punto del piso junto a la pared de donde sale y punto del piso hasta donde llega
    [SerializeField] private Vector2 baseEnPiso;
    [SerializeField] private Vector2 puntaEnPiso;
    [Tooltip("Medio grosor del apéndice junto a la pared y en la punta (en el piso, con la altura contada doble)")]
    [SerializeField] private float anchoBase = 0.2f;
    [SerializeField] private float anchoPunta = 0.07f;

    [Header("Ritmo (segundos, siempre igual)")]
    [SerializeField] private float dentro = 2f;
    [SerializeField] private float aviso = 0.7f;
    [SerializeField] private float sale = 0.15f;
    [SerializeField] private float fuera = 1.3f;
    [SerializeField] private float regresa = 0.32f;
    [SerializeField] private float desfase;

    [Header("Golpe")]
    [SerializeField] private float empuje = 0.45f;
    [SerializeField] private AudioSource latigazo;

    private SpriteRenderer sr;
    private SpriteRenderer sombra;
    private Transform jugador;
    private Rigidbody2D cuerpoJugador;
    private Collider2D piesJugador;
    private SaludJugador salud;
    private Estado estado = Estado.Dentro;
    private float proximoGolpe;
    private int ordenHabitual;
    private Vector2 empujePendiente;

    private float Periodo => dentro + aviso + sale + fuera + regresa;

    public bool Extendido => estado == Estado.Fuera;

    public void Configurar(Sprite[] nuevosCuadros, Vector2 desde, Vector2 hasta, AudioSource sonido, float nuevoDesfase)
    {
        cuadros = nuevosCuadros;
        baseEnPiso = desde;
        puntaEnPiso = hasta;
        latigazo = sonido;
        desfase = nuevoDesfase;
    }

    public void ConfigurarRitmo(float nuevoDentro, float nuevoFuera, float grosor)
    {
        dentro = nuevoDentro;
        fuera = nuevoFuera;
        anchoBase *= grosor;
        anchoPunta *= grosor;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ordenHabitual = sr.sortingOrder;
        CrearSombra();
    }

    private void Start()
    {
        var control = FindAnyObjectByType<PlayerController>();
        if (control != null)
        {
            jugador = control.transform;
            cuerpoJugador = control.GetComponent<Rigidbody2D>();
            salud = control.GetComponent<SaludJugador>();
            foreach (var colision in control.GetComponents<Collider2D>())
                if (!colision.isTrigger)
                    piesJugador = colision;
        }

        Poner(0);
    }

    private void Update()
    {
        var fase = Mathf.Repeat(Time.timeSinceLevelLoad + desfase, Periodo);
        var anterior = estado;
        var extension = 0f;
        var alfaSombra = 0f;

        if (fase < dentro)
        {
            estado = Estado.Dentro;
            Poner(0);
        }
        else if ((fase -= dentro) < aviso)
        {
            // Se asoma temblando y la sombra del piso aparece: avisa antes de atacar (todavía no lastima)
            estado = Estado.Asoma;
            Poner(Mathf.Repeat(fase, 0.2f) < 0.1f ? 1 : 2);
            alfaSombra = Mathf.Lerp(0.15f, 0.6f, fase / aviso) * (Mathf.Repeat(fase, 0.24f) < 0.12f ? 1f : 0.7f);
        }
        else if ((fase -= aviso) < sale)
        {
            // Latigazo: tres cuadros rápidos hasta quedar extendido
            estado = Estado.Sale;
            var cuadro = 3 + Mathf.Min(2, (int)(fase / (sale / 3f)));
            Poner(cuadro);
            extension = Extension[cuadro];
            alfaSombra = 0.6f;
        }
        else if ((fase -= sale) < fuera)
        {
            // Afuera ondula como látigo
            estado = Estado.Fuera;
            Poner(5 + (int)Mathf.Repeat(fase / 0.12f, 3f));
            extension = 1f;
            alfaSombra = 0.6f;
        }
        else
        {
            // Se recoge hacia la pared
            fase -= fuera;
            estado = Estado.Regresa;
            var cuadro = Mathf.Max(0, 4 - (int)(fase / (regresa / 5f)));
            Poner(cuadro);
            extension = Extension[cuadro];
            alfaSombra = 0.6f * (1f - fase / regresa);
        }

        if (estado == Estado.Sale && anterior != Estado.Sale && latigazo != null)
            latigazo.Play();

        if (sombra != null)
            sombra.color = new Color(0.35f, 0f, 0.03f, alfaSombra);

        Ordenar(extension);
        if (extension > 0f)
            Tocar(extension);
    }

    private void FixedUpdate()
    {
        // El empujón se reparte en unos cuantos pasos para que las paredes la detengan
        if (cuerpoJugador == null || empujePendiente.sqrMagnitude < 0.0001f)
            return;

        var paso = Vector2.ClampMagnitude(empujePendiente, 0.07f);
        cuerpoJugador.position += paso;
        empujePendiente -= paso;
    }

    // Pies de Elaki en el espacio del piso: el eje vertical cuenta doble para que la celda isométrica sea cuadrada
    private static Vector2 Plano(Vector2 p) => new Vector2(p.x, p.y * 2f);

    private Vector2 Pies()
    {
        return piesJugador != null ? (Vector2)piesJugador.bounds.center : (Vector2)jugador.position;
    }

    private float RadioPies()
    {
        if (piesJugador == null)
            return 0.1f;

        var mitad = piesJugador.bounds.extents;
        return Mathf.Min(mitad.x, mitad.y * 2f);
    }

    private void Tocar(float extension)
    {
        if (jugador == null || salud == null || Time.time < proximoGolpe)
            return;

        var a = Plano(baseEnPiso);
        var b = Plano(Vector2.Lerp(baseEnPiso, puntaEnPiso, extension));
        var p = Plano(Pies());
        var segmento = b - a;
        var t = Mathf.Clamp01(Vector2.Dot(p - a, segmento) / Mathf.Max(0.0001f, segmento.sqrMagnitude));
        var cercano = a + segmento * t;
        // Más grueso junto a la pared y fino en la punta, como el dibujo
        var largoTotal = (Plano(puntaEnPiso) - a).magnitude;
        var avance = largoTotal > 0f ? (cercano - a).magnitude / largoTotal : 0f;
        var radio = Mathf.Lerp(anchoBase, anchoPunta, avance) + RadioPies();
        var d = p - cercano;
        if (d.magnitude >= radio)
            return;

        proximoGolpe = Time.time + 1.2f;
        salud.Golpear();

        // La empuja de lado, alejándola del apéndice (hacia donde estaba), para que tenga que esperar a que se esconda
        var normal = new Vector2(-segmento.y, segmento.x).normalized;
        var lado = Vector2.Dot(d, normal);
        if (Mathf.Abs(lado) < 0.02f && cuerpoJugador != null)
            lado = -Vector2.Dot(Plano(cuerpoJugador.linearVelocity), normal);
        var direccion = normal * (lado >= 0f ? 1f : -1f);
        // De vuelta al mundo: el eje vertical a la mitad
        empujePendiente = new Vector2(direccion.x, direccion.y * 0.5f).normalized * empuje;
    }

    // Si Elaki queda detrás del apéndice extendido (entre él y la pared), el apéndice se dibuja encima de ella
    private void Ordenar(float extension)
    {
        var encima = false;
        if (extension > 0.3f && jugador != null)
        {
            var a = Plano(baseEnPiso);
            var b = Plano(Vector2.Lerp(baseEnPiso, puntaEnPiso, extension));
            var p = Plano(Pies());
            var segmento = b - a;
            var t = Vector2.Dot(p - a, segmento) / Mathf.Max(0.0001f, segmento.sqrMagnitude);
            if (t > 0f && t < 1f)
            {
                var cercano = Vector2.Lerp(a, b, t);
                // Detrás = más arriba en pantalla que el apéndice en ese punto
                encima = p.y > cercano.y && (p - cercano).magnitude < 1.2f;
            }
        }

        sr.sortingOrder = encima ? ordenHabitual + 20 : ordenHabitual;
    }

    private void CrearSombra()
    {
        if (puntaEnPiso == baseEnPiso)
            return;

        var objeto = new GameObject("SombraAviso");
        objeto.transform.SetParent(transform.parent, false);
        sombra = objeto.AddComponent<SpriteRenderer>();
        var textura = Texture2D.whiteTexture;
        sombra.sprite = Sprite.Create(textura, new Rect(0, 0, textura.width, textura.height), new Vector2(0f, 0.5f), textura.width);
        sombra.sharedMaterial = sr.sharedMaterial;
        sombra.sortingLayerName = "Suelo";
        sombra.sortingOrder = 7;
        var desde = (Vector3)baseEnPiso;
        var hacia = (Vector3)puntaEnPiso - desde;
        objeto.transform.position = desde;
        objeto.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(hacia.y, hacia.x) * Mathf.Rad2Deg);
        objeto.transform.localScale = new Vector3(hacia.magnitude, anchoBase * 0.9f, 1f);
        sombra.color = new Color(0.35f, 0f, 0.03f, 0f);
    }

    private void Poner(int cuadro)
    {
        if (cuadro < cuadros.Length && cuadros[cuadro] != null)
            sr.sprite = cuadros[cuadro];
    }
}
