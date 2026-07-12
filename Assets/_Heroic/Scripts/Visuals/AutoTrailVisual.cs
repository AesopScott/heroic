using UnityEngine;

namespace Heroic.Visuals
{
    [RequireComponent(typeof(TrailRenderer))]
    public class AutoTrailVisual : MonoBehaviour
    {
        private static Material sharedTrailMaterial;

        [SerializeField] private Color startColor = Color.white;
        [SerializeField] private Color endColor = new Color(1f, 1f, 1f, 0f);
        [SerializeField] private float lifetime = 0.18f;
        [SerializeField] private float startWidth = 0.18f;
        [SerializeField] private float endWidth = 0f;

        private void Awake()
        {
            Apply();
        }

        public void Configure(Color newStartColor, Color newEndColor, float newLifetime, float newStartWidth, float newEndWidth)
        {
            startColor = newStartColor;
            endColor = newEndColor;
            lifetime = newLifetime;
            startWidth = newStartWidth;
            endWidth = newEndWidth;
            Apply();
        }

        private void Apply()
        {
            TrailRenderer trail = GetComponent<TrailRenderer>();
            trail.time = lifetime;
            trail.startWidth = startWidth;
            trail.endWidth = endWidth;
            trail.startColor = startColor;
            trail.endColor = endColor;
            trail.autodestruct = false;
            Material trailMaterial = GetTrailMaterial();
            if (trailMaterial != null)
            {
                trail.sharedMaterial = trailMaterial;
            }
            trail.sortingOrder = 2;
        }

        private static Material GetTrailMaterial()
        {
            if (sharedTrailMaterial != null)
            {
                return sharedTrailMaterial;
            }

            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
            }

            if (shader == null)
            {
                return null;
            }

            sharedTrailMaterial = new Material(shader)
            {
                name = "Heroic Procedural Trail",
                hideFlags = HideFlags.HideAndDontSave
            };
            return sharedTrailMaterial;
        }
    }
}
