using System;
using System.Collections.Generic;
using UnityEngine;

namespace PortalesGame
{
    /// <summary>
    /// Controla qué habitación del nivel está encendida: enciende la actual (y sus contiguas como vecinas), apaga
    /// el resto y avisa a quien escuche RoomEntered. HabitacionPorPosicion le dice a cuál cambiar según dónde está Elaki.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance { get; private set; }

        public List<Room> rooms = new List<Room>();

        public Room CurrentRoom { get; private set; }
        public string LastSpawnId { get; private set; }
        public bool IsTransitioning { get; private set; }

        public event Action<Room, string> RoomEntered;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public Room Get(string roomId)
        {
            for (int i = 0; i < rooms.Count; i++)
                if (rooms[i] != null && rooms[i].roomId == roomId) return rooms[i];
            return null;
        }

        /// <summary>Cruce caminando a otra habitación: Elaki sigue donde está y solo cambia lo que se ve.</summary>
        public void SeamlessTo(string roomId, string spawnId)
        {
            var room = Get(roomId);
            if (room == null || room == CurrentRoom || IsTransitioning) return;
            ShowRoom(room);
            LastSpawnId = spawnId;
            room.onEnter?.Invoke();
            RoomEntered?.Invoke(room, spawnId);
        }

        private void ShowRoom(Room room)
        {
            for (int i = 0; i < rooms.Count; i++)
                if (rooms[i] != null && rooms[i] != room && !room.seamlessNeighbors.Contains(rooms[i])) rooms[i].gameObject.SetActive(false);
            // Primero se apaga la luz global de las vecinas y luego se enciende la propia: nunca hay dos a la vez.
            foreach (var n in room.seamlessNeighbors)
            {
                if (n == null) continue;
                n.SetNeighborMode(true);
                n.gameObject.SetActive(true);
                for (int i = 0; i < n.sortables.Count; i++)
                    if (n.sortables[i] != null) n.sortables[i].Apply();
            }
            room.SetNeighborMode(false);
            room.gameObject.SetActive(true);
            CurrentRoom = room;
            for (int i = 0; i < room.sortables.Count; i++)
                if (room.sortables[i] != null) room.sortables[i].Apply();
        }
    }
}
