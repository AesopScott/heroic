using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Player;
using Heroic.Systems;
using Heroic.Visuals;
using UnityEngine;
using System.Collections;

namespace Heroic.Spells
{
    public class FlameShieldCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 7f;
        [SerializeField] private float radius = 1.8f;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float tickInterval = 0.35f;
        [SerializeField] private int damagePerTick = 10;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private SpellEchoCaster spellEcho;
        [SerializeField] private PlayerTemporaryBuffs temporaryBuffs;

        private float nextCastTime;
        private SpellStatModifier spellStats;

        private void Awake()
        {
            if (spellEcho == null)
            {
                spellEcho = GetComponent<SpellEchoCaster>();
            }

            temporaryBuffs ??= GetComponent<PlayerTemporaryBuffs>();
            spellStats = GetComponent<SpellStatModifier>();
        }

        private void Update()
        {
            if (Time.time < nextCastTime)
            {
                return;
            }

            EnemyController target = ArcaneTargeting.FindNearestEnemy(transform.position, ModifiedRange(radius + 4f));
            if (target == null)
            {
                return;
            }

            Cast();
            spellEcho?.Echo(Cast);
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetDamagePerTick(int value)
        {
            damagePerTick = Mathf.Max(0, value);
        }

        public void SetRadius(float value)
        {
            radius = Mathf.Max(0.25f, value);
        }

        public void SetDuration(float value)
        {
            duration = Mathf.Max(0.5f, value);
        }

        public void SetCastInterval(float value)
        {
            castInterval = Mathf.Max(0.25f, value);
        }

        private void Cast()
        {
            temporaryBuffs?.ApplyInvulnerability(duration);
            StartCoroutine(FlameShieldRoutine());
        }

        private IEnumerator FlameShieldRoutine()
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float activeRadius = ModifiedRange(radius);
                TemporaryVisualEffect.CreateCircle(transform.position, new Color(1f, 0.42f, 0.06f, 0.26f), activeRadius, 0.22f);
                DamageNearby(activeRadius);
                elapsed += tickInterval;
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void DamageNearby(float activeRadius)
        {
            Collider2D[] hits = enemyLayers.value == 0
                ? Physics2D.OverlapCircleAll(transform.position, activeRadius)
                : Physics2D.OverlapCircleAll(transform.position, activeRadius, enemyLayers);

            foreach (Collider2D hit in hits)
            {
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
