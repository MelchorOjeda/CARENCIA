using UnityEngine;

namespace PortalesGame
{
    /// <summary>Punto de entrada de una habitación: dónde aparece el protagonista y hacia dónde mira.</summary>
    public class SpawnPoint : MonoBehaviour
    {
        public string spawnId = "default";
        public Dir8 facing = Dir8.Sur;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.3f, 1f, 0.5f, 0.9f);
            Gizmos.DrawWireSphere(transform.position, 0.12f);
            var v = Iso.Dir8ToVector(facing);
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(v.x, v.y * 0.5f, 0) * 0.4f);
        }
    }
}
