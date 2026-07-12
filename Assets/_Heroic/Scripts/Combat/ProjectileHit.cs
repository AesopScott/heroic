using Heroic.Combat;
using UnityEngine;

namespace Heroic.Combat
{
    public class ProjectileHit : MonoBehaviour
    {
        [SerializeField] private int damage = 10;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private int pierceCount;

        public void SetDamage(int newDamage)
        {
            damage = Mathf.Max(0, newDamage);
        }

        public void SetPierceCount(int newPierceCount)
        {
            pierceCount = Mathf.Max(0, newPierceCount);
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var damageable = other.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.ApplyDamage(damage);
                if (pierceCount <= 0)
                {
                    Destroy(gameObject);
                    return;
                }

                pierceCount--;
            }
        }
    }
}
