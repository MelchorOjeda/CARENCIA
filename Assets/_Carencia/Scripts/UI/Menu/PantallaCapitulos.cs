using System;
using TMPro;
using UnityEngine;

// Niveles del Modo historia con el mismo diseño del menú: es una copia de la pantalla del menú con un renglón por nivel.
// Solo aparecen los niveles alcanzados (los demás no se muestran, ni cuántos son); al terminar la historia aparecen
// todos. Cualquiera de los que se ven se puede repetir. Esc vuelve al menú.
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
            // Solo se ven los niveles alcanzados: los demás no aparecen, para no revelar cuántos hay.
            // (En web el renglón de Salir llega apagado: aquí se enciende si le toca un nivel)
            var abierto = i < Capitulos.Todos.Length && ProgresoHistoria.Desbloqueado(i);
            renglon.gameObject.SetActive(abierto);
            renglon.SetHabilitada(abierto);
            if (!abierto)
                continue;

            var capitulo = Capitulos.Todos[i];
            renglon.CambiarTexto(capitulo.Nombre);
            ((RectTransform)renglon.transform).sizeDelta = new Vector2(AnchoRenglon + 60f, ((RectTransform)renglon.transform).sizeDelta.y);
            foreach (var texto in renglon.GetComponentsInChildren<TMP_Text>(true))
            {
                texto.fontSize = TamanoTexto;
                ((RectTransform)texto.transform).sizeDelta = new Vector2(AnchoRenglon, ((RectTransform)texto.transform).sizeDelta.y);
            }
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
