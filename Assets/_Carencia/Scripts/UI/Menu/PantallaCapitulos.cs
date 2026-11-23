using System;
using TMPro;
using UnityEngine;

// Capítulos del Modo historia con el mismo diseño del menú: es una copia de la pantalla del menú con un renglón por capítulo.
// Los alcanzados se pueden repetir; los bloqueados se ven tachados y sin título. Esc vuelve al menú.
public class PantallaCapitulos : MonoBehaviour
{
    private const float TamanoTexto = 46f;
    private const float AnchoRenglon = 1000f;

    private ListaOpciones lista;
    private TMP_Text detalle;
    private bool hayPartida;
    private Action<int> alElegir;
    private Action alVolver;

    // La copia se hace con el menú apagado: así sus scripts no arrancan antes de ajustarla
    public static PantallaCapitulos Abrir(GameObject pantallaMenu, bool hayPartida, Action<int> alElegir, Action alVolver)
    {
        pantallaMenu.SetActive(false);
        var copia = Instantiate(pantallaMenu, pantallaMenu.transform.parent);
        copia.name = "PantallaCapitulos";
        copia.transform.SetSiblingIndex(pantallaMenu.transform.GetSiblingIndex() + 1);

        var pantalla = copia.AddComponent<PantallaCapitulos>();
        pantalla.hayPartida = hayPartida;
        pantalla.alElegir = alElegir;
        pantalla.alVolver = alVolver;
        pantalla.Preparar();
        copia.SetActive(true);
        pantalla.lista.EnfocarPrimera();
        ControlesMenu.Bloquear();
        return pantalla;
    }

    private void Preparar()
    {
        var raiz = transform;
        var frase = raiz.Find("Frase").GetComponent<TMP_Text>();
        frase.text = "Niveles";

        var opciones = raiz.Find("Opciones");
        lista = opciones.GetComponent<ListaOpciones>();
        var renglones = opciones.GetComponentsInChildren<OpcionMenu>(true);
        for (var i = 0; i < renglones.Length; i++)
        {
            var renglon = renglones[i];
            // En web el renglón de Salir llega apagado: aquí cada renglón es un capítulo
            renglon.gameObject.SetActive(i < Capitulos.Todos.Length);
            if (i >= Capitulos.Todos.Length)
                continue;

            var capitulo = Capitulos.Todos[i];
            var abierto = ProgresoHistoria.Desbloqueado(i);
            renglon.CambiarTexto(abierto ? capitulo.Nombre : capitulo.NombreBloqueado);
            ((RectTransform)renglon.transform).sizeDelta = new Vector2(AnchoRenglon + 60f, ((RectTransform)renglon.transform).sizeDelta.y);
            foreach (var texto in renglon.GetComponentsInChildren<TMP_Text>(true))
            {
                texto.fontSize = TamanoTexto;
                ((RectTransform)texto.transform).sizeDelta = new Vector2(AnchoRenglon, ((RectTransform)texto.transform).sizeDelta.y);
            }
            renglon.SetHabilitada(abierto);
        }

        lista.Elegida += Elegir;
        lista.Enfocada += MostrarDetalle;
        detalle = raiz.Find("Cartel/Detalle").GetComponent<TMP_Text>();

        var ayuda = raiz.Find("Ayuda");
        var esc = ayuda.Find("Tecla Esc");
        if (esc != null)
        {
            esc.gameObject.SetActive(true);
            var accion = esc.Find("Accion")?.GetComponent<TMP_Text>();
            if (accion != null)
                accion.text = "volver";
        }
    }

    private void MostrarDetalle(int indice)
    {
        if (!ProgresoHistoria.Desbloqueado(indice))
            detalle.text = "Todavía no llegas aquí.";
        else
            detalle.text = hayPartida ? "Repetirlo no borra tu partida guardada." : "Empieza desde aquí.";
    }

    private void Update()
    {
        if (lista.Activa && ControlesMenu.Pulso(ControlesMenu.Volver))
            Cerrar(alVolver);
    }

    private void Elegir(int indice)
    {
        lista.Activa = false;
        alElegir(indice);
    }

    private void Cerrar(Action despues)
    {
        lista.Activa = false;
        Destroy(gameObject);
        despues?.Invoke();
    }
}
