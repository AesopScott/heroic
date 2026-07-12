using Heroic.Combat;
using Heroic.Player;
using UnityEngine;

namespace Heroic.Enemies
{
    [RequireComponent(typeof(Damageable))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int contactDamage = 10;
        [SerializeField] private float contactRange = 0.85f;
        [SerializeField] private float contactDamageInterval = 1f;
        [SerializeField] private bool destroyAfterContactDamage = true;
        [SerializeField] private bool suppressExperienceOnContactDamage = true;

        private Transform target;
        private float nextContactDamageTime;
        private float slowMultiplier = 1f;
        private float slowEndsAt;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void Configure(float newMoveSpeed, int newContactDamage)
        {
            moveSpeed = Mathf.Max(0f, newMoveSpeed);
            contactDamage = Mathf.Max(0, newContactDamage);
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

            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * (moveSpeed * slowMultiplier * Time.deltaTime);

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
