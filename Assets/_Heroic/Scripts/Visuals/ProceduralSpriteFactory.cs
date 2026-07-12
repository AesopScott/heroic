using UnityEngine;
using System.Collections.Generic;

namespace Heroic.Visuals
{
    public static class ProceduralSpriteFactory
    {
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static Sprite GetCircle(string key, Color color, int size = 64, float edgeSoftness = 0.08f)
        {
            string cacheKey = $"circle:{key}:{ColorUtility.ToHtmlStringRGBA(color)}:{size}:{edgeSoftness}";
            if (Cache.TryGetValue(cacheKey, out Sprite cached))
            {
                return cached;
            }

            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.45f;
            float softEdge = Mathf.Max(1f, size * edgeSoftness);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01((radius - distance) / softEdge);
                    Color pixel = color;
                    pixel.a *= alpha;
                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            Cache[cacheKey] = sprite;
            return sprite;
        }

        public static Sprite GetDiamond(string key, Color color, int size = 64)
        {
            string cacheKey = $"diamond:{key}:{ColorUtility.ToHtmlStringRGBA(color)}:{size}";
            if (Cache.TryGetValue(cacheKey, out Sprite cached))
            {
                return cached;
            }

            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.45f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Mathf.Abs(x - center.x) + Mathf.Abs(y - center.y);
                    Color pixel = color;
                    pixel.a *= distance <= radius ? 1f : 0f;
                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            Cache[cacheKey] = sprite;
            return sprite;
        }

        public static Sprite GetRing(string key, Color color, int size = 64, float thickness = 0.12f, float edgeSoftness = 0.04f)
        {
            string cacheKey = $"ring:{key}:{ColorUtility.ToHtmlStringRGBA(color)}:{size}:{thickness}:{edgeSoftness}";
            if (Cache.TryGetValue(cacheKey, out Sprite cached))
            {
                return cached;
            }

            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float outerRadius = size * 0.45f;
            float innerRadius = outerRadius * Mathf.Clamp01(1f - thickness);
            float softEdge = Mathf.Max(1f, size * edgeSoftness);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float outerAlpha = Mathf.Clamp01((outerRadius - distance) / softEdge);
                    float innerAlpha = Mathf.Clamp01((distance - innerRadius) / softEdge);
                    Color pixel = color;
                    pixel.a *= Mathf.Min(outerAlpha, innerAlpha);
                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            Cache[cacheKey] = sprite;
            return sprite;
        }

        public static Sprite GetTriangle(string key, Color color, int size = 64)
        {
            string cacheKey = $"triangle:{key}:{ColorUtility.ToHtmlStringRGBA(color)}:{size}";
            if (Cache.TryGetValue(cacheKey, out Sprite cached))
            {
                return cached;
            }

            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.45f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float px = (x - center.x) / radius;
                    float py = (y - center.y) / radius;
                    float widthAtY = Mathf.Lerp(0.98f, 0.05f, Mathf.InverseLerp(-0.85f, 0.85f, py));
                    bool inside = py >= -0.85f && py <= 0.85f && Mathf.Abs(px) <= widthAtY;
                    Color pixel = color;
                    pixel.a *= inside ? 1f : 0f;
                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            Cache[cacheKey] = sprite;
            return sprite;
        }

        public static Sprite GetSolid(string key, Color color, int size = 16)
        {
            string cacheKey = $"solid:{key}:{ColorUtility.ToHtmlStringRGBA(color)}:{size}";
            if (Cache.TryGetValue(cacheKey, out Sprite cached))
            {
                return cached;
            }

            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            Cache[cacheKey] = sprite;
            return sprite;
        }
    }
}
