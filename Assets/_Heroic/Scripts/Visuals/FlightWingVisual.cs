using UnityEngine;

namespace Heroic.Visuals
{
    public class FlightWingVisual : MonoBehaviour
    {
        [SerializeField] private float duration = 4f;
        [SerializeField] private float flapSpeed = 12f;

        private Transform leftWing;
        private Transform rightWing;
        private float startedAt;

        public static FlightWingVisual Attach(Transform target, float durationSeconds)
        {
            GameObject root = new GameObject("Flight Wings");
            root.transform.SetParent(target, false);
            root.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            FlightWingVisual visual = root.AddComponent<FlightWingVisual>();
            visual.duration = Mathf.Max(0.2f, durationSeconds);
            visual.Build();
            return visual;
        }

        private void Build()
        {
            startedAt = Time.time;
            leftWing = CreateWing("Left Wing", -1f);
            rightWing = CreateWing("Right Wing", 1f);
        }

        private Transform CreateWing(string wingName, float side)
        {
            GameObject wing = new GameObject(wingName);
            wing.transform.SetParent(transform, false);
            wing.transform.localPosition = new Vector3(side * 0.58f, 0.08f, -0.01f);
            wing.transform.localScale = new Vector3(side * 0.82f, 0.48f, 1f);

            SpriteRenderer renderer = wing.AddComponent<SpriteRenderer>();
            renderer.sprite = ProceduralSpriteFactory.GetTriangle("flight-wing", new Color(0.78f, 0.95f, 1f, 0.82f), 96);
            renderer.sortingOrder = 34;
            return wing.transform;
        }

        private void Update()
        {
            float elapsed = Time.time - startedAt;
            float percent = Mathf.Clamp01(elapsed / duration);
            float flap = Mathf.Sin(elapsed * flapSpeed) * 18f;
            float fade = 1f - Mathf.SmoothStep(0.78f, 1f, percent);

            ApplyWing(leftWing, -1f, -30f - flap, fade);
            ApplyWing(rightWing, 1f, 30f + flap, fade);

            if (elapsed >= duration)
            {
                Destroy(gameObject);
            }
        }

        private static void ApplyWing(Transform wing, float side, float angle, float alpha)
        {
            if (wing == null)
            {
                return;
            }

            wing.localRotation = Quaternion.Euler(0f, 0f, angle);
            wing.localPosition = new Vector3(side * (0.54f + Mathf.Abs(Mathf.Sin(Time.time * 12f)) * 0.08f), 0.08f, -0.01f);
            SpriteRenderer renderer = wing.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = 0.82f * alpha;
                renderer.color = color;
            }
        }
    }
}
