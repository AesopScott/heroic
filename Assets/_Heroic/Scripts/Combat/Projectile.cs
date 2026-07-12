using UnityEngine;

namespace Heroic.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float homingStrength;

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
            if (homingTarget != null && homingStrength > 0f)
            {
                Vector2 targetDirection = (homingTarget.position - transform.position).normalized;
                direction = Vector2.Lerp(direction, targetDirection, homingStrength * Time.deltaTime).normalized;
                transform.right = direction;
            }

            transform.position += (Vector3)(direction * (speed * Time.deltaTime));
        }
    }
}
