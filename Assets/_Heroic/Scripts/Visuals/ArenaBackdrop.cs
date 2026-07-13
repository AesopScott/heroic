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
        [SerializeField] private Texture2D dirtSourceTexture;
        [SerializeField] private Texture2D[] dirtSourceTextures = new Texture2D[0];
        [SerializeField] private bool useSourceTextureDirectly;

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

            if (useSourceTextureDirectly && dirtSourceTexture != null)
            {
                renderer.sprite = Sprite.Create(
                    dirtSourceTexture,
                    new Rect(0f, 0f, dirtSourceTexture.width, dirtSourceTexture.height),
                    new Vector2(0.5f, 0.5f),
                    dirtSourceTexture.width / worldSize.x);
                renderer.sortingOrder = -100;
                transform.localScale = Vector3.one;
                return;
            }

            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
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

                    Color color = HasSourceTextures()
                        ? SampleTerrainSource(uv, softNoise)
                        : Color.Lerp(Color.Lerp(dirtDarkColor, dirtLightColor, softNoise), dirtBaseColor, 0.45f);

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

        private Color SampleTerrainSource(Vector2 uv, float softNoise)
        {
            Texture2D source = PickSourceTexture(uv);
            if (source == null)
            {
                return dirtBaseColor;
            }

            Vector2 tiled = new Vector2(uv.x * 3.2f, uv.y * 3.2f);
            tiled += new Vector2(softNoise * 0.08f, Hash01(Mathf.FloorToInt(uv.x * 17f), Mathf.FloorToInt(uv.y * 17f)) * 0.08f);
            float u = Mathf.PingPong(tiled.x, 1f);
            float v = Mathf.PingPong(tiled.y, 1f);
            Color sampled = source.GetPixelBilinear(u, v);
            return Color.Lerp(sampled, dirtBaseColor, 0.12f);
        }

        private bool HasSourceTextures()
        {
            if (dirtSourceTextures != null)
            {
                for (int i = 0; i < dirtSourceTextures.Length; i++)
                {
                    if (dirtSourceTextures[i] != null)
                    {
                        return true;
                    }
                }
            }

            return dirtSourceTexture != null;
        }

        private Texture2D PickSourceTexture(Vector2 uv)
        {
            if (dirtSourceTextures != null && dirtSourceTextures.Length > 0)
            {
                int cellX = Mathf.FloorToInt(uv.x * 6f);
                int cellY = Mathf.FloorToInt(uv.y * 6f);
                int index = Mathf.Abs(Mathf.FloorToInt(Hash01(cellX, cellY) * dirtSourceTextures.Length)) % dirtSourceTextures.Length;
                Texture2D picked = dirtSourceTextures[index];
                if (picked != null)
                {
                    return picked;
                }
            }

            return dirtSourceTexture;
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
