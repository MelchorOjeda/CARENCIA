using UnityEngine;

namespace PortalesGame
{
    /// <summary>
    /// Objeto estático con huella rectangular en celdas. El constructor de habitaciones le asigna un
    /// orden de dibujo de atrás hacia adelante; los personajes se intercalan con <see cref="IsoDynamicSorter"/>.
    /// </summary>
    public class IsoSortable : MonoBehaviour
    {
        [Tooltip("Huella en celdas de la habitación (x, y, ancho, fondo).")]
        public Rect footprint;
        [Tooltip("Orden base asignado por el constructor (múltiplos de 10).")]
        public int baseOrder;
        [Tooltip("Renderers que componen el objeto. Cada uno se dibuja en baseOrder + su desplazamiento.")]
        public SpriteRenderer[] renderers = new SpriteRenderer[0];
        public int[] offsets = new int[0];

        public void Apply()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) continue;
                renderers[i].sortingOrder = baseOrder + (i < offsets.Length ? offsets[i] : 0);
            }
        }

        public Bounds VisualBounds()
        {
            bool any = false;
            var b = new Bounds(transform.position, Vector3.zero);
            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                if (r == null || !r.enabled || !r.gameObject.activeInHierarchy) continue;
                if (!any) { b = r.bounds; any = true; }
                else b.Encapsulate(r.bounds);
            }
            return b;
        }

        private void OnDrawGizmosSelected()
        {
            var room = GetComponentInParent<Room>();
            if (room == null) return;
            Gizmos.color = Color.yellow;
            Vector3 a = room.CellToWorld(new Vector2(footprint.xMin, footprint.yMin));
            Vector3 b = room.CellToWorld(new Vector2(footprint.xMax, footprint.yMin));
            Vector3 c = room.CellToWorld(new Vector2(footprint.xMax, footprint.yMax));
            Vector3 d = room.CellToWorld(new Vector2(footprint.xMin, footprint.yMax));
            Gizmos.DrawLine(a, b); Gizmos.DrawLine(b, c); Gizmos.DrawLine(c, d); Gizmos.DrawLine(d, a);
        }
    }
}
