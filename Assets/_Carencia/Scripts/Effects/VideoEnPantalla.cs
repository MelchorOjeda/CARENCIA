using UnityEngine;
using UnityEngine.Video;

namespace PortalesGame
{
    /// <summary>
    /// Reproduce un video en una pantalla del escenario (<see cref="WorldScreen"/>) en cuanto se activa este objeto.
    /// Así una Secuencia lo dispara con un paso "Activar", sin cambiar el código de las secuencias.
    /// La pausa del juego también detiene el video (lo hace WorldScreen).
    /// </summary>
    public class VideoEnPantalla : MonoBehaviour
    {
        public WorldScreen pantalla;
        public VideoClip clip;
        [Tooltip("Segundos que se reproduce; con 0 dura lo mismo que el clip.")]
        public float duracion;

        private void OnEnable()
        {
            if (pantalla == null || clip == null || !pantalla.isActiveAndEnabled)
                return;

            // La rutina corre en la pantalla: el video sigue aunque este objeto se desactive.
            pantalla.Stop();
            pantalla.StartCoroutine(pantalla.Play(duracion, null, clip, null));
        }
    }
}
