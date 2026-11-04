using UnityEngine;

// Lleva a Elaki al otro lado de una puerta. Se conecta al evento alInteractuar de un Interactable.
public class PuertaPaso : MonoBehaviour
{
    [SerializeField] private Rigidbody2D jugador;
    [SerializeField] private Transform ladoA;
    [SerializeField] private Transform ladoB;

    public void Cruzar()
    {
        var posicion = jugador.position;
        var alLadoA = Vector2.Distance(posicion, ladoA.position) < Vector2.Distance(posicion, ladoB.position);
        var destino = alLadoA ? ladoB.position : ladoA.position;

        jugador.position = destino;
        jugador.transform.position = destino;
    }
}
