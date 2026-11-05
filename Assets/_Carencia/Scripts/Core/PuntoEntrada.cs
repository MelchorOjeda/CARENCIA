using UnityEngine;

// Lugar donde aparece Elaki al llegar a una escena o al reiniciar desde un punto de control.
public class PuntoEntrada : MonoBehaviour
{
    [SerializeField] private string id;

    public string Id
    {
        get => id;
        set => id = value;
    }

    public static PuntoEntrada Buscar(string id)
    {
        foreach (var entrada in FindObjectsByType<PuntoEntrada>(FindObjectsInactive.Include))
        {
            if (entrada.Id == id)
                return entrada;
        }

        return null;
    }
}
