using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Heroic.Spells
{
    public class FlameWaveCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 2.6f;
        [SerializeField] private float range = 5f;
        [SerializeField] private float width = 2.2f;
        [SerializeField] private float travelDuration = 0.45f;
        [SerializeField] private float tickInterval = 0.06f;
        [SerializeField] private int damage = 28;
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

            EnemyController target = ArcaneTargeting.FindNearestEnemy(transform.position, ModifiedRange(range + 2f));
            if (target == null)
            {
                return;
            }

            Vector2 direction = (target.transform.position - transform.position).normalized;
            Cast(direction);
            spellEcho?.Echo(() => Cast(direction));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetDamage(int value)
        {
            damage = Mathf.Max(0, value);
        }

        public void SetRange(float value)
        {
            range = Mathf.Max(1f, value);
        }

        public void SetWidth(float value)
        {
            width = Mathf.Max(0.5f, value);
        }

        public void SetCastInterval(float value)
        {
            castInterval = Mathf.Max(0.2f, value);
        }

        private void Cast(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = Vector2.right;
            }

            StartCoroutine(FlameWaveRoutine(direction.normalized));
        }

        private IEnumerator FlameWaveRoutine(Vector2 direction)
        {
            Vector2 origin = transform.position;
            float activeRange = ModifiedRange(range);
            float elapsed = 0f;
            HashSet<Damageable> damaged = new HashSet<Damageable>();

            while (elapsed < travelDuration)
            {
                elapsed += tickInterval;
                float percent = Mathf.Clamp01(elapsed / travelDuration);
                float waveDistance = activeRange * percent;
                float waveWidth = Mathf.Lerp(width * 0.35f, width, percent);
                Vector2 waveCenter = origin + direction * waveDistance;
                TemporaryVisualEffect.CreateCircle(waveCenter, new Color(1f, 0.34f, 0.08f, 0.32f), waveWidth, 0.16f);
                DamageWaveBand(origin, direction, waveDistance, waveWidth, activeRange, damaged);
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void DamageWaveBand(Vector2 origin, Vector2 direction, float waveDistance, float waveWidth, float activeRange, HashSet<Damageable> damaged)
        {
            Collider2D[] hits = enemyLayers.value == 0
                ? Physics2D.OverlapCircleAll(origin, activeRange)
                : Physics2D.OverlapCircleAll(origin, activeRange, enemyLayers);

            foreach (Collider2D hit in hits)
            {
                Vector2 offset = (Vector2)hit.transform.position - origin;
                float forwardDistance = Vector2.Dot(offset, direction);
                if (forwardDistance <= 0f || forwardDistance > activeRange)
                {
                    continue;
                }

                if (Mathf.Abs(forwardDistance - waveDistance) > Mathf.Max(0.35f, waveWidth * 0.45f))
                {
                    continue;
                }

                float allowedSideDistance = Mathf.Lerp(width * 0.35f, width, forwardDistance / activeRange);
                float sideDistance = Mathf.Abs(Vector2.Dot(offset, new Vector2(-direction.y, direction.x).normalized));
                if (sideDistance > allowedSideDistance)
                {
                    continue;
                }

                Damageable damageable = hit.GetComponent<Damageable>();
                if (damageable != null && !damaged.Contains(damageable))
                {
                    damaged.Add(damageable);
                    damageable.ApplyDamage(ModifiedDamage(damage));
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
