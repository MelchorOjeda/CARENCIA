using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Lista de pasos narrativos que se ejecutan en orden: bloquear controles, mover la cámara, activar enemigos, cargar escenas.
// Se dispara al iniciar la escena, desde un Interactable o desde una ZonaEvento, y respeta las banderas de EstadoJuego.
public class Secuencia : MonoBehaviour
{
    public enum Tipo
    {
        Esperar,
        Fundir,
        Bloquear,
        Desbloquear,
        ColocarJugador,
        Enfocar,
        VolverEncuadre,
        Activar,
        Desactivar,
        PonerBandera,
        PuntoControl,
        IniciarEnemigo,
        TerminarEnemigo,
        Curar,
        Penumbra,
        Video,
        CuadroEnBlanco,
        CargarEscena,
        // Abre una vista fija con objetos (gabinete, refrigerador) y espera a que se cierre
        AbrirVista,
        VerImagen,
        // Línea del guion: muestra en la caja de diálogo el texto de la clave guardado en SO_Dialogo_Guion
        Dialogo,
        // Fundido a negro que cierra la vista (valor: duración; valor2 = 1: al cambiar de dimensión, cambia el sonido)
        Oscurecer,
        // Pantalla provisional para una imagen o video sin arte; con valor > 0 avanza sola
        Marcador,
        // Ejecuta la secuencia de referencia si ya existen todas las banderas del texto (separadas por coma)
        EjecutarSiBanderas,
        // Suma una visita con la clave del texto; al llegar al valor ejecuta la secuencia de referencia
        Contar,
        // La siguiente muerte de Elaki lleva a esta escena (escena|entrada) en lugar de reiniciar
        MuerteLleva,
        // Lleva la referencia hacia Elaki durante valor segundos, hasta quedar a valor2 de distancia
        Acercar,
        // Cambia la penumbra hasta valor en valor2 segundos
        PenumbraGradual,
        // Con valor 1 Elaki queda sentada: no camina, pero puede revisar lo que tiene cerca. Con 0 se levanta
        Sentar,
        // Efecto de sonido de la referencia (AudioClip), una vez; valor es su volumen
        Sonido,
        // Música de fondo en bucle (AudioClip); sin referencia la detiene
        Musica,
        // Forcejeo con flechas (ForcejeoFlechas): espera a que Elaki se libere
        Forcejeo,
    }

    [Serializable]
    public class Paso
    {
        public Tipo tipo;
        public float valor = 1f;
        public float valor2;
        public string texto;
        public UnityEngine.Object referencia;
    }

    [Tooltip("Identificador para registrar que ya se ejecutó")]
    public string id;

    [Tooltip("Solo se ejecuta si esta bandera existe")]
    public string requiere;

    [Tooltip("No se ejecuta si esta bandera existe")]
    public string prohibe;

    public bool unaVez = true;
    public bool alIniciar;
    public List<Paso> pasos = new List<Paso>();

    private bool enCurso;
    private PlayerController jugador;
    private InteractionDetector detector;
    private CameraController camara;
    private InputAction accionInteractuar;
    private InputAction accionCancelar;
    private InputAction accionClic;

    private string Bandera => "sec_" + id;

    private void Start()
    {
        if (alIniciar)
            Ejecutar();
    }

    public void Ejecutar()
    {
        if (enCurso || !EstadoJuego.Tiene(requiere))
            return;

        if (!string.IsNullOrEmpty(prohibe) && EstadoJuego.Tiene(prohibe))
            return;

        if (unaVez && EstadoJuego.Tiene(Bandera))
            return;

        if (unaVez)
            EstadoJuego.Poner(Bandera);

        StartCoroutine(Correr());
    }

    private IEnumerator Correr()
    {
        enCurso = true;
        jugador = FindAnyObjectByType<PlayerController>();
        detector = jugador != null ? jugador.GetComponentInChildren<InteractionDetector>() : null;
        camara = Camera.main != null ? Camera.main.GetComponent<CameraController>() : null;
        accionInteractuar = InputSystem.actions.FindAction("Player/Interact", true);
        accionCancelar = InputSystem.actions.FindAction("UI/Cancel", true);
        accionClic = InputSystem.actions.FindAction("UI/Click", true);

        foreach (var paso in pasos)
            yield return EjecutarPaso(paso);

        enCurso = false;
    }

    private IEnumerator EjecutarPaso(Paso paso)
    {
        var pantalla = PantallaJuego.Instancia;

        switch (paso.tipo)
        {
            case Tipo.Esperar:
                yield return new WaitForSeconds(paso.valor);
                break;
            case Tipo.Fundir:
                yield return pantalla.Fundir(paso.valor, paso.valor2);
                break;
            case Tipo.Bloquear:
                Bloquear(true);
                break;
            case Tipo.Desbloquear:
                Bloquear(false);
                break;
            case Tipo.ColocarJugador:
                jugador.Colocar(((Transform)paso.referencia).position);
                break;
            case Tipo.Enfocar:
                camara.Enfocar((Transform)paso.referencia, paso.valor, paso.valor2);
                if (string.IsNullOrEmpty(paso.texto))
                    yield return new WaitForSeconds(paso.valor2);
                break;
            case Tipo.VolverEncuadre:
                camara.VolverAlEncuadre(paso.valor);
                yield return new WaitForSeconds(paso.valor);
                break;
            case Tipo.Activar:
                ((GameObject)paso.referencia).SetActive(true);
                break;
            case Tipo.Desactivar:
                ((GameObject)paso.referencia).SetActive(false);
                break;
            case Tipo.PonerBandera:
                EstadoJuego.Poner(paso.texto);
                break;
            case Tipo.PuntoControl:
                EstadoJuego.GuardarPunto(SceneManager.GetActiveScene().name, paso.texto);
                break;
            case Tipo.IniciarEnemigo:
                ((EnemigoPerseguidor)paso.referencia).Iniciar();
                break;
            case Tipo.TerminarEnemigo:
                ((EnemigoPerseguidor)paso.referencia).Terminar();
                break;
            case Tipo.Curar:
                EstadoJuego.Golpes = 0;
                pantalla.SetManchas(0f);
                FindAnyObjectByType<SaludJugador>()?.Curar();
                break;
            case Tipo.Penumbra:
                pantalla.SetPenumbra(paso.valor);
                break;
            case Tipo.Video:
                // Con una PantallaMonitor como referencia, el video se ve en el monitor del escenario
                var rutina = paso.referencia is PantallaMonitor monitor
                    ? VideoEnMonitor(monitor, paso.valor)
                    : ReproducirVideo(paso.valor);
                if (paso.texto == "sinEspera")
                    StartCoroutine(rutina);
                else
                    yield return rutina;
                break;
            case Tipo.CuadroEnBlanco:
                yield return CuadroEnBlanco();
                break;
            case Tipo.CargarEscena:
                // Formato: escena|entrada
                var partes = paso.texto.Split('|');
                pantalla.CargarEscena(partes[0], partes.Length > 1 ? partes[1] : null);
                break;
            case Tipo.VerImagen:
                yield return VerImagen((Sprite)paso.referencia);
                break;
            case Tipo.AbrirVista:
                // Espera a que se cierre, para que lo que sigue (por ejemplo un pensamiento) vaya después
                var vista = (VistaDeObjetos)paso.referencia;
                vista.Abrir();
                yield return null;
                while (vista.Abierta)
                    yield return null;
                // La vista libera la interacción un cuadro después de cerrarse; si el paso siguiente (un diálogo)
                // empezara antes, guardaría "bloqueada" como el estado a restaurar y Elaki ya no podría usar nada
                yield return null;
                yield return null;
                break;
            case Tipo.Dialogo:
                yield return Dialogo(paso.texto);
                break;
            case Tipo.Oscurecer:
                yield return pantalla.Oscurecer(paso.valor);
                break;
            case Tipo.Marcador:
                yield return Marcador(paso.texto, paso.valor);
                break;
            case Tipo.EjecutarSiBanderas:
                if (paso.texto.Split(',').All(b => EstadoJuego.Tiene(b.Trim())))
                    ((Secuencia)paso.referencia).Ejecutar();
                break;
            case Tipo.Contar:
                var visitas = 1;
                while (EstadoJuego.Tiene(paso.texto + "_" + visitas))
                    visitas++;
                EstadoJuego.Poner(paso.texto + "_" + visitas);
                if (visitas == Mathf.RoundToInt(paso.valor))
                    ((Secuencia)paso.referencia).Ejecutar();
                break;
            case Tipo.MuerteLleva:
                var salud = FindAnyObjectByType<SaludJugador>();
                if (salud != null)
                    salud.LlevarAlMorir(paso.texto);
                break;
            case Tipo.Acercar:
                StartCoroutine(Acercar((Transform)paso.referencia, paso.valor, paso.valor2));
                break;
            case Tipo.PenumbraGradual:
                StartCoroutine(PenumbraGradual(paso.valor, paso.valor2));
                break;
            case Tipo.Sentar:
                if (jugador != null)
                    jugador.MovimientoBloqueado = paso.valor > 0.5f;
                if (detector != null)
                    detector.InteraccionBloqueada = false;
                break;
            case Tipo.Sonido:
                pantalla.ReproducirEfecto(paso.referencia as AudioClip, paso.valor);
                break;
            case Tipo.Musica:
                pantalla.ReproducirMusica(paso.referencia as AudioClip);
                break;
            case Tipo.Forcejeo:
                yield return ((ForcejeoFlechas)paso.referencia).Jugar();
                break;
        }
    }

    private void Bloquear(bool bloqueado)
    {
        if (jugador != null)
            jugador.MovimientoBloqueado = bloqueado;

        if (detector != null)
            detector.InteraccionBloqueada = bloqueado;
    }

    // El video definitivo se conecta aquí; mientras tanto se muestra una pantalla con su barra de avance
    private IEnumerator ReproducirVideo(float duracion)
    {
        var pantalla = PantallaJuego.Instancia;
        pantalla.MostrarVideo(true);

        for (var t = 0f; t < duracion; t += Time.deltaTime)
        {
            pantalla.SetAvanceVideo(t / duracion);
            yield return null;
        }

        pantalla.MostrarVideo(false);
    }

    private IEnumerator VideoEnMonitor(PantallaMonitor monitor, float duracion)
    {
        monitor.gameObject.SetActive(true);

        for (var t = 0f; t < duracion; t += Time.deltaTime)
        {
            monitor.SetAvance(t / duracion);
            yield return null;
        }

        monitor.SetAvance(1f);
        yield return new WaitForSeconds(0.3f);
        monitor.gameObject.SetActive(false);
    }

    // Las vistas de inspección y los marcadores se cierran con E, Esc o clic
    private bool Salir()
    {
        return accionInteractuar.WasPressedThisFrame() || accionCancelar.WasPressedThisFrame() || accionClic.WasPressedThisFrame();
    }

    private IEnumerator Dialogo(string clave)
    {
        var linea = PantallaJuego.Instancia.Dialogo.LineaDe(clave);

        // "Sin texto" en el guion: ni se abre la caja ni se detiene a Elaki
        if (linea.TieneTexto)
            yield return MostrarEnCaja(linea);
    }

    // Al terminar deja a Elaki como estaba: si estaba sentada en la mesa, sigue sentada
    private IEnumerator MostrarEnCaja(LineaDialogo linea)
    {
        var pantalla = PantallaJuego.Instancia;
        // Si ya hay otra línea en pantalla, esta espera su turno
        while (pantalla.CuadroVisible)
            yield return null;

        var movimiento = jugador != null && jugador.MovimientoBloqueado;
        var interaccion = detector != null && detector.InteraccionBloqueada;
        Bloquear(true);
        yield return pantalla.Dialogo.Mostrar(linea);

        // La pulsación que cierra la línea no debe activar el objeto que está enfrente
        yield return null;
        if (jugador != null)
            jugador.MovimientoBloqueado = movimiento;
        if (detector != null)
            detector.InteraccionBloqueada = interaccion;
    }

    private IEnumerator Marcador(string texto, float duracion)
    {
        var pantalla = PantallaJuego.Instancia;
        pantalla.MostrarMarcador(texto);
        yield return null;

        if (duracion > 0f)
            yield return new WaitForSeconds(duracion);
        else
            while (!Salir())
                yield return null;

        pantalla.MostrarMarcador(null);
    }

    private IEnumerator Acercar(Transform objeto, float duracion, float distanciaFinal)
    {
        var destino = jugador.transform.position;
        var inicio = objeto.position;
        var direccion = (destino - inicio).normalized;
        var final = destino - direccion * distanciaFinal;
        var animador = objeto.GetComponent<Animator>();
        for (var t = 0f; t < duracion; t += Time.deltaTime)
        {
            objeto.position = Vector3.Lerp(inicio, final, t / duracion);
            if (animador != null)
            {
                animador.SetFloat("DirX", direccion.x);
                animador.SetFloat("DirY", direccion.y);
            }

            yield return null;
        }

        objeto.position = final;
    }

    private IEnumerator PenumbraGradual(float destino, float duracion)
    {
        var pantalla = PantallaJuego.Instancia;
        var inicio = pantalla.Penumbra;
        for (var t = 0f; t < duracion; t += Time.deltaTime)
        {
            pantalla.SetPenumbra(Mathf.Lerp(inicio, destino, t / duracion));
            yield return null;
        }

        pantalla.SetPenumbra(destino);
    }

    // Vista fija de un objeto; se cierra con E o Esc
    private IEnumerator VerImagen(Sprite sprite)
    {
        var pantalla = PantallaJuego.Instancia;
        Bloquear(true);
        pantalla.MostrarImagen(sprite);
        yield return null;

        while (!Salir())
            yield return null;

        pantalla.MostrarImagen(null);
        yield return null;
        Bloquear(false);
    }

    // La caja vacía queda abierta hasta que el jugador la cierra
    private IEnumerator CuadroEnBlanco()
    {
        return MostrarEnCaja(new LineaDialogo());
    }
}
