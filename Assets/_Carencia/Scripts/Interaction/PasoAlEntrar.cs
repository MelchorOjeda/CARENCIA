using UnityEngine;

// Cruza una abertura sin puerta: al entrar Elaki en el área, usa el PuertaPaso del mismo objeto.
[RequireComponent(typeof(PuertaPaso))]
public class PasoAlEntrar : MonoBehaviour
{
    private PuertaPaso paso;

    private void Awake()
    {
        paso = GetComponent<PuertaPaso>();
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.GetComponentInParent<PlayerController>() != null)
            paso.Cruzar();
    }
}
