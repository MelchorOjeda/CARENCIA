using UnityEngine;

// Ojos incrustados en las paredes de carne (Nivel 3): miran y parpadean cada uno a su ritmo.
// Al final (N3.6), Llorar() hace que todos giren hacia el baño y lloren; desde ahí ya no parpadean.
public class OjosPared : MonoBehaviour
{
    // 0 abierto, 1 entreabierto, 2 cerrado, 3 llorando mirando a la izquierda, 4 llorando mirando a la derecha
    [SerializeField] private SpriteRenderer[] ojos = new SpriteRenderer[0];
    [SerializeField] private Sprite[] cuadros = new Sprite[4];
    [SerializeField] private bool llorando;
    // Al aparecer esta bandera, los ojos lloran (la pone la secuencia de N3.6)
    [SerializeField] private string banderaLlanto = "ojos_lloran";

    // Hacia dónde voltean al llorar: la puerta del baño
    [SerializeField] private Transform bano;

    private float[] siguiente;

    public void Configurar(SpriteRenderer[] nuevosOjos, Sprite[] nuevosCuadros, Transform puertaBano)
    {
        ojos = nuevosOjos;
        cuadros = nuevosCuadros;
        bano = puertaBano;
    }

    public void Llorar()
    {
        llorando = true;
        foreach (var ojo in ojos)
        {
            // Los de la izquierda de la puerta miran a la derecha y al revés
            var derecha = bano != null && ojo != null && ojo.transform.position.x < bano.position.x;
            Poner(ojo, derecha ? 4 : 3);
        }
    }

    private void OnEnable()
    {
        siguiente = new float[ojos.Length];
        for (var i = 0; i < siguiente.Length; i++)
            siguiente[i] = Time.time + Random.Range(0.5f, 5f);
    }

    private void Update()
    {
        if (!llorando && EstadoJuego.Tiene(banderaLlanto) && !string.IsNullOrEmpty(banderaLlanto))
            Llorar();
        if (llorando)
            return;

        for (var i = 0; i < ojos.Length; i++)
        {
            var t = Time.time - siguiente[i];
            if (t < 0f)
                Poner(ojos[i], 0);
            else if (t < 0.06f || (t > 0.16f && t < 0.22f))
                Poner(ojos[i], 1);
            else if (t < 0.16f)
                Poner(ojos[i], 2);
            else
                siguiente[i] = Time.time + Random.Range(2.5f, 6f);
        }
    }

    private void Poner(SpriteRenderer ojo, int cuadro)
    {
        if (ojo != null && cuadro < cuadros.Length && cuadros[cuadro] != null)
            ojo.sprite = cuadros[cuadro];
    }
}
