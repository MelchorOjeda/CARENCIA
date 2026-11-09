using UnityEngine;

public enum CategoriaSonido
{
    Musica,
    Efectos,
    Voces,
}

// Se pone junto a un AudioSource para que respete el volumen de su categoría en Opciones.
// El volumen general ya lo aplica AudioListener a todo el juego.
[RequireComponent(typeof(AudioSource))]
public class VolumenPorCategoria : MonoBehaviour
{
    [SerializeField] private CategoriaSonido categoria;

    private AudioSource fuente;
    private float volumenOriginal;

    private void Awake()
    {
        fuente = GetComponent<AudioSource>();
        volumenOriginal = fuente.volume;
    }

    private void OnEnable()
    {
        AjustesJuego.Cambiaron += Aplicar;
        Aplicar(AjustesJuego.EnPantalla);
    }

    private void OnDisable()
    {
        AjustesJuego.Cambiaron -= Aplicar;
    }

    private void Aplicar(Ajustes ajustes)
    {
        fuente.volume = volumenOriginal * VolumenDe(ajustes) / 100f;
    }

    private int VolumenDe(Ajustes ajustes)
    {
        switch (categoria)
        {
            case CategoriaSonido.Musica: return ajustes.volumenMusica;
            case CategoriaSonido.Voces: return ajustes.volumenVoces;
            default: return ajustes.volumenEfectos;
        }
    }
}
