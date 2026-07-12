using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using UnityEngine;

namespace Heroic.Spells
{
    public class ArcaneBlastCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 3f;
        [SerializeField] private float range = 8f;
        [SerializeField] private float radius = 1.25f;
        [SerializeField] private int damage = 20;
        [SerializeField] private int scatterCount;
        [SerializeField] private float scatterRadius = 2f;
        [SerializeField] private float scatterDamageMultiplier = 0.5f;
        [SerializeField] private LayerMask damageableLayers;
        [SerializeField] private ArcaneDoubleCast doubleCast;
        [SerializeField] private SpellEchoCaster spellEcho;

        private float nextCastTime;
        private SpellStatModifier spellStats;

        private void Awake()
        {
            if (doubleCast == null)
            {
                doubleCast = GetComponent<ArcaneDoubleCast>();
            }

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
            doubleCast?.TrySchedule(() => CastAt(targetPosition));
            spellEcho?.Echo(() => CastAt(targetPosition));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetDamage(int value)
        {
            damage = Mathf.Max(0, value);
        }

        public void SetRange(float value)
        {
            range = Mathf.Max(0f, value);
        }

        public void SetScatterCount(int value)
        {
            scatterCount = Mathf.Max(0, value);
        }

        public void SetRadius(float value)
        {
            radius = Mathf.Max(0.1f, value);
        }

        private void CastAt(Vector2 position)
        {
            ApplyBlast(position, ModifiedDamage(damage), radius);

            for (int i = 0; i < scatterCount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * scatterRadius;
                ApplyBlast(position + offset, Mathf.RoundToInt(ModifiedDamage(damage) * scatterDamageMultiplier), radius * 0.65f);
            }
        }

        private void ApplyBlast(Vector2 position, int blastDamage, float blastRadius)
        {
            TemporaryVisualEffect.CreateCircle(position, new Color(0.35f, 0.8f, 1f, 0.5f), blastRadius, 0.2f);

            Collider2D[] hits = damageableLayers.value == 0
                ? Physics2D.OverlapCircleAll(position, blastRadius)
                : Physics2D.OverlapCircleAll(position, blastRadius, damageableLayers);

            foreach (Collider2D hit in hits)
            {
                var damageable = hit.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(blastDamage);
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
