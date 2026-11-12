using UnityEngine;

namespace PortalesGame
{
    /// <summary>Moscas provisionales: puntos oscuros que orbitan de forma irregular sobre un objeto.</summary>
    public class FlySwarm : MonoBehaviour
    {
        public Transform[] flies = new Transform[0];
        public float radius = 0.22f;
        public float speed = 3.5f;

        private void Update()
        {
            float t = Time.time * speed;
            for (int i = 0; i < flies.Length; i++)
            {
                if (flies[i] == null) continue;
                float a = t * (0.8f + i * 0.23f) + i * 2.1f;
                float r = radius * (0.6f + 0.4f * Mathf.Sin(t * 0.7f + i * 1.3f));
                flies[i].localPosition = new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a * 1.3f) * r * 0.6f + 0.12f, 0f);
            }
        }
    }
}
