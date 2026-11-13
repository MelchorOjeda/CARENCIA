using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Antes mostraba un contorno amarillo sobre el objeto bajo el mouse. Ya no se muestra: ahora lo indica
// el cursor espiral (morado sobre algo interactuable). Se conserva para no romper las escenas que lo usan.
public class ResalteAlMouse : MonoBehaviour
{
    private static readonly List<ResalteAlMouse> Activos = new List<ResalteAlMouse>();

    [SerializeField] private SpriteRenderer contorno;
    [SerializeField] private Collider2D zona;
    [SerializeField] private InteractionDetector detector;

    private InputAction accionPuntero;

    private void Awake()
    {
        accionPuntero = InputSystem.actions.FindAction("UI/Point", true);
    }

    private void OnEnable()
    {
        Activos.Add(this);
    }

    private void OnDisable()
    {
        Activos.Remove(this);
        contorno.enabled = false;
    }

    private void Update()
    {
        if (contorno != null && contorno.enabled)
            contorno.enabled = false;
    }

    private bool EstaBajoElMouse()
    {
        if (Camera.main == null || detector.InteraccionBloqueada)
            return false;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return false;

        Vector2 punto = Camera.main.ScreenToWorldPoint(accionPuntero.ReadValue<Vector2>());
        if (!zona.OverlapPoint(punto))
            return false;

        foreach (var otro in Activos)
        {
            if (otro != this && otro.transform.position.y < transform.position.y && otro.zona.OverlapPoint(punto))
                return false;
        }

        return true;
    }
}
