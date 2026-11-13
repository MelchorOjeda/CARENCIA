using UnityEngine;

// Cursor del juego: una espiral blanca, que se vuelve morada cuando el puntero está sobre algo con lo que
// Elaki puede interactuar ahora mismo (cerca y disponible). Se crea sola al iniciar y vive en todas las escenas.
public class CursorEspiral : MonoBehaviour
{
    private const string RutaBlanco = "Cursor/UI_CursorEspiral_Blanco";
    private const string RutaMorado = "Cursor/UI_CursorEspiral_Morado";

    private Texture2D blanco;
    private Texture2D morado;
    private Vector2 centro;
    private InteractionDetector detector;
    private bool? moradoPuesto;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Crear()
    {
        if (FindAnyObjectByType<CursorEspiral>() != null)
            return;

        var objeto = new GameObject("CursorEspiral");
        DontDestroyOnLoad(objeto);
        objeto.AddComponent<CursorEspiral>();
    }

    private void Awake()
    {
        blanco = Resources.Load<Texture2D>(RutaBlanco);
        morado = Resources.Load<Texture2D>(RutaMorado);
        if (blanco != null)
            centro = new Vector2(blanco.width / 2f, blanco.height / 2f);
        Poner(false);
    }

    private void LateUpdate()
    {
        if (detector == null)
            detector = FindAnyObjectByType<InteractionDetector>();

        var sobreAlgo = false;
        if (detector != null && detector.isActiveAndEnabled)
            sobreAlgo = detector.Apuntado(out var alAlcance) != null && alAlcance;

        Poner(sobreAlgo);
    }

    private void Poner(bool sobreAlgo)
    {
        if (moradoPuesto == sobreAlgo)
            return;

        var textura = sobreAlgo ? morado : blanco;
        if (textura == null)
            return;

        moradoPuesto = sobreAlgo;
        Cursor.SetCursor(textura, centro, CursorMode.Auto);
    }

    private void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
