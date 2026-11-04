using UnityEngine;

// Pasa el movimiento de Elaki al Animator. Al detenerse, conserva la última dirección para seguir mirando hacia ahí.
[RequireComponent(typeof(PlayerController), typeof(Animator))]
public class AnimadorJugador : MonoBehaviour
{
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");
    private static readonly int Caminando = Animator.StringToHash("Caminando");

    private PlayerController jugador;
    private Animator animator;
    private Vector2 ultimaDireccion = Vector2.down;

    private void Awake()
    {
        jugador = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (jugador.EstaCaminando)
            ultimaDireccion = jugador.Direccion;

        animator.SetFloat(DirX, ultimaDireccion.x);
        animator.SetFloat(DirY, ultimaDireccion.y);
        animator.SetBool(Caminando, jugador.EstaCaminando);
    }
}
