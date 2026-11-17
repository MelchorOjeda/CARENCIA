using UnityEngine;
using UnityEngine.SceneManagement;

// Teclas que se muestran en pantalla (E para interactuar, Esc para salir) con el estilo de cada nivel.
// Al cargar una escena cambia todas las E del escenario por la de su nivel, sin tocar las escenas.
// Orden de los arreglos: Prólogo, Nivel 1, Nivel 2, Nivel 3.
[CreateAssetMenu(menuName = "Carencia/Indicadores por nivel", fileName = "SO_Indicadores_Teclas")]
public class IndicadoresPorNivel : ScriptableObject
{
    private const string Recurso = "SO_Indicadores_Teclas";

    [Tooltip("La E que tienen colocada las escenas; es la que se reemplaza.")]
    public Sprite original;
    public Sprite[] teclaE = new Sprite[4];
    public Sprite[] teclaEsc = new Sprite[4];

    private static IndicadoresPorNivel datos;

    private static IndicadoresPorNivel Datos
    {
        get
        {
            if (datos == null)
                datos = Resources.Load<IndicadoresPorNivel>(Recurso);
            return datos;
        }
    }

    // -1 si la escena no pertenece a ningún nivel (menú, desenlace): ahí se queda la E original
    public static int NivelDe(string escena)
    {
        if (escena.StartsWith("PRO_")) return 0;
        if (escena.StartsWith("R1_") || escena.StartsWith("N1_")) return 1;
        if (escena.StartsWith("R2_") || escena.StartsWith("N2_")) return 2;
        if (escena.StartsWith("R3_") || escena.StartsWith("N3_")) return 3;
        return -1;
    }

    public static Sprite EscActual
    {
        get
        {
            var nivel = NivelDe(SceneManager.GetActiveScene().name);
            var d = Datos;
            if (d == null || d.teclaEsc.Length == 0)
                return null;
            return d.teclaEsc[Mathf.Clamp(nivel, 0, d.teclaEsc.Length - 1)];
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Registrar()
    {
        SceneManager.sceneLoaded -= AlCargar;
        SceneManager.sceneLoaded += AlCargar;
        for (var i = 0; i < SceneManager.sceneCount; i++)
            Aplicar(SceneManager.GetSceneAt(i));
    }

    private static void AlCargar(Scene escena, LoadSceneMode modo) => Aplicar(escena);

    private static void Aplicar(Scene escena)
    {
        var d = Datos;
        var nivel = NivelDe(escena.name);
        if (d == null || d.original == null || nivel < 0 || nivel >= d.teclaE.Length || d.teclaE[nivel] == null)
            return;

        foreach (var raiz in escena.GetRootGameObjects())
            foreach (var sr in raiz.GetComponentsInChildren<SpriteRenderer>(true))
                if (sr.sprite == d.original)
                    sr.sprite = d.teclaE[nivel];
    }
}
