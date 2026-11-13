using System.Collections;
using UnityEngine;

// Desenlace del Nivel 3: Elaki camina a la orilla de la tina, se recarga, entra al agua y abraza a Margarita
// (cuadros dibujados sobre la misma tina, fundidos uno con otro). Un parpadeo corta la visión y, al volver,
// queda él solo, acostado con los ojos abiertos justo donde estaba ella.
// Empieza al activarse este objeto (paso "Activar" de una Secuencia) y al terminar ejecuta la secuencia siguiente.
public class AbrazoTina : MonoBehaviour
{
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");
    private static readonly int Caminando = Animator.StringToHash("Caminando");

    [Tooltip("Puntos por los que pasa antes de la orilla, para rodear la tina")]
    [SerializeField] private Transform[] camino = new Transform[0];
    [Tooltip("Donde se para Elaki: sus pies coinciden con los del primer cuadro")]
    [SerializeField] private Transform orilla;
    [SerializeField] private Transform enfoque;
    [SerializeField] private float tamanoCamara = 1.9f;
    [SerializeField] private float velocidadCaminar = 1.4f;

    [Header("Cuadros del abrazo")]
    [SerializeField] private SpriteRenderer capa;
    [SerializeField] private SpriteRenderer capaAnterior;
    [SerializeField] private Sprite[] cuadros = new Sprite[0];
    [Tooltip("Desde este cuadro Margarita se incorpora y deja de verse recostada")]
    [SerializeField] private int cuadroSeIncorpora = 7;

    [Header("Tina")]
    [Tooltip("Margarita recostada en el agua; al final, el joven en su lugar")]
    [SerializeField] private SpriteRenderer enTina;
    [SerializeField] private Sprite jovenEnTina;
    [Tooltip("Cuánto se recorre el joven dentro de la tina (local) para que su cabeza quede en el extremo del fondo")]
    [SerializeField] private Vector3 desplazamientoJoven;
    [SerializeField] private AudioClip sonidoAgua;
    [SerializeField] private float esperaFinal = 4f;

    [Header("Cinemática")]
    [Tooltip("Si se asigna, en lugar de los cuadros del abrazo la cámara se acerca a la tina, funde a negro y se reproduce " +
             "esta cinemática (un lienzo con su VideoPlayer) a pantalla completa")]
    [SerializeField] private GameObject cinematica;
    [SerializeField] private float duracionCinematica;
    [SerializeField] private Secuencia alTerminar;

    private bool hecho;

    private void OnEnable()
    {
        if (!hecho)
            StartCoroutine(Rutina());
    }

    private IEnumerator Rutina()
    {
        hecho = true;
        var pantalla = PantallaJuego.Instancia;
        var jugador = FindAnyObjectByType<PlayerController>();
        var detector = jugador != null ? jugador.GetComponentInChildren<InteractionDetector>() : null;
        var camara = Camera.main != null ? Camera.main.GetComponent<CameraController>() : null;
        if (jugador != null)
            jugador.MovimientoBloqueado = true;
        if (detector != null)
            detector.InteraccionBloqueada = true;

        // Con cinemática no hay nada más: en cuanto se presiona E, la cámara se acerca y entra el video
        if (cinematica != null)
        {
            yield return Cinematica(pantalla, camara, jugador);
            yield break;
        }

        if (camara != null && enfoque != null)
            camara.Enfocar(enfoque, tamanoCamara, 1.6f);

        // Camina hasta la orilla, rodeando la tina
        if (jugador != null)
        {
            foreach (var punto in camino)
                if (punto != null)
                    yield return Caminar(jugador, punto.position);
            if (orilla != null)
                yield return Caminar(jugador, orilla.position);
        }
        yield return new WaitForSeconds(0.5f);

        // Relevo: el sprite jugable se funde con el primer cuadro en el mismo punto del piso
        var figura = jugador != null ? jugador.GetComponentInChildren<SpriteRenderer>() : null;
        var hayCuadros = capa != null && capaAnterior != null && cuadros.Length > 0;
        if (hayCuadros)
        {
            capa.sprite = cuadros[0];
            Alfa(capa, 0f);
            capa.enabled = true;
        }

        for (var t = 0f; t < 0.5f; t += Time.deltaTime)
        {
            Alfa(figura, 1f - t / 0.5f);
            Alfa(capa, t / 0.5f);
            yield return null;
        }

        if (figura != null)
            figura.enabled = false;
        Alfa(figura, 1f);
        Alfa(capa, 1f);

        if (hayCuadros)
        {
            // Se acerca, se recarga, entra y la abraza
            for (var i = 1; i < cuadros.Length; i++)
            {
                if (i == cuadroSeIncorpora - 2 && sonidoAgua != null)
                    pantalla.ReproducirEfecto(sonidoAgua, 0.8f);
                yield return Fundir(cuadros[i], i == cuadroSeIncorpora, i < 4 ? 0.45f : 0.6f, i < 4 ? 0.3f : 0.5f);
            }

            yield return new WaitForSeconds(1.6f);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        // Parpadeo: la visión se corta mientras siguen abrazados
        yield return Parpadeo(pantalla);

        if (capa != null)
            capa.enabled = false;
        if (capaAnterior != null)
            capaAnterior.enabled = false;
        if (enTina != null && jovenEnTina != null)
        {
            enTina.sprite = jovenEnTina;
            enTina.transform.localPosition += desplazamientoJoven;
            enTina.enabled = true;
            Alfa(enTina, 1f);
        }

        yield return new WaitForSeconds(1.8f);
        yield return pantalla.Fundir(0f, 2.2f);

        // Solo él, inmóvil en el agua, donde estaba ella
        yield return new WaitForSeconds(esperaFinal);

        if (alTerminar != null)
            alTerminar.Ejecutar();
    }

    // Cambio de cámara: se acerca a la tina mientras todo se va a negro y entra la cinemática a pantalla completa
    private IEnumerator Cinematica(PantallaJuego pantalla, CameraController camara, PlayerController jugador)
    {
        if (camara != null && enfoque != null)
            camara.Enfocar(enfoque, tamanoCamara * 0.7f, 0.8f);
        yield return pantalla.Fundir(1f, 0.6f);

        var figura = jugador != null ? jugador.GetComponentInChildren<SpriteRenderer>() : null;
        if (figura != null)
            figura.enabled = false;

        cinematica.SetActive(true);
        var video = cinematica.GetComponentInChildren<UnityEngine.Video.VideoPlayer>();
        // Espera a que el video arranque (prepararlo tarda un momento) y luego lo que dura
        for (var t = 0f; video != null && !video.isPlaying && t < 3f; t += Time.deltaTime)
            yield return null;
        var duracion = duracionCinematica > 0f ? duracionCinematica : video != null && video.clip != null ? (float)video.clip.length : 3f;
        for (var t = 0f; t < duracion && (video == null || video.isPlaying || t < 0.5f); t += Time.deltaTime)
            yield return null;
        yield return new WaitForSeconds(0.3f);

        // Queda en negro (el fundido sigue arriba) y sigue la escena final
        cinematica.SetActive(false);
        if (alTerminar != null)
            alTerminar.Ejecutar();
    }

    private IEnumerator Caminar(PlayerController jugador, Vector2 destino)
    {
        var animador = jugador.GetComponent<AnimadorJugador>();
        var animator = jugador.GetComponent<Animator>();
        if (animador != null)
            animador.enabled = false;

        // Sin física mientras camina: el choque con la tina lo empujaría lejos del primer cuadro.
        // No hace falta devolverla: después de esto Elaki solo aparece dentro de los cuadros del abrazo.
        var cuerpo = jugador.GetComponent<Rigidbody2D>();
        if (cuerpo != null)
            cuerpo.simulated = false;

        Vector2 inicio = jugador.transform.position;
        var direccion = (destino - inicio).normalized;
        var duracion = Vector2.Distance(inicio, destino) / velocidadCaminar;
        if (animator != null && duracion > 0.05f)
        {
            animator.SetFloat(DirX, direccion.x);
            animator.SetFloat(DirY, direccion.y);
            animator.SetBool(Caminando, true);
        }

        for (var t = 0f; t < duracion; t += Time.deltaTime)
        {
            jugador.Colocar(Vector2.Lerp(inicio, destino, t / duracion));
            yield return null;
        }

        jugador.Colocar(destino);
        if (animator != null)
            animator.SetBool(Caminando, false);
        if (animador != null)
            animador.enabled = true;
    }

    // Funde el cuadro actual con el siguiente; si se pide, también desvanece a Margarita recostada
    private IEnumerator Fundir(Sprite siguiente, bool ocultarRecostada, float fundido, float espera)
    {
        capaAnterior.sprite = capa.sprite;
        capaAnterior.enabled = true;
        Alfa(capaAnterior, 1f);
        capa.sprite = siguiente;
        for (var t = 0f; t < fundido; t += Time.deltaTime)
        {
            var k = t / fundido;
            Alfa(capa, Mathf.Min(1f, k * 2f));
            Alfa(capaAnterior, Mathf.Min(1f, (1f - k) * 2f));
            if (ocultarRecostada)
                Alfa(enTina, 1f - k);
            yield return null;
        }

        Alfa(capa, 1f);
        capaAnterior.enabled = false;
        if (ocultarRecostada && enTina != null)
            enTina.enabled = false;
        yield return new WaitForSeconds(espera);
    }

    // Dos parpadeos y negro
    private static IEnumerator Parpadeo(PantallaJuego pantalla)
    {
        float[] marcas = { 0f, 0.85f, 0.3f, 1f };
        float[] tiempos = { 0.35f, 0.25f, 0.6f };
        for (var i = 0; i < tiempos.Length; i++)
            for (var t = 0f; t < tiempos[i]; t += Time.deltaTime)
            {
                pantalla.SetFundido(Mathf.Lerp(marcas[i], marcas[i + 1], t / tiempos[i]));
                yield return null;
            }

        pantalla.SetFundido(1f);
    }

    private static void Alfa(SpriteRenderer dibujo, float alfa)
    {
        if (dibujo == null)
            return;

        var color = dibujo.color;
        color.a = alfa;
        dibujo.color = color;
    }
}
