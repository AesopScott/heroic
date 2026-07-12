using UnityEngine;
using System;

namespace Heroic.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth = 100;

        private bool invulnerable;

        public event Action<int> Damaged;
        public event Action Died;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsInvulnerable => invulnerable;

        public void TakeDamage(int amount)
        {
            if (currentHealth <= 0 || amount <= 0 || invulnerable)
            {
                return;
            }

            int appliedDamage = Mathf.Min(currentHealth, amount);
            currentHealth = Mathf.Max(0, currentHealth - appliedDamage);
            Damaged?.Invoke(appliedDamage);
            if (currentHealth <= 0)
            {
                Died?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        public void SetMaxHealth(int value)
        {
            maxHealth = Mathf.Max(1, value);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        public void SetInvulnerable(bool value)
        {
            invulnerable = value;
        }
    }
}
