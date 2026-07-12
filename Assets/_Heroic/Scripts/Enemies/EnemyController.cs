using Heroic.Combat;
using Heroic.Player;
using UnityEngine;

namespace Heroic.Enemies
{
    [RequireComponent(typeof(Damageable))]
    public class EnemyController : MonoBehaviour
    {
        public enum EnemyBehavior
        {
            Crash,
            Thrower
        }

        [SerializeField] private EnemyBehavior behavior = EnemyBehavior.Crash;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int contactDamage = 10;
        [SerializeField] private float contactRange = 0.85f;
        [SerializeField] private float contactDamageInterval = 1f;
        [SerializeField] private bool destroyAfterContactDamage = true;
        [SerializeField] private bool suppressExperienceOnContactDamage = true;
        [SerializeField] private EnemyProjectile projectilePrefab;
        [SerializeField] private float ThrowerRange = 50f;
        [SerializeField] private float ThrowerFireInterval = 5f;
        [SerializeField] private float ThrowerProjectileSpeed = 4f;
        [SerializeField] private int ThrowerProjectileDamage = 15;
        [SerializeField] private Transform firePoint;

        private Transform target;
        private float nextContactDamageTime;
        private float nextShotTime;
        private float slowMultiplier = 1f;
        private float slowEndsAt;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (behavior == EnemyBehavior.Thrower)
            {
                nextShotTime = Time.time + ThrowerFireInterval;
            }
        }

        public void Configure(float newMoveSpeed, int newContactDamage)
        {
            moveSpeed = Mathf.Max(0f, newMoveSpeed);
            contactDamage = Mathf.Max(0, newContactDamage);
        }

        public void ConfigureContactBehavior(bool destroyOnContactDamage, bool suppressExperienceDrop)
        {
            destroyAfterContactDamage = destroyOnContactDamage;
            suppressExperienceOnContactDamage = suppressExperienceDrop;
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            if (Time.time >= slowEndsAt)
            {
                slowMultiplier = 1f;
            }

            if (behavior == EnemyBehavior.Thrower)
            {
                UpdateThrower();
                return;
            }

            MoveTowardTarget();
            TryApplyContactDamage();
        }

        private void MoveTowardTarget()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * (moveSpeed * slowMultiplier * Time.deltaTime);
        }

        private void TryApplyContactDamage()
        {
            if (Vector3.Distance(transform.position, target.position) <= contactRange)
            {
                if (Time.time < nextContactDamageTime)
                {
                    return;
                }

                var playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(contactDamage);
                    nextContactDamageTime = Time.time + contactDamageInterval;
                    if (destroyAfterContactDamage)
                    {
                        if (suppressExperienceOnContactDamage)
                        {
                            ExperienceDropper dropper = GetComponent<ExperienceDropper>();
                            dropper?.SuppressNextDrop();
                        }

                        Damageable damageable = GetComponent<Damageable>();
                        if (damageable != null)
                        {
                            damageable.ApplyDamage(damageable.CurrentHealth);
                        }
                        else
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }

        private void UpdateThrower()
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > ThrowerRange)
            {
                MoveTowardTarget();
            }

            if (Time.time >= nextShotTime && distance <= ThrowerRange)
            {
                FireAtCurrentPlayerPosition();
                nextShotTime = Time.time + ThrowerFireInterval;
            }
        }

        private void FireAtCurrentPlayerPosition()
        {
            if (projectilePrefab == null || target == null)
            {
                return;
            }

            Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
            Vector2 direction = (target.position - spawnPosition).normalized;
            EnemyProjectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            projectile.Launch(direction, ThrowerProjectileSpeed, ThrowerProjectileDamage);
        }

        public void Push(Vector2 direction, float distance)
        {
            transform.position += (Vector3)(direction.normalized * distance);
        }

        public void Pull(Vector2 towardPosition, float distance)
        {
            Vector2 direction = towardPosition - (Vector2)transform.position;
            transform.position += (Vector3)(direction.normalized * distance);
        }

        public void ApplySlow(float multiplier, float duration)
        {
            slowMultiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
            slowEndsAt = Time.time + Mathf.Max(0f, duration);
        }
    }
}

