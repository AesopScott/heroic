using Heroic.Combat;
using Heroic.Player;
using UnityEngine;

namespace Heroic.Enemies
{
    [RequireComponent(typeof(Damageable))]
    public class ExperienceDropper : MonoBehaviour
    {
        [SerializeField] private ExperiencePickup pickupPrefab;
        [SerializeField] private int experienceValue = 1;

        private Damageable damageable;
        private bool suppressNextDrop;

        private void Awake()
        {
            damageable = GetComponent<Damageable>();
        }

        private void OnEnable()
        {
            if (damageable != null)
            {
                damageable.Died += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (damageable != null)
            {
                damageable.Died -= HandleDied;
            }
        }

        private void HandleDied(Damageable dead)
        {
            if (suppressNextDrop)
            {
                suppressNextDrop = false;
                return;
            }

            if (pickupPrefab == null || experienceValue <= 0)
            {
                return;
            }

            ExperiencePickup pickup = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            pickup.SetExperienceValue(experienceValue);
        }

        public void SetExperienceValue(int value)
        {
            experienceValue = Mathf.Max(0, value);
        }

        public void SuppressNextDrop()
        {
            suppressNextDrop = true;
        }
    }
}
