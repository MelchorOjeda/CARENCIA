using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Todos los textos del guion y cómo se presentan. Vive en Resources para que la caja de diálogo lo cargue sola
// en cualquier escena. El equipo de diálogos edita aquí los textos sin tocar las secuencias.
[CreateAssetMenu(fileName = "SO_Dialogo_Guion", menuName = "Carencia/Guion de diálogos")]
public class GuionDialogos : ScriptableObject
{
    public const string RutaEnResources = "SO_Dialogo_Guion";

    [SerializeField] private TMP_FontAsset fuente;

    [Tooltip("Se repite mientras el texto se escribe")]
    [SerializeField] private AudioClip sonidoEscritura;

    [Tooltip("0 muestra cada página completa de golpe")]
    [Min(0f)]
    [SerializeField] private float letrasPorSegundo = 40f;

    public List<LineaDialogo> lineas = new List<LineaDialogo>();

    public TMP_FontAsset Fuente => fuente;
    public AudioClip SonidoEscritura => sonidoEscritura;
    public float LetrasPorSegundo => Mathf.Max(0f, letrasPorSegundo);

    public LineaDialogo LineaDe(string clave)
    {
        var linea = lineas.Find(candidata => candidata.clave == clave);
        if (linea != null)
            return linea;

        Debug.LogWarning($"Diálogo sin texto para la clave \"{clave}\" en {name}");
        return new LineaDialogo { clave = clave, quien = "", texto = $"[{clave}]" };
    }
}
