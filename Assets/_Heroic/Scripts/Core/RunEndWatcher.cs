using Heroic.Combat;
using Heroic.Player;
using UnityEngine;

namespace Heroic.Core
{
    public class RunEndWatcher : MonoBehaviour
    {
        [SerializeField] private RunManager runManager;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private Damageable bossDamageable;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }
        }

        private void OnEnable()
        {
            if (playerHealth != null)
            {
                playerHealth.Died += HandlePlayerDied;
            }

            if (bossDamageable != null)
            {
                bossDamageable.Died += HandleBossDied;
            }
        }

        private void OnDisable()
        {
            if (playerHealth != null)
            {
                playerHealth.Died -= HandlePlayerDied;
            }

            if (bossDamageable != null)
            {
                bossDamageable.Died -= HandleBossDied;
            }
        }

        public void SetBoss(Damageable newBoss)
        {
            if (bossDamageable != null)
            {
                bossDamageable.Died -= HandleBossDied;
            }

            bossDamageable = newBoss;

            if (bossDamageable != null && isActiveAndEnabled)
            {
                bossDamageable.Died += HandleBossDied;
            }
        }

        private void HandlePlayerDied()
        {
            runManager?.EndRun(false);
        }

        private void HandleBossDied(Damageable deadBoss)
        {
            runManager?.EndRun(true);
        }
    }
}
