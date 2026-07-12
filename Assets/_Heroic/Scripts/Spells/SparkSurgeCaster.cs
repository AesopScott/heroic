using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroic.Spells
{
    public class SparkSurgeCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 2.7f;
        [SerializeField] private float range = 10f;
        [SerializeField] private int damage = 18;
        [SerializeField] private int sparkCount = 4;
        [SerializeField] private float sparkDelay = 0.12f;
        [SerializeField] private float targetSpreadRadius = 3.5f;
        [SerializeField] private float stunChance = 0.06f;
        [SerializeField] private float stunDuration = 0.25f;
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

            Cast();
            spellEcho?.Echo(Cast);
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetSparkCount(int value) => sparkCount = Mathf.Max(1, value);
        public void SetSparkDelay(float value) => sparkDelay = Mathf.Max(0.03f, value);
        public void SetTargetSpreadRadius(float value) => targetSpreadRadius = Mathf.Max(0.5f, value);

        private void Cast()
        {
            StartCoroutine(SurgeRoutine());
        }

        private IEnumerator SurgeRoutine()
        {
            List<EnemyController> recentTargets = new List<EnemyController>();
            for (int i = 0; i < sparkCount; i++)
            {
                EnemyController target = PickTarget(recentTargets);
                if (target != null)
                {
                    recentTargets.Add(target);
                    Strike(target);
                }

                yield return new WaitForSeconds(sparkDelay);
            }
        }

        private EnemyController PickTarget(List<EnemyController> recentTargets)
        {
            EnemyController target = LightningTargeting.FindNearestEnemy(transform.position, ModifiedRange(range), recentTargets);
            if (target != null)
            {
                return target;
            }

            return LightningTargeting.FindNearestEnemy(transform.position, ModifiedRange(range));
        }

        private void Strike(EnemyController enemy)
        {
            Vector2 position = enemy.transform.position;
            Vector2 offset = Random.insideUnitCircle * targetSpreadRadius;
            EnemyController spreadTarget = LightningTargeting.FindNearestEnemy(position + offset, ModifiedRange(targetSpreadRadius));
            if (spreadTarget != null)
            {
                enemy = spreadTarget;
                position = enemy.transform.position;
            }

            TemporaryVisualEffect.CreateCircle(position, new Color(1f, 0.95f, 0.28f, 0.38f), 0.45f, 0.12f);
            enemy.GetComponent<Damageable>()?.ApplyDamage(ModifiedDamage(damage));
            if (Random.value <= stunChance)
            {
                enemy.ApplyStun(stunDuration);
            }
        }

        private int ModifiedDamage(int value) => spellStats != null ? spellStats.ModifyDamage(value) : value;
        private float ModifiedRange(float value) => spellStats != null ? spellStats.ModifyRange(value) : value;
        private float ModifiedCooldown(float value) => spellStats != null ? spellStats.ModifyCooldown(value) : value;
    }
}
