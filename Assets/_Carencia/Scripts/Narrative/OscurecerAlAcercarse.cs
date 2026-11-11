using UnityEngine;

// La visión de Elaki se oscurece mientras se acerca a un punto, como si se fuera a desmayar.
// Primero se cierran las orillas y al final se va a negro toda la pantalla.
// Se activa desde una secuencia y deja de actuar al desactivarse.
public class OscurecerAlAcercarse : MonoBehaviour
{
    [SerializeField] private Transform destino;
    [SerializeField] private float distanciaInicio = 12f;
    [SerializeField] private float distanciaFin = 1.5f;
    [SerializeField, Range(0f, 1f)] private float penumbraMaxima = 0.95f;
    [SerializeField, Range(0f, 1f)] private float negroMaximo = 0.96f;

    private Transform jugador;

    public void Configurar(Transform nuevoDestino, float inicio, float fin)
    {
        destino = nuevoDestino;
        distanciaInicio = inicio;
        distanciaFin = fin;
    }

    private void OnEnable()
    {
        var controlador = FindAnyObjectByType<PlayerController>();
        jugador = controlador != null ? controlador.transform : null;
    }

    private void Update()
    {
        if (jugador == null || destino == null)
            return;

        var distancia = Vector2.Distance(jugador.position, destino.position);
        var avance = Mathf.InverseLerp(distanciaInicio, distanciaFin, distancia);
        PantallaJuego.Instancia.SetPenumbra(avance * penumbraMaxima);
        // En el último tramo la pantalla entera se va a negro, hasta casi no ver nada frente a la puerta
        PantallaJuego.Instancia.SetFundido(Mathf.SmoothStep(0f, negroMaximo, Mathf.InverseLerp(0.45f, 1f, avance)));
    }
}
