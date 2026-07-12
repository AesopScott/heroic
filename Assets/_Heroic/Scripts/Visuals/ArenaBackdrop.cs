using UnityEngine;

namespace Heroic.Visuals
{
    public class ArenaBackdrop : MonoBehaviour
    {
        [SerializeField] private int textureSize = 512;
        [SerializeField] private int gridSpacing = 32;
        [SerializeField] private Color baseColor = new Color(0.025f, 0.045f, 0.055f, 1f);
        [SerializeField] private Color gridColor = new Color(0.12f, 0.32f, 0.36f, 0.42f);
        [SerializeField] private Color runeColor = new Color(0.24f, 0.74f, 0.88f, 0.28f);
        [SerializeField] private Color vignetteColor = new Color(0.006f, 0.012f, 0.02f, 1f);
        [SerializeField] private Vector2 worldSize = new Vector2(60f, 60f);

        private void Awake()
        {
            Build();
        }

        public void Build()
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = gameObject.AddComponent<SpriteRenderer>();
            }

            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat
            };

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    Vector2 uv = new Vector2(x / (float)(textureSize - 1), y / (float)(textureSize - 1));
                    Vector2 centered = (uv - Vector2.one * 0.5f) * 2f;
                    float distance = centered.magnitude;
                    bool grid = x % gridSpacing == 0 || y % gridSpacing == 0;
                    bool majorGrid = x % (gridSpacing * 4) == 0 || y % (gridSpacing * 4) == 0;
                    float ring = Mathf.Max(RingLine(distance, 0.32f, 0.006f), RingLine(distance, 0.58f, 0.005f));
                    float diagonal = Mathf.Abs(Mathf.Abs(centered.x) - Mathf.Abs(centered.y)) < 0.012f
                        ? 0.08f * Mathf.SmoothStep(0.35f, 1f, distance)
                        : 0f;
                    float star = Hash01(x, y) > 0.997f ? 0.45f : 0f;

                    Color color = baseColor;
                    if (grid)
                    {
                        color = Color.Lerp(color, gridColor, majorGrid ? gridColor.a * 1.5f : gridColor.a);
                    }

                    color = Color.Lerp(color, runeColor, Mathf.Clamp01(ring + diagonal + star));
                    color = Color.Lerp(color, vignetteColor, Mathf.Clamp01(Mathf.InverseLerp(0.45f, 1.25f, distance)));
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            renderer.sprite = Sprite.Create(texture, new Rect(0f, 0f, textureSize, textureSize), new Vector2(0.5f, 0.5f), textureSize / worldSize.x);
            renderer.sortingOrder = -100;
            transform.localScale = Vector3.one;
        }

        private static float RingLine(float distance, float radius, float halfWidth)
        {
            return Mathf.Clamp01(1f - Mathf.Abs(distance - radius) / halfWidth);
        }

        private static float Hash01(int x, int y)
        {
            unchecked
            {
                uint hash = (uint)(x * 374761393 + y * 668265263);
                hash = (hash ^ (hash >> 13)) * 1274126177u;
                return (hash ^ (hash >> 16)) / 4294967295f;
            }
        }
    }
}
