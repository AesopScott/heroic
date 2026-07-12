using Heroic.Combat;
using UnityEngine;

namespace Heroic.Visuals
{
    [RequireComponent(typeof(Damageable))]
    public class DeathBurstVisual : MonoBehaviour
    {
        [SerializeField] private Color burstColor = new Color(1f, 0.35f, 0.35f, 0.55f);
        [SerializeField] private float burstScale = 1.2f;
        [SerializeField] private float burstDuration = 0.22f;

        private Damageable damageable;

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
            TemporaryVisualEffect.CreateCircle(transform.position, burstColor, burstScale, burstDuration);
        }
    }
}
