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

    private readonly List<Vector3> ruta = new List<Vector3>();
    private Transform objetivo;
    private SaludJugador salud;
    private bool persiguiendo;
    private float proximaBusqueda;
    private float proximoGolpe;

    public void Configurar(Grid nuevoGrid, Tilemap nuevasParedes, float nuevaVelocidad)
    {
        grid = nuevoGrid;
        paredes = nuevasParedes;
        velocidad = nuevaVelocidad;
    }

    public void Iniciar()
    {
        var jugador = FindAnyObjectByType<PlayerController>();
        objetivo = jugador.transform;
        salud = jugador.GetComponent<SaludJugador>();
        gameObject.SetActive(true);
        persiguiendo = true;
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
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

        if (ruta.Count > 0 && (transform.position - destino).sqrMagnitude < 0.0025f)
            ruta.RemoveAt(0);
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
