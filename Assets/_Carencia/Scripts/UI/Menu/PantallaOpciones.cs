using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Hoja de Opciones con cuatro pestañas. Los cambios se prueban al momento y se guardan con G;
// Esc con cambios sin guardar pregunta antes de descartarlos, y R restablece solo la pestaña actual.
public class PantallaOpciones : MonoBehaviour
{
    private static readonly string[] NombresPestana = { "Audio", "Video", "Controles", "Accesibilidad" };

    [SerializeField] private PestanaOpciones[] pestanas;
    [SerializeField] private FilaOpcion[] filas;
    [SerializeField] private TMP_Text nombreDescripcion;
    [SerializeField] private TMP_Text textoDescripcion;
    [SerializeField] private GameObject selloSinGuardar;
    [SerializeField] private GameObject avisoGuardado;
    [SerializeField] private GameObject ayudaRestablecer;
    [SerializeField] private GameObject avisoReasignacion;
    [SerializeField] private NotaConfirmacion nota;

    private Ajustes enEdicion;
    private IReadOnlyList<Resolucion> resoluciones;
    private Renglon[] renglones;
    private Categoria categoria;
    private int filaEnfocada;
    private Action alCerrar;
    private Coroutine rutinaAviso;

    private bool HaySinGuardar => !enEdicion.IgualA(AjustesJuego.Guardados);

    private void Awake()
    {
        foreach (var pestana in pestanas)
            pestana.Pulsada += pulsada => CambiarCategoria((Categoria)Array.IndexOf(pestanas, pulsada));

        foreach (var fila in filas)
        {
            fila.Apuntada += apuntada => Enfocar(Array.IndexOf(filas, apuntada));
            fila.Ajustada += (ajustada, direccion) => AjustarFila(Array.IndexOf(filas, ajustada), direccion);
        }
    }

    public void Abrir(Action siCierra)
    {
        alCerrar = siCierra;
        enEdicion = AjustesJuego.Guardados.Copia();
        AjustesJuego.CompletarVideo(enEdicion);
        resoluciones = AjustesJuego.ResolucionesDisponibles();

        gameObject.SetActive(true);
        avisoGuardado.SetActive(false);
        CambiarCategoria(Categoria.Audio);
        ControlesMenu.Bloquear();
    }

    private void Update()
    {
        if (!ControlesMenu.Listo || nota.Abierta)
            return;

        var pestana = ControlesMenu.Pestana();
        var vertical = ControlesMenu.Vertical();
        var horizontal = ControlesMenu.Horizontal();

        if (pestana != 0)
            CambiarCategoria((Categoria)Ciclo.Siguiente((int)categoria, pestanas.Length, pestana));
        else if (vertical != 0)
            Enfocar(Ciclo.Siguiente(filaEnfocada, renglones.Length, vertical));
        else if (horizontal != 0)
            AjustarFila(filaEnfocada, horizontal);
        else if (ControlesMenu.Pulso(ControlesMenu.Confirmar) && !renglones[filaEnfocada].EsDeslizador)
            AjustarFila(filaEnfocada, +1);
        else if (ControlesMenu.Pulso(ControlesMenu.Guardar))
            Guardar();
        else if (ControlesMenu.Pulso(ControlesMenu.Restablecer) && categoria != Categoria.Controles)
            PreguntarRestablecer();
        else if (ControlesMenu.Pulso(ControlesMenu.Volver))
            PedirSalir();
    }

    private void CambiarCategoria(Categoria nueva)
    {
        categoria = nueva;
        renglones = RenglonesDe(nueva);
        filaEnfocada = 0;
        Refrescar();
    }

    private void Enfocar(int fila)
    {
        if (fila < 0 || fila >= renglones.Length)
            return;

        filaEnfocada = fila;
        Refrescar();
    }

    private void AjustarFila(int fila, int direccion)
    {
        if (fila < 0 || fila >= renglones.Length || renglones[fila].Cambiar == null)
            return;

        filaEnfocada = fila;
        renglones[fila].Cambiar(direccion);
        AjustesJuego.Previsualizar(enEdicion);
        Refrescar();
    }

    private void Guardar()
    {
        AjustesJuego.Guardar(enEdicion);
        Refrescar();
        if (rutinaAviso != null)
            StopCoroutine(rutinaAviso);

        rutinaAviso = StartCoroutine(MostrarAvisoGuardado());
    }

    private void PreguntarRestablecer()
    {
        nota.Preguntar(
            "¿Restablecer valores predeterminados?",
            $"Solo cambia la pestaña {NombresPestana[(int)categoria]}. Tu partida guardada no se verá afectada.",
            "Sí, restablecer",
            Restablecer);
    }

    private void Restablecer()
    {
        enEdicion.Restablecer(categoria);
        AjustesJuego.CompletarVideo(enEdicion);
        AjustesJuego.Previsualizar(enEdicion);
        Refrescar();
    }

    private void PedirSalir()
    {
        if (FlujoMenu.AlSalirDeOpciones(HaySinGuardar) == Decision.Hacerlo)
        {
            Cerrar();
            return;
        }

        nota.Preguntar(
            "¿Salir sin guardar?",
            "Los cambios que no guardaste se perderán. Pulsa G para guardarlos.",
            "Sí, salir",
            Cerrar);
    }

    private void Cerrar()
    {
        AjustesJuego.Descartar();
        gameObject.SetActive(false);
        ControlesMenu.Bloquear();
        alCerrar?.Invoke();
    }

    private void Refrescar()
    {
        for (var i = 0; i < pestanas.Length; i++)
            pestanas[i].MostrarActiva(i == (int)categoria);

        for (var i = 0; i < filas.Length; i++)
            MostrarFila(i);

        var enfocado = renglones[filaEnfocada];
        nombreDescripcion.text = enfocado.Nombre;
        textoDescripcion.text = enfocado.Descripcion;
        selloSinGuardar.SetActive(HaySinGuardar);
        ayudaRestablecer.SetActive(categoria != Categoria.Controles);
        avisoReasignacion.SetActive(categoria == Categoria.Controles);
    }

    private void MostrarFila(int i)
    {
        var visible = i < renglones.Length;
        filas[i].gameObject.SetActive(visible);
        if (!visible)
            return;

        var renglon = renglones[i];
        filas[i].Mostrar(renglon.Nombre, renglon.TextoValor(), renglon.Porcentaje?.Invoke());
        filas[i].MostrarEnfoque(i == filaEnfocada);
    }

    private IEnumerator MostrarAvisoGuardado()
    {
        avisoGuardado.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        avisoGuardado.SetActive(false);
        rutinaAviso = null;
    }

    private Renglon[] RenglonesDe(Categoria pestana)
    {
        switch (pestana)
        {
            case Categoria.Audio:
                return new[]
                {
                    Deslizador("Volumen general", "Volumen de todo el juego.", () => enEdicion.volumenGeneral, Ajuste.VolumenGeneral),
                    Deslizador("Música", "Volumen de la música ambiental y de las persecuciones.", () => enEdicion.volumenMusica, Ajuste.VolumenMusica),
                    Deslizador("Efectos de sonido", "Volumen de los pasos, las puertas, los golpes y los demás sonidos de la casa.", () => enEdicion.volumenEfectos, Ajuste.VolumenEfectos),
                    Deslizador("Voces", "Volumen de las voces de los personajes.", () => enEdicion.volumenVoces, Ajuste.VolumenVoces),
                };
            case Categoria.Video:
                return new[]
                {
                    Selector("Resolución", "Tamaño de la imagen. Usa la de tu monitor para que el pixel art se vea nítido. Se aplica al guardar.",
                        () => enEdicion.Resolucion.ToString(), direccion => enEdicion.CambiarResolucion(direccion, resoluciones)),
                    Selector("Modo de pantalla", "Pantalla completa o ventana. Se aplica al guardar.",
                        () => enEdicion.pantallaCompleta ? "Pantalla completa" : "Ventana", direccion => enEdicion.Cambiar(Ajuste.PantallaCompleta, direccion)),
                    Selector("Calidad gráfica", "Calidad de las luces y los efectos. Bájala si el juego va lento. Se aplica al guardar.",
                        () => NombreCalidad(enEdicion.calidad), direccion => enEdicion.CambiarCalidad(direccion, QualitySettings.count)),
                    Deslizador("Brillo", "Aclara u oscurece la imagen. Ajústalo hasta distinguir las sombras sin que se vean grises.", () => enEdicion.brillo, Ajuste.Brillo),
                };
            case Categoria.Controles:
                return new[]
                {
                    SoloLectura("Moverse", "W A S D / flechas", "Mueve a Elaki por la casa."),
                    SoloLectura("Correr", "Shift", "Mantenlo pulsado mientras te mueves para escapar de las persecuciones."),
                    SoloLectura("Interactuar", "E", "Abre puertas, usa objetos y avanza los diálogos."),
                    SoloLectura("Inspeccionar", "Clic izquierdo", "Haz clic en un objeto que se ilumina, estando cerca, para examinarlo."),
                    SoloLectura("Pausa", "Esc", "Detiene el juego. Desde ahí puedes cambiar las opciones o volver al menú."),
                };
            default:
                return new[]
                {
                    Selector("Tamaño de texto", "Agranda o achica los textos de los menús, las notas y las descripciones.",
                        () => NombreTamano(enEdicion.tamanoTexto), direccion => enEdicion.Cambiar(Ajuste.TamanoTexto, direccion)),
                    SiNo("Subtítulos", "Muestra en texto lo que dicen las voces, cuando el juego las tenga.", () => enEdicion.subtitulos, Ajuste.Subtitulos),
                    SiNo("Contraste alto", "Aclara los textos grises para que se lean mejor sobre el fondo oscuro.", () => enEdicion.contrasteAlto, Ajuste.ContrasteAlto),
                    SiNo("Reducir destellos", "Reduce los destellos y parpadeos de la pantalla.", () => enEdicion.reducirDestellos, Ajuste.ReducirDestellos),
                    SiNo("Reducir movimiento", "Detiene el temblor de los títulos y otros movimientos de la pantalla.", () => enEdicion.reducirMovimiento, Ajuste.ReducirMovimiento),
                    SiNo("Indicadores de sonido", "Muestra en pantalla de dónde vienen los sonidos importantes, cuando el juego los tenga.", () => enEdicion.indicadoresSonido, Ajuste.IndicadoresSonido),
                };
        }
    }

    private Renglon Deslizador(string nombre, string descripcion, Func<int> leer, Ajuste ajuste) =>
        new Renglon(nombre, descripcion, () => $"{leer()}%", direccion => enEdicion.Cambiar(ajuste, direccion), leer);

    private Renglon SiNo(string nombre, string descripcion, Func<bool> leer, Ajuste ajuste) =>
        Selector(nombre, descripcion, () => leer() ? "Sí" : "No", direccion => enEdicion.Cambiar(ajuste, direccion));

    private static Renglon Selector(string nombre, string descripcion, Func<string> leer, Action<int> cambiar) =>
        new Renglon(nombre, descripcion, () => $"‹ {leer()} ›", cambiar, null);

    private static Renglon SoloLectura(string nombre, string teclas, string descripcion) =>
        new Renglon(nombre, descripcion, () => teclas, null, null);

    private static string NombreTamano(TamanoTexto tamano)
    {
        switch (tamano)
        {
            case TamanoTexto.Pequeno: return "Pequeño";
            case TamanoTexto.Grande: return "Grande";
            default: return "Mediano";
        }
    }

    // Los niveles de calidad del proyecto se llaman en inglés; el jugador los ve en español
    private static string NombreCalidad(int nivel)
    {
        var nombre = nivel >= 0 && nivel < QualitySettings.names.Length ? QualitySettings.names[nivel] : "";
        switch (nombre)
        {
            case "Very Low": return "Muy baja";
            case "Low": return "Baja";
            case "Medium": return "Media";
            case "High": return "Alta";
            case "Very High": return "Muy alta";
            case "Ultra": return "Ultra";
            default: return nombre;
        }
    }

    private class Renglon
    {
        public readonly string Nombre;
        public readonly string Descripcion;
        public readonly Func<string> TextoValor;
        public readonly Action<int> Cambiar;
        public readonly Func<int> Porcentaje;

        public Renglon(string nombre, string descripcion, Func<string> textoValor, Action<int> cambiar, Func<int> porcentaje)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            TextoValor = textoValor;
            Cambiar = cambiar;
            Porcentaje = porcentaje;
        }

        public bool EsDeslizador => Porcentaje != null;
    }
}
