using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections.Generic;
using UnityEngine;

namespace Heroic.Spells
{
    public class ChainBoltCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 1.15f;
        [SerializeField] private float range = 11f;
        [SerializeField] private int damage = 24;
        [SerializeField] private int jumpCount = 3;
        [SerializeField] private float chainRange = 4.5f;
        [SerializeField] private float stunChance = 0.08f;
        [SerializeField] private float stunDuration = 0.35f;
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

            Cast(target);
            spellEcho?.Echo(() => CastIfAlive(target));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetJumpCount(int value) => jumpCount = Mathf.Max(1, value);
        public void SetDamage(int value) => damage = Mathf.Max(0, value);
        public void SetChainRange(float value) => chainRange = Mathf.Max(0.5f, value);

        private void Cast(EnemyController firstTarget)
        {
            List<EnemyController> hitEnemies = new List<EnemyController>();
            EnemyController current = firstTarget;
            Vector2 previousPosition = transform.position;

            for (int i = 0; i < jumpCount && current != null; i++)
            {
                Strike(current, previousPosition);
                hitEnemies.Add(current);
                previousPosition = current.transform.position;
                current = LightningTargeting.FindNearestEnemy(previousPosition, ModifiedRange(chainRange), hitEnemies);
            }
        }

        private void Strike(EnemyController enemy, Vector2 previousPosition)
        {
            if (enemy == null)
            {
                return;
            }

            Vector2 position = enemy.transform.position;
            TemporaryVisualEffect.CreateCircle(position, new Color(1f, 0.92f, 0.24f, 0.42f), 0.55f, 0.14f);
            TemporaryVisualEffect.CreateCircle((previousPosition + position) * 0.5f, new Color(0.72f, 0.94f, 1f, 0.24f), 0.25f, 0.08f);
            enemy.GetComponent<Damageable>()?.ApplyDamage(ModifiedDamage(damage));
            if (Random.value <= stunChance)
            {
                enemy.ApplyStun(stunDuration);
            }
        }

        private void CastIfAlive(EnemyController target)
        {
            if (target != null)
            {
                Cast(target);
            }
        }

        private int ModifiedDamage(int value) => spellStats != null ? spellStats.ModifyDamage(value) : value;
        private float ModifiedRange(float value) => spellStats != null ? spellStats.ModifyRange(value) : value;
        private float ModifiedCooldown(float value) => spellStats != null ? spellStats.ModifyCooldown(value) : value;
    }
}
