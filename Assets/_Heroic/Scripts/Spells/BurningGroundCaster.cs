using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using UnityEngine;
using System.Collections;

namespace Heroic.Spells
{
    public class BurningGroundCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 4.5f;
        [SerializeField] private float range = 9f;
        [SerializeField] private float radius = 1.45f;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float tickInterval = 0.5f;
        [SerializeField] private int damagePerTick = 8;
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

            Vector2 position = target.transform.position;
            CastAt(position);
            spellEcho?.Echo(() => CastAt(position));
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

        private void CastAt(Vector2 position)
        {
            StartCoroutine(BurningGroundRoutine(position));
        }

        private IEnumerator BurningGroundRoutine(Vector2 position)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float activeRadius = ModifiedRange(radius);
                TemporaryVisualEffect.CreateCircle(position, new Color(1f, 0.2f, 0.02f, 0.22f), activeRadius, 0.28f);
                DamageAt(position, activeRadius);
                elapsed += tickInterval;
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void DamageAt(Vector2 position, float activeRadius)
        {
            Collider2D[] hits = enemyLayers.value == 0
                ? Physics2D.OverlapCircleAll(position, activeRadius)
                : Physics2D.OverlapCircleAll(position, activeRadius, enemyLayers);

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
