using UnityEngine;

// Repite un sonido cada cierto tiempo (fijo o al azar entre un mínimo y un máximo), por ejemplo el nombre
// distorsionado que sale de la tele o los murmullos de una voz femenina. Con espacial, se oye más cerca del objeto.
[RequireComponent(typeof(AudioSource))]
public class SonidoCadaTanto : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private float intervaloMin = 10f;
    [SerializeField] private float intervaloMax = 10f;
    [SerializeField, Range(0f, 1f)] private float volumen = 0.7f;
    [Tooltip("Primera vez: segundos después de activarse")]
    [SerializeField] private float primeraEspera = 3f;

    private AudioSource fuente;
    private float siguiente;

    public void Configurar(AudioClip nuevoClip, float min, float max, float nuevoVolumen, float espera)
    {
        clip = nuevoClip;
        intervaloMin = min;
        intervaloMax = max;
        volumen = nuevoVolumen;
        primeraEspera = espera;
    }

    private void OnEnable()
    {
        fuente = GetComponent<AudioSource>();
        fuente.playOnAwake = false;
        siguiente = Time.time + primeraEspera;
    }

    private void Update()
    {
        if (clip == null || AudioListener.pause || Time.time < siguiente)
            return;

        fuente.PlayOneShot(clip, volumen);
        siguiente = Time.time + clip.length + Random.Range(intervaloMin, intervaloMax);
    }
}
