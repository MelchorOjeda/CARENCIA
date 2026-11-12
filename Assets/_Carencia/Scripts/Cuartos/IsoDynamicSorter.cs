using UnityEngine;

namespace PortalesGame
{
    /// <summary>
    /// Ordena un personaje móvil respecto a los muebles de la habitación activa.
    /// Compara la posición de los pies con la huella de cada <see cref="IsoSortable"/> cuyo dibujo
    /// se solapa en pantalla y elige un orden que lo deje delante o detrás según corresponda.
    /// </summary>
    public class IsoDynamicSorter : MonoBehaviour
    {
        public SpriteRenderer[] renderers = new SpriteRenderer[0];
        [Tooltip("Ancho y alto visibles del personaje en unidades de mundo, medidos desde los pies.")]
        public Vector2 visualSize = new Vector2(0.8f, 2f);
        [Tooltip("Margen en unidades de mundo para considerar que dos dibujos se solapan.")]
        public float overlapMargin = 0.05f;
        [Tooltip("Sentado frente a un mueble: se dibuja delante de todo lo que toca (la mano queda sobre el escritorio).")]
        public bool forceFront;

        private void LateUpdate()
        {
            var rm = RoomManager.Instance;
            var room = rm != null ? rm.CurrentRoom : null;
            if (room == null || renderers.Length == 0 || renderers[0] == null) return;

            Vector2 cell = room.WorldToCell(transform.position);
            // Caja visible del personaje medida desde los pies (el sprite real tiene mucho margen transparente).
            var mine = new Bounds(transform.position + new Vector3(0, visualSize.y * 0.5f, 0),
                new Vector3(visualSize.x + overlapMargin * 2f, visualSize.y + overlapMargin * 2f, 1f));

            int maxFront = int.MinValue;   // mayor orden entre los objetos que quedan detrás del personaje
            int minBehind = int.MaxValue;  // menor orden entre los objetos que quedan delante del personaje
            int minAll = int.MaxValue;

            var list = room.sortables;
            for (int i = 0; i < list.Count; i++)
            {
                var s = list[i];
                if (s == null || !s.gameObject.activeInHierarchy) continue;
                if (s.baseOrder < minAll) minAll = s.baseOrder;

                var b = s.VisualBounds();
                if (b.size.sqrMagnitude < 1e-6f) continue;
                if (mine.min.x > b.max.x || mine.max.x < b.min.x || mine.min.y > b.max.y || mine.max.y < b.min.y) continue;

                var f = s.footprint;
                bool inFront = cell.x <= f.xMin || cell.y <= f.yMin;   // el personaje está más cerca de la cámara
                bool behind = cell.x >= f.xMax || cell.y >= f.yMax;    // el personaje está más lejos
                if (inFront && behind)
                {
                    // Posición lateral: decide la profundidad del centro.
                    inFront = (cell.x + cell.y) < (f.center.x + f.center.y);
                    behind = !inFront;
                }
                else if (!inFront && !behind)
                {
                    inFront = (cell.x + cell.y) < (f.center.x + f.center.y);
                    behind = !inFront;
                }

                if (inFront) { if (s.baseOrder > maxFront) maxFront = s.baseOrder; }
                else if (behind) { if (s.baseOrder < minBehind) minBehind = s.baseOrder; }
            }

            int order;
            // Los muebles ocupan baseOrder..baseOrder+14 (sus piezas hijas); los órdenes base van de 20 en 20.
            if (maxFront != int.MinValue && (minBehind == int.MaxValue || maxFront < minBehind)) order = maxFront + 15;
            else if (minBehind != int.MaxValue) order = minBehind - 5;
            else order = (minAll == int.MaxValue ? 0 : minAll) - 5;

            if (forceFront) order = 30000;   // sentado frente al escritorio: el brazo y la silla quedan delante de los muebles
            for (int i = 0; i < renderers.Length; i++)
                if (renderers[i] != null) renderers[i].sortingOrder = order + i;
        }
    }
}
