using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Player;
using Heroic.Systems;
using Heroic.Visuals;
using System.Collections;
using UnityEngine;

namespace Heroic.Spells
{
    public class BloodAbilityCaster : MonoBehaviour
    {
        public enum BloodSkill
        {
            BloodBolt,
            SanguinePact,
            BloodNova,
            LeechBind,
            CrimsonFrenzy
        }

        [SerializeField] private BloodSkill skill;
        [SerializeField] private float castInterval = 2.4f;
        [SerializeField] private float range = 8f;
        [SerializeField] private float radius = 1.6f;
        [SerializeField] private int damage = 22;
        [SerializeField] private int count = 1;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float tickInterval = 0.5f;
        [SerializeField] private float lifestealMultiplier = 0.25f;
        [SerializeField] private int sacrificeCost = 12;
        [SerializeField] private float powerMultiplier = 1.2f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private SpellEchoCaster spellEcho;

        private float nextCastTime;
        private float frenzyEndsAt;
        private PlayerHealth playerHealth;
        private SpellStatModifier spellStats;

        private void Awake()
        {
            if (spellEcho == null)
            {
                spellEcho = GetComponent<SpellEchoCaster>();
            }

            playerHealth = GetComponent<PlayerHealth>();
            spellStats = GetComponent<SpellStatModifier>();
        }

        private void Update()
        {
            if (Time.time < nextCastTime)
            {
                return;
            }

            EnemyController target = ArcaneTargeting.FindNearestEnemy(transform.position, ModifiedRange(range));
            if (target == null && skill != BloodSkill.CrimsonFrenzy && skill != BloodSkill.SanguinePact)
            {
                return;
            }

            Vector2 targetPosition = target != null ? target.transform.position : transform.position;
            CastAt(targetPosition);
            spellEcho?.Echo(() => CastAt(targetPosition));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetDamage(int value) => damage = Mathf.Max(0, value);
        public void SetRadius(float value) => radius = Mathf.Max(0.25f, value);
        public void SetCount(int value) => count = Mathf.Max(1, value);
        public void SetDuration(float value) => duration = Mathf.Max(0.25f, value);
        public void SetLifestealMultiplier(float value) => lifestealMultiplier = Mathf.Max(0f, value);
        public void SetSacrificeCost(int value) => sacrificeCost = Mathf.Max(0, value);
        public void SetPowerMultiplier(float value) => powerMultiplier = Mathf.Max(1f, value);
        public bool IsFrenzied => Time.time < frenzyEndsAt;

        private void CastAt(Vector2 targetPosition)
        {
            switch (skill)
            {
                case BloodSkill.BloodBolt:
                    BloodBolt(targetPosition);
                    break;
                case BloodSkill.SanguinePact:
                    SanguinePact();
                    break;
                case BloodSkill.BloodNova:
                    BloodNova();
                    break;
                case BloodSkill.LeechBind:
                    StartCoroutine(LeechBindRoutine(targetPosition));
                    break;
                case BloodSkill.CrimsonFrenzy:
                    CrimsonFrenzy();
                    break;
            }
        }

        private void BloodBolt(Vector2 targetPosition)
        {
            EnemyController target = ArcaneTargeting.FindNearestEnemy(targetPosition, ModifiedRange(radius + 1f));
            if (target == null)
            {
                return;
            }

            TemporaryVisualEffect.CreateCircle(target.transform.position, new Color(0.85f, 0.05f, 0.15f, 0.36f), 0.65f, 0.16f);
            int dealt = ApplyDamage(target.GetComponent<Damageable>(), damage);
            HealFromDamage(dealt);

            foreach (Collider2D hit in Overlap(target.transform.position, ModifiedRange(radius)))
            {
                if (hit.GetComponent<EnemyController>() == target)
                {
                    continue;
                }

                int splash = ApplyDamage(hit.GetComponent<Damageable>(), Mathf.RoundToInt(damage * 0.45f));
                HealFromDamage(splash);
            }
        }

        private void SanguinePact()
        {
            if (playerHealth == null || playerHealth.CurrentHealth <= sacrificeCost + 1)
            {
                return;
            }

            playerHealth.TakeDamage(sacrificeCost);
            frenzyEndsAt = Mathf.Max(frenzyEndsAt, Time.time + duration);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.95f, 0.05f, 0.18f, 0.28f), 1.4f, 0.35f);
            playerHealth.Heal(Mathf.RoundToInt(sacrificeCost * lifestealMultiplier));
        }

        private void BloodNova()
        {
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.9f, 0.02f, 0.15f, 0.3f), ModifiedRange(radius), 0.18f);
            foreach (Collider2D hit in Overlap(transform.position, ModifiedRange(radius)))
            {
                int dealt = ApplyDamage(hit.GetComponent<Damageable>(), damage);
                HealFromDamage(dealt);
            }
        }

        private IEnumerator LeechBindRoutine(Vector2 targetPosition)
        {
            for (int i = 0; i < count; i++)
            {
                EnemyController target = ArcaneTargeting.FindNearestEnemy(targetPosition, ModifiedRange(range));
                if (target != null)
                {
                    StartCoroutine(DrainTarget(target));
                    targetPosition = (Vector2)target.transform.position + Random.insideUnitCircle * radius;
                }
            }

            yield break;
        }

        private IEnumerator DrainTarget(EnemyController target)
        {
            float elapsed = 0f;
            while (target != null && elapsed < duration)
            {
                TemporaryVisualEffect.CreateCircle(target.transform.position, new Color(0.88f, 0.03f, 0.16f, 0.28f), 0.75f, 0.18f);
                int dealt = ApplyDamage(target.GetComponent<Damageable>(), Mathf.RoundToInt(damage * 0.45f));
                HealFromDamage(dealt);
                elapsed += tickInterval;
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void CrimsonFrenzy()
        {
            frenzyEndsAt = Mathf.Max(frenzyEndsAt, Time.time + duration);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(1f, 0.05f, 0.2f, 0.22f), 1.2f, duration);
        }

        private int ApplyDamage(Damageable damageable, int baseDamage)
        {
            if (damageable == null)
            {
                return 0;
            }

            int finalDamage = Mathf.RoundToInt(baseDamage * (IsFrenzied ? powerMultiplier : 1f));
            int before = damageable.CurrentHealth;
            damageable.ApplyDamage(ModifiedDamage(finalDamage));
            return Mathf.Max(0, before - damageable.CurrentHealth);
        }

        private void HealFromDamage(int dealt)
        {
            if (playerHealth != null && dealt > 0)
            {
                playerHealth.Heal(Mathf.RoundToInt(dealt * lifestealMultiplier));
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
