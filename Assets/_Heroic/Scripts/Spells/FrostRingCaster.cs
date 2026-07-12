using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroic.Spells
{
    public class FrostRingCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 3.2f;
        [SerializeField] private float radius = 3.4f;
        [SerializeField] private float expandDuration = 0.42f;
        [SerializeField] private float tickInterval = 0.06f;
        [SerializeField] private int damage = 18;
        [SerializeField] private float slowMultiplier = 0.62f;
        [SerializeField] private float slowDuration = 2f;
        [SerializeField] private float freezeChance = 0.08f;
        [SerializeField] private float freezeDuration = 0.55f;
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
            if (Time.time < nextCastTime || ArcaneTargeting.FindNearestEnemy(transform.position, ModifiedRange(radius + 1f)) == null)
            {
                return;
            }

            Cast();
            spellEcho?.Echo(Cast);
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetRadius(float value) => radius = Mathf.Max(0.5f, value);
        public void SetSlowMultiplier(float value) => slowMultiplier = Mathf.Clamp(value, 0.1f, 1f);
        public void SetFreezeChance(float value) => freezeChance = Mathf.Clamp01(value);

        private void Cast()
        {
            StartCoroutine(RingRoutine(transform.position));
        }

        private IEnumerator RingRoutine(Vector2 origin)
        {
            float elapsed = 0f;
            HashSet<Damageable> damaged = new HashSet<Damageable>();
            while (elapsed < expandDuration)
            {
                elapsed += tickInterval;
                float activeRadius = ModifiedRange(radius) * Mathf.Clamp01(elapsed / expandDuration);
                TemporaryVisualEffect.CreateCircle(origin, new Color(0.55f, 0.92f, 1f, 0.28f), activeRadius, 0.16f);
                DamageAt(origin, activeRadius, damaged);
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void DamageAt(Vector2 origin, float activeRadius, HashSet<Damageable> damaged)
        {
            Collider2D[] hits = enemyLayers.value == 0 ? Physics2D.OverlapCircleAll(origin, activeRadius) : Physics2D.OverlapCircleAll(origin, activeRadius, enemyLayers);
            foreach (Collider2D hit in hits)
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (damageable == null || damaged.Contains(damageable))
                {
                    continue;
                }

                damaged.Add(damageable);
                damageable.ApplyDamage(ModifiedDamage(damage));
                ApplyCold(enemy);
            }
        }

        private void ApplyCold(EnemyController enemy)
        {
            if (enemy == null)
            {
                return;
            }

            enemy.ApplySlow(slowMultiplier, slowDuration);
            if (Random.value <= freezeChance)
            {
                enemy.ApplyFreeze(freezeDuration);
            }
        }

        private int ModifiedDamage(int value) => spellStats != null ? spellStats.ModifyDamage(value) : value;
        private float ModifiedRange(float value) => spellStats != null ? spellStats.ModifyRange(value) : value;
        private float ModifiedCooldown(float value) => spellStats != null ? spellStats.ModifyCooldown(value) : value;
    }
}
