using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections;
using UnityEngine;

namespace Heroic.Spells
{
    public class MindAbilityCaster : MonoBehaviour
    {
        public enum MindSkill
        {
            PsychicLance,
            FearWave,
            IllusionClone,
            Confuse,
            MindCrush
        }

        [SerializeField] private MindSkill skill;
        [SerializeField] private float castInterval = 1.6f;
        [SerializeField] private float range = 6f;
        [SerializeField] private float radius = 1.4f;
        [SerializeField] private float width = 1.2f;
        [SerializeField] private int damage = 18;
        [SerializeField] private int count = 1;
        [SerializeField] private float duration = 2f;
        [SerializeField] private float executionMultiplier = 1f;
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

        public void SetDamage(int value) => damage = Mathf.Max(0, value);
        public void SetRange(float value) => range = Mathf.Max(1f, value);
        public void SetRadius(float value) => radius = Mathf.Max(0.25f, value);
        public void SetWidth(float value) => width = Mathf.Max(0.25f, value);
        public void SetDuration(float value) => duration = Mathf.Max(0.1f, value);
        public void SetCount(int value) => count = Mathf.Max(1, value);
        public void SetExecutionMultiplier(float value) => executionMultiplier = Mathf.Max(1f, value);

        private void CastAt(Vector2 targetPosition)
        {
            switch (skill)
            {
                case MindSkill.PsychicLance:
                    LineStrike(targetPosition, false, false);
                    break;
                case MindSkill.FearWave:
                    Cone(targetPosition, true, false);
                    break;
                case MindSkill.IllusionClone:
                    StartCoroutine(CloneRoutine(targetPosition));
                    break;
                case MindSkill.Confuse:
                    Area(targetPosition, false, true, damage);
                    break;
                case MindSkill.MindCrush:
                    MindCrush(targetPosition);
                    break;
            }
        }

        private void LineStrike(Vector2 targetPosition, bool fear, bool confuse)
        {
            Vector2 origin = transform.position;
            Vector2 direction = (targetPosition - origin).normalized;
            TemporaryVisualEffect.CreateCircle(origin + direction * (ModifiedRange(range) * 0.5f), new Color(0.74f, 0.28f, 1f, 0.25f), width, 0.14f);
            foreach (Collider2D hit in Overlap(origin, ModifiedRange(range)))
            {
                Vector2 offset = (Vector2)hit.transform.position - origin;
                float forward = Vector2.Dot(offset, direction);
                float side = Mathf.Abs(Vector2.Dot(offset, new Vector2(-direction.y, direction.x).normalized));
                if (forward <= 0f || forward > ModifiedRange(range) || side > width)
                {
                    continue;
                }

                ApplyMindHit(hit, damage, fear, confuse);
            }
        }

        private void Cone(Vector2 targetPosition, bool fear, bool confuse)
        {
            Vector2 origin = transform.position;
            Vector2 direction = (targetPosition - origin).normalized;
            TemporaryVisualEffect.CreateCircle(origin + direction * (ModifiedRange(range) * 0.45f), new Color(0.72f, 0.24f, 1f, 0.22f), ModifiedRange(width), 0.18f);
            foreach (Collider2D hit in Overlap(origin, ModifiedRange(range)))
            {
                Vector2 offset = (Vector2)hit.transform.position - origin;
                float forward = Vector2.Dot(offset, direction);
                float side = Mathf.Abs(Vector2.Dot(offset, new Vector2(-direction.y, direction.x).normalized));
                float allowed = Mathf.Lerp(width * 0.25f, width, Mathf.Clamp01(forward / ModifiedRange(range)));
                if (forward <= 0f || forward > ModifiedRange(range) || side > allowed)
                {
                    continue;
                }

                ApplyMindHit(hit, damage, fear, confuse);
            }
        }

        private IEnumerator CloneRoutine(Vector2 center)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 position = center + Random.insideUnitCircle * radius;
                TemporaryVisualEffect.CreateCircle(position, new Color(0.78f, 0.48f, 1f, 0.26f), 1f, duration);
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    foreach (Collider2D hit in Overlap(position, ModifiedRange(radius + 1f)))
                    {
                        EnemyController enemy = hit.GetComponent<EnemyController>();
                        enemy?.Pull(position, 0.18f);
                    }

                    elapsed += 0.35f;
                    yield return new WaitForSeconds(0.35f);
                }

                Area(position, false, true, Mathf.RoundToInt(damage * 0.75f));
            }
        }

        private void Area(Vector2 position, bool fear, bool confuse, int baseDamage)
        {
            TemporaryVisualEffect.CreateCircle(position, new Color(0.72f, 0.32f, 1f, 0.28f), ModifiedRange(radius), 0.18f);
            foreach (Collider2D hit in Overlap(position, ModifiedRange(radius)))
            {
                ApplyMindHit(hit, baseDamage, fear, confuse);
            }
        }

        private void MindCrush(Vector2 position)
        {
            TemporaryVisualEffect.CreateCircle(position, new Color(0.85f, 0.22f, 1f, 0.35f), ModifiedRange(radius), 0.18f);
            foreach (Collider2D hit in Overlap(position, ModifiedRange(radius)))
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                if (damageable == null)
                {
                    continue;
                }

                int finalDamage = damage;
                if (damageable.CurrentHealth <= damageable.MaxHealth * 0.35f)
                {
                    finalDamage = Mathf.RoundToInt(finalDamage * executionMultiplier);
                }

                ApplyMindHit(hit, finalDamage, false, true);
            }
        }

        private void ApplyMindHit(Collider2D hit, int baseDamage, bool fear, bool confuse)
        {
            Damageable damageable = hit.GetComponent<Damageable>();
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (damageable == null)
            {
                return;
            }

            damageable.ApplyDamage(ModifiedDamage(baseDamage));
            if (fear)
            {
                enemy?.ApplyFear(transform.position, duration);
            }

            if (confuse)
            {
                enemy?.ApplyConfuse(duration);
            }
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
