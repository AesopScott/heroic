using Heroic.Combat;
using Heroic.Core;
using Heroic.Data;
using Heroic.Visuals;
using System;
using UnityEngine;

namespace Heroic.Enemies
{
    public class BossSpawner : MonoBehaviour
    {
        [SerializeField] private RunManager runManager;
        [SerializeField] private RunEndWatcher runEndWatcher;
        [SerializeField] private EnemyDefinition bossDefinition;
        [SerializeField] private EnemyController fallbackBossPrefab;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private float spawnAtSeconds = 600f;
        [SerializeField] private float spawnDistance = 10f;

        private bool spawned;

        public event Action<EnemyController> BossSpawned;

        public bool HasSpawned => spawned;
        public float SpawnAtSeconds => spawnAtSeconds;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }

            if (runEndWatcher == null)
            {
                runEndWatcher = FindAnyObjectByType<RunEndWatcher>();
            }
        }

        private void Update()
        {
            if (spawned || runManager == null || runManager.CurrentState != RunManager.RunState.Playing)
            {
                return;
            }

            if (runManager.RunTimeSeconds >= spawnAtSeconds)
            {
                SpawnBoss();
            }
        }

        private void SpawnBoss()
        {
            EnemyController prefab = ResolveBossPrefab();
            if (prefab == null || playerTarget == null)
            {
                return;
            }

            spawned = true;
            Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * spawnDistance;
            Vector3 spawnPosition = playerTarget.position + new Vector3(offset.x, offset.y, 0f);
            EnemyController boss = Instantiate(prefab, spawnPosition, Quaternion.identity);
            boss.SetTarget(playerTarget);
            var bossController = boss.GetComponent<BossController>();
            if (bossController != null)
            {
                bossController.SetTarget(playerTarget);
            }

            if (bossDefinition != null)
            {
                boss.Configure(bossDefinition.MoveSpeed, bossDefinition.ContactDamage);

                var damageable = boss.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.SetMaxHealth(bossDefinition.MaxHealth);
                    runEndWatcher?.SetBoss(damageable);
                }

                var dropper = boss.GetComponent<ExperienceDropper>();
                if (dropper != null)
                {
                    dropper.SetExperienceValue(bossDefinition.ExperienceValue);
                }

                var visual = boss.GetComponent<VisualPresetApplier>();
                if (visual != null)
                {
                    visual.ApplyPreset(bossDefinition.VisualPreset);
                }

                ConfigureBossReadability(boss.gameObject);
            }
            else
            {
                runEndWatcher?.SetBoss(boss.GetComponent<Damageable>());
                ConfigureBossReadability(boss.gameObject);
            }

            BossSpawned?.Invoke(boss);
        }

        private void ConfigureBossReadability(GameObject bossObject)
        {
            var healthBar = bossObject.GetComponent<WorldHealthBar>();
            if (healthBar != null)
            {
                healthBar.Configure(new Vector2(0f, 1.65f), new Vector2(2.35f, 0.14f), false, new Color(1f, 0.12f, 0.42f, 0.98f));
            }

            var damageNumbers = bossObject.GetComponent<DamageNumberEmitter>();
            if (damageNumbers != null)
            {
                damageNumbers.Configure(new Vector2(0f, 1.78f), new Color(1f, 0.72f, 0.3f), 5.4f);
            }
        }

        private EnemyController ResolveBossPrefab()
        {
            if (bossDefinition != null && bossDefinition.Prefab != null)
            {
                return bossDefinition.Prefab.GetComponent<EnemyController>();
            }

            return fallbackBossPrefab;
        }

        public void SetSpawnAtSeconds(float seconds)
        {
            spawnAtSeconds = Mathf.Max(0f, seconds);
        }
    }
}
