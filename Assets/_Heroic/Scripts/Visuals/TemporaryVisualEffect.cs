using UnityEngine;

namespace Heroic.Visuals
{
    public class TemporaryVisualEffect : MonoBehaviour
    {
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private float startScale = 1f;
        [SerializeField] private float endScale = 1.35f;

        private float startedAt;
        private SpriteRenderer spriteRenderer;
        private Color startColor;

        public static TemporaryVisualEffect CreateCircle(Vector2 position, Color color, float scale, float durationSeconds)
        {
            GameObject effectObject = new GameObject("Temporary Visual Effect");
            effectObject.transform.position = position;

            var renderer = effectObject.AddComponent<SpriteRenderer>();
            renderer.sprite = ProceduralSpriteFactory.GetCircle("temporary-effect", color);
            renderer.sortingOrder = 40;

            var effect = effectObject.AddComponent<TemporaryVisualEffect>();
            effect.duration = Mathf.Max(0.05f, durationSeconds);
            effect.startScale = scale;
            effect.endScale = scale * 1.35f;
            effect.Initialize(renderer, color);
            return effect;
        }

        private void Initialize(SpriteRenderer renderer, Color color)
        {
            spriteRenderer = renderer;
            startColor = color;
            transform.localScale = Vector3.one * startScale;
        }

        private void Awake()
        {
            startedAt = Time.time;
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
            }
        }

        private void Update()
        {
            float percent = Mathf.Clamp01((Time.time - startedAt) / duration);
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, percent);

            if (spriteRenderer != null)
            {
                Color color = startColor;
                color.a *= 1f - percent;
                spriteRenderer.color = color;
            }

            if (percent >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
