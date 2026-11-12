using UnityEngine;

namespace PortalesGame
{
    /// <summary>Dimensión visual/narrativa a la que pertenece una habitación.</summary>
    public enum Dimension { Reality = 0, D1 = 1, D2 = 2, D3 = 3 }

    /// <summary>Ocho direcciones de pantalla. Coinciden con los nombres de los sprites del protagonista.</summary>
    public enum Dir8 { Sur = 0, Sureste = 1, Este = 2, Noreste = 3, Norte = 4, Noroeste = 5, Oeste = 6, Suroeste = 7 }

    /// <summary>
    /// Matemática de la proyección isométrica 2:1 del proyecto.
    /// Una celda mide 64x32 px (1 x 0.5 unidades a 64 PPU), igual que los azulejos reales.
    /// El eje X de celda sube hacia la derecha de la pantalla y el eje Y sube hacia la izquierda.
    /// </summary>
    public static class Iso
    {
        public const float PixelsPerUnit = 64f;
        public const float HalfW = 0.5f;
        public const float HalfH = 0.25f;
        /// <summary>Altura en unidades de mundo de un nivel de pared (32 px).</summary>
        public const float LevelHeight = 0.5f;

        public static Vector2 CellToLocal(Vector2 cell)
        {
            return new Vector2((cell.x - cell.y) * HalfW, (cell.x + cell.y) * HalfH);
        }

        public static Vector2 CellToLocal(float x, float y)
        {
            return new Vector2((x - y) * HalfW, (x + y) * HalfH);
        }

        public static Vector2 LocalToCell(Vector2 local)
        {
            float a = local.x / HalfW;   // x - y
            float b = local.y / HalfH;   // x + y
            return new Vector2((a + b) * 0.5f, (b - a) * 0.5f);
        }

        /// <summary>Convierte una dirección de pantalla (derecha, arriba) en un vector de celda normalizado.</summary>
        public static Vector2 ScreenDirToCellDir(Vector2 screen)
        {
            if (screen.sqrMagnitude < 1e-6f) return Vector2.zero;
            var c = new Vector2(screen.x + screen.y, screen.y - screen.x);
            return c.normalized;
        }

        /// <summary>Convierte un desplazamiento en celdas en la dirección de pantalla equivalente (sin el achatado isométrico).</summary>
        public static Vector2 CellDirToScreenDir(Vector2 cell)
        {
            return new Vector2(cell.x - cell.y, cell.x + cell.y);
        }

        public static Dir8 ScreenDirToDir8(Vector2 screen, Dir8 fallback)
        {
            if (screen.sqrMagnitude < 1e-6f) return fallback;
            float ang = Mathf.Atan2(screen.y, screen.x) * Mathf.Rad2Deg;   // 0 = este, 90 = norte
            int sector = Mathf.RoundToInt(ang / 45f);
            sector = ((sector % 8) + 8) % 8;
            switch (sector)
            {
                case 0: return Dir8.Este;
                case 1: return Dir8.Noreste;
                case 2: return Dir8.Norte;
                case 3: return Dir8.Noroeste;
                case 4: return Dir8.Oeste;
                case 5: return Dir8.Suroeste;
                case 6: return Dir8.Sur;
                default: return Dir8.Sureste;
            }
        }

        /// <summary>Vector de pantalla unitario de una dirección; se usa como parámetro del árbol de mezcla del Animator.</summary>
        public static Vector2 Dir8ToVector(Dir8 d)
        {
            switch (d)
            {
                case Dir8.Este: return new Vector2(1, 0);
                case Dir8.Noreste: return new Vector2(0.7071f, 0.7071f);
                case Dir8.Norte: return new Vector2(0, 1);
                case Dir8.Noroeste: return new Vector2(-0.7071f, 0.7071f);
                case Dir8.Oeste: return new Vector2(-1, 0);
                case Dir8.Suroeste: return new Vector2(-0.7071f, -0.7071f);
                case Dir8.Sur: return new Vector2(0, -1);
                default: return new Vector2(0.7071f, -0.7071f);
            }
        }

        public static string Dir8Name(Dir8 d)
        {
            return d.ToString().ToLowerInvariant();
        }
    }
}
