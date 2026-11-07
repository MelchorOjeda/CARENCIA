using System;
using System.Collections;
using System.Collections.Generic;
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
        AbrirGabinete,
        VerImagen,
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
            case Tipo.AbrirGabinete:
                ((VistaGabinete)paso.referencia).Abrir();
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

    // Las vistas de inspección se cierran con E o con Esc
    private bool Salir()
    {
        return accionInteractuar.WasPressedThisFrame() || accionCancelar.WasPressedThisFrame();
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

    // Queda abierto hasta pulsar E: el equipo de diálogos pondrá aquí sus textos
    private IEnumerator CuadroEnBlanco()
    {
        var pantalla = PantallaJuego.Instancia;
        Bloquear(true);
        pantalla.MostrarCuadro(true);
        yield return null;

        while (!Salir())
            yield return null;

        pantalla.MostrarCuadro(false);
        yield return null;
        Bloquear(false);
    }
}
