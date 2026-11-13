using UnityEngine;
using UnityEngine.Video;

namespace PortalesGame
{
    /// <summary>
    /// Paredes orgánicas del nivel 3: el video de referencia ("pared con ojos") se reproduce como textura de la
    /// propia superficie de las paredes. Cada pared es una malla con la forma exacta de la cara isométrica, la textura
    /// se repite a lo largo sin deformarse y recibe la iluminación 2D de la habitación (no es una imagen encima).
    /// </summary>
    public class FleshWall : MonoBehaviour
    {
        public VideoClip clip;
        [Tooltip("Textura fija en lugar del video (papel tapiz deteriorado del nivel 2). Se repite a lo largo del muro.")]
        public Texture texture;
        [Tooltip("Material iluminado 2D (Sprite-Lit-Default) que se copia para las paredes.")]
        public Material litMaterial;
        public Color tint = new Color(0.78f, 0.52f, 0.52f, 1f);
        public string sortingLayer = "ObjectsBehind";
        public int sortingOrder = 1500;
        [Tooltip("Cuatro esquinas por pared (inf. inicio, inf. fin, sup. fin, sup. inicio), en coordenadas locales.")]
        public Vector3[] corners = new Vector3[0];
        [Tooltip("Pulso lento de brillo: la carne parece respirar.")]
        public float breath = 0.08f;

        private VideoPlayer player;
        private RenderTexture rt;
        private Material mat;
        private Mesh[] meshes;
        private Color32[][] colors;

        private void Awake()
        {
            int walls = corners.Length / 4;
            if (walls == 0) return;

            if (texture == null)
            {
                rt = new RenderTexture(512, 288, 0, RenderTextureFormat.ARGB32) { wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Bilinear };
                rt.Create();
            }
            if (clip != null && texture == null)
            {
                player = gameObject.AddComponent<VideoPlayer>();
                player.playOnAwake = false;
                player.clip = clip;
                player.isLooping = true;
                player.renderMode = VideoRenderMode.RenderTexture;
                player.targetTexture = rt;
                player.audioOutputMode = VideoAudioOutputMode.None;
                player.aspectRatio = VideoAspectRatio.Stretch;
            }
            mat = litMaterial != null ? new Material(litMaterial) : new Material(Shader.Find("Sprites/Default"));
            mat.mainTexture = texture != null ? texture : (Texture)rt;

            float aspect = texture != null ? texture.width / (float)Mathf.Max(1, texture.height)
                : clip != null && clip.height > 0 ? clip.width / (float)clip.height : 1.6f;
            meshes = new Mesh[walls];
            colors = new Color32[walls][];
            for (int w = 0; w < walls; w++)
            {
                Vector3 b0 = corners[w * 4], b1 = corners[w * 4 + 1], t1 = corners[w * 4 + 2], t0 = corners[w * 4 + 3];
                float height = Vector3.Distance(b0, t0);
                float length = Vector3.Distance(b0, b1);
                // Un mosaico por cada tramo de pared con las proporciones del video: la imagen no se estira.
                float u = length / Mathf.Max(0.01f, height * aspect);
                var mesh = new Mesh { name = "FleshWall_" + w };
                mesh.vertices = new[] { b0, b1, t1, t0 };
                mesh.uv = new[] { new Vector2(0, 0), new Vector2(u, 0), new Vector2(u, 1), new Vector2(0, 1) };
                mesh.triangles = new[] { 0, 2, 1, 0, 3, 2 };
                Color32 c = tint;
                colors[w] = new[] { c, c, c, c };
                mesh.colors32 = colors[w];
                mesh.RecalculateBounds();
                meshes[w] = mesh;

                var go = new GameObject((texture != null ? "WallSurface_" : "FleshWallSurface_") + w);
                go.transform.SetParent(transform, false);
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
                mr.sortingLayerName = sortingLayer;
                mr.sortingOrder = sortingOrder;
            }
        }

        private void OnEnable()
        {
            if (player != null) player.Play();
        }

        private void OnDisable()
        {
            if (player != null) player.Pause();
        }

        private void Update()
        {
            if (meshes == null || breath <= 0f) return;
            float k = 1f - breath * (0.5f + 0.5f * Mathf.Sin(Time.time * 0.9f));
            Color32 c = new Color(tint.r * k, tint.g * k, tint.b * k, 1f);
            for (int w = 0; w < meshes.Length; w++)
            {
                var cs = colors[w];
                cs[0] = cs[1] = cs[2] = cs[3] = c;
                meshes[w].colors32 = cs;
            }
        }

        private void OnDestroy()
        {
            if (rt != null) { rt.Release(); Destroy(rt); }
            if (mat != null) Destroy(mat);
            if (meshes != null) foreach (var m in meshes) if (m != null) Destroy(m);
        }
    }
}
