using Heroic.Combat;
using Heroic.Player;
using System;
using UnityEngine;

namespace Heroic.Enemies
{
    [RequireComponent(typeof(Damageable))]
    public class ExperienceDropper : MonoBehaviour
    {
        [Serializable]
        public class LootTier
        {
            [SerializeField] private int tier = 1;
            [SerializeField] private int minimumPlayerLevel = 1;
            [SerializeField] private float dropChance = 0.05f;
            [SerializeField] private int value = 5;
            [SerializeField] private float duration = 0f;
            [SerializeField] private float multiplier = 1f;

            public int Tier => Mathf.Clamp(tier, 1, 5);
            public int MinimumPlayerLevel => Mathf.Max(1, minimumPlayerLevel);
            public float DropChance => Mathf.Clamp01(dropChance);
            public int Value => Mathf.Max(1, value);
            public float Duration => Mathf.Max(0f, duration);
            public float Multiplier => Mathf.Max(1f, multiplier);

            public LootTier(int tier, int minimumPlayerLevel, float dropChance, int value, float duration = 0f, float multiplier = 1f)
            {
                this.tier = tier;
                this.minimumPlayerLevel = minimumPlayerLevel;
                this.dropChance = dropChance;
                this.value = value;
                this.duration = duration;
                this.multiplier = multiplier;
            }
        }

        [SerializeField] private ExperiencePickup pickupPrefab;
        [SerializeField] private LootPickup healthRestorePrefab;
        [SerializeField] private LootPickup experienceBoostPrefab;
        [SerializeField] private LootPickup speedBoostPrefab;
        [SerializeField] private LootPickup invulnerabilityPrefab;
        [SerializeField] private int experienceValue = 1;
        [SerializeField] private LootTier[] healthRestoreTiers =
        {
            new LootTier(1, 1, 0.08f, 8),
            new LootTier(2, 3, 0.065f, 14),
            new LootTier(3, 5, 0.05f, 22),
            new LootTier(4, 8, 0.035f, 35),
            new LootTier(5, 12, 0.02f, 55)
        };
        [SerializeField] private LootTier[] experienceBoostTiers =
        {
            new LootTier(1, 1, 0.03f, 0, 10f, 1.5f),
            new LootTier(2, 3, 0.026f, 0, 10f, 2f),
            new LootTier(3, 5, 0.02f, 0, 10f, 2.5f),
            new LootTier(4, 8, 0.014f, 0, 10f, 3f),
            new LootTier(5, 12, 0.008f, 0, 10f, 3.5f)
        };
        [SerializeField] private LootTier[] speedBoostTiers =
        {
            new LootTier(1, 1, 0.035f, 0, 3f, 1.2f),
            new LootTier(2, 3, 0.03f, 0, 3.5f, 1.3f),
            new LootTier(3, 5, 0.022f, 0, 4f, 1.45f),
            new LootTier(4, 8, 0.014f, 0, 4.5f, 1.6f),
            new LootTier(5, 12, 0.008f, 0, 5f, 1.8f)
        };
        [SerializeField] private LootTier[] invulnerabilityTiers =
        {
            new LootTier(1, 2, 0.012f, 0, 1.25f),
            new LootTier(2, 4, 0.01f, 0, 1.75f),
            new LootTier(3, 6, 0.008f, 0, 2.25f),
            new LootTier(4, 9, 0.006f, 0, 3f),
            new LootTier(5, 13, 0.004f, 0, 4f)
        };

        private Damageable damageable;
        private bool suppressNextDrop;

        private void Awake()
        {
            damageable = GetComponent<Damageable>();
        }

        private void OnEnable()
        {
            if (damageable != null)
            {
                damageable.Died += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (damageable != null)
            {
                damageable.Died -= HandleDied;
            }
        }

        private void HandleDied(Damageable dead)
        {
            if (suppressNextDrop)
            {
                suppressNextDrop = false;
                return;
            }

            int playerLevel = ResolvePlayerLevel();
            if (pickupPrefab == null || experienceValue <= 0)
            {
                DropExtraLoot(playerLevel);
                return;
            }

            ExperiencePickup pickup = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            pickup.SetExperienceValue(experienceValue);
            DropExtraLoot(playerLevel);
        }

        public void SetExperienceValue(int value)
        {
            experienceValue = Mathf.Max(0, value);
        }

        public void SuppressNextDrop()
        {
            suppressNextDrop = true;
        }

        private void DropExtraLoot(int playerLevel)
        {
            TryDropLoot(healthRestorePrefab, LootPickup.LootKind.HealthRestore, healthRestoreTiers, playerLevel);
            TryDropLoot(experienceBoostPrefab, LootPickup.LootKind.ExperienceBoost, experienceBoostTiers, playerLevel);
            TryDropLoot(speedBoostPrefab, LootPickup.LootKind.SpeedBoost, speedBoostTiers, playerLevel);
            TryDropLoot(invulnerabilityPrefab, LootPickup.LootKind.Invulnerability, invulnerabilityTiers, playerLevel);
        }

        private void TryDropLoot(LootPickup prefab, LootPickup.LootKind kind, LootTier[] tiers, int playerLevel)
        {
            if (prefab == null || tiers == null || tiers.Length == 0)
            {
                return;
            }

            LootTier tier = RollTier(tiers, playerLevel);
            if (tier == null)
            {
                return;
            }

            Vector2 scatter = UnityEngine.Random.insideUnitCircle * 0.55f;
            LootPickup loot = Instantiate(prefab, (Vector2)transform.position + scatter, Quaternion.identity);
            loot.Configure(kind, tier.Value, tier.Tier, tier.Duration, tier.Multiplier);
        }

        private static LootTier RollTier(LootTier[] tiers, int playerLevel)
        {
            LootTier selected = null;
            for (int i = 0; i < tiers.Length; i++)
            {
                LootTier tier = tiers[i];
                if (tier == null || playerLevel < tier.MinimumPlayerLevel)
                {
                    continue;
                }

                if (UnityEngine.Random.value <= tier.DropChance)
                {
                    selected = tier;
                }
            }

            return selected;
        }

        private static int ResolvePlayerLevel()
        {
            PlayerExperience playerExperience = FindAnyObjectByType<PlayerExperience>();
            return playerExperience != null ? playerExperience.Level : 1;
        }
    }
}
