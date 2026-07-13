using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using UnityEngine;

namespace Heroic.Spells
{
    public class MagicMissileCaster : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float castInterval = 0.75f;
        [SerializeField] private float range = 10f;
        [SerializeField] private float projectileSpeed = 12f;
        [SerializeField] private int damage = 10;
        [SerializeField] private int projectileCount = 1;
        [SerializeField] private float spreadAngle = 12f;
        [SerializeField] private float homingStrength = 4f;
        [SerializeField] private int pierceCount = 0;
        [SerializeField] private Transform firePoint;
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

            var target = FindNearestEnemy();
            if (target == null)
            {
                return;
            }

            Cast(target.transform);
            doubleCast?.TrySchedule(CastAtNearestTarget);
            spellEcho?.Echo(CastAtNearestTarget);
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetProjectileCount(int value)
        {
            projectileCount = Mathf.Max(1, value);
        }

        public void SetHomingStrength(float value)
        {
            homingStrength = Mathf.Max(0f, value);
        }

        public void SetPierceCount(int value)
        {
            pierceCount = Mathf.Max(0, value);
        }

        public void SetDamage(int value)
        {
            damage = Mathf.Max(0, value);
        }

        public void SetCastInterval(float value)
        {
            castInterval = Mathf.Max(0.05f, value);
        }

        private void Cast(Transform target)
        {
            if (projectilePrefab == null || target == null)
            {
                return;
            }

            Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
            Vector2 direction = (target.position - spawnPosition).normalized;
            int count = Mathf.Max(1, projectileCount);

            for (int i = 0; i < count; i++)
            {
                float angleOffset = count == 1 ? 0f : Mathf.Lerp(-spreadAngle, spreadAngle, i / (float)(count - 1));
                Vector3 rotatedDirection = Quaternion.Euler(0f, 0f, angleOffset) * (Vector3)direction;
                Vector2 missileDirection = rotatedDirection.normalized;
                Projectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
                projectile.Launch(missileDirection, projectileSpeed, target, Mathf.Max(1.5f, homingStrength));

                var hit = projectile.GetComponent<ProjectileHit>();
                if (hit != null)
                {
                    hit.SetDamage(ModifiedDamage(damage));
                    hit.SetPierceCount(pierceCount);
                }
            }
        }

        private void CastIfTargetAlive(Transform target)
        {
            if (target == null)
            {
                return;
            }

            Cast(target);
        }

        private void CastAtNearestTarget()
        {
            EnemyController target = FindNearestEnemy();
            if (target != null)
            {
                Cast(target.transform);
            }
        }

        private EnemyController FindNearestEnemy()
        {
            return ArcaneTargeting.FindNearestEnemy(transform.position, ModifiedRange(range));
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
