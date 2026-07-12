using UnityEngine;
using System;

namespace Heroic.Combat
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] private int health = 1;
        [SerializeField] private int maxHealth = 1;

        public event Action<Damageable, int> Damaged;
        public event Action<Damageable> Died;

        public int CurrentHealth => health;
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            if (maxHealth < health)
            {
                maxHealth = health;
            }
        }

        public void SetMaxHealth(int newMaxHealth, bool healToFull = true)
        {
            maxHealth = Mathf.Max(1, newMaxHealth);
            health = healToFull ? maxHealth : Mathf.Min(health, maxHealth);
        }

        public void ApplyDamage(int amount)
        {
            if (health <= 0 || amount <= 0)
            {
                return;
            }

            int appliedDamage = Mathf.Min(health, amount);
            health = Mathf.Max(0, health - appliedDamage);
            Damaged?.Invoke(this, appliedDamage);
            if (health <= 0)
            {
                Died?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
