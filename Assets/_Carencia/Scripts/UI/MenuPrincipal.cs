using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Menú inicial: "pulsa cualquier tecla", luego Jugar, Continuar, Opciones y Salir.
// Continuar aparece tachado si no hay partida guardada (o si el archivo está dañado).
// Lo que borra la partida o cierra el juego pregunta antes; una vez elegido, el menú deja de responder.
public class MenuPrincipal : MonoBehaviour
{
    private const int Jugar = 0;
    private const int Continuar = 1;
    private const int Opciones = 2;
    private const int Salir = 3;

    private static readonly Dictionary<string, string> NombresDeEscena = new Dictionary<string, string>
    {
        { "PRO_Casa", "Prólogo — La casa" },
        { "N1_Laberinto", "Nivel 1 — El laberinto" },
        { "N1_Pasillo", "Nivel 1 — El pasillo" },
        { "N2_Casa", "Nivel 2 — La casa" },
        { "R2_Casa", "Nivel 2 — El despertar" },
        { "N3_Casa", "Nivel 3 — La casa" },
    };

    [SerializeField] private string primeraEscena = "PRO_Casa";
    [SerializeField] private GameObject pantallaInicio;
    [SerializeField] private GameObject pantallaMenu;
    [SerializeField] private ListaOpciones lista;
    [SerializeField] private OpcionMenu opcionContinuar;
    [SerializeField] private TMP_Text detalleCartel;
    [SerializeField] private TMP_Text version;
    [SerializeField] private PantallaOpciones pantallaOpciones;
    [SerializeField] private NotaConfirmacion nota;
    [SerializeField] private Image vineta;
    [SerializeField] private Sprite vinetaMenu;
    [SerializeField] private Sprite vinetaOpciones;

    private readonly AccionUnica salida = new AccionUnica();
    private (string escena, DateTime fecha)? partida;

    private void Awake()
    {
        lista.Elegida += Elegir;
    }

    private void Start()
    {
        PantallaJuego.Instancia.Fundir(0f, 0.3f);
        PantallaJuego.Instancia.SetPenumbra(0f);
        PantallaJuego.Instancia.SetManchas(0f);

        version.text = $"demo v{Application.version}";
        LeerPartida();
        pantallaOpciones.gameObject.SetActive(false);
        nota.gameObject.SetActive(false);
        MostrarInicio();
    }

    private void Update()
    {
        if (pantallaInicio.activeSelf && ControlesMenu.Pulso(ControlesMenu.CualquierTecla))
            MostrarMenu(Jugar);
        else if (pantallaMenu.activeSelf && lista.Activa && !nota.Abierta && ControlesMenu.Pulso(ControlesMenu.Volver))
            PreguntarSalir();
    }

    private void MostrarInicio()
    {
        pantallaInicio.SetActive(true);
        pantallaMenu.SetActive(false);
    }

    private void MostrarMenu(int enfocada)
    {
        pantallaInicio.SetActive(false);
        pantallaMenu.SetActive(true);
        vineta.sprite = vinetaMenu;
        lista.Enfocar(enfocada);
        ControlesMenu.Bloquear();
    }

    private void Elegir(int opcion)
    {
        switch (opcion)
        {
            case Jugar: PedirJugar(); break;
            case Continuar: ContinuarPartida(); break;
            case Opciones: AbrirOpciones(); break;
            case Salir: PreguntarSalir(); break;
        }
    }

    private void PedirJugar()
    {
        if (FlujoMenu.AlElegirJugar(partida.HasValue) == Decision.Hacerlo)
        {
            EmpezarPartida();
            return;
        }

        nota.Preguntar(
            "¿Empezar desde el principio?",
            $"Se borrará tu partida guardada ({NombreDe(partida.Value.escena)}).",
            "Sí, empezar de nuevo",
            EmpezarPartida);
    }

    private void EmpezarPartida()
    {
        if (!salida.Tomar())
            return;

        lista.Activa = false;
        EstadoJuego.NuevaPartida();
        PantallaJuego.Instancia.CargarEscena(primeraEscena, null);
    }

    private void ContinuarPartida()
    {
        // El archivo pudo borrarse o dañarse después de abrir el menú: Continuar se tacha y no pasa nada más
        if (!EstadoJuego.CargarPartida())
        {
            LeerPartida();
            lista.EnfocarPrimera();
            return;
        }

        if (!salida.Tomar())
            return;

        lista.Activa = false;
        PantallaJuego.Instancia.CargarEscena(EstadoJuego.EscenaControl, EstadoJuego.EntradaControl);
    }

    private void AbrirOpciones()
    {
        pantallaMenu.SetActive(false);
        vineta.sprite = vinetaOpciones;
        pantallaOpciones.Abrir(() => MostrarMenu(Opciones));
    }

    private void PreguntarSalir()
    {
        nota.Preguntar(
            "¿Salir del juego?",
            partida.HasValue ? "Tu progreso está guardado." : "Todavía no has empezado una partida.",
            "Sí, salir",
            SalirDelJuego);
    }

    private void SalirDelJuego()
    {
        if (!salida.Tomar())
            return;

        lista.Activa = false;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LeerPartida()
    {
        partida = EstadoJuego.LeerPartidaGuardada();
        opcionContinuar.SetHabilitada(partida.HasValue);
        detalleCartel.text = partida.HasValue
            ? $"Último rastro: {NombreDe(partida.Value.escena)}\n{HaceCuanto(partida.Value.fecha)}"
            : "Nadie ha empezado a buscar.";
    }

    private static string NombreDe(string escena) =>
        NombresDeEscena.TryGetValue(escena, out var nombre) ? nombre : escena;

    private static string HaceCuanto(DateTime fecha)
    {
        var tiempo = DateTime.Now - fecha;
        if (tiempo.TotalMinutes < 1)
            return "hace un momento";
        if (tiempo.TotalHours < 1)
            return $"hace {(int)tiempo.TotalMinutes} min";
        if (tiempo.TotalDays < 1)
            return $"hace {(int)tiempo.TotalHours} h";

        return (int)tiempo.TotalDays == 1 ? "hace 1 día" : $"hace {(int)tiempo.TotalDays} días";
    }
}
