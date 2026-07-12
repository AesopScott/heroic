using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using UnityEngine;

namespace Heroic.Spells
{
    public class FireBoltCaster : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float castInterval = 1f;
        [SerializeField] private float range = 10f;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private int damage = 18;
        [SerializeField] private int projectileCount = 1;
        [SerializeField] private float spreadAngle = 18f;
        [SerializeField] private int pierceCount;
        [SerializeField] private Transform firePoint;
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

            Transform targetTransform = target.transform;
            Cast(targetTransform);
            spellEcho?.Echo(() => CastIfTargetAlive(targetTransform));
            nextCastTime = Time.time + ModifiedCooldown(castInterval);
        }

        public void SetDamage(int value)
        {
            damage = Mathf.Max(0, value);
        }

        public void SetProjectileCount(int value)
        {
            projectileCount = Mathf.Max(1, value);
        }

        public void SetPierceCount(int value)
        {
            pierceCount = Mathf.Max(0, value);
        }

        public void SetCastInterval(float value)
        {
            castInterval = Mathf.Max(0.1f, value);
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
                Vector2 boltDirection = (Quaternion.Euler(0f, 0f, angleOffset) * (Vector3)direction).normalized;
                Projectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
                projectile.Launch(boltDirection, projectileSpeed, target, 0.4f);

                ProjectileHit hit = projectile.GetComponent<ProjectileHit>();
                if (hit != null)
                {
                    hit.SetDamage(ModifiedDamage(damage));
                    hit.SetPierceCount(pierceCount);
                }
            }
        }

        private void CastIfTargetAlive(Transform target)
        {
            if (target != null)
            {
                Cast(target);
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
