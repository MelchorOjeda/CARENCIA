using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Va en Elaki. Detecta los Interactable cuyo área toca y, si hay varios, elige el más cercano.
// Con E se ejecuta la acción del objeto elegido; la interacción nunca mueve al personaje.
public class InteractionDetector : MonoBehaviour
{
    [Tooltip("Durante persecuciones, cinemáticas o pausa no se puede interactuar")]
    [SerializeField] private bool interaccionBloqueada;

    private readonly List<Interactable> cercanos = new List<Interactable>();
    private InputAction accionInteractuar;

    public Interactable Actual { get; private set; }

    public bool InteraccionBloqueada
    {
        get => interaccionBloqueada;
        set => interaccionBloqueada = value;
    }

    private void Awake()
    {
        accionInteractuar = InputSystem.actions.FindAction("Player/Interact", true);
    }

    private void Update()
    {
        ElegirActual();

        if (Actual != null && accionInteractuar.WasPressedThisFrame())
            Actual.Interactuar();
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        var interactable = otro.GetComponentInParent<Interactable>();
        if (interactable != null && !cercanos.Contains(interactable))
            cercanos.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        var interactable = otro.GetComponentInParent<Interactable>();
        if (interactable == null)
            return;

        cercanos.Remove(interactable);
        interactable.MostrarIndicador(false);
    }

    private void OnDisable()
    {
        CambiarActual(null);
    }

    private void ElegirActual()
    {
        Interactable masCercano = null;

        if (!interaccionBloqueada)
        {
            var menorDistancia = float.MaxValue;
            cercanos.RemoveAll(i => i == null);

            foreach (var interactable in cercanos)
            {
                if (!interactable.Disponible)
                    continue;

                var distancia = ((Vector2)interactable.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    masCercano = interactable;
                }
            }
        }

        CambiarActual(masCercano);
    }

    private void CambiarActual(Interactable nuevo)
    {
        if (Actual == nuevo)
            return;

        if (Actual != null)
            Actual.MostrarIndicador(false);

        Actual = nuevo;

        if (Actual != null)
            Actual.MostrarIndicador(true);
    }
}
