using UnityEngine;

namespace Heroic.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float homingStrength;
        [SerializeField] private float retargetRange = 12f;

        private Vector2 direction = Vector2.right;
        private Transform homingTarget;

        public void Launch(Vector2 launchDirection, float launchSpeed, Transform target = null, float newHomingStrength = 0f)
        {
            direction = launchDirection.normalized;
            speed = launchSpeed;
            homingTarget = target;
            homingStrength = Mathf.Max(0f, newHomingStrength);
            transform.right = direction;
        }

        private void Update()
        {
            if (homingStrength > 0f && homingTarget == null)
            {
                homingTarget = FindNearestTarget();
            }

            if (homingTarget != null && homingStrength > 0f)
            {
                Vector2 targetDirection = (homingTarget.position - transform.position).normalized;
                direction = Vector2.Lerp(direction, targetDirection, homingStrength * Time.deltaTime).normalized;
                transform.right = direction;
            }

            transform.position += (Vector3)(direction * (speed * Time.deltaTime));
        }

        private Transform FindNearestTarget()
        {
            Heroic.Enemies.EnemyController[] enemies = FindObjectsByType<Heroic.Enemies.EnemyController>(FindObjectsSortMode.None);
            Transform best = null;
            float bestDistance = retargetRange * retargetRange;
            Vector2 position = transform.position;

            foreach (Heroic.Enemies.EnemyController enemy in enemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                float distance = ((Vector2)enemy.transform.position - position).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = enemy.transform;
                }
            }

            return best;
        }
    }
}
