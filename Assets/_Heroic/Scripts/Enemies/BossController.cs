using Heroic.Player;
using UnityEngine;
using System.Collections;

namespace Heroic.Enemies
{
    [RequireComponent(typeof(EnemyController))]
    public class BossController : MonoBehaviour
    {
        [SerializeField] private float pulseInterval = 4f;
        [SerializeField] private float pulseRadius = 3f;
        [SerializeField] private int pulseDamage = 12;
        [SerializeField] private float surgeInterval = 7f;
        [SerializeField] private float surgeDistance = 3f;
        [SerializeField] private LayerMask playerLayers;

        private Transform target;
        private float nextPulseTime;
        private float nextSurgeTime;

        private void Start()
        {
            nextPulseTime = Time.time + pulseInterval;
            nextSurgeTime = Time.time + surgeInterval;
        }

        private void Update()
        {
            if (Time.time >= nextPulseTime)
            {
                Pulse();
                nextPulseTime = Time.time + pulseInterval;
            }

            if (Time.time >= nextSurgeTime)
            {
                StartCoroutine(SurgeRoutine());
                nextSurgeTime = Time.time + surgeInterval;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void Pulse()
        {
            Collider2D[] hits = playerLayers.value == 0
                ? Physics2D.OverlapCircleAll(transform.position, pulseRadius)
                : Physics2D.OverlapCircleAll(transform.position, pulseRadius, playerLayers);

            foreach (Collider2D hit in hits)
            {
                var playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(pulseDamage);
                }
            }
        }

        private IEnumerator SurgeRoutine()
        {
            if (target == null)
            {
                yield break;
            }

            Vector2 start = transform.position;
            Vector2 direction = ((Vector2)target.position - start).normalized;
            Vector2 destination = start + direction * surgeDistance;
            float duration = 0.2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector2.Lerp(start, destination, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
        }
    }
}
