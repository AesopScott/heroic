using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Player;
using Heroic.Systems;
using Heroic.Visuals;
using UnityEngine;

namespace Heroic.Spells
{
    public class ArcaneUtilityCaster : MonoBehaviour
    {
        public enum ArcaneUtilityMode
        {
            PhaseLance,
            ForceField,
            TimeWarp,
            Haste
        }

        [SerializeField] private ArcaneUtilityMode mode = ArcaneUtilityMode.PhaseLance;
        [SerializeField] private float castInterval = 4f;
        [SerializeField] private float range = 8f;
        [SerializeField] private float radius = 1.4f;
        [SerializeField] private int damage = 12;
        [SerializeField] private float duration = 2.5f;
        [SerializeField] private float slowMultiplier = 0.45f;
        [SerializeField] private float speedMultiplier = 1.45f;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private ArcaneDoubleCast doubleCast;
        [SerializeField] private SpellEchoCaster spellEcho;

        private float nextCastTime;
        private float hasteEndsAt;
        private PlayerController playerController;
        private SpellStatModifier spellStats;

        private void Awake()
        {
            doubleCast ??= GetComponent<ArcaneDoubleCast>();
            spellEcho ??= GetComponent<SpellEchoCaster>();
            playerController = GetComponent<PlayerController>();
            spellStats = GetComponent<SpellStatModifier>();
        }

        private void OnDisable()
        {
            if (mode == ArcaneUtilityMode.Haste && playerController != null)
            {
                playerController.SetTemporarySpeedMultiplier(1f);
            }
        }

        private void Update()
        {
            if (mode == ArcaneUtilityMode.Haste && hasteEndsAt > 0f && Time.time >= hasteEndsAt)
            {
                hasteEndsAt = 0f;
                playerController?.SetTemporarySpeedMultiplier(1f);
            }

            if (Time.time < nextCastTime)
            {
                return;
            }

            Cast();
            doubleCast?.TrySchedule(Cast);
            spellEcho?.Echo(Cast);
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetMode(ArcaneUtilityMode newMode)
        {
            mode = newMode;
        }

        public void SetDamage(int value)
        {
            damage = Mathf.Max(0, value);
        }

        public void SetRange(float value)
        {
            range = Mathf.Max(0.1f, value);
        }

        public void SetRadius(float value)
        {
            radius = Mathf.Max(0.1f, value);
        }

        public void SetDuration(float value)
        {
            duration = Mathf.Max(0.1f, value);
        }

        public void SetCastInterval(float value)
        {
            castInterval = Mathf.Max(0.1f, value);
        }

        public void SetSlowMultiplier(float value)
        {
            slowMultiplier = Mathf.Clamp(value, 0.1f, 1f);
        }

        public void SetSpeedMultiplier(float value)
        {
            speedMultiplier = Mathf.Max(1f, value);
        }

        private void Cast()
        {
            switch (mode)
            {
                case ArcaneUtilityMode.PhaseLance:
                    CastPhaseLance();
                    break;
                case ArcaneUtilityMode.ForceField:
                    CastForceField();
                    break;
                case ArcaneUtilityMode.TimeWarp:
                    CastTimeWarp();
                    break;
                case ArcaneUtilityMode.Haste:
                    CastHaste();
                    break;
            }
        }

        private void CastPhaseLance()
        {
            EnemyController target = ArcaneTargeting.FindNearestEnemy(transform.position, ModifiedRange(range));
            if (target == null)
            {
                return;
            }

            Vector2 origin = transform.position;
            Vector2 direction = ((Vector2)target.transform.position - origin).normalized;
            float activeRange = ModifiedRange(range);
            int activeDamage = ModifiedDamage(damage);
            float step = Mathf.Max(0.35f, radius * 0.6f);

            for (float distance = 0f; distance <= activeRange; distance += step)
            {
                Vector2 point = origin + direction * distance;
                TemporaryVisualEffect.CreateCircle(point, new Color(0.56f, 0.34f, 1f, 0.22f), radius * 0.42f, 0.12f);
                DamageEnemies(point, radius * 0.45f, activeDamage);
            }
        }

        private void CastForceField()
        {
            float activeRadius = ModifiedRange(radius);
            int activeDamage = ModifiedDamage(damage);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.6f, 0.42f, 1f, 0.36f), activeRadius, 0.32f);

            Collider2D[] hits = Overlap(transform.position, activeRadius);
            foreach (Collider2D hit in hits)
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(activeDamage);
                }

                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.Push((Vector2)hit.transform.position - (Vector2)transform.position, activeRadius * 0.55f);
                }
            }
        }

        private void CastTimeWarp()
        {
            EnemyController target = ArcaneTargeting.FindNearestEnemy(transform.position, ModifiedRange(range));
            Vector2 center = target != null ? target.transform.position : transform.position;
            float activeRadius = ModifiedRange(radius);
            int activeDamage = ModifiedDamage(damage);
            TemporaryVisualEffect.CreateCircle(center, new Color(0.45f, 0.28f, 1f, 0.35f), activeRadius, 0.45f);

            Collider2D[] hits = Overlap(center, activeRadius);
            foreach (Collider2D hit in hits)
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(activeDamage);
                }

                EnemyController enemy = hit.GetComponent<EnemyController>();
                enemy?.ApplySlow(slowMultiplier, duration);
            }
        }

        private void CastHaste()
        {
            playerController ??= GetComponent<PlayerController>();
            if (playerController == null)
            {
                return;
            }

            playerController.SetTemporarySpeedMultiplier(speedMultiplier);
            hasteEndsAt = Time.time + duration;
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.68f, 0.46f, 1f, 0.34f), ModifiedRange(radius), 0.22f);
        }

        private void DamageEnemies(Vector2 position, float activeRadius, int activeDamage)
        {
            Collider2D[] hits = Overlap(position, activeRadius);
            foreach (Collider2D hit in hits)
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                damageable?.ApplyDamage(activeDamage);
            }
        }

        private Collider2D[] Overlap(Vector2 position, float activeRadius)
        {
            return targetLayers.value == 0
                ? Physics2D.OverlapCircleAll(position, activeRadius)
                : Physics2D.OverlapCircleAll(position, activeRadius, targetLayers);
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
