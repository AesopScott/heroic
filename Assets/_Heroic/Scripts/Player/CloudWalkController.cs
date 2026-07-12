using Heroic.Enemies;
using Heroic.Visuals;
using System.Collections.Generic;
using UnityEngine;

namespace Heroic.Player
{
    public class CloudWalkController : MonoBehaviour
    {
        [SerializeField] private float initialStandardSpeedMultiplier = 1.25f;
        [SerializeField] private float standardSpeedMultiplierPerTier = 0.2f;
        [SerializeField] private float cloudSpeedMultiplier = 1.5f;
        [SerializeField] private float standardPickupRange = 20f;
        [SerializeField] private float knockbackRange = 50f;
        [SerializeField] private float knockbackDistance = 3f;
        [SerializeField] private LayerMask enemyLayers;

        private readonly Dictionary<EnemyController, float> nextKnockbackByEnemy = new Dictionary<EnemyController, float>();
        private readonly HashSet<EnemyController> knockedOnce = new HashSet<EnemyController>();

        private PlayerController playerController;
        private PlayerPickupMagnet pickupMagnet;
        private Coroutine activeCloudWalk;
        private int knockbackTier;
        private float nextProcScanTime;
        private float startingBaseMoveSpeed;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            pickupMagnet = GetComponent<PlayerPickupMagnet>();
            startingBaseMoveSpeed = playerController != null ? playerController.BaseMoveSpeed : 6f;
        }

        private void Update()
        {
            if (knockbackTier <= 0 || Time.time < nextProcScanTime)
            {
                return;
            }

            nextProcScanTime = Time.time + 0.25f;
            ProcKnockback();
        }

        private void OnDisable()
        {
            playerController?.SetTemporarySpeedMultiplier(1f);
        }

        public void EnableCloudWalk()
        {
            SetStandardMovementTier(0);
            SetPickupRangeTier(0);
        }

        public void BeginCloudWalk(float duration)
        {
            if (activeCloudWalk != null)
            {
                StopCoroutine(activeCloudWalk);
            }

            activeCloudWalk = StartCoroutine(CloudWalkRoutine(duration));
        }

        public void SetStandardMovementTier(int tier)
        {
            float multiplier = initialStandardSpeedMultiplier + Mathf.Clamp(tier, 0, 5) * standardSpeedMultiplierPerTier;
            playerController?.SetBaseMoveSpeed(startingBaseMoveSpeed * multiplier);
        }

        public void SetPickupRangeTier(int tier)
        {
            float multiplier = 1f + Mathf.Clamp(tier, 0, 5) * 0.5f;
            pickupMagnet?.SetPickupRange(standardPickupRange * multiplier);
        }

        public void SetKnockbackTier(int tier)
        {
            knockbackTier = Mathf.Clamp(tier, 0, 5);
        }

        private System.Collections.IEnumerator CloudWalkRoutine(float duration)
        {
            playerController?.SetTemporarySpeedMultiplier(cloudSpeedMultiplier);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.78f, 1f, 0.92f, 0.28f), 1.25f, 0.22f);
            yield return new WaitForSeconds(Mathf.Max(0.1f, duration));
            playerController?.SetTemporarySpeedMultiplier(1f);
            activeCloudWalk = null;
        }

        private void ProcKnockback()
        {
            Collider2D[] hits = enemyLayers.value == 0
                ? Physics2D.OverlapCircleAll(transform.position, knockbackRange)
                : Physics2D.OverlapCircleAll(transform.position, knockbackRange, enemyLayers);

            foreach (Collider2D hit in hits)
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy == null)
                {
                    continue;
                }

                if (!CanKnockback(enemy))
                {
                    continue;
                }

                Vector2 direction = (Vector2)(enemy.transform.position - transform.position);
                enemy.Push(direction.sqrMagnitude > 0.001f ? direction : Vector2.right, knockbackDistance);
                TemporaryVisualEffect.CreateCircle(enemy.transform.position, new Color(0.76f, 1f, 0.86f, 0.22f), 0.8f, 0.12f);
                RecordKnockback(enemy);
            }
        }

        private bool CanKnockback(EnemyController enemy)
        {
            if (knockbackTier <= 1)
            {
                return !knockedOnce.Contains(enemy);
            }

            return !nextKnockbackByEnemy.TryGetValue(enemy, out float nextAllowedTime) || Time.time >= nextAllowedTime;
        }

        private void RecordKnockback(EnemyController enemy)
        {
            knockedOnce.Add(enemy);
            if (knockbackTier <= 1)
            {
                return;
            }

            nextKnockbackByEnemy[enemy] = Time.time + KnockbackCooldown();
        }

        private float KnockbackCooldown()
        {
            switch (knockbackTier)
            {
                case 2:
                    return 10f;
                case 3:
                    return 8f;
                case 4:
                    return 6f;
                default:
                    return 4f;
            }
        }
    }
}
