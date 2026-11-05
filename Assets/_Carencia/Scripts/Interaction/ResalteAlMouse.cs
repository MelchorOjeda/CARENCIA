using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Muestra el contorno de un objeto del escenario mientras el mouse está sobre su parte visible.
// Si dos objetos se encuentran bajo el mouse, solo se resalta el que está más al frente.
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
        contorno.enabled = EstaBajoElMouse();
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
