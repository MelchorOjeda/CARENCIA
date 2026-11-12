using PortalesGame;
using UnityEngine;

// Movimiento antinatural de "ella" (Nivel 2): al llegar a un cuarto cae a cuatro patas y sigue persiguiendo gateando.
// Cambia sus animaciones de correr por las de gatear y ya no vuelve a levantarse.
[RequireComponent(typeof(EnemigoPerseguidor))]
public class GatearEnCuarto : MonoBehaviour
{
    [Tooltip("Cuarto donde empieza a gatear")]
    [SerializeField] private string cuarto = "sala_d2";

    [Tooltip("Las mismas animaciones con las hojas de gateo en lugar de las de correr")]
    [SerializeField] private RuntimeAnimatorController gateo;

    private EnemigoPerseguidor enemigo;
    private Animator animador;
    private bool gateando;

    private void Awake()
    {
        enemigo = GetComponent<EnemigoPerseguidor>();
        animador = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (gateando || gateo == null || animador == null || !enemigo.Persiguiendo)
            return;

        var actual = RoomManager.Instance != null ? RoomManager.Instance.CurrentRoom : null;
        if (actual == null || actual.roomId != cuarto)
            return;

        gateando = true;
        animador.runtimeAnimatorController = gateo;
    }
}
