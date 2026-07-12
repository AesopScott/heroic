using Heroic.Enemies;
using UnityEngine;

namespace Heroic.Combat
{
    public class ColdProjectileHit : MonoBehaviour
    {
        [SerializeField] private int damage = 12;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private int pierceCount;
        [SerializeField] private float slowMultiplier = 0.6f;
        [SerializeField] private float slowDuration = 1.8f;
        [SerializeField] private float freezeChance = 0.08f;
        [SerializeField] private float freezeDuration = 0.55f;
        [SerializeField] private float controlledDamageMultiplier = 1f;

        public void Configure(int newDamage, int newPierceCount, float newSlowMultiplier, float newSlowDuration, float newFreezeChance, float newFreezeDuration, float newControlledDamageMultiplier)
        {
            damage = Mathf.Max(0, newDamage);
            pierceCount = Mathf.Max(0, newPierceCount);
            slowMultiplier = Mathf.Clamp(newSlowMultiplier, 0.1f, 1f);
            slowDuration = Mathf.Max(0f, newSlowDuration);
            freezeChance = Mathf.Clamp01(newFreezeChance);
            freezeDuration = Mathf.Max(0f, newFreezeDuration);
            controlledDamageMultiplier = Mathf.Max(1f, newControlledDamageMultiplier);
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Damageable damageable = other.GetComponent<Damageable>();
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (damageable == null)
            {
                return;
            }

            int finalDamage = damage;
            if (enemy != null && enemy.IsColdControlled)
            {
                finalDamage = Mathf.RoundToInt(finalDamage * controlledDamageMultiplier);
            }

            damageable.ApplyDamage(finalDamage);
            if (enemy != null)
            {
                enemy.ApplySlow(slowMultiplier, slowDuration);
                if (Random.value <= freezeChance)
                {
                    enemy.ApplyFreeze(freezeDuration);
                }
            }

            if (pierceCount <= 0)
            {
                Destroy(gameObject);
                return;
            }

            pierceCount--;
        }
    }
}
