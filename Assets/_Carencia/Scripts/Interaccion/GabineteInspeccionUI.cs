using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

// Administra la vista fija en primera persona del gabinete del baño detrás del espejo.
// Bloquea al jugador, permite hacer clic sobre los medicamentos y objetos con el mouse,
// muestra los pensamientos de Elaki y dispara el evento del espejo al terminar la inspección obligatoria.
public class GabineteInspeccionUI : MonoBehaviour
{
    public static GabineteInspeccionUI Instancia { get; private set; }

    [Header("Referencias")]
    [SerializeField] private GameObject contenedorVista;
    [SerializeField] private PlayerController jugador;
    [SerializeField] private TextMeshProUGUI textoNombreHover;
    [SerializeField] private TextMeshProUGUI textoPensamiento;
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private GameObject botonCerrar;

    [Header("Objetos Inspeccionables")]
    [SerializeField] private ObjetoInspeccionable[] objetosEnGabinete;

    [Header("Eventos Narrativos")]
    [Tooltip("Se dispara al cerrar el gabinete si ya se inspeccionaron los elementos obligatorios (activa la distorsión del espejo)")]
    public UnityEvent AlCompletarInspeccion;

    private readonly HashSet<string> inspeccionados = new HashSet<string>();
    private bool vistaActiva;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        if (contenedorVista != null)
            contenedorVista.SetActive(false);

        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }

    private void Start()
    {
        if (jugador == null)
            jugador = FindFirstObjectByType<PlayerController>();

        if (objetosEnGabinete == null || objetosEnGabinete.Length == 0)
            objetosEnGabinete = GetComponentsInChildren<ObjetoInspeccionable>(true);
    }

    private void Update()
    {
        if (!vistaActiva) return;

        // Cerrar con Escape o clic derecho
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            CerrarGabinete();
        }
    }

    public void AbrirGabinete()
    {
        vistaActiva = true;

        if (jugador != null)
            jugador.MovimientoBloqueado = true;

        if (contenedorVista != null)
            contenedorVista.SetActive(true);

        OcultarNombreHover();

        if (textoPensamiento != null)
            textoPensamiento.text = string.Empty;

        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }

    public void CerrarGabinete()
    {
        vistaActiva = false;

        if (contenedorVista != null)
            contenedorVista.SetActive(false);

        if (jugador != null)
            jugador.MovimientoBloqueado = false;

        if (TodosLosObligatoriosInspeccionados())
        {
            AlCompletarInspeccion?.Invoke();
        }
    }

    public void MostrarNombreHover(string nombre)
    {
        if (textoNombreHover != null)
        {
            textoNombreHover.text = nombre;
            textoNombreHover.gameObject.SetActive(true);
        }
    }

    public void OcultarNombreHover()
    {
        if (textoNombreHover != null)
        {
            textoNombreHover.text = string.Empty;
            textoNombreHover.gameObject.SetActive(false);
        }
    }

    public void RegistrarInspeccion(ObjetoInspeccionable objeto)
    {
        if (objeto == null) return;

        inspeccionados.Add(objeto.Id);

        if (panelDialogo != null)
            panelDialogo.SetActive(true);

        if (textoPensamiento != null)
            textoPensamiento.text = objeto.Pensamiento;
    }

    public bool TodosLosObligatoriosInspeccionados()
    {
        if (objetosEnGabinete == null) return true;

        foreach (var obj in objetosEnGabinete)
        {
            if (obj != null && obj.Obligatorio && !inspeccionados.Contains(obj.Id))
                return false;
        }
        return true;
    }
}

