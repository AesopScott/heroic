using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Visuals;
using UnityEngine;

namespace Heroic.Spells
{
    public class WarpPulseCaster : MonoBehaviour
    {
        public enum WarpMode
        {
            Push,
            Pull,
            Slow
        }

        [SerializeField] private float castInterval = 4f;
        [SerializeField] private float radius = 3f;
        [SerializeField] private int damage = 5;
        [SerializeField] private WarpMode mode = WarpMode.Push;
        [SerializeField] private float displacementDistance = 1.5f;
        [SerializeField] private float slowMultiplier = 0.5f;
        [SerializeField] private float slowDuration = 2f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private ArcaneDoubleCast doubleCast;
        [SerializeField] private SpellEchoCaster spellEcho;

        private float nextCastTime;

        private void Awake()
        {
            if (doubleCast == null)
            {
                doubleCast = GetComponent<ArcaneDoubleCast>();
            }

            if (spellEcho == null)
            {
                spellEcho = GetComponent<SpellEchoCaster>();
            }
        }

        private void Update()
        {
            if (Time.time < nextCastTime)
            {
                return;
            }

            Cast();
            doubleCast?.TrySchedule(Cast);
            spellEcho?.Echo(Cast);
            nextCastTime = Time.time + castInterval;
        }

        public void SetMode(WarpMode newMode)
        {
            mode = newMode;
        }

        public void SetDisplacementDistance(float value)
        {
            displacementDistance = Mathf.Max(0f, value);
        }

        public void SetSlow(float multiplier, float duration)
        {
            slowMultiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
            slowDuration = Mathf.Max(0f, duration);
        }

        public void SetRadius(float value)
        {
            radius = Mathf.Max(0.1f, value);
        }

        public void SetDamage(int value)
        {
            damage = Mathf.Max(0, value);
        }

        private void Cast()
        {
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.55f, 0.9f, 1f, 0.35f), radius, 0.22f);

            Collider2D[] hits = enemyLayers.value == 0
                ? Physics2D.OverlapCircleAll(transform.position, radius)
                : Physics2D.OverlapCircleAll(transform.position, radius, enemyLayers);

            foreach (Collider2D hit in hits)
            {
                var enemy = hit.GetComponent<EnemyController>();
                var damageable = hit.GetComponent<Damageable>();

                if (damageable != null)
                {
                    damageable.ApplyDamage(damage);
                }

                if (enemy == null)
                {
                    continue;
                }

                if (mode == WarpMode.Push)
                {
                    enemy.Push((Vector2)(hit.transform.position - transform.position), displacementDistance);
                }
                else if (mode == WarpMode.Pull)
                {
                    enemy.Pull(transform.position, displacementDistance);
                }
                else
                {
                    enemy.ApplySlow(slowMultiplier, slowDuration);
                }
            }
        }
    }
}
