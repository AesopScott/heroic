using Heroic.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heroic.Visuals
{
    public class ArenaBackdrop : MonoBehaviour
    {
        [SerializeField] private int textureSize = 512;
        [SerializeField] private Color dirtBaseColor = new Color(0.35f, 0.22f, 0.12f, 1f);
        [SerializeField] private Color dirtLightColor = new Color(0.47f, 0.31f, 0.17f, 1f);
        [SerializeField] private Color dirtDarkColor = new Color(0.22f, 0.13f, 0.07f, 1f);
        [SerializeField] private Color rockColor = new Color(0.30f, 0.28f, 0.24f, 1f);
        [SerializeField] private Color pebbleColor = new Color(0.42f, 0.36f, 0.28f, 1f);
        [SerializeField] private Color edgeVignetteColor = new Color(0.12f, 0.07f, 0.035f, 1f);
        [SerializeField] private Vector2 worldSize = new Vector2(60f, 60f);

        private void Awake()
        {
            Build();
            EnsureTerrainManager();
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

                    float fineNoise = Hash01(x, y);
                    float softNoise = (
                        Hash01(x / 4, y / 4) +
                        Hash01((x + 19) / 8, (y + 37) / 8) +
                        Hash01((x + 71) / 16, (y + 11) / 16)) / 3f;

                    Color color = Color.Lerp(dirtDarkColor, dirtLightColor, softNoise);
                    color = Color.Lerp(color, dirtBaseColor, 0.45f);

                    // Decorative only: rocks and pebbles are baked into the backdrop texture, no colliders.
                    float rock = RockMask(x, y, 41, 0.16f);
                    float pebble = fineNoise > 0.982f ? 0.32f : 0f;
                    color = Color.Lerp(color, pebbleColor, pebble);
                    color = Color.Lerp(color, rockColor, rock);

                    float grain = (fineNoise - 0.5f) * 0.08f;
                    color.r = Mathf.Clamp01(color.r + grain);
                    color.g = Mathf.Clamp01(color.g + grain);
                    color.b = Mathf.Clamp01(color.b + grain);

                    color = Color.Lerp(color, edgeVignetteColor, Mathf.Clamp01(Mathf.InverseLerp(0.7f, 1.3f, distance)) * 0.45f);
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            renderer.sprite = Sprite.Create(texture, new Rect(0f, 0f, textureSize, textureSize), new Vector2(0.5f, 0.5f), textureSize / worldSize.x);
            renderer.sortingOrder = -100;
            transform.localScale = Vector3.one;
        }

        private static float RockMask(int x, int y, int cellSize, float chance)
        {
            int cellX = Mathf.FloorToInt(x / (float)cellSize);
            int cellY = Mathf.FloorToInt(y / (float)cellSize);
            float cellRoll = Hash01(cellX, cellY);
            if (cellRoll > chance)
            {
                return 0f;
            }

            float centerX = (cellX + 0.5f + (Hash01(cellX + 13, cellY) - 0.5f) * 0.45f) * cellSize;
            float centerY = (cellY + 0.5f + (Hash01(cellX, cellY + 29) - 0.5f) * 0.45f) * cellSize;
            float radius = Mathf.Lerp(3f, 8f, Hash01(cellX + 7, cellY + 5));
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
            return Mathf.Clamp01(1f - distance / radius);
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

        private void EnsureTerrainManager()
        {
            if (SceneManager.GetActiveScene().name != "Game")
            {
                return;
            }

            if (FindAnyObjectByType<TerrainManager>() != null)
            {
                return;
            }

            GameObject terrainObject = new GameObject("TerrainManager");
            terrainObject.AddComponent<TerrainManager>();
        }
    }
}
