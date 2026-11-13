using UnityEngine;

// Ojos incrustados en las paredes de carne (Nivel 3): miran y parpadean cada uno a su ritmo.
// En el pasillo (vigilancia) se abren y se cierran todos a la vez: cerrados hay un poco más de luz;
// abiertos todo se oscurece y solo se ven los ojos. Solo con los ojos abiertos se puede pasar al baño.
// Al final (N3.6), Llorar() hace que todos giren hacia el baño y lloren; desde ahí ya no parpadean.
public class OjosPared : MonoBehaviour
{
    // 0 abierto, 1 entreabierto, 2 cerrado, 3 llorando mirando a la izquierda, 4 llorando mirando a la derecha
    [SerializeField] private SpriteRenderer[] ojos = new SpriteRenderer[0];
    [SerializeField] private Sprite[] cuadros = new Sprite[4];
    [SerializeField] private bool llorando;
    // Al aparecer esta bandera, los ojos lloran (la pone la secuencia de N3.6)
    [SerializeField] private string banderaLlanto = "ojos_lloran";

    // Hacia dónde voltean al llorar: la puerta del baño
    [SerializeField] private Transform bano;

    [Header("Vigilancia del pasillo")]
    [SerializeField] private Collider2D zona;
    [SerializeField] private Collider2D puerta;
    [SerializeField] private Secuencia alPasar;
    [SerializeField] private SpriteRenderer oscuridad;
    [SerializeField] private float segundosCerrados = 4f;
    [SerializeField] private float segundosAbiertos = 3f;

    private float[] siguiente;
    private Transform jugador;
    private bool dentro;
    private float penumbraAlEntrar;
    private string[] capasOriginales;

    public void ConfigurarVigilancia(Collider2D nuevaZona, Collider2D nuevaPuerta, Secuencia nuevoAlPasar, SpriteRenderer nuevaOscuridad)
    {
        zona = nuevaZona;
        puerta = nuevaPuerta;
        alPasar = nuevoAlPasar;
        oscuridad = nuevaOscuridad;
    }

    public void Configurar(SpriteRenderer[] nuevosOjos, Sprite[] nuevosCuadros, Transform puertaBano)
    {
        ojos = nuevosOjos;
        cuadros = nuevosCuadros;
        bano = puertaBano;
    }

    public void Llorar()
    {
        llorando = true;
        foreach (var ojo in ojos)
        {
            // Los de la izquierda de la puerta miran a la derecha y al revés
            var derecha = bano != null && ojo != null && ojo.transform.position.x < bano.position.x;
            Poner(ojo, derecha ? 4 : 3);
        }
    }

    private void OnEnable()
    {
        siguiente = new float[ojos.Length];
        for (var i = 0; i < siguiente.Length; i++)
            siguiente[i] = Time.time + Random.Range(0.5f, 5f);
    }

    private void Update()
    {
        if (!llorando && EstadoJuego.Tiene(banderaLlanto) && !string.IsNullOrEmpty(banderaLlanto))
            Llorar();
        if (llorando)
        {
            Vigilar(false, false);
            return;
        }

        if (zona != null)
        {
            if (jugador == null)
            {
                var controlador = FindAnyObjectByType<PlayerController>();
                jugador = controlador != null ? controlador.transform : null;
            }

            var ahoraDentro = jugador != null && zona.OverlapPoint(jugador.position);
            if (ahoraDentro)
            {
                var ciclo = (Time.time % (segundosCerrados + segundosAbiertos));
                var abiertos = ciclo >= segundosCerrados;
                foreach (var ojo in ojos)
                    Poner(ojo, abiertos ? 0 : 2);
                Vigilar(true, abiertos);
                if (abiertos && puerta != null && alPasar != null && puerta.OverlapPoint(jugador.position))
                    alPasar.Ejecutar();
                return;
            }

            Vigilar(false, false);
        }

        for (var i = 0; i < ojos.Length; i++)
        {
            var t = Time.time - siguiente[i];
            if (t < 0f)
                Poner(ojos[i], 0);
            else if (t < 0.06f || (t > 0.16f && t < 0.22f))
                Poner(ojos[i], 1);
            else if (t < 0.16f)
                Poner(ojos[i], 2);
            else
                siguiente[i] = Time.time + Random.Range(2.5f, 6f);
        }
    }

    // Luz, oscuridad y capa de los ojos según el momento de la vigilancia
    private void Vigilar(bool ahoraDentro, bool abiertos)
    {
        if (capasOriginales == null)
        {
            capasOriginales = new string[ojos.Length];
            for (var i = 0; i < ojos.Length; i++)
                capasOriginales[i] = ojos[i] != null ? Grupo(ojos[i]) : "Mundo";
        }

        var pantalla = PantallaJuego.Instancia;
        if (ahoraDentro && !dentro)
            penumbraAlEntrar = pantalla.Penumbra;
        if (ahoraDentro)
            pantalla.SetPenumbra(abiertos ? penumbraAlEntrar : penumbraAlEntrar * 0.55f);
        else if (dentro)
            pantalla.SetPenumbra(penumbraAlEntrar);
        dentro = ahoraDentro;

        for (var i = 0; i < ojos.Length; i++)
            if (ojos[i] != null)
                Grupo(ojos[i], ahoraDentro && abiertos ? "UI" : capasOriginales[i]);

        if (oscuridad != null)
        {
            var color = oscuridad.color;
            color.a = Mathf.MoveTowards(color.a, ahoraDentro && abiertos ? 0.96f : 0f, Time.deltaTime * 3f);
            oscuridad.color = color;
            oscuridad.enabled = color.a > 0.01f;
            if (Camera.main != null)
                oscuridad.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0f);
        }
    }

    // Capa de dibujo del ojo (la de su grupo de orden, si lo tiene)
    private static string Grupo(SpriteRenderer ojo, string nuevaCapa = null)
    {
        var grupo = ojo.GetComponentInParent<UnityEngine.Rendering.SortingGroup>();
        if (nuevaCapa != null)
        {
            if (grupo != null)
                grupo.sortingLayerName = nuevaCapa;
            else
                ojo.sortingLayerName = nuevaCapa;
        }

        return grupo != null ? grupo.sortingLayerName : ojo.sortingLayerName;
    }

    private void Poner(SpriteRenderer ojo, int cuadro)
    {
        if (ojo != null && cuadro < cuadros.Length && cuadros[cuadro] != null)
            ojo.sprite = cuadros[cuadro];
    }
}
