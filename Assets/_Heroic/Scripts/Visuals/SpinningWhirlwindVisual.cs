using UnityEngine;

namespace Heroic.Visuals
{
    public class SpinningWhirlwindVisual : MonoBehaviour
    {
        private Transform target;
        private SpriteRenderer[] renderers;
        private Color[] startColors;
        private float radius;
        private float duration;
        private float spinSpeed;
        private float startedAt;

        public static SpinningWhirlwindVisual Attach(Transform target, float radius, float duration, float spinSpeed)
        {
            GameObject effectObject = new GameObject("Whirlwind Visual");
            var visual = effectObject.AddComponent<SpinningWhirlwindVisual>();
            visual.Initialize(target, radius, duration, spinSpeed);
            return visual;
        }

        private void Initialize(Transform followTarget, float visualRadius, float lifetime, float degreesPerSecond)
        {
            target = followTarget;
            radius = Mathf.Max(0.25f, visualRadius);
            duration = Mathf.Max(0.1f, lifetime);
            spinSpeed = degreesPerSecond;
            startedAt = Time.time;

            if (target != null)
            {
                transform.position = target.position;
            }

            renderers = new SpriteRenderer[4];
            startColors = new Color[renderers.Length];
            CreateRing(0, radius * 2.1f, new Color(1f, 0.72f, 0.18f, 0.42f), 35f);
            CreateRing(1, radius * 1.55f, new Color(1f, 0.38f, 0.08f, 0.34f), -20f);
            CreateSlash(2, radius * 1.9f, new Color(1f, 0.92f, 0.42f, 0.5f), 0f);
            CreateSlash(3, radius * 1.45f, new Color(1f, 0.42f, 0.08f, 0.45f), 90f);
        }

        private void CreateRing(int index, float scale, Color color, float rotation)
        {
            GameObject child = new GameObject("Whirlwind Ring");
            child.transform.SetParent(transform, false);
            child.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            child.transform.localScale = Vector3.one * scale;

            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = ProceduralSpriteFactory.GetRing($"whirlwind-ring-{index}", color, 96, 0.18f, 0.03f);
            renderer.sortingOrder = 45 + index;
            renderer.color = color;
            renderers[index] = renderer;
            startColors[index] = color;
        }

        private void CreateSlash(int index, float scale, Color color, float rotation)
        {
            GameObject child = new GameObject("Whirlwind Slash");
            child.transform.SetParent(transform, false);
            child.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            child.transform.localScale = new Vector3(scale * 0.18f, scale, 1f);

            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = ProceduralSpriteFactory.GetDiamond($"whirlwind-slash-{index}", color, 48);
            renderer.sortingOrder = 50 + index;
            renderer.color = color;
            renderers[index] = renderer;
            startColors[index] = color;
        }

        private void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            float percent = Mathf.Clamp01((Time.time - startedAt) / duration);
            transform.position = target.position;
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

            float pulse = 1f + Mathf.Sin(Time.time * 18f) * 0.08f;
            transform.localScale = Vector3.one * pulse;

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null)
                {
                    continue;
                }

                Color color = startColors[i];
                color.a *= 1f - percent;
                renderers[i].color = color;
            }

            if (percent >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
