using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using UnityEngine;

namespace Heroic.Spells
{
    public class ShatterLineCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 3.6f;
        [SerializeField] private float range = 6.5f;
        [SerializeField] private float width = 1.1f;
        [SerializeField] private int damage = 22;
        [SerializeField] private float controlledDamageMultiplier = 1.35f;
        [SerializeField] private float freezeDuration = 0.35f;
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

            EnemyController target = ArcaneTargeting.FindNearestEnemy(transform.position, ModifiedRange(range));
            if (target == null)
            {
                return;
            }

            Vector2 direction = (target.transform.position - transform.position).normalized;
            Cast(direction);
            spellEcho?.Echo(() => Cast(direction));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetWidth(float value) => width = Mathf.Max(0.3f, value);
        public void SetRange(float value) => range = Mathf.Max(1f, value);
        public void SetControlledDamageMultiplier(float value) => controlledDamageMultiplier = Mathf.Max(1f, value);

        private void Cast(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = Vector2.right;
            }

            Vector2 origin = transform.position;
            float activeRange = ModifiedRange(range);
            Vector2 center = origin + direction.normalized * (activeRange * 0.5f);
            TemporaryVisualEffect.CreateCircle(center, new Color(0.62f, 0.94f, 1f, 0.3f), Mathf.Max(width, activeRange * 0.18f), 0.18f);

            Collider2D[] hits = enemyLayers.value == 0 ? Physics2D.OverlapCircleAll(origin, activeRange) : Physics2D.OverlapCircleAll(origin, activeRange, enemyLayers);
            foreach (Collider2D hit in hits)
            {
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
                if (enemy != null && enemy.IsColdControlled)
                {
                    finalDamage = Mathf.RoundToInt(finalDamage * controlledDamageMultiplier);
                }

                damageable.ApplyDamage(finalDamage);
                enemy?.ApplyFreeze(freezeDuration);
            }
        }

        private int ModifiedDamage(int value) => spellStats != null ? spellStats.ModifyDamage(value) : value;
        private float ModifiedRange(float value) => spellStats != null ? spellStats.ModifyRange(value) : value;
        private float ModifiedCooldown(float value) => spellStats != null ? spellStats.ModifyCooldown(value) : value;
    }
}
