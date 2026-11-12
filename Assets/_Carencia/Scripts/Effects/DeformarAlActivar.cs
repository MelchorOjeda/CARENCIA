using PortalesGame;
using UnityEngine;

// Al activarse (desde una Secuencia) hace que un cuadro que respira se deforme: late más fuerte y se ondula.
// Va en un objeto hijo inactivo del cuadro; Activar ese hijo inicia la deformación y Desactivarlo la calma.
public class DeformarAlActivar : MonoBehaviour
{
    [SerializeField] private Breathing respiracion;
    [SerializeField] private float amplitud = 0.1f;
    [SerializeField] private float ondulacion = 0.07f;
    [SerializeField] private float ritmo = 1f;

    private float amplitudNormal;
    private float ondulacionNormal;
    private float ritmoNormal;

    private void OnEnable()
    {
        if (respiracion == null)
            return;

        amplitudNormal = respiracion.amplitude;
        ondulacionNormal = respiracion.skew;
        ritmoNormal = respiracion.rate;
        respiracion.enabled = true;
        respiracion.amplitude = amplitud;
        respiracion.skew = ondulacion;
        respiracion.rate = ritmo;
    }

    private void OnDisable()
    {
        if (respiracion == null)
            return;

        respiracion.amplitude = amplitudNormal;
        respiracion.skew = ondulacionNormal;
        respiracion.rate = ritmoNormal;
    }
}
