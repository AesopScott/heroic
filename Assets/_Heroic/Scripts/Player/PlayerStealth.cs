using UnityEngine;

namespace Heroic.Player
{
    public class PlayerStealth : MonoBehaviour
    {
        [SerializeField] private float invisibleAlpha = 0.34f;
        [SerializeField] private Color shimmerColor = new Color(0.58f, 0.72f, 1f, 1f);

        private SpriteRenderer[] renderers = new SpriteRenderer[0];
        private bool invisible;

        public bool IsInvisible => invisible;
        public float VisualAlpha => invisible ? invisibleAlpha : 1f;

        private void Awake()
        {
            RefreshRenderers();
        }

        private void OnDisable()
        {
            SetInvisible(false);
        }

        public void SetInvisible(bool value)
        {
            invisible = value;
            ApplyVisualState();
        }

        public Color ApplyToBaseColor(Color baseColor)
        {
            if (!invisible)
            {
                baseColor.a = 1f;
                return baseColor;
            }

            Color ghosted = Color.Lerp(baseColor, shimmerColor, 0.45f);
            ghosted.a = invisibleAlpha;
            return ghosted;
        }

        private void ApplyVisualState()
        {
            RefreshRenderers();
            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                renderer.color = ApplyToBaseColor(renderer.color);
            }
        }

        private void RefreshRenderers()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>(true);
        }
    }
}
