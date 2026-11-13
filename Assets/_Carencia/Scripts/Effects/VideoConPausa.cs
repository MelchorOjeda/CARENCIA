using System.Collections;
using UnityEngine;
using UnityEngine.Video;

// Video que se reproduce más lento y se detiene un momento en un cuadro (por ejemplo, cuando un texto termina de
// escribirse) para que dé tiempo de leerlo. Empieza al activarse su objeto.
public class VideoConPausa : MonoBehaviour
{
    [SerializeField] private VideoPlayer video;
    [SerializeField] private float velocidad = 0.5f;
    [Tooltip("Segundo del video en el que se queda quieto")]
    [SerializeField] private float pausarEn = 4.6f;
    [SerializeField] private float segundosDePausa = 4f;

    // Lo que dura en pantalla, para la Secuencia que espera
    public float Duracion => pausarEn / Mathf.Max(0.05f, velocidad) + segundosDePausa;

    private void OnEnable()
    {
        if (video != null)
            StartCoroutine(Rutina());
    }

    private IEnumerator Rutina()
    {
        video.playbackSpeed = velocidad;
        video.Play();
        while (!video.isPlaying)
            yield return null;
        while (video.time < pausarEn)
            yield return null;
        video.Pause();
    }
}
