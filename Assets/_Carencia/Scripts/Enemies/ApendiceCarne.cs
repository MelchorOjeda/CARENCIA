using UnityEngine;

// Apéndice que sale de la pared de carne (Nivel 3): descansa dentro, asoma temblando y latiguea a lo largo del piso.
// Mientras está fuera, si toca a Elaki la lastima. El aviso (asomarse) da tiempo de esperar o pasar corriendo.
[RequireComponent(typeof(SpriteRenderer))]
public class ApendiceCarne : MonoBehaviour
{
    private enum Estado { Dentro, Asoma, Sale, Fuera, Regresa }

    // 8 cuadros: 0 dentro, 1-2 asoma, 3-5 sale de golpe, 6-7 fuera ondulando
    [SerializeField] private Sprite[] cuadros = new Sprite[8];
    // Qué tanto del largo alcanza en cada cuadro (para saber si toca a Elaki)
    private static readonly float[] Extension = { 0f, 0.12f, 0.3f, 0.55f, 0.8f, 1f, 1f, 1f };
    // Punto del piso junto a la pared de donde sale y punto del piso hasta donde llega
    [SerializeField] private Vector2 baseEnPiso;
    [SerializeField] private Vector2 puntaEnPiso;
    [SerializeField] private float ancho = 0.2f;
    [SerializeField] private Vector2 descanso = new Vector2(1.6f, 3.2f);
    [SerializeField] private float aviso = 0.6f;
    [SerializeField] private float fuera = 0.9f;
    [SerializeField] private AudioSource latigazo;

    private SpriteRenderer sr;
    private Transform jugador;
    private SaludJugador salud;
    private Estado estado;
    private float tiempo;
    private float duracion;
    private float proximoGolpe;

    public void Configurar(Sprite[] nuevosCuadros, Vector2 desde, Vector2 hasta, AudioSource sonido, float primeraEspera)
    {
        cuadros = nuevosCuadros;
        baseEnPiso = desde;
        puntaEnPiso = hasta;
        latigazo = sonido;
        duracion = primeraEspera;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var control = FindAnyObjectByType<PlayerController>();
        if (control != null)
        {
            jugador = control.transform;
            salud = control.GetComponent<SaludJugador>();
        }

        if (duracion <= 0f)
            duracion = Random.Range(descanso.x, descanso.y);
        Poner(0);
    }

    private void Update()
    {
        tiempo += Time.deltaTime;
        switch (estado)
        {
            case Estado.Dentro:
                Poner(0);
                if (tiempo >= duracion)
                    Cambiar(Estado.Asoma, aviso);
                break;
            case Estado.Asoma:
                // Se asoma temblando: avisa antes de atacar
                Poner(Mathf.Repeat(tiempo, 0.2f) < 0.1f ? 1 : 2);
                if (tiempo >= duracion)
                {
                    Cambiar(Estado.Sale, 0.15f);
                    if (latigazo != null)
                        latigazo.Play();
                }
                break;
            case Estado.Sale:
            {
                // Latigazo: tres cuadros rápidos hasta quedar extendido
                var cuadro = 3 + Mathf.Min(2, (int)(tiempo / 0.05f));
                Poner(cuadro);
                Tocar(Extension[cuadro]);
                if (tiempo >= duracion)
                    Cambiar(Estado.Fuera, fuera);
                break;
            }
            case Estado.Fuera:
            {
                // Afuera ondula como látigo
                var cuadro = 5 + (int)Mathf.Repeat(tiempo / 0.12f, 3f);
                Poner(cuadro);
                Tocar(1f);
                if (tiempo >= duracion)
                    Cambiar(Estado.Regresa, 0.32f);
                break;
            }
            case Estado.Regresa:
            {
                // Se recoge poco a poco hacia la pared
                var cuadro = Mathf.Max(0, 4 - (int)(tiempo / 0.08f));
                Poner(cuadro);
                Tocar(Extension[cuadro]);
                if (tiempo >= duracion)
                    Cambiar(Estado.Dentro, Random.Range(descanso.x, descanso.y));
                break;
            }
        }
    }

    private void Tocar(float extension)
    {
        if (jugador == null || salud == null || Time.time < proximoGolpe)
            return;

        var punta = Vector2.Lerp(baseEnPiso, puntaEnPiso, extension);
        var p = (Vector2)jugador.position;
        var segmento = punta - baseEnPiso;
        var t = Mathf.Clamp01(Vector2.Dot(p - baseEnPiso, segmento) / Mathf.Max(0.0001f, segmento.sqrMagnitude));
        var cercano = baseEnPiso + segmento * t;
        var d = p - cercano;
        // En el piso isométrico una distancia vertical cuenta doble
        if (new Vector2(d.x, d.y * 2f).magnitude < ancho)
        {
            proximoGolpe = Time.time + 1.2f;
            salud.Golpear();
        }
    }

    private void Cambiar(Estado nuevo, float nuevaDuracion)
    {
        estado = nuevo;
        tiempo = 0f;
        duracion = nuevaDuracion;
    }

    private void Poner(int cuadro)
    {
        if (cuadro < cuadros.Length && cuadros[cuadro] != null)
            sr.sprite = cuadros[cuadro];
    }
}
