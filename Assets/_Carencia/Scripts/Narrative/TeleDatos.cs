using UnityEngine;

// Datos de una tele que se puede encender desde su vista de inspección (paso VerTele de Secuencia).
[CreateAssetMenu(menuName = "Carencia/Tele", fileName = "SO_Tele_")]
public class TeleDatos : ScriptableObject
{
    [Tooltip("Vista fija de la tele apagada.")]
    public Sprite vista;

    [Tooltip("Lo que se ve al encenderla. Se ajusta a la pantalla con barras negras si no tiene su proporción.")]
    public Sprite canal;

    [Tooltip("Audio del canal, en bucle mientras la tele está encendida.")]
    public AudioClip voz;

    [Range(0f, 1f)] public float volumen = 0.8f;

    [Tooltip("Pantalla dentro de la vista, en píxeles medidos desde la esquina superior izquierda.")]
    public RectInt pantalla = new RectInt(140, 77, 361, 163);

    [Tooltip("Segundos de estática al encender.")]
    public float estatica = 0.35f;
}
