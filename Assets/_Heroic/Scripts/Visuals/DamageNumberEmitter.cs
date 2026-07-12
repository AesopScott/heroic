using Heroic.Combat;
using Heroic.Player;
using UnityEngine;

namespace Heroic.Visuals
{
    public class DamageNumberEmitter : MonoBehaviour
    {
        [SerializeField] private Vector2 offset = new Vector2(0f, 0.65f);
        [SerializeField] private Color damageColor = new Color(1f, 0.92f, 0.45f);
        [SerializeField] private float fontSize = 4.2f;
        [SerializeField] private float minimumEmitInterval = 0.08f;

        private Damageable damageable;
        private PlayerHealth playerHealth;
        private float lastEmitAt = -999f;

        private void Awake()
        {
            damageable = GetComponent<Damageable>();
            playerHealth = GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            if (damageable != null)
            {
                damageable.Damaged += HandleDamageableDamaged;
            }

            if (playerHealth != null)
            {
                playerHealth.Damaged += HandlePlayerDamaged;
            }
        }

        private void OnDisable()
        {
            if (damageable != null)
            {
                damageable.Damaged -= HandleDamageableDamaged;
            }

            if (playerHealth != null)
            {
                playerHealth.Damaged -= HandlePlayerDamaged;
            }
        }

        public void Configure(Vector2 newOffset, Color newDamageColor, float newFontSize)
        {
            offset = newOffset;
            damageColor = newDamageColor;
            fontSize = newFontSize;
        }

        private void HandleDamageableDamaged(Damageable target, int amount)
        {
            Emit(amount);
        }

        private void HandlePlayerDamaged(int amount)
        {
            Emit(amount);
        }

        private void Emit(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            if (Time.time - lastEmitAt < minimumEmitInterval)
            {
                return;
            }

            lastEmitAt = Time.time;
            Vector3 jitter = new Vector3(Random.Range(-0.18f, 0.18f), Random.Range(-0.05f, 0.12f), 0f);
            FloatingCombatText.Create(amount.ToString(), transform.position + (Vector3)offset + jitter, damageColor, fontSize);
        }
    }
}
