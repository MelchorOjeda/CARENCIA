using System.Collections;
using UnityEngine;

// La tele del Nivel 3 sigue con interferencia mientras suena el audio del anuncio (sale de la tele);
// al terminar el audio la tele se apaga: pantalla negra, sin estática ni su ruido.
// Empieza al activarse este objeto (una secuencia lo activa).
public class TeleQueSeApaga : MonoBehaviour
{
    [SerializeField] private AudioSource anuncio;
    [SerializeField] private GameObject pantallaApagada;
    [SerializeField] private Behaviour[] interferencia;
    [SerializeField] private AudioSource ruido;

    public bool Apagada { get; private set; }

    public void Configurar(AudioSource nuevoAnuncio, GameObject apagada, Behaviour[] estatica, AudioSource nuevoRuido)
    {
        anuncio = nuevoAnuncio;
        pantallaApagada = apagada;
        interferencia = estatica;
        ruido = nuevoRuido;
    }

    private void OnEnable()
    {
        if (!Apagada)
            StartCoroutine(Reproducir());
    }

    private IEnumerator Reproducir()
    {
        if (anuncio != null && anuncio.clip != null)
        {
            anuncio.Play();
            yield return null;
            // Con el juego en pausa el audio se detiene pero no ha terminado
            while (anuncio.isPlaying || AudioListener.pause)
                yield return null;
        }

        Apagar();
    }

    private void Apagar()
    {
        Apagada = true;
        if (pantallaApagada != null)
            pantallaApagada.SetActive(true);

        if (interferencia != null)
            foreach (var efecto in interferencia)
                if (efecto != null)
                    efecto.enabled = false;

        if (ruido != null)
            ruido.Stop();
    }
}
