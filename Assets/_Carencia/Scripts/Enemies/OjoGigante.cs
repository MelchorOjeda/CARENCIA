using UnityEngine;
using UnityEngine.UI;

// Nivel 3: el ojo enorme de la ventana al fondo del pasillo. Se enciende al activar este objeto y se apaga al desactivarlo.
// Cerrado, Elaki puede caminar. Cuando se abre, la pantalla se oscurece y el ojo la sigue con la pupila:
// si se mueve dentro del pasillo, se lastima poco a poco; si se queda quieta, el daño para.
// El ojo se dibuja también por encima de la oscuridad, para que brille al fondo aunque todo lo demás sea negro.
public class OjoGigante : MonoBehaviour
{
    private enum Fase { Cerrado, Abriendo, Abierto, Cerrando }

    [SerializeField] private SpriteRenderer ojo;
    // 0 cerrado, 1 entreabierto, 2 abierto
    [SerializeField] private Sprite[] cuadros = new Sprite[3];
    [SerializeField] private Sprite pupila;
    [SerializeField] private Vector2 centroPupila;
    [SerializeField] private float radioPupila = 0.08f;
    [SerializeField] private Collider2D zona;
    [SerializeField] private float danoPorSegundo = 0.8f;
    [Tooltip("El piso del pasillo es blando: dentro de la zona Elaki camina más lento")]
    [SerializeField] private float freno = 0.45f;
    [SerializeField] private Vector2 cerrado = new Vector2(1.5f, 2.3f);
    [SerializeField] private float abriendo = 0.8f;
    [SerializeField] private Vector2 abierto = new Vector2(2.6f, 3.6f);
    [SerializeField] private AudioSource sonidoAbrir;
    [SerializeField] private AudioSource sonidoDano;

    private PlayerController jugador;
    private SaludJugador salud;
    private Fase fase;
    private float tiempo;
    private float duracion;
    private float oscuridad;
    private Canvas capa;
    private Image ojoUI;
    private Image pupilaUI;
    private Transform pupilaMundo;

    public bool Abierto => fase == Fase.Abierto;

    public void Configurar(SpriteRenderer nuevoOjo, Sprite[] nuevosCuadros, Sprite nuevaPupila, Vector2 nuevoCentro, Collider2D nuevaZona, AudioSource abrir, AudioSource dano)
    {
        ojo = nuevoOjo;
        cuadros = nuevosCuadros;
        pupila = nuevaPupila;
        centroPupila = nuevoCentro;
        zona = nuevaZona;
        sonidoAbrir = abrir;
        sonidoDano = dano;
    }

    private void OnEnable()
    {
        var control = FindAnyObjectByType<PlayerController>();
        jugador = control;
        salud = control != null ? control.GetComponent<SaludJugador>() : null;
        CrearCapa();
        // La primera vez se abre pronto: Elaki acaba de leer que no se puede mover cuando la ve
        Cambiar(Fase.Cerrado, 2f);
    }

    private void OnDisable()
    {
        if (jugador != null)
            jugador.Freno = 1f;
        oscuridad = 0f;
        if (PantallaJuego.Instancia != null)
            PantallaJuego.Instancia.SetMirada(0f);
        if (sonidoDano != null)
            sonidoDano.Stop();
        if (capa != null)
            capa.gameObject.SetActive(false);
        Poner(0);
    }

    private void Update()
    {
        tiempo += Time.deltaTime;
        if (jugador != null)
            jugador.Freno = zona == null || zona.OverlapPoint(jugador.transform.position) ? freno : 1f;
        var meta = 0f;
        switch (fase)
        {
            case Fase.Cerrado:
                Poner(0);
                if (tiempo >= duracion)
                {
                    Cambiar(Fase.Abriendo, abriendo);
                    if (sonidoAbrir != null)
                        sonidoAbrir.Play();
                }
                break;
            case Fase.Abriendo:
                Poner(1);
                meta = 0.55f;
                if (tiempo >= duracion)
                    Cambiar(Fase.Abierto, Random.Range(abierto.x, abierto.y));
                break;
            case Fase.Abierto:
                Poner(2);
                meta = 0.93f;
                Vigilar();
                if (tiempo >= duracion)
                    Cambiar(Fase.Cerrando, 0.35f);
                break;
            case Fase.Cerrando:
                Poner(1);
                meta = 0.4f;
                if (tiempo >= duracion)
                    Cambiar(Fase.Cerrado, Random.Range(cerrado.x, cerrado.y));
                break;
        }

        if (fase != Fase.Abierto && sonidoDano != null && sonidoDano.isPlaying)
            sonidoDano.Stop();

        oscuridad = Mathf.MoveTowards(oscuridad, meta, Time.deltaTime * 2.5f);
        if (PantallaJuego.Instancia != null)
            PantallaJuego.Instancia.SetMirada(oscuridad);
    }

    private void Vigilar()
    {
        if (jugador == null || salud == null)
            return;

        var dentro = zona == null || zona.OverlapPoint(jugador.transform.position);
        var moviendose = dentro && jugador.EstaCaminando && !jugador.MovimientoBloqueado;
        if (moviendose)
        {
            salud.Herir(danoPorSegundo * Time.deltaTime);
            if (sonidoDano != null && !sonidoDano.isPlaying)
                sonidoDano.Play();
        }
        else if (sonidoDano != null && sonidoDano.isPlaying)
        {
            sonidoDano.Stop();
        }
    }

    private void Cambiar(Fase nueva, float nuevaDuracion)
    {
        fase = nueva;
        tiempo = 0f;
        duracion = nuevaDuracion;
    }

    private void Poner(int cuadro)
    {
        if (ojo == null || cuadro >= cuadros.Length || cuadros[cuadro] == null)
            return;
        ojo.sprite = cuadros[cuadro];
        if (ojoUI != null)
            ojoUI.sprite = cuadros[cuadro];
        if (pupilaMundo != null)
            pupilaMundo.gameObject.SetActive(cuadro == 2);
        if (pupilaUI != null)
            pupilaUI.enabled = cuadro == 2;
    }

    // Copia del ojo en una capa de interfaz por encima de la penumbra y de la oscuridad de la mirada
    private void CrearCapa()
    {
        if (capa != null)
        {
            capa.gameObject.SetActive(true);
            return;
        }

        var objeto = new GameObject("OjoSobreLaOscuridad");
        objeto.transform.SetParent(transform, false);
        capa = objeto.AddComponent<Canvas>();
        capa.renderMode = RenderMode.ScreenSpaceOverlay;
        // Encima de la penumbra (100) pero sin tapar la caja de diálogo, que vive en ese mismo lienzo
        capa.sortingOrder = 101;
        ojoUI = Imagen("Ojo", objeto.transform);
        pupilaUI = Imagen("Pupila", objeto.transform);
        pupilaUI.sprite = pupila;

        if (pupila != null && ojo != null)
        {
            var p = new GameObject("PupilaMundo");
            p.transform.SetParent(ojo.transform, false);
            var srP = p.AddComponent<SpriteRenderer>();
            srP.sprite = pupila;
            srP.sortingLayerID = ojo.sortingLayerID;
            srP.sortingOrder = ojo.sortingOrder + 1;
            srP.sharedMaterial = ojo.sharedMaterial;
            pupilaMundo = p.transform;
        }
    }

    private static Image Imagen(string nombre, Transform padre)
    {
        var objeto = new GameObject(nombre, typeof(RectTransform));
        objeto.transform.SetParent(padre, false);
        var imagen = objeto.AddComponent<Image>();
        imagen.raycastTarget = false;
        imagen.preserveAspect = true;
        return imagen;
    }

    private void LateUpdate()
    {
        var camara = Camera.main;
        if (camara == null || ojo == null || ojoUI == null)
            return;

        // La pupila sigue a Elaki
        var centro = ojo.transform.TransformPoint(centroPupila);
        var hacia = jugador != null ? ((Vector2)(jugador.transform.position - centro)).normalized : Vector2.zero;
        var posPupila = centro + (Vector3)(hacia * radioPupila);
        if (pupilaMundo != null)
            pupilaMundo.position = new Vector3(posPupila.x, posPupila.y, ojo.transform.position.z);

        // Pixeles de pantalla por unidad del mundo
        var escala = Screen.height / (2f * camara.orthographicSize);
        var tamano = Tamano(ojo.sprite, ojo.transform.lossyScale, escala);
        var enPantalla = (Vector2)camara.WorldToScreenPoint(ojo.transform.position);
        // La ventana queda al fondo del pasillo, casi siempre fuera de cuadro: el ojo se asoma por el borde
        // de la pantalla del lado de la ventana, así Elaki lo tiene a la vista mientras cruza
        var margen = tamano * 0.5f + new Vector2(12f, 12f);
        var visible = new Vector2(
            Mathf.Clamp(enPantalla.x, margen.x, Screen.width - margen.x),
            Mathf.Clamp(enPantalla.y, margen.y, Screen.height - margen.y));
        Colocar(ojoUI, ojo.sprite, tamano, visible);
        ojoUI.color = new Color(1f, 1f, 1f, fase == Fase.Cerrado ? 0.55f : 1f);
        if (pupilaUI.enabled && pupila != null)
        {
            // La pupila, sobre la copia del ojo, corrida hacia donde está Elaki en la pantalla
            var aJugador = jugador != null ? ((Vector2)camara.WorldToScreenPoint(jugador.transform.position) - visible).normalized : Vector2.zero;
            Colocar(pupilaUI, pupila, Tamano(pupila, ojo.transform.lossyScale, escala), visible + aJugador * radioPupila * escala);
        }
    }

    // Dónde quedó el ojo en la pantalla (para las pruebas)
    public Vector2 PosicionEnPantalla => ojoUI != null ? (Vector2)ojoUI.rectTransform.position : Vector2.zero;

    private static Vector2 Tamano(Sprite sprite, Vector3 escalaMundo, float escala)
    {
        if (sprite == null)
            return Vector2.zero;
        var t = sprite.rect.size / sprite.pixelsPerUnit * escala;
        return new Vector2(t.x * Mathf.Abs(escalaMundo.x), t.y * Mathf.Abs(escalaMundo.y));
    }

    private static void Colocar(Image imagen, Sprite sprite, Vector2 tamano, Vector2 centro)
    {
        if (sprite == null)
            return;
        var rect = imagen.rectTransform;
        rect.sizeDelta = tamano;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchorMin = rect.anchorMax = Vector2.zero;
        rect.position = centro;
    }
}
