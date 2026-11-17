using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Base de un mueble en el piso isométrico: cuatro puntos (izquierda, frente, derecha, fondo) en coordenadas locales.
// Con colisión, crea un PolygonCollider2D con esa forma para que Elaki se detenga en su borde.
// Con orden dinámico corrige el único caso en que el orden por altura falla con muebles anchos: Elaki enfrente de
// un borde de la base pero más arriba que el punto más bajo del mueble. Entonces el punto de orden del mueble se
// recorre justo detrás de ella; en cualquier otro caso se queda donde estaba. Nunca cambia el orden (sortingOrder):
// así el mueble sigue igual frente a paredes, personas y otros muebles.
// Lo que va encima del mueble (trastes, platos) se recorre con él.
public class HuellaIso : MonoBehaviour
{
    [SerializeField] private Vector2[] puntos = new Vector2[4];
    [SerializeField] private bool colision = true;
    [SerializeField] private bool ordenDinamico = true;
    [SerializeField] private Component[] seguidores = new Component[0];

    private Transform envoltura;
    private float claveBase;
    private Transform jugador;
    private Rigidbody2D cuerpoJugador;
    private Vector3 posicionPropia;
    private readonly List<(Transform raiz, Vector3 pos, Transform[] hijos, Vector3[] posHijos)> seguidoresBase =
        new List<(Transform, Vector3, Transform[], Vector3[])>();
    private float ultimo;

    public Vector2[] PuntosMundo
    {
        get
        {
            var r = new Vector2[puntos.Length];
            for (var i = 0; i < puntos.Length; i++)
                r[i] = transform.TransformPoint(puntos[i]);
            return r;
        }
    }

    public void Configurar(Vector2[] locales, bool conColision, bool conOrden, Component[] encima)
    {
        puntos = locales;
        colision = conColision;
        ordenDinamico = conOrden;
        seguidores = encima ?? new Component[0];
    }

    private void Awake()
    {
        if (colision && puntos.Length >= 3)
        {
            var poligono = GetComponent<PolygonCollider2D>();
            if (poligono == null)
                poligono = gameObject.AddComponent<PolygonCollider2D>();
            poligono.isTrigger = false;
            poligono.points = puntos;
        }

        if (!ordenDinamico || puntos.Length < 4)
            return;

        // Un grupo de orden por encima del mueble: se recorre él y el mueble regresa lo mismo, así que el dibujo y
        // la colisión no se mueven; solo cambia el punto con el que se ordena
        var grupo = GetComponent<SortingGroup>();
        var sr = GetComponent<SpriteRenderer>();
        envoltura = new GameObject("Orden_" + name).transform;
        envoltura.SetParent(transform.parent, false);
        envoltura.position = transform.position;
        envoltura.SetSiblingIndex(transform.GetSiblingIndex());
        var orden = envoltura.gameObject.AddComponent<SortingGroup>();
        orden.sortingLayerID = grupo != null ? grupo.sortingLayerID : sr != null ? sr.sortingLayerID : 0;
        orden.sortingOrder = grupo != null ? grupo.sortingOrder : sr != null ? sr.sortingOrder : 0;
        transform.SetParent(envoltura, true);
        claveBase = envoltura.position.y;
        posicionPropia = transform.position;

        foreach (var s in seguidores)
        {
            if (s == null)
                continue;
            var raiz = s.transform;
            var hijos = new Transform[raiz.childCount];
            var pos = new Vector3[raiz.childCount];
            for (var i = 0; i < raiz.childCount; i++)
            {
                hijos[i] = raiz.GetChild(i);
                pos[i] = hijos[i].position;
            }
            seguidoresBase.Add((raiz, raiz.position, hijos, pos));
        }
    }

    private void LateUpdate()
    {
        if (envoltura == null)
            return;

        if (jugador == null)
        {
            var control = FindAnyObjectByType<PlayerController>();
            if (control == null)
                return;
            jugador = control.transform;
            cuerpoJugador = control.GetComponent<Rigidbody2D>();
        }

        // Sentada (cuerpo cinemático) su dibujo es el de la silla: el orden normal ya es el correcto
        if (cuerpoJugador != null && cuerpoJugador.bodyType != RigidbodyType2D.Dynamic)
        {
            if (ultimo != 0f)
                Recorrer(0f);
            return;
        }

        // 0 izquierda, 1 frente (el punto más bajo), 2 derecha
        Vector2 izq = transform.TransformPoint(puntos[0]);
        Vector2 frente = transform.TransformPoint(puntos[1]);
        Vector2 der = transform.TransformPoint(puntos[2]);
        Vector2 pies = jugador.position;

        var corrimiento = 0f;
        if (pies.x > izq.x && pies.x < der.x)
        {
            var borde = pies.x < frente.x
                ? Mathf.Lerp(izq.y, frente.y, Mathf.InverseLerp(izq.x, frente.x, pies.x))
                : Mathf.Lerp(frente.y, der.y, Mathf.InverseLerp(frente.x, der.x, pies.x));
            // Solo enfrente: el mueble queda detrás de ella. Si está detrás o sentada a la mesa, el orden normal
            // ya es el correcto y no se toca
            if (pies.y < borde)
                corrimiento = Mathf.Max(0f, pies.y + 0.02f - claveBase);
        }

        if (Mathf.Approximately(corrimiento, ultimo))
            return;
        Recorrer(corrimiento);
    }

    private void Recorrer(float corrimiento)
    {
        ultimo = corrimiento;
        var arriba = Vector3.up * corrimiento;
        // Todo en coordenadas del mundo: el padre puede tener escala
        envoltura.position = new Vector3(envoltura.position.x, claveBase, envoltura.position.z) + arriba;
        transform.position = posicionPropia;
        foreach (var (raiz, pos, hijos, posHijos) in seguidoresBase)
        {
            if (raiz == null)
                continue;
            raiz.position = pos + arriba;
            for (var i = 0; i < hijos.Length; i++)
                if (hijos[i] != null)
                    hijos[i].position = posHijos[i];
        }
    }

    private void OnDrawGizmosSelected()
    {
        var p = PuntosMundo;
        Gizmos.color = Color.cyan;
        for (var i = 0; i < p.Length; i++)
            Gizmos.DrawLine(p[i], p[(i + 1) % p.Length]);
    }
}
