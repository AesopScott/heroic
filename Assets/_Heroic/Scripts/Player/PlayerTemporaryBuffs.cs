using UnityEngine;

namespace Heroic.Player
{
    public class PlayerTemporaryBuffs : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerHealth playerHealth;

        private float activeSpeedMultiplier = 1f;
        private float activeExperienceMultiplier = 1f;
        private float speedBoostEndsAt;
        private float experienceBoostEndsAt;
        private float invulnerabilityEndsAt;

        public bool HasActiveSpeedBoost => Time.time < speedBoostEndsAt;
        public bool HasActiveExperienceBoost => Time.time < experienceBoostEndsAt;
        public bool HasActiveInvulnerability => Time.time < invulnerabilityEndsAt;
        public float ActiveSpeedMultiplier => HasActiveSpeedBoost ? activeSpeedMultiplier : 1f;
        public float ActiveExperienceMultiplier => HasActiveExperienceBoost ? activeExperienceMultiplier : 1f;
        public float SpeedBoostRemaining => Mathf.Max(0f, speedBoostEndsAt - Time.time);
        public float ExperienceBoostRemaining => Mathf.Max(0f, experienceBoostEndsAt - Time.time);
        public float InvulnerabilityRemaining => Mathf.Max(0f, invulnerabilityEndsAt - Time.time);

        private void Awake()
        {
            playerController ??= GetComponent<PlayerController>();
            playerHealth ??= GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            RefreshBuffState();
        }

        private void OnDisable()
        {
            playerController?.SetLootSpeedMultiplier(1f);
            playerHealth?.SetInvulnerable(false);
        }

        public void ApplySpeedBoost(float multiplier, float duration)
        {
            float clampedMultiplier = Mathf.Max(1f, multiplier);
            float endTime = Time.time + Mathf.Max(0.1f, duration);

            if (!HasActiveSpeedBoost || clampedMultiplier >= activeSpeedMultiplier)
            {
                activeSpeedMultiplier = clampedMultiplier;
            }

            speedBoostEndsAt = Mathf.Max(speedBoostEndsAt, endTime);
            RefreshBuffState();
        }

        public void ApplyInvulnerability(float duration)
        {
            invulnerabilityEndsAt = Mathf.Max(invulnerabilityEndsAt, Time.time + Mathf.Max(0.1f, duration));
            RefreshBuffState();
        }

        public void ApplyExperienceBoost(float multiplier, float duration)
        {
            float clampedMultiplier = Mathf.Max(1f, multiplier);
            float endTime = Time.time + Mathf.Max(0.1f, duration);

            if (!HasActiveExperienceBoost || clampedMultiplier >= activeExperienceMultiplier)
            {
                activeExperienceMultiplier = clampedMultiplier;
            }

            experienceBoostEndsAt = Mathf.Max(experienceBoostEndsAt, endTime);
            RefreshBuffState();
        }

        private void RefreshBuffState()
        {
            playerController?.SetLootSpeedMultiplier(HasActiveSpeedBoost ? activeSpeedMultiplier : 1f);
            playerHealth?.SetInvulnerable(HasActiveInvulnerability);

            if (!HasActiveSpeedBoost)
            {
                activeSpeedMultiplier = 1f;
            }

            if (!HasActiveExperienceBoost)
            {
                activeExperienceMultiplier = 1f;
            }
        }
    }
}
