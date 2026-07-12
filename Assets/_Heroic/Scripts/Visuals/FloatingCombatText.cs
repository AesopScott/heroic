using TMPro;
using UnityEngine;

namespace Heroic.Visuals
{
    public class FloatingCombatText : MonoBehaviour
    {
        [SerializeField] private float duration = 0.65f;
        [SerializeField] private float riseDistance = 0.75f;

        private static TMP_FontAsset runtimeFont;
        private TMP_Text label;
        private Vector3 startPosition;
        private Color startColor;
        private float startedAt;

        public static FloatingCombatText Create(string text, Vector3 position, Color color, float fontSize = 4.5f)
        {
            GameObject go = new GameObject("FloatingCombatText");
            go.transform.position = position;

            TextMeshPro textMesh = go.AddComponent<TextMeshPro>();
            TMP_FontAsset fontAsset = ResolveRuntimeFont();
            if (fontAsset != null)
            {
                textMesh.font = fontAsset;
            }

            textMesh.text = text;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            Renderer textRenderer = textMesh.GetComponent<Renderer>();
            if (textRenderer != null)
            {
                textRenderer.sortingOrder = 90;
            }

            FloatingCombatText floatingText = go.AddComponent<FloatingCombatText>();
            floatingText.Initialize(textMesh, color);
            return floatingText;
        }

        private static TMP_FontAsset ResolveRuntimeFont()
        {
            if (runtimeFont == null)
            {
                runtimeFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Heroic Runtime SDF");
            }

            if (runtimeFont == null)
            {
                runtimeFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            }

            return runtimeFont;
        }

        private void Initialize(TMP_Text textLabel, Color color)
        {
            label = textLabel;
            startColor = color;
        }

        private void Awake()
        {
            startedAt = Time.time;
            startPosition = transform.position;
            if (label == null)
            {
                label = GetComponent<TMP_Text>();
                startColor = label != null ? label.color : Color.white;
            }
        }

        private void Update()
        {
            float percent = Mathf.Clamp01((Time.time - startedAt) / duration);
            transform.position = Vector3.Lerp(startPosition, startPosition + Vector3.up * riseDistance, percent);

            if (label != null)
            {
                Color color = startColor;
                color.a *= 1f - percent;
                label.color = color;
            }

            if (percent >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
