using UnityEngine;

// Sacude la cámara un instante (cuando algo agarra a Elaki). Corre después del seguimiento de la cámara:
// quita su desplazamiento antes de que la cámara se mueva y lo vuelve a poner al final del cuadro,
// así nunca se acumula en la posición real.
[DefaultExecutionOrder(1000)]
public class SacudidaCamara : MonoBehaviour
{
    private Vector3 desplazamiento;
    private float fuerza;
    private float duracion;
    private float restante;

    public static void Sacudir(float nuevaFuerza, float nuevaDuracion)
    {
        var camara = Camera.main;
        if (camara == null)
            return;

        var sacudida = camara.GetComponent<SacudidaCamara>();
        if (sacudida == null)
            sacudida = camara.gameObject.AddComponent<SacudidaCamara>();

        sacudida.fuerza = Mathf.Max(nuevaFuerza, sacudida.restante > 0f ? sacudida.fuerza : 0f);
        sacudida.duracion = nuevaDuracion;
        sacudida.restante = nuevaDuracion;
    }

    private void Update()
    {
        transform.position -= desplazamiento;
        desplazamiento = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (restante <= 0f)
            return;

        restante -= Time.deltaTime;
        var caida = Mathf.Clamp01(restante / duracion);
        var empuje = Random.insideUnitCircle * fuerza * caida;
        desplazamiento = new Vector3(empuje.x, empuje.y, 0f);
        transform.position += desplazamiento;
    }

    private void OnDisable()
    {
        transform.position -= desplazamiento;
        desplazamiento = Vector3.zero;
        restante = 0f;
    }
}
