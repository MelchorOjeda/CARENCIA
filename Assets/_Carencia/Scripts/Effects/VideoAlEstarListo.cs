using UnityEngine;
using UnityEngine.Video;

// Oculta el dibujo del video hasta que tiene su primer cuadro: mientras arranca se ve lo que hay debajo
// (la imagen fija del portal) en lugar de un cristal negro.
public class VideoAlEstarListo : MonoBehaviour
{
    [SerializeField] private VideoPlayer video;
    [SerializeField] private SpriteRenderer dibujo;

    public void Configurar(VideoPlayer nuevoVideo, SpriteRenderer nuevoDibujo)
    {
        video = nuevoVideo;
        dibujo = nuevoDibujo;
    }

    private void OnEnable()
    {
        if (dibujo != null)
            dibujo.enabled = false;
    }

    private void Update()
    {
        if (dibujo != null && !dibujo.enabled && video != null && video.isPlaying && video.frame > 0)
            dibujo.enabled = true;
    }
}
