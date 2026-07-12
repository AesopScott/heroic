using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections;
using UnityEngine;

namespace Heroic.Spells
{
    public class StormCallCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 5.2f;
        [SerializeField] private float range = 10f;
        [SerializeField] private float radius = 1.35f;
        [SerializeField] private int damage = 30;
        [SerializeField] private int strikeCount = 4;
        [SerializeField] private float strikeDelay = 0.42f;
        [SerializeField] private float scatterRadius = 3f;
        [SerializeField] private float stunChance = 0.16f;
        [SerializeField] private float stunDuration = 0.45f;
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

            Vector2 center = target.transform.position;
            CastAt(center);
            spellEcho?.Echo(() => CastAt(center));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetStrikeCount(int value) => strikeCount = Mathf.Max(1, value);
        public void SetStrikeDelay(float value) => strikeDelay = Mathf.Max(0.05f, value);
        public void SetDamage(int value) => damage = Mathf.Max(0, value);
        public void SetStunChance(float value) => stunChance = Mathf.Clamp01(value);

        private void CastAt(Vector2 center)
        {
            StartCoroutine(StormRoutine(center));
        }

        private IEnumerator StormRoutine(Vector2 center)
        {
            for (int i = 0; i < strikeCount; i++)
            {
                Vector2 position = center + Random.insideUnitCircle * scatterRadius;
                EnemyController target = LightningTargeting.FindNearestEnemy(position, ModifiedRange(scatterRadius));
                if (target != null)
                {
                    position = target.transform.position;
                }

                StrikeAt(position);
                yield return new WaitForSeconds(strikeDelay);
            }
        }

        private void StrikeAt(Vector2 position)
        {
            float activeRadius = ModifiedRange(radius);
            TemporaryVisualEffect.CreateCircle(position, new Color(1f, 0.9f, 0.18f, 0.48f), activeRadius, 0.16f);
            Collider2D[] hits = enemyLayers.value == 0 ? Physics2D.OverlapCircleAll(position, activeRadius) : Physics2D.OverlapCircleAll(position, activeRadius, enemyLayers);
            foreach (Collider2D hit in hits)
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (damageable == null)
                {
                    continue;
                }

                damageable.ApplyDamage(ModifiedDamage(damage));
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
