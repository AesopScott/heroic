using UnityEngine;

namespace Heroic.Visuals
{
    public class VisualPresetApplier : MonoBehaviour
    {
        public enum Preset
        {
            Player,
            BasicEnemy,
            FastEnemy,
            TankEnemy,
            Boss,
            MagicMissile,
            ArcaneOrb,
            ExperiencePickup,
            ArcaneArea
        }

        [SerializeField] private Preset preset;
        [SerializeField] private bool applyOnAwake = true;

        private void Awake()
        {
            if (applyOnAwake)
            {
                Apply();
            }
        }

        public void Apply()
        {
            ClearGeneratedLayers();

            AutoSpriteVisual visual = GetComponent<AutoSpriteVisual>();
            if (visual == null)
            {
                visual = gameObject.AddComponent<AutoSpriteVisual>();
            }

            ApplyPresetObject(preset, visual);
        }

        public void ApplyPreset(Preset newPreset)
        {
            preset = newPreset;
            Apply();
        }

        private void ApplyPresetObject(Preset selectedPreset, AutoSpriteVisual visual)
        {
            // Preset values are intentionally stored by serialized fields on AutoSpriteVisual after Apply in the editor.
            switch (selectedPreset)
            {
                case Preset.Player:
                    ApplyVisual(visual, AutoSpriteVisual.Shape.Diamond, new Color(0.08f, 0.22f, 0.3f), new Vector2(0.85f, 0.85f), 20, true, false, 0.025f, 2.4f);
                    EnsureLayer("SpellbookPages", AutoSpriteVisual.Shape.Diamond, new Color(0.58f, 0.9f, 1f, 0.9f), new Vector2(0.5f, 0.66f), 21, true, false, Vector2.zero, 45f, 0.025f, 3.2f);
                    EnsureLayer("SpellbookGem", AutoSpriteVisual.Shape.Circle, new Color(0.92f, 1f, 1f), new Vector2(0.22f, 0.22f), 22, true, false, Vector2.zero, 0f, 0.09f, 5.4f);
                    break;
                case Preset.BasicEnemy:
                    ApplyVisual(visual, AutoSpriteVisual.Shape.Diamond, new Color(0.95f, 0.12f, 0.14f), new Vector2(0.78f, 0.78f), 10, false, true, 0.08f, 3f, 70f);
                    EnsureLayer("EnemyOutline", AutoSpriteVisual.Shape.Ring, new Color(1f, 0.48f, 0.26f, 0.62f), new Vector2(0.94f, 0.94f), 9, false, true, Vector2.zero, 0f, 0.08f, 3.5f, -55f);
                    EnsureLayer("EnemyCore", AutoSpriteVisual.Shape.Circle, new Color(1f, 0.82f, 0.38f), new Vector2(0.24f, 0.24f), 11, true, false, Vector2.zero, 0f, 0.08f, 5.2f);
                    EnsureLayer("EnemyEye", AutoSpriteVisual.Shape.Diamond, new Color(0.16f, 0.02f, 0.025f, 0.95f), new Vector2(0.18f, 0.11f), 12, false, false);
                    break;
                case Preset.FastEnemy:
                    ApplyVisual(visual, AutoSpriteVisual.Shape.Triangle, new Color(1f, 0.86f, 0.08f), new Vector2(0.48f, 0.82f), 12, false, true, 0.08f, 3f, 190f);
                    EnsureLayer("FastAfterimageA", AutoSpriteVisual.Shape.Triangle, new Color(1f, 0.54f, 0.08f, 0.34f), new Vector2(0.72f, 1.02f), 9, true, false, new Vector2(0f, -0.12f), 180f, 0.06f, 6.4f);
                    EnsureLayer("FastAfterimageB", AutoSpriteVisual.Shape.Triangle, new Color(1f, 0.32f, 0.06f, 0.22f), new Vector2(0.88f, 1.24f), 8, true, false, new Vector2(0f, -0.24f), 180f, 0.05f, 7.2f);
                    EnsureLayer("FastNeedle", AutoSpriteVisual.Shape.Diamond, new Color(1f, 0.98f, 0.58f, 0.9f), new Vector2(0.12f, 0.48f), 13, true, false, new Vector2(0f, 0.18f), 0f, 0.05f, 7.8f);
                    break;
                case Preset.TankEnemy:
                    ApplyVisual(visual, AutoSpriteVisual.Shape.Circle, new Color(0.68f, 0.1f, 0.12f), new Vector2(1.22f, 1.22f), 10, false, false);
                    EnsureLayer("TankPlate", AutoSpriteVisual.Shape.Ring, new Color(1f, 0.52f, 0.24f, 0.9f), new Vector2(1.42f, 1.42f), 11, false, true, Vector2.zero, 0f, 0.08f, 3f, 32f);
                    EnsureLayer("TankShoulderA", AutoSpriteVisual.Shape.Diamond, new Color(0.96f, 0.32f, 0.18f, 0.82f), new Vector2(0.42f, 0.72f), 12, false, false, new Vector2(-0.42f, 0f), 0f);
                    EnsureLayer("TankShoulderB", AutoSpriteVisual.Shape.Diamond, new Color(0.96f, 0.32f, 0.18f, 0.82f), new Vector2(0.42f, 0.72f), 12, false, false, new Vector2(0.42f, 0f), 0f);
                    EnsureLayer("TankCore", AutoSpriteVisual.Shape.Diamond, new Color(0.16f, 0.025f, 0.035f, 0.95f), new Vector2(0.5f, 0.5f), 13, false, false);
                    EnsureLayer("TankWarning", AutoSpriteVisual.Shape.Circle, new Color(1f, 0.86f, 0.32f, 0.9f), new Vector2(0.18f, 0.18f), 14, true, false, Vector2.zero, 0f, 0.08f, 4.8f);
                    break;
                case Preset.Boss:
                    ApplyVisual(visual, AutoSpriteVisual.Shape.Diamond, new Color(0.96f, 0.02f, 0.34f), new Vector2(2.18f, 2.18f), 12, true, true, 0.035f, 2.2f, 22f);
                    EnsureLayer("BossOuterHalo", AutoSpriteVisual.Shape.Ring, new Color(1f, 0.3f, 0.74f, 0.28f), new Vector2(3.05f, 3.05f), 10, true, true, Vector2.zero, 0f, 0.03f, 1.4f, 12f);
                    EnsureLayer("BossHalo", AutoSpriteVisual.Shape.Ring, new Color(1f, 0.16f, 0.62f, 0.56f), new Vector2(2.62f, 2.62f), 11, true, true, Vector2.zero, 0f, 0.035f, 1.7f, -18f);
                    EnsureLayer("BossCrossA", AutoSpriteVisual.Shape.Diamond, new Color(1f, 0.5f, 0.82f, 0.42f), new Vector2(0.32f, 2.82f), 12, true, false, Vector2.zero, 0f, 0.025f, 1.8f);
                    EnsureLayer("BossCrossB", AutoSpriteVisual.Shape.Diamond, new Color(1f, 0.5f, 0.82f, 0.42f), new Vector2(2.82f, 0.32f), 12, true, false, Vector2.zero, 0f, 0.025f, 1.8f);
                    EnsureLayer("BossCore", AutoSpriteVisual.Shape.Circle, new Color(0.08f, 0.01f, 0.06f, 0.95f), new Vector2(0.92f, 0.92f), 13, true, false, Vector2.zero, 0f, 0.05f, 4.2f);
                    EnsureLayer("BossEye", AutoSpriteVisual.Shape.Diamond, new Color(1f, 0.92f, 1f, 0.96f), new Vector2(0.5f, 0.18f), 15, true, false, Vector2.zero, 0f, 0.06f, 5.4f);
                    EnsureLayer("BossNorthRune", AutoSpriteVisual.Shape.Triangle, new Color(1f, 0.84f, 0.95f, 0.84f), new Vector2(0.42f, 0.5f), 14, true, false, new Vector2(0f, 1.22f), 0f, 0.07f, 4.7f);
                    EnsureLayer("BossSouthRune", AutoSpriteVisual.Shape.Triangle, new Color(1f, 0.84f, 0.95f, 0.84f), new Vector2(0.42f, 0.5f), 14, true, false, new Vector2(0f, -1.22f), 180f, 0.07f, 4.7f);
                    EnsureLayer("BossEastRune", AutoSpriteVisual.Shape.Triangle, new Color(1f, 0.84f, 0.95f, 0.84f), new Vector2(0.42f, 0.5f), 14, true, false, new Vector2(1.22f, 0f), -90f, 0.07f, 4.7f);
                    EnsureLayer("BossWestRune", AutoSpriteVisual.Shape.Triangle, new Color(1f, 0.84f, 0.95f, 0.84f), new Vector2(0.42f, 0.5f), 14, true, false, new Vector2(-1.22f, 0f), 90f, 0.07f, 4.7f);
                    break;
                case Preset.MagicMissile:
                    ApplyVisual(visual, AutoSpriteVisual.Shape.Circle, new Color(0.7f, 0.95f, 1f), new Vector2(0.28f, 0.28f), 30, true, false, 0.12f, 8f);
                    EnsureLayer("MissileHalo", AutoSpriteVisual.Shape.Ring, new Color(0.35f, 0.9f, 1f, 0.75f), new Vector2(0.48f, 0.48f), 29, true, true, Vector2.zero, 0f, 0.08f, 7f, 180f);
                    EnsureTrail(new Color(0.4f, 0.9f, 1f, 0.8f));
                    break;
                case Preset.ArcaneOrb:
                    ApplyVisual(visual, AutoSpriteVisual.Shape.Ring, new Color(0.45f, 0.75f, 1f, 0.86f), new Vector2(0.35f, 0.35f), 25, true, true, 0.05f, 4f, 120f);
                    EnsureLayer("OrbCore", AutoSpriteVisual.Shape.Circle, new Color(0.88f, 1f, 1f), new Vector2(0.18f, 0.18f), 26, true, false, Vector2.zero, 0f, 0.1f, 6.4f);
                    EnsureTrail(new Color(0.35f, 0.8f, 1f, 0.7f));
                    break;
                case Preset.ExperiencePickup:
                    ApplyVisual(visual, AutoSpriteVisual.Shape.Diamond, new Color(0.35f, 1f, 0.55f), new Vector2(0.35f, 0.35f), 15, true, true, 0.1f, 4.6f, 95f);
                    EnsureLayer("PickupSparkle", AutoSpriteVisual.Shape.Ring, new Color(0.8f, 1f, 0.72f, 0.55f), new Vector2(0.52f, 0.52f), 14, true, true, Vector2.zero, 0f, 0.08f, 5.2f, -110f);
                    break;
                case Preset.ArcaneArea:
                    ApplyVisual(visual, AutoSpriteVisual.Shape.Ring, new Color(0.35f, 0.8f, 1f, 0.45f), new Vector2(1.55f, 1.55f), 5, true, true, 0.04f, 2.6f, 35f);
                    EnsureLayer("AreaCore", AutoSpriteVisual.Shape.Circle, new Color(0.2f, 0.65f, 1f, 0.16f), new Vector2(1.28f, 1.28f), 4, true, false, Vector2.zero, 0f, 0.03f, 2.1f);
                    break;
            }
        }

        private void ApplyVisual(AutoSpriteVisual visual, AutoSpriteVisual.Shape shape, Color color, Vector2 size, int sortingOrder, bool pulse, bool rotate, float pulseAmount = 0.08f, float pulseSpeed = 3f, float rotationSpeed = 90f)
        {
            visual.Configure(shape, color, size, sortingOrder, pulse, rotate, pulseAmount, pulseSpeed, rotationSpeed);
        }

        private AutoSpriteVisual EnsureLayer(string key, AutoSpriteVisual.Shape shape, Color color, Vector2 size, int sortingOrder, bool pulse, bool rotate, Vector2 offset = default, float rotationDegrees = 0f, float pulseAmount = 0.08f, float pulseSpeed = 3f, float rotationSpeed = 90f)
        {
            GameObject layer = new GameObject("VisualLayer_" + key);
            layer.transform.SetParent(transform, false);
            layer.transform.localPosition = offset;
            layer.transform.localRotation = Quaternion.Euler(0f, 0f, rotationDegrees);

            AutoSpriteVisual visual = layer.AddComponent<AutoSpriteVisual>();
            visual.Configure(shape, color, size, sortingOrder, pulse, rotate, pulseAmount, pulseSpeed, rotationSpeed);
            return visual;
        }

        private void ClearGeneratedLayers()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (!child.name.StartsWith("VisualLayer_"))
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private void EnsureTrail(Color color)
        {
            TrailRenderer trail = GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = gameObject.AddComponent<TrailRenderer>();
            }

            AutoTrailVisual autoTrail = GetComponent<AutoTrailVisual>();
            if (autoTrail == null)
            {
                autoTrail = gameObject.AddComponent<AutoTrailVisual>();
            }

            autoTrail.Configure(color, new Color(color.r, color.g, color.b, 0f), 0.2f, 0.18f, 0f);
        }
    }
}
