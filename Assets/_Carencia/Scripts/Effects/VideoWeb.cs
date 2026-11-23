using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// En la versión web (WebGL) Unity no puede reproducir un VideoClip: solo videos por URL.
// Este ajuste cambia cada VideoPlayer a la copia del mp4 que el build web pone en
// StreamingAssets/Videos (ver Editor/VideosParaWeb.cs). Como el build web no incluye los
// VideoClip, el nombre y la duración salen de CatalogoVideosWeb. Fuera de WebGL no hace nada.
public static class VideoWeb
{
    public const string Carpeta = "Videos";

#if UNITY_WEBGL && !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Iniciar()
    {
        SceneManager.sceneLoaded += (escena, _) => AjustarEscena(escena);
    }

    private static void AjustarEscena(Scene escena)
    {
        foreach (var raiz in escena.GetRootGameObjects())
            foreach (var video in raiz.GetComponentsInChildren<VideoPlayer>(true))
                Ajustar(video);
    }
#endif

    // Llamar justo después de asignar video.clip y antes de Play().
    // info: los datos del video cuando no están en el mismo objeto que el VideoPlayer.
    public static void Ajustar(VideoPlayer video, VideoWebInfo info = null)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (video == null)
            return;

        if (info == null)
            info = Info(video.gameObject);
        var nombre = video.clip != null ? video.clip.name : info != null ? info.nombre : null;
        if (string.IsNullOrEmpty(nombre))
            return;

        var url = Application.streamingAssetsPath + "/" + Carpeta + "/" + nombre + ".mp4";
        if (video.source == VideoSource.Url && video.url == url)
            return;

        // Si ya había empezado (playOnAwake), se vuelve a lanzar con la URL.
        bool reproducir = video.isPlaying || (video.playOnAwake && video.isActiveAndEnabled);
        video.url = url;
        video.source = VideoSource.Url;
        if (reproducir)
            video.Play();
#endif
    }

    // Duración del video: la del clip, o la guardada para web; 0 si no se sabe.
    public static float Duracion(VideoPlayer video)
    {
        if (video == null)
            return 0f;
        if (video.clip != null)
            return (float)video.clip.length;
        var info = Info(video.gameObject);
        return info != null ? info.duracion : 0f;
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private static System.Collections.Generic.Dictionary<string, CatalogoVideosWeb.Entrada> catalogo;
#endif

    // Datos del video de un objeto en la versión web (su clip llega vacío); null fuera de web o si no tiene.
    public static VideoWebInfo Info(GameObject objeto)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (objeto == null)
            return null;
        var info = objeto.GetComponent<VideoWebInfo>();
        if (info != null)
            return info;

        if (catalogo == null)
        {
            catalogo = new System.Collections.Generic.Dictionary<string, CatalogoVideosWeb.Entrada>();
            var lista = Resources.Load<CatalogoVideosWeb>(CatalogoVideosWeb.Recurso);
            if (lista != null)
                foreach (var e in lista.entradas)
                    catalogo[e.escena + "|" + e.ruta] = e;
        }

        if (!catalogo.TryGetValue(objeto.scene.path + "|" + CatalogoVideosWeb.RutaDe(objeto.transform), out var entrada))
            return null;

        info = objeto.AddComponent<VideoWebInfo>();
        info.nombre = entrada.nombre;
        info.duracion = entrada.duracion;
        info.ancho = entrada.ancho;
        info.alto = entrada.alto;
        return info;
#else
        return null;
#endif
    }

    // Espera a que un video empiece (en web tarda en descargarse), con un límite.
    public static System.Collections.IEnumerator EsperarInicio(GameObject objeto, float limite = 8f)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        var video = objeto != null ? objeto.GetComponentInChildren<VideoPlayer>() : null;
        if (video == null || video.source != VideoSource.Url || !video.isActiveAndEnabled
            || (!video.playOnAwake && !video.isPlaying))
            yield break;
        for (var t = 0f; t < limite && !(video.isPlaying && video.frame > 0); t += Time.unscaledDeltaTime)
            yield return null;
#else
        yield break;
#endif
    }
}
