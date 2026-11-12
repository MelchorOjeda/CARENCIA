using UnityEngine;

// Mientras este enemigo persigue a Elaki, los bordes de la pantalla se tiñen de rojo, más cuanto más cerca está.
// Se quita solo al terminar la persecución, al desactivar al enemigo o al cambiar de escena.
[RequireComponent(typeof(EnemigoPerseguidor))]
public class AmenazaCercana : MonoBehaviour
{
    [Tooltip("Tinte con el enemigo lejos")]
    [SerializeField] private float minimo = 0.12f;

    [Tooltip("Tinte con el enemigo encima de Elaki")]
    [SerializeField] private float maximo = 0.62f;

    [Tooltip("Desde esta distancia el tinte ya es el mínimo")]
    [SerializeField] private float distanciaLejana = 3.5f;

    private EnemigoPerseguidor enemigo;
    private Transform elaki;
    private bool aplicado;

    private void Awake()
    {
        enemigo = GetComponent<EnemigoPerseguidor>();
    }

    private void Update()
    {
        if (!enemigo.Persiguiendo)
        {
            Quitar();
            return;
        }

        if (elaki == null)
        {
            var jugador = FindAnyObjectByType<PlayerController>();
            if (jugador == null)
                return;

            elaki = jugador.transform;
        }

        var distancia = Vector2.Distance(transform.position, elaki.position);
        PantallaJuego.Instancia.SetAmenaza(Mathf.Lerp(maximo, minimo, distancia / distanciaLejana));
        aplicado = true;
    }

    private void OnDisable()
    {
        Quitar();
    }

    // Solo limpia el tinte que puso este enemigo, para no borrar el de otro que siga persiguiendo
    private void Quitar()
    {
        if (!aplicado)
            return;

        aplicado = false;
        if (PantallaJuego.Existe)
            PantallaJuego.Instancia.SetAmenaza(0f);
    }
}
