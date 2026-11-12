using PortalesGame;
using UnityEngine;

// Va junto al RoomManager de un nivel hecho por cuartos (Nivel 2): muestra el cuarto donde está Elaki.
// Al cruzar caminando de un cuarto a otro (pasillo y sala), el nuevo se enciende y el anterior se apaga.
[RequireComponent(typeof(RoomManager))]
public class HabitacionPorPosicion : MonoBehaviour
{
    private RoomManager cuartos;
    private PlayerController jugador;

    private void Start()
    {
        cuartos = GetComponent<RoomManager>();
        jugador = FindAnyObjectByType<PlayerController>();
        Actualizar(true);
    }

    private void LateUpdate()
    {
        Actualizar(false);
    }

    private void Actualizar(bool alIniciar)
    {
        if (cuartos == null || jugador == null)
            return;

        var actual = cuartos.CurrentRoom;
        var posicion = jugador.transform.position;

        // Mientras siga dentro del cuarto actual no cambia, aunque también pise el borde del vecino
        if (!alIniciar && actual != null && Contiene(actual, posicion))
            return;

        foreach (var cuarto in cuartos.rooms)
        {
            if (cuarto != null && cuarto != actual && Contiene(cuarto, posicion))
            {
                cuartos.SeamlessTo(cuarto.roomId, "");
                return;
            }
        }

        // Al iniciar fuera de todos los cuartos (no debería pasar) se queda el que esté encendido
        if (alIniciar && actual == null)
        {
            foreach (var cuarto in cuartos.rooms)
            {
                if (cuarto != null && cuarto.gameObject.activeSelf)
                {
                    cuartos.SeamlessTo(cuarto.roomId, "");
                    return;
                }
            }
        }
    }

    private static bool Contiene(Room cuarto, Vector3 posicion)
    {
        return cuarto.ContainsCell(cuarto.WorldToCell(posicion));
    }
}
