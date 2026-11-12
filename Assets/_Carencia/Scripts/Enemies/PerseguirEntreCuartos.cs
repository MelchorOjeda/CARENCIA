using System;
using System.Collections;
using PortalesGame;
using UnityEngine;
using UnityEngine.Tilemaps;

// Para un enemigo de un nivel hecho por cuartos (Nivel 2): si Elaki cruza a otro cuarto mientras la persigue,
// el enemigo desaparece de la vista, tarda lo que tardaría en llegar a la abertura y entra por ella al cuarto nuevo.
[RequireComponent(typeof(EnemigoPerseguidor))]
public class PerseguirEntreCuartos : MonoBehaviour
{
    [Serializable]
    public class Cuarto
    {
        public Room habitacion;
        public Grid grid;

        [Tooltip("Celdas que el enemigo no puede pisar: paredes y muebles")]
        public Tilemap bloqueos;

        [Tooltip("Abertura de este cuarto: por ahí sale y por ahí entra")]
        public Transform llegada;
    }

    [SerializeField] private Cuarto[] cuartos = new Cuarto[0];

    [Tooltip("Segundos extra que tarda en cruzar la abertura detrás de Elaki")]
    [SerializeField] private float retraso = 0.9f;

    [Tooltip("Cuarto donde está al empezar la persecución")]
    [SerializeField] private Room cuartoInicial;

    private EnemigoPerseguidor enemigo;
    private SpriteRenderer[] dibujos;
    private Cuarto donde;
    private Coroutine cruce;

    private void Awake()
    {
        enemigo = GetComponent<EnemigoPerseguidor>();
        dibujos = GetComponentsInChildren<SpriteRenderer>(true);
        donde = Buscar(cuartoInicial);
    }

    private void OnEnable()
    {
        if (RoomManager.Instance != null)
            RoomManager.Instance.RoomEntered += AlEntrarCuarto;
    }

    private void OnDisable()
    {
        if (RoomManager.Instance != null)
            RoomManager.Instance.RoomEntered -= AlEntrarCuarto;
    }

    private void AlEntrarCuarto(Room habitacion, string entrada)
    {
        var destino = Buscar(habitacion);
        if (destino == null || (!enemigo.Persiguiendo && cruce == null))
            return;

        if (cruce != null)
            StopCoroutine(cruce);

        // Elaki volvió al cuarto donde sigue el enemigo: reaparece donde estaba y sigue
        if (destino == donde)
        {
            Mostrar(true);
            enemigo.Reanudar();
            cruce = null;
            return;
        }

        cruce = StartCoroutine(Cruzar(destino));
    }

    private IEnumerator Cruzar(Cuarto destino)
    {
        var espera = retraso;
        if (donde != null && donde.llegada != null)
            espera += Vector2.Distance(transform.position, donde.llegada.position) / Mathf.Max(0.5f, enemigo.VelocidadActual);

        enemigo.Pausar();
        Mostrar(false);
        yield return new WaitForSeconds(espera);

        if (destino.llegada != null)
            transform.position = destino.llegada.position;

        enemigo.CambiarRejilla(destino.grid, destino.bloqueos);
        donde = destino;
        Mostrar(true);
        enemigo.Reanudar();
        cruce = null;
    }

    private void Mostrar(bool visible)
    {
        foreach (var dibujo in dibujos)
        {
            if (dibujo != null)
                dibujo.enabled = visible;
        }
    }

    private Cuarto Buscar(Room habitacion)
    {
        if (habitacion == null)
            return null;

        foreach (var cuarto in cuartos)
        {
            if (cuarto.habitacion == habitacion)
                return cuarto;
        }

        return null;
    }
}
