using UnityEngine;

// Datos de un video para la versión web. El build WebGL no incluye los VideoClip (llegan vacíos),
// así que en web VideoWeb.Info lo agrega en juego con los datos de CatalogoVideosWeb. No se agrega a mano.
public class VideoWebInfo : MonoBehaviour
{
    public string nombre;
    public float duracion;
    public int ancho;
    public int alto;
}
