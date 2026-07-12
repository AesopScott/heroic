using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using UnityEngine;

namespace Heroic.Spells
{
    public class ThunderLanceCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 2.2f;
        [SerializeField] private float range = 11f;
        [SerializeField] private float width = 0.75f;
        [SerializeField] private int damage = 34;
        [SerializeField] private int pierceCount = 3;
        [SerializeField] private float isolatedDamageMultiplier = 1f;
        [SerializeField] private float stunDuration = 0.25f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private SpellEchoCaster spellEcho;

        private float nextCastTime;
        private SpellStatModifier spellStats;

        private void Awake()
        {
            if (spellEcho == null)
            {
                spellEcho = GetComponent<SpellEchoCaster>();
            }

            spellStats = GetComponent<SpellStatModifier>();
        }

        private void Update()
        {
            if (Time.time < nextCastTime)
            {
                return;
            }

            EnemyController target = LightningTargeting.FindNearestEnemy(transform.position, ModifiedRange(range));
            if (target == null)
            {
                return;
            }

            Vector2 direction = (target.transform.position - transform.position).normalized;
            Cast(direction);
            spellEcho?.Echo(() => Cast(direction));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetPierceCount(int value) => pierceCount = Mathf.Max(1, value);
        public void SetWidth(float value) => width = Mathf.Max(0.2f, value);
        public void SetIsolatedDamageMultiplier(float value) => isolatedDamageMultiplier = Mathf.Max(1f, value);

        private void Cast(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = Vector2.right;
            }

            Vector2 origin = transform.position;
            float activeRange = ModifiedRange(range);
            TemporaryVisualEffect.CreateCircle(origin + direction * (activeRange * 0.5f), new Color(1f, 0.9f, 0.18f, 0.28f), Mathf.Max(width, activeRange * 0.12f), 0.16f);
            Collider2D[] hits = enemyLayers.value == 0 ? Physics2D.OverlapCircleAll(origin, activeRange) : Physics2D.OverlapCircleAll(origin, activeRange, enemyLayers);
            int remainingPierce = pierceCount;

            foreach (Collider2D hit in hits)
            {
                if (remainingPierce <= 0)
                {
                    return;
                }

                Vector2 offset = (Vector2)hit.transform.position - origin;
                float forwardDistance = Vector2.Dot(offset, direction);
                if (forwardDistance <= 0f || forwardDistance > activeRange)
                {
                    continue;
                }

                float sideDistance = Mathf.Abs(Vector2.Dot(offset, new Vector2(-direction.y, direction.x).normalized));
                if (sideDistance > width)
                {
                    continue;
                }

                Damageable damageable = hit.GetComponent<Damageable>();
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (damageable == null)
                {
                    continue;
                }

                int finalDamage = ModifiedDamage(damage);
                if (CountNearbyEnemies(hit.transform.position, 2.2f) <= 1)
                {
                    finalDamage = Mathf.RoundToInt(finalDamage * isolatedDamageMultiplier);
                }

                damageable.ApplyDamage(finalDamage);
                enemy?.ApplyStun(stunDuration);
                remainingPierce--;
            }
        }

        private int CountNearbyEnemies(Vector2 position, float radius)
        {
            int count = 0;
            EnemyController[] enemies = Object.FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
            foreach (EnemyController enemy in enemies)
            {
                if (enemy != null && Vector2.Distance(position, enemy.transform.position) <= radius)
                {
                    count++;
                }
            }

            return count;
        }

        private int ModifiedDamage(int value) => spellStats != null ? spellStats.ModifyDamage(value) : value;
        private float ModifiedRange(float value) => spellStats != null ? spellStats.ModifyRange(value) : value;
        private float ModifiedCooldown(float value) => spellStats != null ? spellStats.ModifyCooldown(value) : value;
    }
}
