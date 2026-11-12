using System;
using System.Collections.Generic;
using UnityEngine;

namespace PortalesGame
{
    /// <summary>
    /// Rejilla de navegación de una habitación y búsqueda de rutas A*.
    /// Cada celda isométrica se subdivide en <see cref="resolution"/> x <see cref="resolution"/> nodos.
    /// Las coordenadas públicas siempre están en celdas de la habitación (no en unidades de mundo).
    /// </summary>
    [Serializable]
    public class NavGrid
    {
        public int resolution = 2;
        public int width;    // nodos en X
        public int height;   // nodos en Y
        public bool[] blocked = new bool[0];

        [NonSerialized] private float[] gScore;
        [NonSerialized] private int[] cameFrom;
        [NonSerialized] private int[] visitStamp;
        [NonSerialized] private int stamp;
        [NonSerialized] private List<int> open;

        public void Init(int cellsX, int cellsY, int res)
        {
            resolution = Mathf.Max(1, res);
            width = cellsX * resolution;
            height = cellsY * resolution;
            blocked = new bool[width * height];
        }

        public bool InBounds(int nx, int ny) { return nx >= 0 && ny >= 0 && nx < width && ny < height; }

        public bool IsWalkableNode(int nx, int ny)
        {
            return InBounds(nx, ny) && !blocked[ny * width + nx];
        }

        public Vector2Int CellToNode(Vector2 cell)
        {
            return new Vector2Int(Mathf.FloorToInt(cell.x * resolution), Mathf.FloorToInt(cell.y * resolution));
        }

        public Vector2 NodeToCell(int nx, int ny)
        {
            return new Vector2((nx + 0.5f) / resolution, (ny + 0.5f) / resolution);
        }

        public bool IsWalkableCell(Vector2 cell)
        {
            var n = CellToNode(cell);
            return IsWalkableNode(n.x, n.y);
        }

        /// <summary>Marca como bloqueados los nodos cuyo centro cae dentro del rectángulo (en celdas), ampliado por un margen.</summary>
        public void BlockRect(Rect cellRect, float margin)
        {
            float x0 = cellRect.xMin - margin, x1 = cellRect.xMax + margin;
            float y0 = cellRect.yMin - margin, y1 = cellRect.yMax + margin;
            for (int ny = 0; ny < height; ny++)
            {
                for (int nx = 0; nx < width; nx++)
                {
                    var c = NodeToCell(nx, ny);
                    if (c.x > x0 && c.x < x1 && c.y > y0 && c.y < y1) blocked[ny * width + nx] = true;
                }
            }
        }

        /// <summary>Nodo transitable más cercano a una posición; busca en anillos crecientes.</summary>
        public bool NearestWalkable(Vector2 cell, out Vector2 result, int maxRing = 12)
        {
            var n = CellToNode(cell);
            n.x = Mathf.Clamp(n.x, 0, width - 1);
            n.y = Mathf.Clamp(n.y, 0, height - 1);
            if (IsWalkableNode(n.x, n.y)) { result = InBoundsCell(cell) ? cell : NodeToCell(n.x, n.y); return true; }

            float best = float.MaxValue;
            result = cell;
            bool found = false;
            for (int r = 1; r <= maxRing && !found; r++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    for (int dx = -r; dx <= r; dx++)
                    {
                        if (Mathf.Abs(dx) != r && Mathf.Abs(dy) != r) continue;
                        int x = n.x + dx, y = n.y + dy;
                        if (!IsWalkableNode(x, y)) continue;
                        var c = NodeToCell(x, y);
                        float d = (c - cell).sqrMagnitude;
                        if (d < best) { best = d; result = c; found = true; }
                    }
                }
            }
            return found;
        }

        private bool InBoundsCell(Vector2 cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < width / (float)resolution && cell.y < height / (float)resolution;
        }

        /// <summary>
        /// Calcula una ruta entre dos posiciones en celdas. Devuelve false si el destino no es alcanzable.
        /// La ruta se simplifica con comprobaciones de línea de visión para que el movimiento sea natural.
        /// </summary>
        public bool FindPath(Vector2 fromCell, Vector2 toCell, List<Vector2> path)
        {
            path.Clear();
            if (width == 0 || height == 0) return false;

            Vector2 startPos, goalPos;
            if (!NearestWalkable(fromCell, out startPos)) return false;
            if (!NearestWalkable(toCell, out goalPos)) return false;

            var s = CellToNode(startPos);
            var g = CellToNode(goalPos);
            int start = s.y * width + s.x;
            int goal = g.y * width + g.x;

            int count = width * height;
            if (gScore == null || gScore.Length != count)
            {
                gScore = new float[count];
                cameFrom = new int[count];
                visitStamp = new int[count];
                open = new List<int>(128);
            }
            stamp++;
            open.Clear();

            gScore[start] = 0;
            cameFrom[start] = -1;
            visitStamp[start] = stamp;
            open.Add(start);

            bool reached = false;
            int guard = count * 8;
            while (open.Count > 0 && guard-- > 0)
            {
                int bestIdx = 0;
                float bestF = float.MaxValue;
                for (int i = 0; i < open.Count; i++)
                {
                    int n = open[i];
                    float f = gScore[n] + Heuristic(n, goal);
                    if (f < bestF) { bestF = f; bestIdx = i; }
                }
                int current = open[bestIdx];
                open[bestIdx] = open[open.Count - 1];
                open.RemoveAt(open.Count - 1);

                if (current == goal) { reached = true; break; }

                int cx = current % width, cy = current / width;
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        int nx = cx + dx, ny = cy + dy;
                        if (!IsWalkableNode(nx, ny)) continue;
                        // Sin atajos por esquinas: una diagonal exige que los dos ortogonales estén libres.
                        if (dx != 0 && dy != 0 && (!IsWalkableNode(cx + dx, cy) || !IsWalkableNode(cx, cy + dy))) continue;

                        int nIdx = ny * width + nx;
                        float tentative = gScore[current] + ((dx != 0 && dy != 0) ? 1.4142f : 1f);
                        if (visitStamp[nIdx] == stamp && tentative >= gScore[nIdx]) continue;
                        bool isNew = visitStamp[nIdx] != stamp;
                        visitStamp[nIdx] = stamp;
                        gScore[nIdx] = tentative;
                        cameFrom[nIdx] = current;
                        if (isNew || !open.Contains(nIdx)) open.Add(nIdx);
                    }
                }
            }

            if (!reached) return false;

            var raw = new List<Vector2>();
            for (int n = goal; n != -1; n = cameFrom[n]) raw.Add(NodeToCell(n % width, n / width));
            raw.Reverse();
            raw[0] = startPos;
            raw[raw.Count - 1] = goalPos;

            // Simplificación por línea de visión.
            int anchor = 0;
            path.Add(raw[0]);
            while (anchor < raw.Count - 1)
            {
                int next = raw.Count - 1;
                while (next > anchor + 1 && !HasLineOfSight(raw[anchor], raw[next])) next--;
                path.Add(raw[next]);
                anchor = next;
            }
            return true;
        }

        private float Heuristic(int a, int b)
        {
            int dx = Mathf.Abs(a % width - b % width);
            int dy = Mathf.Abs(a / width - b / width);
            return (dx + dy) + (1.4142f - 2f) * Mathf.Min(dx, dy);
        }

        public bool HasLineOfSight(Vector2 a, Vector2 b)
        {
            float dist = Vector2.Distance(a, b);
            int steps = Mathf.Max(1, Mathf.CeilToInt(dist * resolution * 3f));
            for (int i = 0; i <= steps; i++)
            {
                var p = Vector2.Lerp(a, b, i / (float)steps);
                if (!IsWalkableCell(p)) return false;
            }
            return true;
        }
    }
}
