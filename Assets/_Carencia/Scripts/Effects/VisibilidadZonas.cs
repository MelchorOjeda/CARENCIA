using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Muestra solo la zona donde está el jugador y las conectadas a ella; las demás se funden con el fondo.
// Sirve para que en un laberinto no se vean los cuartos vecinos por encima de los muros.
public class VisibilidadZonas : MonoBehaviour
{
    [Serializable]
    public class Zona
    {
        public string nombre;
        public RectInt[] areas;
        public Tilemap[] capas;
        public SpriteRenderer[] figuras = new SpriteRenderer[0];
        public int[] vecinas;
        // Un paso corto deja ver lo que hay al otro lado
        public bool paso;
    }

    [SerializeField] private Zona[] zonas = new Zona[0];
    [SerializeField] private float duracionFundido = 0.3f;

    private Grid grid;
    private Transform jugador;
    private float[] alfa;
    private bool[] visible;
    private int actual = -1;

    public void Configurar(Zona[] nuevas)
    {
        zonas = nuevas;
    }

    public bool EsVisible(string nombre)
    {
        var i = Array.FindIndex(zonas, z => z.nombre == nombre);
        return i >= 0 && alfa != null && alfa[i] > 0.01f;
    }

    private void Start()
    {
        grid = GetComponentInParent<Grid>();
        var controlador = FindAnyObjectByType<PlayerController>();
        jugador = controlador != null ? controlador.transform : null;
        alfa = new float[zonas.Length];
        visible = new bool[zonas.Length];
        Actualizar(true);
    }

    private void Update()
    {
        Actualizar(false);
    }

    private void Actualizar(bool inmediato)
    {
        if (jugador == null || grid == null)
            return;

        var celda = (Vector2Int)grid.WorldToCell(jugador.position);
        var zona = Buscar(celda);
        if (zona >= 0 && zona != actual)
        {
            actual = zona;
            MarcarVisibles();
        }

        var paso = inmediato || duracionFundido <= 0f ? 1f : Time.deltaTime / duracionFundido;
        for (var i = 0; i < zonas.Length; i++)
        {
            var meta = visible[i] ? 1f : 0f;
            if (Mathf.Approximately(alfa[i], meta) && !inmediato)
                continue;

            alfa[i] = Mathf.MoveTowards(alfa[i], meta, paso);
            foreach (var capa in zonas[i].capas)
            {
                var color = capa.color;
                color.a = alfa[i];
                capa.color = color;
                capa.GetComponent<TilemapRenderer>().enabled = alfa[i] > 0.01f;
            }

            foreach (var figura in zonas[i].figuras)
            {
                if (figura == null)
                    continue;

                var color = figura.color;
                color.a = alfa[i];
                figura.color = color;
                figura.enabled = alfa[i] > 0.01f;
            }
        }
    }

    private int Buscar(Vector2Int celda)
    {
        for (var i = 0; i < zonas.Length; i++)
        {
            foreach (var area in zonas[i].areas)
            {
                if (area.Contains(celda))
                    return i;
            }
        }

        return -1;
    }

    private void MarcarVisibles()
    {
        Array.Clear(visible, 0, visible.Length);
        var pendientes = new Queue<(int zona, bool sigue)>();
        pendientes.Enqueue((actual, true));
        while (pendientes.Count > 0)
        {
            var (zona, sigue) = pendientes.Dequeue();
            if (visible[zona] && zona != actual)
                continue;

            visible[zona] = true;
            if (!sigue)
                continue;

            foreach (var vecina in zonas[zona].vecinas)
            {
                if (!visible[vecina])
                    pendientes.Enqueue((vecina, zonas[vecina].paso));
            }
        }
    }
}
