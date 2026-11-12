using UnityEngine;

namespace PortalesGame
{
    public enum TVState { Off, Static, Broadcast }

    /// <summary>
    /// Pantalla de un televisor o monitor en el mundo: apagada, con estática o con transmisión.
    /// El sonido de la estática lo pone la escena con los sonidos del equipo de audio.
    /// </summary>
    public class TVScreen : MonoBehaviour
    {
        public SpriteRenderer screenRenderer;
        public Sprite offSprite;
        public Sprite[] staticSprites = new Sprite[0];
        public Sprite broadcastSprite;
        public TVState state = TVState.Off;
        [Tooltip("Pantalla física: si existe, el televisor muestra la estática dentro de su pantalla en lugar de cambiar sprites.")]
        public WorldScreen world;

        private float nextFrame;
        private int frame;

        private void OnEnable()
        {
            Apply();
        }

        public void SetState(TVState s)
        {
            state = s;
            Apply();
        }

        private void Apply()
        {
            if (world != null) world.SetIdle(state == TVState.Off ? ScreenIdle.Dark : ScreenIdle.Static);
            else if (screenRenderer != null)
            {
                if (state == TVState.Off && offSprite != null) screenRenderer.sprite = offSprite;
                else if (state == TVState.Broadcast && broadcastSprite != null) screenRenderer.sprite = broadcastSprite;
            }
        }

        private void Update()
        {
            if (state != TVState.Static) return;
            if (world == null && screenRenderer != null && staticSprites.Length > 0 && Time.time >= nextFrame)
            {
                nextFrame = Time.time + 0.09f;
                frame = (frame + 1) % staticSprites.Length;
                screenRenderer.sprite = staticSprites[frame];
            }
        }
    }
}
