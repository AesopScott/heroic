using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Visuals;
using UnityEngine;

namespace Heroic.Spells
{
    [RequireComponent(typeof(Damageable))]
    public class PoisonedEnemy : MonoBehaviour
    {
        private Damageable damageable;
        private EnemyController enemy;
        private int damagePerTick;
        private float tickInterval;
        private float expiresAt;
        private float nextTickAt;
        private float spreadRadius;
        private float spreadInterval;
        private float nextSpreadAt;
        private float burstRadius;
        private int burstDamage;

        private void Awake()
        {
            damageable = GetComponent<Damageable>();
            enemy = GetComponent<EnemyController>();
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

        private void Update()
        {
            if (Time.time >= expiresAt)
            {
                Destroy(this);
                return;
            }

            if (Time.time >= nextTickAt)
            {
                damageable.ApplyDamage(damagePerTick);
                TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.45f, 1f, 0.18f, 0.24f), 0.45f, 0.12f);
                nextTickAt = Time.time + tickInterval;
            }

            if (spreadRadius > 0f && Time.time >= nextSpreadAt)
            {
                Spread();
                nextSpreadAt = Time.time + spreadInterval;
            }
        }

        public void Configure(int newDamagePerTick, float duration, float newTickInterval, float newSpreadRadius, float newSpreadInterval, int newBurstDamage, float newBurstRadius)
        {
            damagePerTick = Mathf.Max(0, newDamagePerTick);
            tickInterval = Mathf.Max(0.1f, newTickInterval);
            expiresAt = Mathf.Max(expiresAt, Time.time + Mathf.Max(0.1f, duration));
            spreadRadius = Mathf.Max(spreadRadius, newSpreadRadius);
            spreadInterval = Mathf.Max(0.2f, newSpreadInterval);
            burstDamage = Mathf.Max(burstDamage, newBurstDamage);
            burstRadius = Mathf.Max(burstRadius, newBurstRadius);
            nextTickAt = nextTickAt <= 0f ? Time.time : Mathf.Min(nextTickAt, Time.time + tickInterval);
            nextSpreadAt = nextSpreadAt <= 0f ? Time.time + spreadInterval : Mathf.Min(nextSpreadAt, Time.time + spreadInterval);
        }

        private void Spread()
        {
            EnemyController[] enemies = Object.FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
            foreach (EnemyController nearby in enemies)
            {
                if (nearby == null || nearby == enemy || Vector2.Distance(transform.position, nearby.transform.position) > spreadRadius)
                {
                    continue;
                }

                PoisonedEnemy poison = nearby.GetComponent<PoisonedEnemy>();
                if (poison == null)
                {
                    poison = nearby.gameObject.AddComponent<PoisonedEnemy>();
                }

                poison.Configure(damagePerTick, 2.5f, tickInterval, 0f, spreadInterval, burstDamage, burstRadius);
                return;
            }
        }

        private void HandleDied(Damageable dead)
        {
            if (burstDamage <= 0 || burstRadius <= 0f)
            {
                return;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, burstRadius);
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject)
                {
                    continue;
                }

                Damageable otherDamageable = hit.GetComponent<Damageable>();
                if (otherDamageable != null)
                {
                    otherDamageable.ApplyDamage(burstDamage);
                }
            }
        }
    }
}
