using UnityEngine;

namespace PortalesGame
{
    /// <summary>
    /// Marca un objeto cuyo arte es provisional. El sprite vive en <c>VisualRoot/PlaceholderSprite</c>;
    /// colisiones, interacción y eventos están en el objeto raíz y no dependen del sprite,
    /// así que el dibujo definitivo se puede asignar sin perder comportamiento.
    /// </summary>
    public class PlaceholderVisual : MonoBehaviour
    {
        [Tooltip("Identificador del arte provisional. Coincide con el nombre del PNG en carpetas Provisional.")]
        public string placeholderId;
        [Tooltip("Renderer que muestra el arte provisional (VisualRoot/PlaceholderSprite).")]
        public SpriteRenderer placeholderRenderer;
        [Tooltip("Marcar cuando ya se asignó el dibujo definitivo.")]
        public bool hasFinalArt;
        [TextArea] public string notes;

        public void SetSprite(Sprite sprite)
        {
            if (placeholderRenderer != null) placeholderRenderer.sprite = sprite;
        }
    }
}
