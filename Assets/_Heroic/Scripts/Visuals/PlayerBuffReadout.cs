using Heroic.Player;
using TMPro;
using UnityEngine;

namespace Heroic.Visuals
{
    public class PlayerBuffReadout : MonoBehaviour
    {
        [SerializeField] private Vector2 offset = new Vector2(0f, -0.72f);
        [SerializeField] private float fontSize = 2.6f;
        [SerializeField] private Color textColor = new Color(0.72f, 1f, 0.72f, 1f);
        [SerializeField] private Color invulnerableColor = new Color(1f, 0.68f, 1f, 1f);

        private PlayerTemporaryBuffs buffs;
        private MovementCaster movementCaster;
        private TextMeshPro textMesh;

        private void Awake()
        {
            buffs = GetComponent<PlayerTemporaryBuffs>();
            movementCaster = GetComponent<MovementCaster>();
            CreateText();
        }

        private void LateUpdate()
        {
            if (textMesh == null || buffs == null)
            {
                return;
            }

            string text = BuildBuffText(out Color color);
            textMesh.text = text;
            textMesh.color = color;
            textMesh.gameObject.SetActive(!string.IsNullOrEmpty(text));
        }

        private void CreateText()
        {
            GameObject textObject = new GameObject("PlayerBuffReadout");
            textObject.transform.SetParent(transform, false);
            textObject.transform.localPosition = offset;
            textMesh = textObject.AddComponent<TextMeshPro>();
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSize = fontSize;
            textMesh.fontStyle = FontStyles.Bold;
            textMesh.sortingOrder = 45;
            textMesh.enableWordWrapping = false;
            textMesh.text = string.Empty;
            textMesh.gameObject.SetActive(false);
        }

        private string BuildBuffText(out Color color)
        {
            color = textColor;
            string result = string.Empty;

            if (buffs.HasActiveSpeedBoost)
            {
                result += $"+{Mathf.RoundToInt((buffs.ActiveSpeedMultiplier - 1f) * 100f)}% speed {buffs.SpeedBoostRemaining:0.0}s";
            }

            if (buffs.HasActiveExperienceBoost)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += "\n";
                }

                result += $"+{Mathf.RoundToInt((buffs.ActiveExperienceMultiplier - 1f) * 100f)}% XP {buffs.ExperienceBoostRemaining:0.0}s";
            }

            if (buffs.HasActiveInvulnerability)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += "\n";
                }

                color = invulnerableColor;
                result += $"Invulnerable {buffs.InvulnerabilityRemaining:0.0}s";
            }

            AddMovementEffect(ref result, "Flight", movementCaster != null && movementCaster.HasActiveFlight, movementCaster != null ? movementCaster.FlightRemaining : 0f);
            AddMovementEffect(ref result, "Invisible", movementCaster != null && movementCaster.HasActiveInvisibility, movementCaster != null ? movementCaster.InvisibilityRemaining : 0f);
            AddMovementEffect(ref result, "Stoneskin", movementCaster != null && movementCaster.HasActiveStoneskin, movementCaster != null ? movementCaster.StoneskinRemaining : 0f);
            AddMovementEffect(ref result, "Tunnel", movementCaster != null && movementCaster.HasActiveTunnel, movementCaster != null ? movementCaster.TunnelRemaining : 0f);
            AddMovementEffect(ref result, "Whirlwind", movementCaster != null && movementCaster.HasActiveWhirlwind, movementCaster != null ? movementCaster.WhirlwindRemaining : 0f);

            return result;
        }

        private static void AddMovementEffect(ref string result, string label, bool active, float remaining)
        {
            if (!active)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result))
            {
                result += "\n";
            }

            result += $"{label} {remaining:0.0}s";
        }
    }
}
