using Heroic.Combat;
using UnityEngine;

namespace Heroic.Spells
{
    public class ArcaneOrbitOrb : MonoBehaviour
    {
        private Transform center;
        private float angle;
        private float radius;
        private float rotationSpeed;
        private int damage;
        private bool consumed;

        public void Initialize(Transform center, float startAngle, float orbitRadius, float speed, int hitDamage)
        {
            this.center = center;
            angle = startAngle;
            radius = orbitRadius;
            rotationSpeed = speed;
            damage = hitDamage;
        }

        public void SetRotationSpeed(float speed)
        {
            rotationSpeed = speed;
        }

        public void SetRadius(float orbitRadius)
        {
            radius = orbitRadius;
        }

        public void SetDamage(int hitDamage)
        {
            damage = hitDamage;
        }

        private void Update()
        {
            if (center == null)
            {
                Destroy(gameObject);
                return;
            }

            angle += rotationSpeed * Time.deltaTime;
            float radians = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * radius;
            transform.position = center.position + offset;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var damageable = other.GetComponent<Damageable>();
            if (damageable != null && !consumed)
            {
                consumed = true;
                damageable.ApplyDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
