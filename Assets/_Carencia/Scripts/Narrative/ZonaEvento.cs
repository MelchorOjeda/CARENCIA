using UnityEngine;

// Área del suelo que dispara una secuencia cuando Elaki está dentro y se cumplen las banderas de esa secuencia.
[RequireComponent(typeof(Collider2D))]
public class ZonaEvento : MonoBehaviour
{
    [SerializeField] private Secuencia secuencia;

    public Secuencia Secuencia
    {
        get => secuencia;
        set => secuencia = value;
    }

    private void OnTriggerStay2D(Collider2D otro)
    {
        if (otro.GetComponentInParent<PlayerController>() != null)
            secuencia.Ejecutar();
    }
}
