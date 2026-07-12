using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Visuals;
using UnityEngine;
using System.Collections;

namespace Heroic.Spells
{
    public class BurningGroundCaster : MonoBehaviour
    {
        [SerializeField] private float castInterval = 4.5f;
        [SerializeField] private float range = 9f;
        [SerializeField] private float radius = 1.45f;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float tickInterval = 0.5f;
        [SerializeField] private int damagePerTick = 8;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private SpellEchoCaster spellEcho;

        private float nextCastTime;

        private void Awake()
        {
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

            EnemyController target = ArcaneTargeting.FindNearestEnemy(transform.position, range);
            if (target == null)
            {
                return;
            }

            Vector2 position = target.transform.position;
            CastAt(position);
            spellEcho?.Echo(() => CastAt(position));
            nextCastTime = Time.time + castInterval;
        }

        public void SetDamagePerTick(int value)
        {
            damagePerTick = Mathf.Max(0, value);
        }

        public void SetRadius(float value)
        {
            radius = Mathf.Max(0.25f, value);
        }

        public void SetDuration(float value)
        {
            duration = Mathf.Max(0.5f, value);
        }

        public void SetCastInterval(float value)
        {
            castInterval = Mathf.Max(0.25f, value);
        }

        private void CastAt(Vector2 position)
        {
            StartCoroutine(BurningGroundRoutine(position));
        }

        private IEnumerator BurningGroundRoutine(Vector2 position)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                TemporaryVisualEffect.CreateCircle(position, new Color(1f, 0.2f, 0.02f, 0.22f), radius, 0.28f);
                DamageAt(position);
                elapsed += tickInterval;
                yield return new WaitForSeconds(tickInterval);
            }
        }

        private void DamageAt(Vector2 position)
        {
            Collider2D[] hits = enemyLayers.value == 0
                ? Physics2D.OverlapCircleAll(position, radius)
                : Physics2D.OverlapCircleAll(position, radius, enemyLayers);

            foreach (Collider2D hit in hits)
            {
                Damageable damageable = hit.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(damagePerTick);
                }
            }
        }
    }
}
