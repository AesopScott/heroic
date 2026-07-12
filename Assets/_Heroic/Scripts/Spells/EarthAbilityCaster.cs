using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections;
using UnityEngine;

namespace Heroic.Spells
{
    public class EarthAbilityCaster : MonoBehaviour
    {
        public enum EarthSkill
        {
            StoneSpike,
            BoulderToss,
            EarthWall,
            Quake,
            MudTrap
        }

        [SerializeField] private EarthSkill skill;
        [SerializeField] private float castInterval = 4f;
        [SerializeField] private float range = 13f;
        [SerializeField] private float radius = 1.6f;
        [SerializeField] private int damage = 24;
        [SerializeField] private int count = 3;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float tickInterval = 0.65f;
        [SerializeField] private float slowMultiplier = 0.55f;
        [SerializeField] private float stunDuration = 0.35f;
        [SerializeField] private float bonusDamageMultiplier = 1f;
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

            Vector2 targetPosition = target.transform.position;
            CastAt(targetPosition);
            spellEcho?.Echo(() => CastAt(targetPosition));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetCount(int value) => count = Mathf.Max(1, value);
        public void SetDamage(int value) => damage = Mathf.Max(0, value);
        public void SetRadius(float value) => radius = Mathf.Max(0.25f, value);
        public void SetDuration(float value) => duration = Mathf.Max(0.5f, value);
        public void SetSlowMultiplier(float value) => slowMultiplier = Mathf.Clamp(value, 0.1f, 1f);
        public void SetBonusDamageMultiplier(float value) => bonusDamageMultiplier = Mathf.Max(1f, value);
        public void SetStunDuration(float value) => stunDuration = Mathf.Max(0f, value);

        private void CastAt(Vector2 targetPosition)
        {
            switch (skill)
            {
                case EarthSkill.StoneSpike:
                    StartCoroutine(StoneSpikeRoutine(targetPosition));
                    break;
                case EarthSkill.BoulderToss:
                    BoulderToss(targetPosition);
                    break;
                case EarthSkill.EarthWall:
                    EarthWall(targetPosition);
                    break;
                case EarthSkill.Quake:
                    StartCoroutine(QuakeRoutine(targetPosition));
                    break;
                case EarthSkill.MudTrap:
                    StartCoroutine(MudTrapRoutine(targetPosition));
                    break;
            }
        }

        private IEnumerator StoneSpikeRoutine(Vector2 center)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 position = center + Random.insideUnitCircle * radius;
                TemporaryVisualEffect.CreateCircle(position, new Color(0.62f, 0.46f, 0.26f, 0.38f), 0.75f, 0.18f);
                DamageAt(position, 0.85f, damage, true, true, 0f);
                yield return new WaitForSeconds(0.12f);
            }
        }

        private void BoulderToss(Vector2 targetPosition)
        {
            Vector2 origin = transform.position;
            Vector2 direction = (targetPosition - origin).normalized;
            float activeRange = ModifiedRange(range);
            TemporaryVisualEffect.CreateCircle(origin + direction * (activeRange * 0.5f), new Color(0.55f, 0.38f, 0.18f, 0.3f), Mathf.Max(radius, activeRange * 0.12f), 0.2f);
            Collider2D[] hits = Overlap(origin, activeRange);
            int remaining = count;
            foreach (Collider2D hit in hits)
            {
                if (remaining <= 0)
                {
                    return;
                }

                Vector2 offset = (Vector2)hit.transform.position - origin;
                float forward = Vector2.Dot(offset, direction);
                float side = Mathf.Abs(Vector2.Dot(offset, new Vector2(-direction.y, direction.x).normalized));
                if (forward <= 0f || forward > activeRange || side > radius)
                {
                    continue;
                }

                Damageable damageable = hit.GetComponent<Damageable>();
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (damageable == null)
                {
                    continue;
                }

                damageable.ApplyDamage(ModifiedDamage(damage));
                enemy?.Push(direction, 1.4f * bonusDamageMultiplier);
                enemy?.ApplyStun(stunDuration);
                remaining--;
            }
        }

        private void EarthWall(Vector2 center)
        {
            Vector2 side = Random.insideUnitCircle.normalized;
            if (side.sqrMagnitude <= 0.001f)
            {
                side = Vector2.right;
            }

            for (int i = 0; i < count; i++)
            {
                float offset = (i - (count - 1) * 0.5f) * radius;
                Vector2 position = center + side * offset;
                TemporaryVisualEffect.CreateCircle(position, new Color(0.45f, 0.34f, 0.2f, 0.42f), radius, duration);
                DamageAt(position, radius, Mathf.RoundToInt(damage * 0.55f), true, false, 0.55f);
            }
        }

        private IEnumerator QuakeRoutine(Vector2 center)
        {
            for (int i = 0; i < count; i++)
            {
                float activeRadius = ModifiedRange(radius + i * 0.45f);
                TemporaryVisualEffect.CreateCircle(center, new Color(0.62f, 0.48f, 0.25f, 0.3f), activeRadius, 0.18f);
                DamageAt(center, activeRadius, damage, true, true, 0f);
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private IEnumerator MudTrapRoutine(Vector2 center)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                TemporaryVisualEffect.CreateCircle(center, new Color(0.34f, 0.24f, 0.12f, 0.26f), ModifiedRange(radius), 0.24f);
                DamageAt(center, ModifiedRange(radius), Mathf.RoundToInt(damage * bonusDamageMultiplier), false, false, slowMultiplier);
                elapsed += tickInterval;
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void DamageAt(Vector2 position, float activeRadius, int baseDamage, bool stun, bool knockback, float slow)
        {
            Collider2D[] hits = Overlap(position, activeRadius);
            foreach (Collider2D hit in hits)
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (damageable == null)
                {
                    continue;
                }

                damageable.ApplyDamage(ModifiedDamage(baseDamage));
                if (stun)
                {
                    enemy?.ApplyStun(stunDuration);
                }

                if (knockback && enemy != null)
                {
                    enemy.Push((hit.transform.position - (Vector3)position).normalized, 0.55f);
                }

                if (slow > 0f)
                {
                    enemy?.ApplySlow(slow, 1.2f);
                }
            }
        }

        private Collider2D[] Overlap(Vector2 position, float activeRadius)
        {
            return enemyLayers.value == 0 ? Physics2D.OverlapCircleAll(position, activeRadius) : Physics2D.OverlapCircleAll(position, activeRadius, enemyLayers);
        }

        private int ModifiedDamage(int value) => spellStats != null ? spellStats.ModifyDamage(value) : value;
        private float ModifiedRange(float value) => spellStats != null ? spellStats.ModifyRange(value) : value;
        private float ModifiedCooldown(float value) => spellStats != null ? spellStats.ModifyCooldown(value) : value;
    }
}
