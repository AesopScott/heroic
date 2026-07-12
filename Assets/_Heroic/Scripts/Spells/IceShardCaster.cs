using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Systems;
using Heroic.Visuals;
using UnityEngine;

namespace Heroic.Spells
{
    public class IceShardCaster : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float castInterval = 2.4f;
        [SerializeField] private float range = 7f;
        [SerializeField] private float projectileSpeed = 11f;
        [SerializeField] private int damage = 16;
        [SerializeField] private int projectileCount = 1;
        [SerializeField] private float spreadAngle = 16f;
        [SerializeField] private int pierceCount;
        [SerializeField] private float slowMultiplier = 0.65f;
        [SerializeField] private float slowDuration = 1.6f;
        [SerializeField] private float freezeChance = 0.06f;
        [SerializeField] private float freezeDuration = 0.45f;
        [SerializeField] private float controlledDamageMultiplier = 1f;
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

        public void SetProjectileCount(int value) => projectileCount = Mathf.Max(1, value);
        public void SetPierceCount(int value) => pierceCount = Mathf.Max(0, value);
        public void SetControlledDamageMultiplier(float value) => controlledDamageMultiplier = Mathf.Max(1f, value);

        private void Cast(Transform target)
        {
            if (target == null)
            {
                return;
            }

            Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
            Vector2 direction = (target.position - spawnPosition).normalized;
            int count = Mathf.Max(1, projectileCount);

            for (int i = 0; i < count; i++)
            {
                float angleOffset = count == 1 ? 0f : Mathf.Lerp(-spreadAngle, spreadAngle, i / (float)(count - 1));
                Vector2 shardDirection = (Quaternion.Euler(0f, 0f, angleOffset) * (Vector3)direction).normalized;
                Projectile projectile = CreateProjectile(spawnPosition);
                projectile.Launch(shardDirection, projectileSpeed, target, 0.25f);

                ColdProjectileHit hit = projectile.GetComponent<ColdProjectileHit>();
                if (hit != null)
                {
                    hit.Configure(ModifiedDamage(damage), pierceCount, slowMultiplier, slowDuration, freezeChance, freezeDuration, controlledDamageMultiplier);
                }
            }
        }

        private Projectile CreateProjectile(Vector3 spawnPosition)
        {
            if (projectilePrefab != null)
            {
                return Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            }

            GameObject go = new GameObject("Projectile_IceShard");
            go.transform.position = spawnPosition;
            CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            go.AddComponent<ColdProjectileHit>();
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            visual.ApplyPreset(VisualPresetApplier.Preset.ColdProjectile);
            return go.AddComponent<Projectile>();
        }

        private void CastIfTargetAlive(Transform target)
        {
            if (target != null)
            {
                Cast(target);
            }
        }

        private int ModifiedDamage(int value) => spellStats != null ? spellStats.ModifyDamage(value) : value;
        private float ModifiedRange(float value) => spellStats != null ? spellStats.ModifyRange(value) : value;
        private float ModifiedCooldown(float value) => spellStats != null ? spellStats.ModifyCooldown(value) : value;
    }
}
