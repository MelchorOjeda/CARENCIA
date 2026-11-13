using UnityEngine;

namespace PortalesGame
{
    /// <summary>
    /// Pulso de escala: paredes que respiran, órganos, el corazón de la puerta, cuadros que se deforman.
    /// Con <see cref="beat"/> el pulso es un latido seco en lugar de una onda suave.
    /// </summary>
    public class Breathing : MonoBehaviour
    {
        public float rate = 0.35f;
        public float amplitude = 0.03f;
        [Tooltip("Deformación horizontal adicional (ondulación).")]
        public float skew;
        public bool beat;
        public float phase;

        private Vector3 baseScale;
        private float clock;

        private void Awake()
        {
            baseScale = transform.localScale;
        }

        private void OnEnable()
        {
            if (baseScale == Vector3.zero) baseScale = transform.localScale;
        }

        private void Update()
        {
            clock += Time.deltaTime * rate;
            float t = (clock + phase) * Mathf.PI * 2f;
            float k;
            if (beat)
            {
                float f = (clock + phase) % 1f;
                k = f < 0.12f ? Mathf.Sin(f / 0.12f * Mathf.PI) : (f > 0.22f && f < 0.32f ? 0.6f * Mathf.Sin((f - 0.22f) / 0.1f * Mathf.PI) : 0f);
            }
            else k = Mathf.Sin(t);
            float sx = 1f + k * amplitude + Mathf.Sin(t * 0.7f) * skew;
            float sy = 1f + k * amplitude - Mathf.Sin(t * 0.7f) * skew;
            transform.localScale = new Vector3(baseScale.x * sx, baseScale.y * sy, baseScale.z);
        }

        public void ResetScale()
        {
            transform.localScale = baseScale;
        }
    }
}
