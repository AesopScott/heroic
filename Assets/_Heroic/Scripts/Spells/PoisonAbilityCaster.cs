using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections;
using UnityEngine;

namespace Heroic.Spells
{
    public class PoisonAbilityCaster : MonoBehaviour
    {
        public enum PoisonSkill
        {
            PoisonDart,
            ToxicCloud,
            VenomTrail,
            Infection,
            RotBloom
        }

        [SerializeField] private PoisonSkill skill;
        [SerializeField] private float castInterval = 3.4f;
        [SerializeField] private float range = 8f;
        [SerializeField] private float radius = 1.7f;
        [SerializeField] private int damage = 10;
        [SerializeField] private int count = 1;
        [SerializeField] private float duration = 4f;
        [SerializeField] private float tickInterval = 0.55f;
        [SerializeField] private float spreadRadius = 0f;
        [SerializeField] private float slowMultiplier = 1f;
        [SerializeField] private int burstDamage = 0;
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
        public void SetDuration(float value) => duration = Mathf.Max(0.25f, value);
        public void SetTickInterval(float value) => tickInterval = Mathf.Max(0.1f, value);
        public void SetSpreadRadius(float value) => spreadRadius = Mathf.Max(0f, value);
        public void SetSlowMultiplier(float value) => slowMultiplier = Mathf.Clamp(value, 0.1f, 1f);
        public void SetBurstDamage(int value) => burstDamage = Mathf.Max(0, value);

        private void CastAt(Vector2 targetPosition)
        {
            switch (skill)
            {
                case PoisonSkill.PoisonDart:
                    PoisonDart(targetPosition);
                    break;
                case PoisonSkill.ToxicCloud:
                    StartCoroutine(PoisonZoneRoutine(targetPosition, false));
                    break;
                case PoisonSkill.VenomTrail:
                    StartCoroutine(PoisonZoneRoutine(transform.position, true));
                    break;
                case PoisonSkill.Infection:
                    Infection(targetPosition);
                    break;
                case PoisonSkill.RotBloom:
                    RotBloom(targetPosition);
                    break;
            }
        }

        private void PoisonDart(Vector2 targetPosition)
        {
            for (int i = 0; i < count; i++)
            {
                EnemyController target = ArcaneTargeting.FindNearestEnemy(targetPosition + Random.insideUnitCircle * radius, ModifiedRange(range));
                if (target == null)
                {
                    continue;
                }

                TemporaryVisualEffect.CreateCircle(target.transform.position, new Color(0.36f, 1f, 0.12f, 0.28f), 0.55f, 0.14f);
                ApplyPoison(target.gameObject);
            }
        }

        private IEnumerator PoisonZoneRoutine(Vector2 center, bool followsCaster)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                Vector2 position = followsCaster ? transform.position : center;
                TemporaryVisualEffect.CreateCircle(position, new Color(0.28f, 0.9f, 0.12f, 0.22f), ModifiedRange(radius), 0.24f);
                foreach (Collider2D hit in Overlap(position, ModifiedRange(radius)))
                {
                    ApplyPoison(hit.gameObject);
                    if (slowMultiplier < 0.99f)
                    {
                        hit.GetComponent<EnemyController>()?.ApplySlow(slowMultiplier, 1.1f);
                    }
                }

                elapsed += tickInterval;
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void Infection(Vector2 targetPosition)
        {
            foreach (Collider2D hit in Overlap(targetPosition, ModifiedRange(radius)))
            {
                ApplyPoison(hit.gameObject);
            }
        }

        private void RotBloom(Vector2 targetPosition)
        {
            TemporaryVisualEffect.CreateCircle(targetPosition, new Color(0.38f, 1f, 0.1f, 0.32f), ModifiedRange(radius), 0.2f);
            foreach (Collider2D hit in Overlap(targetPosition, ModifiedRange(radius)))
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                damageable?.ApplyDamage(ModifiedDamage(damage * 2));
                ApplyPoison(hit.gameObject);
            }

            StartCoroutine(PoisonZoneRoutine(targetPosition, false));
        }

        private void ApplyPoison(GameObject target)
        {
            Damageable damageable = target.GetComponent<Damageable>();
            if (damageable == null)
            {
                return;
            }

            PoisonedEnemy poison = target.GetComponent<PoisonedEnemy>();
            if (poison == null)
            {
                poison = target.AddComponent<PoisonedEnemy>();
            }

            poison.Configure(ModifiedDamage(damage), duration, tickInterval, ModifiedRange(spreadRadius), 1f, ModifiedDamage(burstDamage), ModifiedRange(radius));
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
