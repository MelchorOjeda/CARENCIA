using System;
using System.Collections.Generic;
using UnityEngine;

// Lista de los videos de cada escena (en qué objeto están, nombre y duración) para la versión web,
// donde los VideoClip no se incluyen. La genera Editor/CatalogoVideosWebBuild.cs antes de cada build
// WebGL; VideoWeb la consulta. No se edita a mano.
public class CatalogoVideosWeb : ScriptableObject
{
    public const string Recurso = "CatalogoVideosWeb";

    [Serializable]
    public class Entrada
    {
        public string escena;   // ruta de la escena
        public string ruta;     // ruta del objeto en la jerarquía (Nombre#n/Hijo#n/...)
        public string nombre;
        public float duracion;
        public int ancho;
        public int alto;
    }

    public List<Entrada> entradas = new List<Entrada>();

    // Ruta de un objeto: cada tramo es su nombre y cuántos hermanos con el mismo nombre tiene antes.
    public static string RutaDe(Transform objeto)
    {
        var tramos = new List<string>();
        for (var t = objeto; t != null; t = t.parent)
        {
            var n = 0;
            if (t.parent != null)
            {
                for (var i = 0; i < t.GetSiblingIndex(); i++)
                    if (t.parent.GetChild(i).name == t.name)
                        n++;
            }
            else
            {
                foreach (var raiz in t.gameObject.scene.GetRootGameObjects())
                {
                    if (raiz.transform == t)
                        break;
                    if (raiz.name == t.name)
                        n++;
                }
            }
            tramos.Insert(0, t.name + "#" + n);
        }
        return string.Join("/", tramos);
    }
}
