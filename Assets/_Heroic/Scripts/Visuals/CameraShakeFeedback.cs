using Heroic.Combat;
using Heroic.Enemies;
using Heroic.Player;
using UnityEngine;

namespace Heroic.Visuals
{
    [DefaultExecutionOrder(100)]
    public class CameraShakeFeedback : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private MovementCaster movementCaster;
        [SerializeField] private BossSpawner bossSpawner;
        [SerializeField] private float movementIntensity = 0.08f;
        [SerializeField] private float movementDuration = 0.1f;
        [SerializeField] private float playerHitIntensity = 0.16f;
        [SerializeField] private float playerHitDuration = 0.14f;
        [SerializeField] private float bossSpawnIntensity = 0.2f;
        [SerializeField] private float bossSpawnDuration = 0.22f;
        [SerializeField] private float bossDeathIntensity = 0.3f;
        [SerializeField] private float bossDeathDuration = 0.32f;

        private Damageable bossDamageable;
        private float remainingDuration;
        private float currentDuration;
        private float currentIntensity;
        private int shakeSeed;

        private void Awake()
        {
            if (playerHealth == null)
            {
                playerHealth = FindAnyObjectByType<PlayerHealth>();
            }

            if (movementCaster == null)
            {
                movementCaster = FindAnyObjectByType<MovementCaster>();
            }

            if (bossSpawner == null)
            {
                bossSpawner = FindAnyObjectByType<BossSpawner>();
            }
        }

        private void OnEnable()
        {
            if (playerHealth != null)
            {
                playerHealth.Damaged += HandlePlayerDamaged;
            }

            if (movementCaster != null)
            {
                movementCaster.MovementActivated += HandleMovementActivated;
            }

            if (bossSpawner != null)
            {
                bossSpawner.BossSpawned += HandleBossSpawned;
            }
        }

        private void OnDisable()
        {
            if (playerHealth != null)
            {
                playerHealth.Damaged -= HandlePlayerDamaged;
            }

            if (movementCaster != null)
            {
                movementCaster.MovementActivated -= HandleMovementActivated;
            }

            if (bossSpawner != null)
            {
                bossSpawner.BossSpawned -= HandleBossSpawned;
            }

            UnsubscribeBoss();
        }

        private void LateUpdate()
        {
            if (remainingDuration <= 0f || currentDuration <= 0f)
            {
                return;
            }

            remainingDuration = Mathf.Max(0f, remainingDuration - Time.unscaledDeltaTime);
            float fade = remainingDuration / currentDuration;
            float noiseX = Mathf.PerlinNoise(shakeSeed, Time.unscaledTime * 42f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(shakeSeed + 31, Time.unscaledTime * 42f) - 0.5f;
            Vector3 offset = new Vector3(noiseX, noiseY, 0f) * currentIntensity * fade;
            transform.position += offset;
        }

        private void HandlePlayerDamaged(int amount)
        {
            Shake(playerHitIntensity, playerHitDuration);
        }

        private void HandleMovementActivated(MovementCaster.MovementSkillId skill)
        {
            Shake(movementIntensity, movementDuration);
        }

        private void HandleBossSpawned(EnemyController boss)
        {
            Shake(bossSpawnIntensity, bossSpawnDuration);
            UnsubscribeBoss();
            bossDamageable = boss != null ? boss.GetComponent<Damageable>() : null;
            if (bossDamageable != null)
            {
                bossDamageable.Died += HandleBossDied;
            }
        }

        private void HandleBossDied(Damageable deadBoss)
        {
            Shake(bossDeathIntensity, bossDeathDuration);
            UnsubscribeBoss();
        }

        private void Shake(float intensity, float duration)
        {
            if (intensity <= 0f || duration <= 0f)
            {
                return;
            }

            currentIntensity = Mathf.Max(currentIntensity * 0.75f, intensity);
            currentDuration = duration;
            remainingDuration = Mathf.Max(remainingDuration, duration);
            shakeSeed = Random.Range(0, 10000);
        }

        private void UnsubscribeBoss()
        {
            if (bossDamageable != null)
            {
                bossDamageable.Died -= HandleBossDied;
                bossDamageable = null;
            }
        }
    }
}
