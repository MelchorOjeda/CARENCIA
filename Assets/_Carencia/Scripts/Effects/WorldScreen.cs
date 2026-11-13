using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;

namespace PortalesGame
{
    public enum ScreenIdle { None, Dark, Static, Glow }

    /// <summary>
    /// Pantalla física de un objeto del escenario (televisor, monitor). Todo lo que muestra (video, estática,
    /// fotogramas) se dibuja como un sprite pegado a la superficie de la pantalla, con su forma y su perspectiva,
    /// a la misma densidad de píxel que el escenario. No hay interfaz superpuesta: para verla, la cámara se acerca.
    /// </summary>
    public class WorldScreen : MonoBehaviour
    {
        public SpriteRenderer screen;
        /// <summary>Zona de la pantalla dentro del sprite (0–1, origen abajo a la izquierda).</summary>
        public Rect opaque = new Rect(0f, 0f, 1f, 1f);
        /// <summary>Esquinas de la pantalla (sup. izq., sup. der., inf. der., inf. izq.) normalizadas dentro de <see cref="opaque"/>; si existen, el contenido toma esa forma sin máscara.</summary>
        public Vector2[] quad;

        [Header("Tratamiento de imagen")]
        [Tooltip("Resolución a la que se dibuja el contenido: igual a la densidad de píxel del escenario para que no parezca una imagen pegada.")]
        public int pixelWidth = 96;
        public int pixelHeight = 54;
        public bool pointFilter = true;
        [Tooltip("Qué muestra la pantalla cuando no reproduce nada.")]
        public ScreenIdle idle = ScreenIdle.None;
        [Range(0f, 1f)] public float scanlines = 0.18f;

        [Header("Emisión")]
        public Light2D glow;
        public float glowIntensity = 0.9f;
        [Tooltip("Material sin iluminar (lo asigna el constructor): la pantalla emite su propia luz y no se apaga con la penumbra.")]
        public Material unlitMaterial;

        private Texture2D tex;
        private Sprite contentSprite;
        private SpriteRenderer content;
        private SpriteRenderer picture;
        private SpriteMask mask;
        private VideoPlayer vp;
        private AudioSource videoAudio;
        private RenderTexture rt;
        private Color32[] buffer;
        private float nextStaticFrame;
        private float glowLevel;

        /// <summary>Si existe, decide qué fotograma se ve en cada momento (labios y parpadeo de la presentadora).</summary>
        public Func<int> frameSelector;

        public bool IsPlaying { get; private set; }
        public Vector3 CenterWorld { get { return screen != null ? WorldBounds().center : transform.position; } }
        private bool Shaped { get { return quad != null && quad.Length == 4; } }
        /// <summary>Un cuadrilátero que ocupa todo el rectángulo no necesita geometría propia.</summary>
        private bool NeedsGeometry
        {
            get
            {
                if (!Shaped) return false;
                foreach (var q in quad) if ((q.x > 0.001f && q.x < 0.999f) || (q.y > 0.001f && q.y < 0.999f)) return true;
                return false;
            }
        }
        private bool started;

        public Bounds WorldBounds()
        {
            Bounds lb = screen.sprite.bounds;
            Vector3 min = lb.min + new Vector3(opaque.x * lb.size.x, opaque.y * lb.size.y, 0f);
            Vector3 max = min + new Vector3(opaque.width * lb.size.x, opaque.height * lb.size.y, 0f);
            var b = new Bounds(screen.transform.TransformPoint(min), Vector3.zero);
            b.Encapsulate(screen.transform.TransformPoint(max));
            return b;
        }

        private void Start()
        {
            started = true;
            ApplyIdle();
        }

        /// <summary>Cambia lo que muestra la pantalla en reposo (lo usa <see cref="TVScreen"/>).</summary>
        public void SetIdle(ScreenIdle mode)
        {
            idle = mode;
            if (started) ApplyIdle();   // antes de Start solo se recuerda: la pantalla se construye una vez cargada la escena
        }

        private void ApplyIdle()
        {
            if (IsPlaying || screen == null) return;
            if (idle == ScreenIdle.None)
            {
                if (content != null) content.enabled = false;
                return;
            }
            EnsureContent();
            content.enabled = true;
            if (idle == ScreenIdle.Dark) DrawDark(); else if (idle == ScreenIdle.Glow) DrawGlow(); else DrawStatic(1f);
        }

        public IEnumerator Play(float duration, Sprite[] frames, VideoClip clip, Action<float> onProgress)
        {
            if (screen == null) yield break;
            IsPlaying = true;
            EnsureContent();
            content.enabled = true;

            bool useVideo = clip != null;
            if (useVideo)
            {
                EnsureVideo();
                vp.clip = clip;
                vp.isLooping = true;   // si el clip es más corto que la secuencia, se repite: el jugador se queda mirando
                vp.Play();
                if (duration <= 0f) duration = (float)clip.length;
            }

            float t = 0f;
            int frameIndex = -1;
            while (t < duration)
            {
                if (useVideo)
                {
                    if (Time.timeScale == 0f) { if (vp.isPlaying) vp.Pause(); }
                    else if (vp.isPaused) vp.Play();
                }
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / duration);
                onProgress?.Invoke(k);

                if (useVideo) CopyVideo();
                else if (frames != null && frames.Length > 0)
                {
                    // El fotograma se dibuja dentro de la textura de la pantalla: toma su forma y perspectiva.
                    int idx = frameSelector != null ? Mathf.Clamp(frameSelector(), 0, frames.Length - 1) : Mathf.Min(frames.Length - 1, Mathf.FloorToInt(k * frames.Length));
                    if (idx != frameIndex) { frameIndex = idx; CaptureFrame(frames[idx]); }
                    if (Time.time >= nextStaticFrame)
                    {
                        nextStaticFrame = Time.time + 0.06f;
                        DrawFrameWithNoise(0.03f + 0.05f * Mathf.PerlinNoise(t * 3f, 0f));   // interferencia leve: la imagen se lee
                    }
                }
                else if (Time.time >= nextStaticFrame) { nextStaticFrame = Time.time + 0.06f; DrawStatic(1f); }
                yield return null;
            }
            onProgress?.Invoke(1f);
            Stop();
        }

        public void Stop()
        {
            frameSelector = null;
            if (vp != null && vp.isPlaying) vp.Stop();
            if (picture != null) picture.enabled = false;
            IsPlaying = false;
            ApplyIdle();
        }

        private void Update()
        {
            UpdateScreen();
        }

        private void UpdateScreen()
        {
            if (!IsPlaying && content != null && content.enabled && idle == ScreenIdle.Static && Time.time >= nextStaticFrame)
            {
                nextStaticFrame = Time.time + 0.07f;
                DrawStatic(1f);
            }

            // La pantalla encendida ilumina un poco lo que tiene cerca, con el parpadeo propio de un tubo.
            if (glow != null)
            {
                bool on = IsPlaying || idle == ScreenIdle.Static || idle == ScreenIdle.Glow;
                float flicker = 0.8f + 0.25f * Mathf.PerlinNoise(Time.time * 9f, 0.3f);
                float target = on ? glowIntensity * flicker : 0f;
                glowLevel = Mathf.MoveTowards(glowLevel, target, Time.deltaTime * 4f);
                glow.intensity = glowLevel;
                glow.enabled = glowLevel > 0.01f;
            }
        }

        private void LateUpdate()
        {
            if (content != null && content.enabled) UpdateOrder();
        }

        private void OnDestroy()
        {
            if (rt != null) { rt.Release(); Destroy(rt); }
            if (contentSprite != null) Destroy(contentSprite);
            if (tex != null) Destroy(tex);
        }

        // ---------- Construcción ----------

        private void EnsureContent()
        {
            if (content != null) return;
            int w = Mathf.Max(8, pixelWidth), h = Mathf.Max(8, pixelHeight);
            tex = new Texture2D(w, h, TextureFormat.RGB24, false)
            {
                filterMode = pointFilter ? FilterMode.Point : FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            buffer = new Color32[w * h];
            contentSprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
            if (NeedsGeometry)
            {
                var verts = new Vector2[4];
                for (int i = 0; i < 4; i++) verts[i] = new Vector2(quad[i].x * w, quad[i].y * h);   // píxeles dentro del rectángulo del sprite
                contentSprite.OverrideGeometry(verts, new ushort[] { 0, 1, 2, 0, 2, 3 });
            }

            var go = new GameObject("ScreenContent");
            go.transform.SetParent(screen.transform, false);
            content = go.AddComponent<SpriteRenderer>();
            content.sprite = contentSprite;
            content.sortingLayerID = screen.sortingLayerID;
            content.maskInteraction = Shaped ? SpriteMaskInteraction.None : SpriteMaskInteraction.VisibleInsideMask;
            if (unlitMaterial != null) content.sharedMaterial = unlitMaterial;

            var pgo = new GameObject("ScreenPicture");
            pgo.transform.SetParent(screen.transform, false);
            picture = pgo.AddComponent<SpriteRenderer>();
            picture.sortingLayerID = screen.sortingLayerID;
            picture.maskInteraction = Shaped ? SpriteMaskInteraction.None : SpriteMaskInteraction.VisibleInsideMask;
            picture.enabled = false;

            if (!Shaped)
            {
                var mgo = new GameObject("ScreenMask");
                mgo.transform.SetParent(screen.transform, false);
                mask = mgo.AddComponent<SpriteMask>();
                mask.sprite = screen.sprite;
                mask.alphaCutoff = 0.01f;
                mask.isCustomRangeActive = true;
            }
            UpdateOrder();
        }

        private void EnsureVideo()
        {
            if (vp != null) return;
            var vgo = new GameObject("ScreenVideo");
            vgo.transform.SetParent(transform, false);
            vp = vgo.AddComponent<VideoPlayer>();
            // El video se reduce a la resolución de la pantalla del escenario: así comparte su textura de píxel.
            rt = new RenderTexture(tex.width, tex.height, 0, RenderTextureFormat.ARGB32) { filterMode = FilterMode.Bilinear };
            vp.playOnAwake = false;
            vp.renderMode = VideoRenderMode.RenderTexture;
            vp.targetTexture = rt;
            // El sonido del video pasa por una fuente propia que sigue al canal Voces (y al volumen general).
            videoAudio = vgo.AddComponent<AudioSource>();
            videoAudio.playOnAwake = false;
            vgo.AddComponent<VolumenPorCategoria>().Categoria = CategoriaSonido.Voces;
            vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
            vp.controlledAudioTrackCount = 1;
            vp.EnableAudioTrack(0, true);
            vp.SetTargetAudioSource(0, videoAudio);
            vp.aspectRatio = VideoAspectRatio.FitOutside;   // llena la pantalla física, sin franjas negras
        }

        private void UpdateOrder()
        {
            // La imagen va justo encima de la pantalla y dentro del rango de órdenes de su mueble (baseOrder..+14):
            // con +40 quedaba por encima de los personajes que pasan delante del escritorio y les tapaba la cara.
            int o = screen.sortingOrder;
            content.sortingOrder = o + 2;
            picture.sortingOrder = o + 3;
            if (mask != null)
            {
                mask.frontSortingLayerID = screen.sortingLayerID;
                mask.backSortingLayerID = screen.sortingLayerID;
                mask.backSortingOrder = o + 2;
                mask.frontSortingOrder = o + 3;
                mask.sprite = screen.sprite;
            }
            Fit(content.transform, contentSprite.bounds.size);
        }

        /// <summary>Ajusta un sprite hijo a la zona de la pantalla.</summary>
        private void Fit(Transform t, Vector2 spriteSize)
        {
            Bounds b = WorldBounds();
            t.position = b.center;
            t.rotation = Quaternion.identity;
            Vector3 p = screen.transform.lossyScale;
            t.localScale = new Vector3(
                b.size.x / Mathf.Max(0.0001f, spriteSize.x) / Mathf.Max(0.0001f, Mathf.Abs(p.x)),
                b.size.y / Mathf.Max(0.0001f, spriteSize.y) / Mathf.Max(0.0001f, Mathf.Abs(p.y)), 1f);
        }

        private void FitPicture()
        {
            if (picture.sprite == null) return;
            Bounds b = WorldBounds();
            var pt = picture.transform;
            pt.position = b.center;
            pt.rotation = Quaternion.identity;
            Vector2 ss = picture.sprite.bounds.size;
            float s = Mathf.Min(b.size.x / Mathf.Max(0.0001f, ss.x), b.size.y / Mathf.Max(0.0001f, ss.y)) * 0.75f;
            Vector3 p = screen.transform.lossyScale;
            pt.localScale = new Vector3(s / Mathf.Max(0.0001f, Mathf.Abs(p.x)), s / Mathf.Max(0.0001f, Mathf.Abs(p.y)), 1f);
        }

        // ---------- Dibujo ----------

        private void CopyVideo()
        {
            if (rt == null || tex == null) return;
            var prev = RenderTexture.active;
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
            RenderTexture.active = prev;
            ApplyScanlines();
        }

        private Color32[] framePixels;

        /// <summary>Copia un sprite (aunque no sea legible) a la resolución de la pantalla mediante la GPU.</summary>
        private void CaptureFrame(Sprite s)
        {
            if (s == null || tex == null) { framePixels = null; return; }
            Color32[] cached;
            if (frameCache.TryGetValue(s, out cached)) { framePixels = cached; return; }
            var t = s.texture;
            Rect r = s.textureRect;
            var tmp = RenderTexture.GetTemporary(tex.width, tex.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(t, tmp, new Vector2(r.width / t.width, r.height / t.height), new Vector2(r.x / t.width, r.y / t.height));
            var prev = RenderTexture.active;
            RenderTexture.active = tmp;
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(tmp);
            framePixels = tex.GetPixels32();
            if (NeedsGeometry) framePixels = WarpToQuad(framePixels, tex.width, tex.height);
            frameCache[s] = framePixels;
        }

        private readonly System.Collections.Generic.Dictionary<Sprite, Color32[]> frameCache = new System.Collections.Generic.Dictionary<Sprite, Color32[]>();
        private Color32[] blendFrom;
        private float blendStart, blendLength;

        /// <summary>La próxima imagen entra fundida con la actual (cambios de toma, no los labios).</summary>
        public void BlendNext(float seconds)
        {
            blendFrom = framePixels;
            blendStart = Time.time;
            blendLength = seconds;
        }

        /// <summary>Lleva el fotograma a la forma de la pantalla (sigue su perspectiva): sin esto, el contorno solo lo recortaba.</summary>
        private Color32[] WarpToQuad(Color32[] src, int w, int h)
        {
            var dst = new Color32[src.Length];
            Vector2 tl = quad[0], tr = quad[1], br = quad[2], bl = quad[3];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float nx = (x + 0.5f) / w, ny = (y + 0.5f) / h;
                    float u = Mathf.Clamp01(Mathf.InverseLerp(tl.x, tr.x, nx));
                    float top = Mathf.Lerp(tl.y, tr.y, u), bottom = Mathf.Lerp(bl.y, br.y, u);
                    float v = Mathf.Clamp01((ny - bottom) / Mathf.Max(0.001f, top - bottom));
                    dst[y * w + x] = src[Mathf.Min(h - 1, (int)(v * h)) * w + Mathf.Min(w - 1, (int)(u * w))];
                }
            return dst;
        }

        /// <summary>El fotograma con interferencia de tubo y una banda que corre.</summary>
        private void DrawFrameWithNoise(float noise)
        {
            if (framePixels == null) { DrawStatic(1f); return; }
            int w = tex.width, h = tex.height;
            float band = (Time.time * 23f) % h;
            float mixB = blendFrom != null && blendLength > 0f ? 1f - (Time.time - blendStart) / blendLength : 0f;
            if (mixB <= 0f) blendFrom = null;
            for (int y = 0; y < h; y++)
            {
                float roll = Mathf.Abs(y - band) < 1.5f ? 0.35f : 0f;
                for (int x = 0; x < w; x++)
                {
                    var c = framePixels[y * w + x];
                    if (mixB > 0f) c = Color32.Lerp(blendFrom[y * w + x], c, 1f - mixB);
                    float n = UnityEngine.Random.value * 255f;
                    float a = noise + roll;
                    buffer[y * w + x] = new Color32((byte)Mathf.Lerp(c.r, n, a), (byte)Mathf.Lerp(c.g, n, a), (byte)Mathf.Lerp(c.b, n, a), 255);
                }
            }
            tex.SetPixels32(buffer);
            ApplyScanlines();
        }

        /// <summary>Estática de tubo: ruido gris con una banda que se desplaza.</summary>
        private void DrawStatic(float strength)
        {
            int w = tex.width, h = tex.height;
            float band = (Time.time * 23f) % h;
            for (int y = 0; y < h; y++)
            {
                float roll = Mathf.Abs(y - band) < 2f ? 0.35f : 0f;
                for (int x = 0; x < w; x++)
                {
                    float n = UnityEngine.Random.value * strength + roll;
                    byte v = (byte)Mathf.Clamp(18f + n * 200f, 0f, 255f);
                    buffer[y * w + x] = new Color32(v, v, (byte)Mathf.Min(255, v + 8), 255);
                }
            }
            tex.SetPixels32(buffer);
            ApplyScanlines();
        }

        /// <summary>Pantalla apagada: vidrio oscuro con un reflejo tenue en diagonal.</summary>
        /// <summary>Pantalla encendida sin imagen: azul brillante con un reflejo diagonal (monitor del nivel 2).</summary>
        private void DrawGlow()
        {
            int w = tex.width, h = tex.height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float cx = x / (float)w - 0.5f, cy = y / (float)h - 0.5f;
                    float center = Mathf.Clamp01(1f - Mathf.Sqrt(cx * cx + cy * cy) * 1.5f);
                    float d = (x / (float)w) + (y / (float)h);
                    float shine = Mathf.Clamp01(1f - Mathf.Abs(d - 0.9f) * 5f) * 0.35f;
                    float k = Mathf.Clamp01(0.35f + center * 0.55f + shine);
                    buffer[y * w + x] = new Color32((byte)(30 + 150 * k), (byte)(80 + 150 * k), (byte)(170 + 85 * k), 255);
                }
            tex.SetPixels32(buffer);
            tex.Apply(false);
        }

        private void DrawDark()
        {
            int w = tex.width, h = tex.height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float d = (x / (float)w) + (y / (float)h);
                    float shine = Mathf.Clamp01(1f - Mathf.Abs(d - 1.35f) * 6f) * 0.07f;
                    byte v = (byte)(12 + shine * 255f);
                    buffer[y * w + x] = new Color32(v, v, (byte)(v + 4), 255);
                }
            tex.SetPixels32(buffer);
            tex.Apply(false);
        }

        private void ApplyScanlines()
        {
            if (scanlines > 0f)
            {
                var px = tex.GetPixels32();
                int w = tex.width;
                float k = 1f - scanlines;
                for (int y = 0; y < tex.height; y += 2)
                    for (int x = 0; x < w; x++)
                    {
                        var c = px[y * w + x];
                        px[y * w + x] = new Color32((byte)(c.r * k), (byte)(c.g * k), (byte)(c.b * k), 255);
                    }
                tex.SetPixels32(px);
            }
            tex.Apply(false);
        }

        public string DebugInfo()
        {
            if (screen == null) return "sin pantalla";
            Bounds wb = WorldBounds();
            return "playing=" + IsPlaying + " idle=" + idle + " content=" + (content != null && content.enabled) +
                   " bounds=" + wb.center + "/" + wb.size + " res=" + pixelWidth + "x" + pixelHeight +
                   " glow=" + (glow != null ? glow.intensity.ToString("0.00") : "-") +
                   " vp=" + (vp != null ? vp.isPlaying + "/" + vp.frame : "-");
        }
    }
}
