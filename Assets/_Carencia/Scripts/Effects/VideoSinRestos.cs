using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Las cinemáticas comparten texturas de video: al empezar, la textura todavía tiene el último cuadro del video
// anterior (por ejemplo, las ambulancias del final). Este componente la limpia a negro y no muestra la imagen
// hasta que el video ya tiene su primer cuadro.
public class VideoSinRestos : MonoBehaviour
{
    [SerializeField] private VideoPlayer video;
    [SerializeField] private RawImage imagen;

    public void Configurar(VideoPlayer nuevoVideo, RawImage nuevaImagen)
    {
        video = nuevoVideo;
        imagen = nuevaImagen;
    }

    private void OnEnable()
    {
        if (video != null && video.targetTexture != null)
        {
            var previa = RenderTexture.active;
            RenderTexture.active = video.targetTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = previa;
        }

        if (imagen != null)
            imagen.enabled = false;
    }

    private void Update()
    {
        if (imagen != null && !imagen.enabled && video != null && video.isPlaying && video.frame > 0)
            imagen.enabled = true;
    }
}
