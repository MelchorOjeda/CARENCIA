using UnityEngine;

// Cuenta las veces que Elaki entra al área y, al llegar al número indicado, ejecuta una secuencia.
// En el laberinto del Nivel 1 marca la tercera vuelta al cuarto de lavado.
[RequireComponent(typeof(Collider2D))]
public class ContarEntradas : MonoBehaviour
{
    [SerializeField] private Secuencia secuencia;
    [SerializeField] private int entradas = 3;

    private int cuenta;
    // Elaki puede tener varios colliders: una entrada cuenta solo cuando antes no tocaba el área
    private int contactos;

    public void Configurar(Secuencia nuevaSecuencia, int nuevasEntradas)
    {
        secuencia = nuevaSecuencia;
        entradas = nuevasEntradas;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.GetComponentInParent<PlayerController>() == null || contactos++ > 0)
            return;

        cuenta++;
        if (cuenta == entradas)
            secuencia.Ejecutar();
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.GetComponentInParent<PlayerController>() != null && contactos > 0)
            contactos--;
    }
}
