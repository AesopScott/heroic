using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections;
using UnityEngine;

namespace Heroic.Spells
{
    public class StaticFieldCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 3.8f;
        [SerializeField] private float range = 9f;
        [SerializeField] private float radius = 1.8f;
        [SerializeField] private float duration = 2.8f;
        [SerializeField] private float tickInterval = 0.45f;
        [SerializeField] private int damagePerTick = 10;
        [SerializeField] private float stunChance = 0.08f;
        [SerializeField] private float stunDuration = 0.35f;
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

            Vector2 position = target.transform.position;
            CastAt(position);
            spellEcho?.Echo(() => CastAt(position));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetRadius(float value) => radius = Mathf.Max(0.25f, value);
        public void SetTickInterval(float value) => tickInterval = Mathf.Max(0.08f, value);
        public void SetStunChance(float value) => stunChance = Mathf.Clamp01(value);

        private void CastAt(Vector2 position)
        {
            StartCoroutine(FieldRoutine(position));
        }

        private IEnumerator FieldRoutine(Vector2 position)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float activeRadius = ModifiedRange(radius);
                TemporaryVisualEffect.CreateCircle(position, new Color(1f, 0.92f, 0.2f, 0.22f), activeRadius, 0.22f);
                DamageAt(position, activeRadius);
                elapsed += tickInterval;
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void DamageAt(Vector2 position, float activeRadius)
        {
            Collider2D[] hits = enemyLayers.value == 0 ? Physics2D.OverlapCircleAll(position, activeRadius) : Physics2D.OverlapCircleAll(position, activeRadius, enemyLayers);
            foreach (Collider2D hit in hits)
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (damageable == null)
                {
                    continue;
                }

                damageable.ApplyDamage(ModifiedDamage(damagePerTick));
                if (enemy != null && Random.value <= stunChance)
                {
                    enemy.ApplyStun(stunDuration);
                }
            }
        }

        private int ModifiedDamage(int value) => spellStats != null ? spellStats.ModifyDamage(value) : value;
        private float ModifiedRange(float value) => spellStats != null ? spellStats.ModifyRange(value) : value;
        private float ModifiedCooldown(float value) => spellStats != null ? spellStats.ModifyCooldown(value) : value;
    }
}
