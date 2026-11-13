using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Va en Elaki. Detecta los Interactable cuyo área toca y, si hay varios, elige el más cercano.
// Todo se usa con E o con clic: con E se ejecuta la acción del objeto elegido; con clic izquierdo, la del objeto
// señalado (su dibujo o su indicador) si está al alcance.
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
        var interactable = Apuntado(out var alAlcance);
        if (interactable == null)
            return;

        if (alAlcance)
            interactable.Interactuar();
        else
            alFaltarDistancia.Invoke();
    }

    // Distancia (en unidades) entre el puntero y el indicador de E de un objeto para contar como señalado
    private const float RadioIndicador = 0.45f;

    // El objeto con el que se puede interactuar bajo el puntero: el que tiene su dibujo debajo o, entre los
    // que están al alcance, el que tiene su indicador de E cerca del puntero. alAlcance dice si Elaki está cerca
    public Interactable Apuntado(out bool alAlcance)
    {
        alAlcance = false;
        if (interaccionBloqueada || Camera.main == null || accionPuntero == null)
            return null;

        // Sobre un botón de la interfaz el clic no llega al escenario
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return null;

        Vector2 punto = Camera.main.ScreenToWorldPoint(accionPuntero.ReadValue<Vector2>());

        foreach (var colision in Physics2D.OverlapPointAll(punto))
        {
            var interactable = colision.GetComponentInParent<Interactable>();
            if (interactable == null || !interactable.Disponible || !interactable.AceptaClicEn(punto))
                continue;

            alAlcance = cercanos.Contains(interactable);
            return interactable;
        }

        cercanos.RemoveAll(i => i == null);
        foreach (var interactable in cercanos)
        {
            if (!interactable.Disponible || interactable.Indicador == null)
                continue;

            if (Vector2.Distance(punto, interactable.Indicador.transform.position) <= RadioIndicador)
            {
                alAlcance = true;
                return interactable;
            }
        }

        return null;
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
                // Los que antes eran solo de clic también se usan con E cuando Elaki está cerca
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
