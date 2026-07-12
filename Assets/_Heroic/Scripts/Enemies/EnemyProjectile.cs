using Heroic.Player;
using UnityEngine;

namespace Heroic.Enemies
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 4f;
        [SerializeField] private int damage = 15;
        [SerializeField] private float lifetime = 12f;

        private Vector2 direction = Vector2.right;

        public void Launch(Vector2 launchDirection, float launchSpeed, int hitDamage)
        {
            direction = launchDirection.sqrMagnitude > 0.001f ? launchDirection.normalized : Vector2.right;
            speed = Mathf.Max(0.1f, launchSpeed);
            damage = Mathf.Max(0, hitDamage);
            transform.right = direction;
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                return;
            }

            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
