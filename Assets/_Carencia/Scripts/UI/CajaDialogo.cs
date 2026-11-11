using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Caja de diálogo al pie de la pantalla: un rectángulo negro translúcido con el nombre de quien habla
// y el texto escribiéndose letra por letra, con el sonido de tecleo mientras se escribe.
// Enter, clic o E completan la página; la siguiente pulsación pasa de página o cierra.
public class CajaDialogo : MonoBehaviour
{
    // Medidas en la resolución de referencia de PantallaJuego (640 × 360)
    private const float Ancho = 560f;
    // Cabe una página de tres renglones con Tamaño de texto Grande
    private const float Alto = 112f;
    private const float MargenBorde = 10f;
    private const float Relleno = 14f;
    private const float AltoNombre = 20f;
    private const float TamanoNombre = 13f;
    private const float TamanoTexto = 16f;

    // Un tirón (por ejemplo al cargar escena) no debe escribir media línea de golpe
    private const float MaximoSegundosPorFotograma = 0.1f;

    private static readonly Color ColorFondo = new Color(0f, 0f, 0f, 0.88f);
    private static readonly Color ColorNombre = new Color32(0xE8, 0xC6, 0x6A, 0xFF);
    private static readonly Color ColorTexto = new Color32(0xFA, 0xF6, 0xEE, 0xFF);
    private static readonly string ColorEtiquetaPensamiento = "#A39C8C";

    private GuionDialogos guion;
    private TextMeshProUGUI nombre;
    private TextMeshProUGUI texto;
    private AudioSource sonido;
    private InputAction[] accionesAvanzar;
    private LineaEnPantalla linea;

    public bool Visible => gameObject.activeSelf;

    public static CajaDialogo Crear(Transform canvas)
    {
        var objeto = new GameObject("CajaDialogo", typeof(RectTransform));
        objeto.transform.SetParent(canvas, false);
        var caja = objeto.AddComponent<CajaDialogo>();
        caja.Construir();
        objeto.SetActive(false);
        return caja;
    }

    public LineaDialogo LineaDe(string clave)
    {
        return guion.LineaDe(clave);
    }

    // Corre dentro de una corrutina y termina cuando el jugador cierra la última página.
    // Arriba sirve en vistas fijas como el gabinete, para no tapar el objeto que se está viendo
    public IEnumerator Mostrar(LineaDialogo datos, bool arriba = false)
    {
        Abrir(datos, arriba);

        // La pulsación que abrió la línea (E sobre un objeto) no debe completarla
        yield return null;

        while (linea != null && !linea.Terminada)
        {
            if (SePulsoAvanzar())
                linea.Pulsar();
            else
                linea.Avanzar(Mathf.Min(Time.unscaledDeltaTime, MaximoSegundosPorFotograma));

            Refrescar();
            yield return null;
        }

        Ocultar();
    }

    // También la llama PantallaJuego al cambiar de escena, aunque la línea no haya terminado
    public void Ocultar()
    {
        linea = null;
        sonido.Stop();
        gameObject.SetActive(false);
    }

    private void Construir()
    {
        guion = CargarGuion();
        accionesAvanzar = new[]
        {
            InputSystem.actions.FindAction("UI/Submit", true),
            InputSystem.actions.FindAction("UI/Click", true),
            InputSystem.actions.FindAction("Player/Interact", true),
            InputSystem.actions.FindAction("UI/Cancel", true),
        };

        ConstruirFondo();
        nombre = CrearTexto("Nombre", TamanoNombre, ColorNombre);
        nombre.fontStyle = FontStyles.Bold;
        // Solo el nombre usa etiquetas, para pintar la de pensamiento más tenue
        nombre.richText = true;
        Colocar(nombre.rectTransform, new Vector2(Relleno, Alto - Relleno * 0.6f - AltoNombre), new Vector2(-Relleno, -Relleno * 0.6f));
        texto = CrearTexto("Texto", TamanoTexto, ColorTexto);
        Colocar(texto.rectTransform, new Vector2(Relleno, Relleno * 0.6f), new Vector2(-Relleno, -Relleno * 0.6f - AltoNombre));
        sonido = CrearSonido();
    }

    // Sin el asset la caja sigue funcionando: muestra las claves entre corchetes
    private static GuionDialogos CargarGuion()
    {
        var cargado = Resources.Load<GuionDialogos>(GuionDialogos.RutaEnResources);
        if (cargado != null)
            return cargado;

        Debug.LogError($"Falta Resources/{GuionDialogos.RutaEnResources}: los diálogos mostrarán sus claves");
        return ScriptableObject.CreateInstance<GuionDialogos>();
    }

    private void ConstruirFondo()
    {
        ((RectTransform)transform).sizeDelta = new Vector2(Ancho, Alto);

        var fondo = gameObject.AddComponent<Image>();
        fondo.color = ColorFondo;
        fondo.raycastTarget = false;
    }

    private TextMeshProUGUI CrearTexto(string nombreObjeto, float tamano, Color color)
    {
        var objeto = new GameObject(nombreObjeto, typeof(RectTransform));
        objeto.transform.SetParent(transform, false);
        var tmp = objeto.AddComponent<TextMeshProUGUI>();
        if (guion.Fuente != null)
            tmp.font = guion.Fuente;

        tmp.fontSize = tamano;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        // Sin etiquetas, cada letra del texto es una letra en pantalla y la escritura no se salta nada
        tmp.richText = false;
        tmp.raycastTarget = false;

        // Va después de fijar el tamaño: TextoAccesible lo toma como base para Opciones → Tamaño de texto
        objeto.AddComponent<TextoAccesible>();
        return tmp;
    }

    // Esquinas medidas desde la esquina inferior izquierda (min) y superior derecha (max) de la caja
    private static void Colocar(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private AudioSource CrearSonido()
    {
        var fuente = gameObject.AddComponent<AudioSource>();
        fuente.clip = guion.SonidoEscritura;
        fuente.loop = true;
        fuente.playOnAwake = false;
        gameObject.AddComponent<VolumenPorCategoria>().Categoria = CategoriaSonido.Efectos;
        return fuente;
    }

    private void Abrir(LineaDialogo datos, bool arriba)
    {
        linea = new LineaEnPantalla(datos, guion.LetrasPorSegundo);
        nombre.text = Encabezado(linea);
        nombre.gameObject.SetActive(nombre.text.Length > 0);
        texto.fontStyle = linea.EsPensamiento ? FontStyles.Italic : FontStyles.Normal;
        ColocarEnPantalla(arriba);
        gameObject.SetActive(true);
        Refrescar();
    }

    // Los pensamientos llevan la etiqueta junto al nombre para distinguirlos de lo que se dice en voz alta
    private static string Encabezado(LineaEnPantalla linea)
    {
        if (!linea.EsPensamiento)
            return linea.Quien;

        var etiqueta = $"<color={ColorEtiquetaPensamiento}><i>pensamiento</i></color>";
        return linea.Quien.Length > 0 ? $"{linea.Quien}  ·  {etiqueta}" : etiqueta;
    }

    // Centrada, pegada al borde de abajo o al de arriba con el mismo margen
    private void ColocarEnPantalla(bool arriba)
    {
        var borde = arriba ? 1f : 0f;
        var rect = (RectTransform)transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, borde);
        rect.pivot = new Vector2(0.5f, borde);
        rect.anchoredPosition = new Vector2(0f, arriba ? -MargenBorde : MargenBorde);
    }

    private bool SePulsoAvanzar()
    {
        return accionesAvanzar.Any(accion => accion.WasPressedThisFrame());
    }

    private void Refrescar()
    {
        if (linea == null)
            return;

        if (texto.text != linea.Pagina)
            texto.text = linea.Pagina;

        texto.maxVisibleCharacters = linea.LetrasVisibles;
        SonarMientrasEscribe(linea.Escribiendo);
    }

    private void SonarMientrasEscribe(bool escribiendo)
    {
        if (escribiendo && !sonido.isPlaying && sonido.clip != null)
            sonido.Play();
        else if (!escribiendo && sonido.isPlaying)
            sonido.Stop();
    }
}
