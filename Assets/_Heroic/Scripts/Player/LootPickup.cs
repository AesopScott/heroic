using System;
using UnityEngine;

namespace Heroic.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class LootPickup : MonoBehaviour
    {
        public enum LootKind
        {
            HealthRestore,
            ExperienceBoost,
            SpeedBoost,
            Invulnerability
        }

        [SerializeField] private LootKind kind = LootKind.HealthRestore;
        [SerializeField] private int value = 5;
        [SerializeField] private int tier = 1;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float multiplier = 1.25f;

        public event Action<LootPickup> Collected;

        public LootKind Kind => kind;
        public int Value => value;
        public int Tier => tier;
        public float Duration => duration;
        public float Multiplier => multiplier;

        public void Configure(LootKind lootKind, int lootValue, int lootTier, float lootDuration = 0f, float lootMultiplier = 1f)
        {
            kind = lootKind;
            value = Mathf.Max(1, lootValue);
            tier = Mathf.Clamp(lootTier, 1, 5);
            duration = Mathf.Max(0f, lootDuration);
            multiplier = Mathf.Max(1f, lootMultiplier);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (kind == LootKind.HealthRestore)
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health == null)
                {
                    return;
                }

                health.Heal(value);
            }
            else if (kind == LootKind.ExperienceBoost)
            {
                PlayerTemporaryBuffs buffs = other.GetComponent<PlayerTemporaryBuffs>();
                if (buffs == null)
                {
                    return;
                }

                buffs.ApplyExperienceBoost(multiplier, duration);
            }
            else
            {
                PlayerTemporaryBuffs buffs = other.GetComponent<PlayerTemporaryBuffs>();
                if (buffs == null)
                {
                    return;
                }

                if (kind == LootKind.SpeedBoost)
                {
                    buffs.ApplySpeedBoost(multiplier, duration);
                }
                else
                {
                    buffs.ApplyInvulnerability(duration);
                }
            }

            Collected?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
