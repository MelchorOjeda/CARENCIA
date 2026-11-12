using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PortalesGame
{
    /// <summary>
    /// Una habitación isométrica: identificador, tamaño en celdas, rejilla de navegación,
    /// puntos de entrada, encuadre de cámara y lista de objetos ordenables.
    /// Las habitaciones se activan de una en una a través de <see cref="RoomManager"/>.
    /// </summary>
    public class Room : MonoBehaviour
    {
        [Header("Identidad")]
        public string roomId;
        public string displayName;
        public Dimension dimension;
        [Tooltip("Variante narrativa, por ejemplo Bathroom_RealityAfterLevel1.")]
        public string variantName;

        [Header("Tamaño (celdas)")]
        public int width = 6;
        public int depth = 6;
        [Tooltip("Escala del dibujo de los personajes en esta habitación (los escenarios de referencia usan 1 m = 1.125 u).")]
        public float characterScale = 1f;
        [Tooltip("Estatura del protagonista de pie respecto a su dibujo, encima de characterScale. Sentado no cambia: su dibujo se ajusta a la silla.")]
        public float playerHeightScale = 1.1f;

        [Header("Navegación")]
        public NavGrid nav = new NavGrid();

        [Header("Entradas y contenido")]
        public List<SpawnPoint> spawns = new List<SpawnPoint>();
        public List<IsoSortable> sortables = new List<IsoSortable>();

        [Header("Cámara")]
        public float cameraSize = 3.2f;
        public bool cameraFollow;
        [Tooltip("Centro del encuadre fijo, en coordenadas locales de la habitación.")]
        public Vector2 cameraCenter;
        [Tooltip("Límites (locales) dentro de los que puede moverse el centro de la cámara al seguir al jugador.")]
        public Rect cameraBounds;
        public Color backgroundColor = new Color(0.03f, 0.02f, 0.04f);

        [Header("Presentación (decorativa)")]
        [Tooltip("Paisaje lejano detrás de la habitación (ver AmbientBackdrop).")]
        public Sprite backdrop;
        [Tooltip("Partículas ambientales por segundo (0 = ninguna).")]
        public float ambientParticles;
        public Color particleColor = Color.white;

        [Header("Eventos")]
        public UnityEvent onEnter;

        [Header("Habitaciones contiguas")]
        [Tooltip("Habitaciones que comparten el mismo espacio del mundo con esta (sin pared entre ellas): siguen visibles mientras esta está activa y se cruza a ellas caminando, sin fundido ni corte de cámara.")]
        public List<Room> seamlessNeighbors = new List<Room>();
        [Tooltip("Piezas colgadas en muros que, vistas desde la habitación contigua, quedarían flotando sobre un muro bajo.")]
        public List<GameObject> hideWhenNeighbor = new List<GameObject>();

        /// <summary>
        /// Visible como vecina de la habitación activa: se dibuja y conserva su física, pero su exterior decorativo y su
        /// luz global se apagan para no duplicar los de la habitación activa.
        /// </summary>
        public void SetNeighborMode(bool neighbor)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var c = transform.GetChild(i);
                if (c.name.StartsWith("Presentacion")) c.gameObject.SetActive(!neighbor);
            }
            foreach (var go in hideWhenNeighbor) if (go != null) go.SetActive(!neighbor);
            foreach (var t in GetComponentsInChildren<Transform>(true))
            {
                // Los muros de cada cuarto se dibujan para su propio punto de vista (altos atrás, bajos delante):
                // los de la vecina se ocultan y se ven los de la habitación activa, cuyo mapa ya incluye la contigua.
                if (t.name == "GlobalLight" || t.name == "Muros" || (t.name == "Walls" && t.parent == transform))
                    t.gameObject.SetActive(!neighbor);
            }
        }

        public Vector3 CellToWorld(Vector2 cell)
        {
            Vector2 l = Iso.CellToLocal(cell);
            return transform.TransformPoint(new Vector3(l.x, l.y, 0));
        }

        public Vector2 WorldToCell(Vector3 world)
        {
            Vector3 l = transform.InverseTransformPoint(world);
            return Iso.LocalToCell(new Vector2(l.x, l.y));
        }

        public bool ContainsCell(Vector2 cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x <= width && cell.y <= depth;
        }

        public SpawnPoint GetSpawn(string id)
        {
            for (int i = 0; i < spawns.Count; i++)
                if (spawns[i] != null && spawns[i].spawnId == id) return spawns[i];
            return spawns.Count > 0 ? spawns[0] : null;
        }

        public Vector3 CameraCenterWorld()
        {
            return transform.TransformPoint(new Vector3(cameraCenter.x, cameraCenter.y, 0));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.8f);
            Vector3 a = CellToWorld(new Vector2(0, 0)), b = CellToWorld(new Vector2(width, 0));
            Vector3 c = CellToWorld(new Vector2(width, depth)), d = CellToWorld(new Vector2(0, depth));
            Gizmos.DrawLine(a, b); Gizmos.DrawLine(b, c); Gizmos.DrawLine(c, d); Gizmos.DrawLine(d, a);

            if (nav == null || nav.blocked == null || nav.blocked.Length != nav.width * nav.height) return;
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.5f);
            for (int y = 0; y < nav.height; y++)
                for (int x = 0; x < nav.width; x++)
                    if (nav.blocked[y * nav.width + x]) Gizmos.DrawSphere(CellToWorld(nav.NodeToCell(x, y)), 0.04f);
        }
    }
}
