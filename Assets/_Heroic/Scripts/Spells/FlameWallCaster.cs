using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using UnityEngine;
using System.Collections;

namespace Heroic.Spells
{
    public class FlameWallCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 5.5f;
        [SerializeField] private float range = 7f;
        [SerializeField] private float length = 4.2f;
        [SerializeField] private float width = 0.75f;
        [SerializeField] private float duration = 3.5f;
        [SerializeField] private float tickInterval = 0.4f;
        [SerializeField] private int damagePerTick = 14;
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

        public void SetDamagePerTick(int value)
        {
            damagePerTick = Mathf.Max(0, value);
        }

        public void SetLength(float value)
        {
            length = Mathf.Max(0.75f, value);
        }

        public void SetDuration(float value)
        {
            duration = Mathf.Max(0.5f, value);
        }

        public void SetCastInterval(float value)
        {
            castInterval = Mathf.Max(0.25f, value);
        }

        private void Cast(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = Vector2.right;
            }

            Vector2 center = (Vector2)transform.position + direction.normalized * (ModifiedRange(range) * 0.55f);
            Vector2 wallAxis = new Vector2(-direction.y, direction.x).normalized;
            StartCoroutine(FlameWallRoutine(center, wallAxis));
        }

        private IEnumerator FlameWallRoutine(Vector2 center, Vector2 wallAxis)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float activeLength = ModifiedRange(length);
                DrawWall(center, wallAxis, activeLength);
                DamageWall(center, wallAxis, activeLength);
                elapsed += tickInterval;
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void DrawWall(Vector2 center, Vector2 wallAxis, float activeLength)
        {
            int segments = Mathf.Max(3, Mathf.CeilToInt(activeLength / 0.65f));
            for (int i = 0; i < segments; i++)
            {
                float t = segments == 1 ? 0.5f : i / (float)(segments - 1);
                Vector2 position = center + wallAxis * Mathf.Lerp(-activeLength * 0.5f, activeLength * 0.5f, t);
                TemporaryVisualEffect.CreateCircle(position, new Color(1f, 0.22f, 0.02f, 0.28f), width, 0.24f);
            }
        }

        private void DamageWall(Vector2 center, Vector2 wallAxis, float activeLength)
        {
            Collider2D[] hits = enemyLayers.value == 0
                ? Physics2D.OverlapCircleAll(center, activeLength * 0.6f + width)
                : Physics2D.OverlapCircleAll(center, activeLength * 0.6f + width, enemyLayers);

            Vector2 normal = new Vector2(-wallAxis.y, wallAxis.x).normalized;
            foreach (Collider2D hit in hits)
            {
                Vector2 offset = (Vector2)hit.transform.position - center;
                float along = Mathf.Abs(Vector2.Dot(offset, wallAxis));
                float across = Mathf.Abs(Vector2.Dot(offset, normal));
                if (along > activeLength * 0.5f || across > width)
                {
                    continue;
                }

                Damageable damageable = hit.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(ModifiedDamage(damagePerTick));
                }
            }
        }

        private int ModifiedDamage(int value)
        {
            return spellStats != null ? spellStats.ModifyDamage(value) : value;
        }

        private float ModifiedRange(float value)
        {
            return spellStats != null ? spellStats.ModifyRange(value) : value;
        }

        private float ModifiedCooldown(float value)
        {
            return spellStats != null ? spellStats.ModifyCooldown(value) : value;
        }
    }
}
