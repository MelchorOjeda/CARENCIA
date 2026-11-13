using System.Collections;
using UnityEngine;

// Margarita aparece en el baño final y camina despacio hacia la tina, de espaldas a Elaki, como si lo llevara.
// Al llegar se desvanece y aparece recostada en el agua. Empieza al activarse este objeto (paso "Activar").
public class GuiaMargarita : MonoBehaviour
{
    [SerializeField] private SpriteRenderer figura;
    [SerializeField] private Sprite deFrente;
    [SerializeField] private Sprite deEspaldas;
    [SerializeField] private Transform destino;
    [SerializeField] private float aparecer = 1.4f;
    [SerializeField] private float mirarlo = 1.6f;
    [SerializeField] private float caminar = 3.2f;
    [SerializeField] private SpriteRenderer recostada;
    [SerializeField] private AudioClip voz;

    // Lo que tarda de principio a fin, para la Secuencia que espera
    public float Duracion => figura == null ? 2.4f : aparecer + mirarlo + caminar + 1.6f;

    private void OnEnable()
    {
        StartCoroutine(figura == null ? SoloEnLaTina() : Rutina());
    }

    // Sin figura de pie: Margarita solo aparece poco a poco, recostada en el agua
    private IEnumerator SoloEnLaTina()
    {
        if (recostada == null)
            yield break;

        recostada.enabled = true;
        if (voz != null)
            PantallaJuego.Instancia.ReproducirEfecto(voz, 0.7f);
        for (var t = 0f; t < 2.4f; t += Time.deltaTime)
        {
            Alfa(recostada, t / 2.4f);
            yield return null;
        }

        Alfa(recostada, 1f);
    }

    private IEnumerator Rutina()
    {
        if (recostada != null)
            recostada.enabled = false;

        figura.sprite = deFrente;
        figura.enabled = true;
        for (var t = 0f; t < aparecer; t += Time.deltaTime)
        {
            Alfa(figura, t / aparecer);
            yield return null;
        }

        Alfa(figura, 1f);
        if (voz != null)
            PantallaJuego.Instancia.ReproducirEfecto(voz, 0.7f);
        yield return new WaitForSeconds(mirarlo);

        // Le da la espalda y camina hacia la tina
        figura.sprite = deEspaldas;
        var inicio = transform.position;
        var fin = destino != null ? destino.position : inicio;
        for (var t = 0f; t < caminar; t += Time.deltaTime)
        {
            var k = t / caminar;
            transform.position = Vector3.Lerp(inicio, fin, Mathf.SmoothStep(0f, 1f, k));
            // Un leve vaivén al caminar
            figura.transform.localPosition = new Vector3(0f, Mathf.Abs(Mathf.Sin(t * 7f)) * 0.025f, 0f);
            yield return null;
        }

        figura.transform.localPosition = Vector3.zero;

        // Se hunde en el agua: ella de pie se desvanece y aparece recostada
        if (recostada != null)
        {
            recostada.enabled = true;
            Alfa(recostada, 0f);
        }

        for (var t = 0f; t < 1.6f; t += Time.deltaTime)
        {
            var k = t / 1.6f;
            Alfa(figura, 1f - k);
            Alfa(recostada, k);
            yield return null;
        }

        figura.enabled = false;
        Alfa(recostada, 1f);
    }

    private static void Alfa(SpriteRenderer dibujo, float alfa)
    {
        if (dibujo == null)
            return;

        var color = dibujo.color;
        color.a = alfa;
        dibujo.color = color;
    }
}
