using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Va en Elaki. Detecta los Interactable cuyo área toca y, si hay varios, elige el más cercano.
// Con E se ejecuta la acción del objeto elegido; con clic izquierdo, la del objeto señalado si está al alcance.
// La interacción nunca mueve al personaje.
public class InteractionDetector : MonoBehaviour
{
    [Tooltip("Durante persecuciones, cinemáticas o pausa no se puede interactuar")]
    [SerializeField] private bool interaccionBloqueada;

    [Tooltip("Se lanza al hacer clic en un objeto fuera de alcance, para mostrar el aviso de acercarse")]
    [SerializeField] private UnityEvent alFaltarDistancia;

    private readonly List<Interactable> cercanos = new List<Interactable>();
    private InputAction accionInteractuar;
    private InputAction accionClic;
    private InputAction accionPuntero;

    public Interactable Actual { get; private set; }

    public bool InteraccionBloqueada
    {
        get => interaccionBloqueada;
        set => interaccionBloqueada = value;
    }

    private void Awake()
    {
        accionInteractuar = InputSystem.actions.FindAction("Player/Interact", true);
        accionClic = InputSystem.actions.FindAction("UI/Click", true);
        accionPuntero = InputSystem.actions.FindAction("UI/Point", true);
    }

    private void Update()
    {
        ElegirActual();

        if (Actual != null && accionInteractuar.WasPressedThisFrame())
            Actual.Interactuar();
        else if (accionClic.WasPressedThisFrame())
            InteractuarConClic();
    }

    private void InteractuarConClic()
    {
        if (interaccionBloqueada || Camera.main == null)
            return;

        // Un clic sobre un botón de la interfaz no debe llegar al escenario
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        var punto = Camera.main.ScreenToWorldPoint(accionPuntero.ReadValue<Vector2>());

        foreach (var colision in Physics2D.OverlapPointAll(punto))
        {
            var interactable = colision.GetComponentInParent<Interactable>();
            if (interactable == null || !interactable.Disponible)
                continue;

            if (cercanos.Contains(interactable))
                interactable.Interactuar();
            else
                alFaltarDistancia.Invoke();

            return;
        }
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
