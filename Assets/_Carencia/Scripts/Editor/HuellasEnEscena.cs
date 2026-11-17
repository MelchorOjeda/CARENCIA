using System.Collections.Generic;
using System.IO;
using System.Linq;
using PortalesGame;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

// Agrega a los muebles de la escena abierta su base isométrica (HuellaIso): colisión con la forma de su base y,
// en las escenas sin el sistema de cuartos, orden de dibujo según esa base. Se puede volver a correr: reemplaza
// lo que puso antes. La base sale del dibujo: su punto más bajo (frente), su orilla izquierda y su orilla derecha.
public static class HuellasEnEscena
{
    // Cosas de pared, de encima de otro mueble, personajes o demasiado pequeñas para estorbar
    private static readonly string[] Excluir =
    {
        "poster", "cuadro", "foto", "jamba", "ventana", "puerta", "umbral", "marco", "plato", "traste", "bebida",
        "mosca", "lugarvacio", "contorno", "tapete", "hoja", "libro", "laptop", "lapicero", "cepillo", "pasta",
        "jabon", "papel", "despertador", "control_remoto", "teclado", "raton", "calendario", "indicador", "toalla",
        "espejo", "toallero", "reloj", "portal", "spr_pj_", "elaki", "madre", "padre", "periodico", "techo", "luz", "sombra",
        "mancha", "alfombra", "tapete", "gusano", "estatica", "pantalla", "reflejo", "cortina",
        "stain", "enemigos/", "silueta", "tramo", "margarita", "ella", "pared", "wall", "placeholder", "muro", "jamba",
    };

    // Aunque su nombre coincida con lo de arriba, sí estorban (la bañera se llama "..._cortina_...")
    private static readonly string[] Incluir = { "banera" };

    private const float AltoMinimo = 0.35f;   // unidades: menos que esto no estorba el paso
    private const float Encogido = 0.12f;     // la base se encoge un poco hacia su centro para no trabar a Elaki

    [MenuItem("Carencia/Agregar huellas y colisiones a la escena")]
    private static void Menu()
    {
        var resultado = Aplicar();
        Debug.Log(resultado);
    }

    public static string Aplicar()
    {
        var escena = EditorSceneManager.GetActiveScene();
        var conCuartos = Object.FindAnyObjectByType<RoomManager>() != null;
        // HuellaIso crea su collider al empezar el juego: en el editor solo hay que quitar el componente
        foreach (var viejo in Object.FindObjectsByType<HuellaIso>(FindObjectsInactive.Include))
            Object.DestroyImmediate(viejo);

        var jugador = Object.FindAnyObjectByType<PlayerController>();
        var candidatos = new List<(Transform raiz, SpriteRenderer sr, Vector2[] base_)>();
        foreach (var sr in Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include))
        {
            if (sr.sprite == null || sr.sortingLayerName == "Suelo" || sr.sortingLayerName == "UI" || sr.sortingLayerName == "Fondo")
                continue;
            if (jugador != null && sr.transform.IsChildOf(jugador.transform))
                continue;
            if (sr.GetComponentInParent<EnemigoPerseguidor>(true) != null)
                continue;
            // Solo el nombre del objeto, de su raíz y de su dibujo: la ruta completa incluye "Muros_Bano" y demás cuartos
            var raizNombre = sr.GetComponentInParent<SortingGroup>(true) != null ? sr.GetComponentInParent<SortingGroup>(true).name : "";
            var nombre = (raizNombre + " " + sr.name + " " + (sr.transform.parent != null ? sr.transform.parent.name : "") + " " + sr.sprite.name).ToLowerInvariant();
            if (sr.transform.root.name == "Enemigos")
                continue;
            // Las bañeras partidas en tramos ya traen su colisión y su orden contra la pared
            if (nombre.Contains("tramo"))
                continue;
            if (Excluir.Any(nombre.Contains) && !Incluir.Any(nombre.Contains))
                continue;

            var grupo = sr.GetComponentInParent<SortingGroup>(true);
            var raiz = grupo != null ? grupo.transform : sr.transform;
            if (candidatos.Any(c => c.raiz == raiz))
                continue;

            // La barra de la cocina es una L: su base es la L (un rombo taparía el hueco donde se camina)
            var esL = sr.sprite.name.StartsWith("SPR_OBJ_BarraCocina");
            var base_ = esL ? BaseEnL(sr) : BaseDelDibujo(sr);
            if (base_ == null)
                continue;
            var alto = sr.bounds.size.y;
            // Colgado de la pared o encima de otro mueble: su base dibujada queda muy por encima de su punto de orden
            var elevado = base_[1].y - raiz.position.y > 0.25f;
            if (alto < AltoMinimo || elevado)
                continue;
            candidatos.Add((raiz, sr, base_));
        }

        var colocadas = 0;
        foreach (var (raiz, sr, base_) in candidatos)
        {
            // Lo que está encima (en pantalla, dentro del dibujo del mueble y con su base más arriba): lo sigue
            var encima = new List<Component>();
            if (!conCuartos)
            {
                foreach (var otro in Object.FindObjectsByType<SortingGroup>(FindObjectsInactive.Include))
                {
                    if (otro.transform == raiz || otro.transform.IsChildOf(raiz) || raiz.IsChildOf(otro.transform))
                        continue;
                    var dibujoOtro = otro.GetComponentInChildren<SpriteRenderer>(true);
                    if (dibujoOtro == null || candidatos.Any(c => c.raiz == otro.transform))
                        continue;
                    var b = dibujoOtro.bounds;
                    if (sr.bounds.Contains(new Vector3(b.center.x, b.min.y, sr.bounds.center.z)) && b.min.y > base_[1].y + 0.05f && b.center.x > base_[0].x && b.center.x < base_[2].x
                        && otro.transform.position.y > base_[1].y - 0.1f && otro.transform.position.y < base_[3].y - 0.05f)
                        encima.Add(otro);
                }
            }

            // Si el mueble ya tiene su colisión (los del sistema de cuartos o los esparcidos con base), no se duplica
            var dueno = raiz.GetComponentInParent<IsoSortable>(true) != null ? raiz.GetComponentInParent<IsoSortable>(true).transform : raiz;
            var yaChoca = dueno.GetComponentsInChildren<Collider2D>(true).Any(c => !c.isTrigger && !(c is TilemapCollider2D) && !(c is CompositeCollider2D));
            if (yaChoca && conCuartos)
                continue;
            // Los muebles en L (la barra del Nivel 3) ya deciden su orden y su base no es un rombo: se dejan como están
            if (raiz.GetComponent<OrdenSegunJugador>() != null)
                continue;

            var centro = (base_[0] + base_[1] + base_[2] + base_[3]) / 4f;
            var encoger = base_.Length > 4 ? 0f : Encogido;
            var locales = base_.Select(p => (Vector2)raiz.InverseTransformPoint(Vector2.Lerp(p, centro, encoger))).ToArray();
            var huella = raiz.gameObject.AddComponent<HuellaIso>();
            // En las sillas donde Elaki se sienta su dibujo sentada va dentro de la silla: solo colisión
            var silla = raiz.GetComponentInChildren<SillaSentable>(true) != null || raiz.GetComponentInParent<SillaSentable>(true) != null;
            huella.Configurar(locales, !yaChoca, !conCuartos && !silla, encima.ToArray());
            EditorUtility.SetDirty(raiz.gameObject);
            colocadas++;
        }

        EditorSceneManager.MarkSceneDirty(escena);
        return escena.name + ": " + colocadas + " muebles con base" + (conCuartos ? " (solo colisión: la escena usa el sistema de cuartos)" : "");
    }

    // Izquierda, frente, derecha y fondo de la base en el mundo; null si el dibujo no se puede leer
    private static Vector2[] BaseDelDibujo(SpriteRenderer sr)
    {
        var ruta = AssetDatabase.GetAssetPath(sr.sprite);
        if (string.IsNullOrEmpty(ruta) || !File.Exists(ruta))
            return null;
        var textura = new Texture2D(2, 2);
        textura.LoadImage(File.ReadAllBytes(ruta));
        var rect = sr.sprite.rect;
        int x0 = (int)rect.x, y0 = (int)rect.y, w = (int)rect.width, h = (int)rect.height;
        var abajo = new int[w];
        for (var x = 0; x < w; x++)
        {
            abajo[x] = -1;
            for (var y = 0; y < h; y++)
            {
                if (textura.GetPixel(x0 + x, y0 + y).a > 0.5f)
                {
                    abajo[x] = y;
                    break;
                }
            }
        }

        Object.DestroyImmediate(textura);
        var columnas = Enumerable.Range(0, w).Where(x => abajo[x] >= 0).ToArray();
        if (columnas.Length < 3)
            return null;
        var minimo = columnas.Min(x => abajo[x]);
        var delFrente = columnas.Where(x => abajo[x] == minimo).ToArray();
        var frente = (float)delFrente.Average();
        // La base ocupa todo el ancho del dibujo: sus esquinas izquierda y derecha están sobre las líneas que suben
        // 1 px cada 2 px desde el punto más bajo (sirve igual para cajas y para muebles con patas)
        var izq = columnas.First();
        var der = columnas.Last();
        if (der - izq < 4)
            return null;

        Vector2 Mundo(float px, float py)
        {
            var local = (new Vector2(px, py) - sr.sprite.pivot) / sr.sprite.pixelsPerUnit;
            if (sr.flipX)
                local.x = -local.x;
            return sr.transform.TransformPoint(local);
        }

        var a = Mundo(izq, minimo + (frente - izq) * 0.5f);
        var f = Mundo(frente, minimo);
        var c = Mundo(der, minimo + (der - frente) * 0.5f);
        // Volteado en X por escala: izquierda y derecha se invierten en el mundo
        if (a.x > c.x)
            (a, c) = (c, a);
        var fondo = a + c - f;
        return new[] { a, f, c, fondo };
    }

    // Barra en L: lado largo hacia la izquierda y regreso hacia arriba a la derecha, ambos de 52 px de fondo.
    // Puntos: izquierda, esquina de enfrente, final del regreso, su fondo, esquina de adentro, fondo de la izquierda
    private static Vector2[] BaseEnL(SpriteRenderer sr)
    {
        var plano = BaseDelDibujo(sr);
        if (plano == null)
            return null;
        // BaseDelDibujo ya da izquierda, esquina (punto más bajo) y derecha sobre las líneas del piso
        const float fondo = 52f;
        var ppu = sr.sprite.pixelsPerUnit;
        var escala = sr.transform.lossyScale;
        var arribaDerecha = new Vector2(fondo, fondo / 2f) / ppu * new Vector2(Mathf.Abs(escala.x), Mathf.Abs(escala.y));
        var arribaIzquierda = new Vector2(-arribaDerecha.x, arribaDerecha.y);
        var a = plano[0]; var f = plano[1]; var c = plano[2];
        return new[] { a, f, c, c + arribaIzquierda, f + arribaDerecha + arribaIzquierda, a + arribaDerecha };
    }

    private static bool Dentro(Vector2[] base_, Vector2 p, float margen)
    {
        var centro = (base_[0] + base_[1] + base_[2] + base_[3]) / 4f;
        var q = base_.Select(v => centro + (v - centro) * margen).ToArray();
        var dentro = false;
        for (int i = 0, j = q.Length - 1; i < q.Length; j = i++)
        {
            if ((q[i].y > p.y) != (q[j].y > p.y) && p.x < (q[j].x - q[i].x) * (p.y - q[i].y) / (q[j].y - q[i].y) + q[i].x)
                dentro = !dentro;
        }
        return dentro;
    }

    private static string Ruta(Transform t) => t.parent == null ? t.name : Ruta(t.parent) + "/" + t.name;
}
