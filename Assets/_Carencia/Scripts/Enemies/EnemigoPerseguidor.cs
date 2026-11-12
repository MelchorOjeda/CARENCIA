using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Enemigo de persecución: inactivo hasta su evento, busca a Elaki por las celdas libres y la golpea por contacto.
// No se le puede dañar; la persecución termina cuando una secuencia lo indica.
public class EnemigoPerseguidor : MonoBehaviour
{
    private const int LimiteDeBusqueda = 4000;

    private static readonly Vector3Int[] Vecinos =
    {
        Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down,
    };

    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap paredes;
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float alcance = 0.42f;
    [SerializeField] private float esperaTrasGolpe = 1.1f;
    [SerializeField] private float cadaCuantoBusca = 0.25f;

    [Header("Persecución que se intensifica (opcional)")]
    [Tooltip("Cuánto sube la velocidad por segundo mientras persigue; 0 = siempre la misma")]
    [SerializeField] private float aceleracion;

    [Tooltip("Velocidad máxima a la que llega acelerando")]
    [SerializeField] private float velocidadMaxima = 5f;

    [Tooltip("Su primer golpe ya es el último: Elaki no puede escapar de ella")]
    [SerializeField] private bool golpeFinal;

    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");
    private static readonly int Ataque = Animator.StringToHash("Ataque");

    private readonly List<Vector3> ruta = new List<Vector3>();
    private Animator animator;
    private Transform objetivo;
    private SaludJugador salud;
    private bool persiguiendo;
    private float proximaBusqueda;
    private float proximoGolpe;
    private float velocidadActual;

    public bool Persiguiendo => persiguiendo;
    public float VelocidadActual => velocidadActual;

    // Deja de perseguir un momento sin desaparecer ni perder la velocidad que llevaba (al cruzar a otro cuarto)
    public void Pausar()
    {
        persiguiendo = false;
    }

    public void Reanudar()
    {
        if (objetivo == null)
            return;

        persiguiendo = true;
        ruta.Clear();
        proximoGolpe = Mathf.Max(proximoGolpe, Time.time + esperaTrasGolpe * 0.5f);
    }

    public void Configurar(Grid nuevoGrid, Tilemap nuevasParedes, float nuevaVelocidad)
    {
        grid = nuevoGrid;
        paredes = nuevasParedes;
        velocidad = nuevaVelocidad;
    }

    // Cambia la rejilla por la que busca camino, por ejemplo al seguir a Elaki a otro cuarto
    public void CambiarRejilla(Grid nuevoGrid, Tilemap nuevasParedes)
    {
        grid = nuevoGrid;
        paredes = nuevasParedes;
        ruta.Clear();
    }

    public void Iniciar()
    {
        animator = GetComponentInChildren<Animator>();
        var jugador = FindAnyObjectByType<PlayerController>();
        objetivo = jugador.transform;
        salud = jugador.GetComponent<SaludJugador>();
        gameObject.SetActive(true);
        persiguiendo = true;
        velocidadActual = velocidad;
        proximoGolpe = Time.time + esperaTrasGolpe;
    }

    public void Terminar()
    {
        persiguiendo = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!persiguiendo)
            return;

        if (aceleracion > 0f)
            velocidadActual = Mathf.Min(velocidadMaxima, velocidadActual + aceleracion * Time.deltaTime);

        if (Time.time >= proximaBusqueda)
        {
            proximaBusqueda = Time.time + cadaCuantoBusca;
            BuscarRuta();
        }

        Avanzar();

        var distancia = Vector2.Distance(transform.position, objetivo.position);
        if (distancia <= alcance && Time.time >= proximoGolpe)
        {
            proximoGolpe = Time.time + esperaTrasGolpe;
            Mirar(objetivo.position - transform.position);
            if (animator != null)
                animator.SetTrigger(Ataque);

            if (golpeFinal)
                salud.Matar();
            else
                salud.Golpear();
        }
    }

    private void Avanzar()
    {
        // Tras golpear se detiene un momento, para que Elaki pueda alejarse
        if (Time.time < proximoGolpe - esperaTrasGolpe * 0.5f)
            return;

        var destino = ruta.Count > 0 ? ruta[0] : objetivo.position;
        destino.z = transform.position.z;
        var antes = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidadActual * Time.deltaTime);
        Mirar(transform.position - antes);

        if (ruta.Count > 0 && (transform.position - destino).sqrMagnitude < 0.0025f)
            ruta.RemoveAt(0);
    }

    // La animación mira hacia donde se mueve, en las 8 direcciones de la pantalla
    private void Mirar(Vector3 movimiento)
    {
        if (animator == null || movimiento.sqrMagnitude < 0.000001f)
            return;

        var direccion = ((Vector2)movimiento).normalized;
        animator.SetFloat(DirX, direccion.x);
        animator.SetFloat(DirY, direccion.y);
    }

    // Búsqueda por anchura sobre la rejilla: respeta paredes y muebles
    private void BuscarRuta()
    {
        ruta.Clear();
        var inicio = grid.WorldToCell(transform.position);
        var meta = grid.WorldToCell(objetivo.position);
        if (inicio == meta)
            return;

        var padres = new Dictionary<Vector3Int, Vector3Int> { [inicio] = inicio };
        var cola = new Queue<Vector3Int>();
        cola.Enqueue(inicio);

        while (cola.Count > 0 && padres.Count < LimiteDeBusqueda)
        {
            var actual = cola.Dequeue();
            if (actual == meta)
                break;

            foreach (var vecino in Vecinos)
            {
                var siguiente = actual + vecino;
                if (padres.ContainsKey(siguiente))
                    continue;

                if (siguiente != meta && paredes.HasTile(siguiente))
                    continue;

                padres[siguiente] = actual;
                cola.Enqueue(siguiente);
            }
        }

        if (!padres.ContainsKey(meta))
            return;

        var celda = meta;
        while (celda != inicio)
        {
            var centro = grid.GetCellCenterWorld(celda);
            ruta.Insert(0, centro);
            celda = padres[celda];
        }
    }
}
