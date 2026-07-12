using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections;
using UnityEngine;

namespace Heroic.Spells
{
    public class CrystalPrisonCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 5.5f;
        [SerializeField] private float range = 7f;
        [SerializeField] private float radius = 1.15f;
        [SerializeField] private float triggerDelay = 0.35f;
        [SerializeField] private int prisonCount = 1;
        [SerializeField] private float scatterRadius = 1.8f;
        [SerializeField] private int damage = 14;
        [SerializeField] private float freezeDuration = 1.1f;
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

        public void SetPrisonCount(int value) => prisonCount = Mathf.Max(1, value);
        public void SetTriggerDelay(float value) => triggerDelay = Mathf.Max(0.05f, value);
        public void SetFreezeDuration(float value) => freezeDuration = Mathf.Max(0f, value);

        private void CastAt(Vector2 center)
        {
            for (int i = 0; i < prisonCount; i++)
            {
                Vector2 position = center + Random.insideUnitCircle * scatterRadius;
                StartCoroutine(PrisonRoutine(position));
            }
        }

        private IEnumerator PrisonRoutine(Vector2 position)
        {
            TemporaryVisualEffect.CreateCircle(position, new Color(0.62f, 0.95f, 1f, 0.16f), ModifiedRange(radius), triggerDelay);
            yield return new WaitForSeconds(triggerDelay);
            TemporaryVisualEffect.CreateCircle(position, new Color(0.72f, 0.96f, 1f, 0.4f), ModifiedRange(radius), 0.22f);
            Collider2D[] hits = enemyLayers.value == 0 ? Physics2D.OverlapCircleAll(position, ModifiedRange(radius)) : Physics2D.OverlapCircleAll(position, ModifiedRange(radius), enemyLayers);
            foreach (Collider2D hit in hits)
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(ModifiedDamage(damage));
                    enemy?.ApplyFreeze(freezeDuration);
                }
            }
        }

        private int ModifiedDamage(int value) => spellStats != null ? spellStats.ModifyDamage(value) : value;
        private float ModifiedRange(float value) => spellStats != null ? spellStats.ModifyRange(value) : value;
        private float ModifiedCooldown(float value) => spellStats != null ? spellStats.ModifyCooldown(value) : value;
    }
}
