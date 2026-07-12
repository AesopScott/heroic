using UnityEngine;
using Heroic.Combat;
using Heroic.Core;
using Heroic.Data;
using Heroic.Visuals;

namespace Heroic.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyController enemyPrefab;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private RunManager runManager;
        [SerializeField] private WaveDefinition[] waves = new WaveDefinition[0];
        [SerializeField] private float spawnRadius = 8f;
        [SerializeField] private float spawnInterval = 2f;

        private float nextSpawnTime;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }
        }

        private void Update()
        {
            if (runManager != null && runManager.CurrentState != RunManager.RunState.Playing)
            {
                return;
            }

            if (Time.time < nextSpawnTime || playerTarget == null)
            {
                return;
            }

            WaveDefinition activeWave = GetActiveWave();
            int spawnCount = GetCurrentSpawnCount(activeWave);
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnEnemy(activeWave);
            }

            nextSpawnTime = Time.time + GetCurrentSpawnInterval(activeWave);
        }

        public void StartWave(int waveIndex)
        {
            foreach (WaveDefinition wave in waves)
            {
                if (wave != null && wave.WaveIndex == waveIndex)
                {
                    nextSpawnTime = Time.time;
                    return;
                }
            }
        }

        public void SetBaseSpawnInterval(float interval)
        {
            spawnInterval = Mathf.Max(0.1f, interval);
        }

        private void SpawnEnemy(WaveDefinition activeWave)
        {
            EnemyDefinition enemyDefinition = ChooseEnemy(activeWave);
            EnemyController prefab = ResolveEnemyPrefab(enemyDefinition);
            if (prefab == null)
            {
                return;
            }

            Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPosition = playerTarget.position + new Vector3(offset.x, offset.y, 0f);
            EnemyController enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
            enemy.SetTarget(playerTarget);
            ApplyDefinition(enemy, enemyDefinition);
        }

        private WaveDefinition GetActiveWave()
        {
            if (runManager == null)
            {
                return null;
            }

            foreach (WaveDefinition wave in waves)
            {
                if (wave != null && wave.IsActiveAt(runManager.RunTimeSeconds))
                {
                    return wave;
                }
            }

            return null;
        }

        private float GetCurrentSpawnInterval(WaveDefinition activeWave)
        {
            if (activeWave == null)
            {
                return spawnInterval;
            }

            return Mathf.Max(0.1f, activeWave.SpawnInterval);
        }

        private int GetCurrentSpawnCount(WaveDefinition activeWave)
        {
            if (activeWave == null)
            {
                return 1;
            }

            return Random.Range(activeWave.MinSpawnCount, activeWave.MaxSpawnCount + 1);
        }

        private EnemyDefinition ChooseEnemy(WaveDefinition activeWave)
        {
            if (activeWave == null || activeWave.SpawnEntries == null || activeWave.SpawnEntries.Length == 0)
            {
                return null;
            }

            int totalWeight = 0;
            foreach (WaveDefinition.SpawnEntry entry in activeWave.SpawnEntries)
            {
                if (entry != null && entry.Enemy != null)
                {
                    totalWeight += entry.Weight;
                }
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            int roll = Random.Range(0, totalWeight);
            foreach (WaveDefinition.SpawnEntry entry in activeWave.SpawnEntries)
            {
                if (entry == null || entry.Enemy == null)
                {
                    continue;
                }

                roll -= entry.Weight;
                if (roll < 0)
                {
                    return entry.Enemy;
                }
            }

            return null;
        }

        private EnemyController ResolveEnemyPrefab(EnemyDefinition enemyDefinition)
        {
            if (enemyDefinition != null && enemyDefinition.Prefab != null)
            {
                return enemyDefinition.Prefab.GetComponent<EnemyController>();
            }

            return enemyPrefab;
        }

        private void ApplyDefinition(EnemyController enemy, EnemyDefinition definition)
        {
            if (enemy == null || definition == null)
            {
                return;
            }

            enemy.Configure(definition.MoveSpeed, definition.ContactDamage);

            var damageable = enemy.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.SetMaxHealth(definition.MaxHealth);
            }

            var dropper = enemy.GetComponent<ExperienceDropper>();
            if (dropper != null)
            {
                dropper.SetExperienceValue(definition.ExperienceValue);
            }

            var visual = enemy.GetComponent<VisualPresetApplier>();
            if (visual != null)
            {
                visual.ApplyPreset(definition.VisualPreset);
            }

            ConfigureCombatReadability(enemy.gameObject, definition.VisualPreset);
        }

        private void ConfigureCombatReadability(GameObject enemyObject, VisualPresetApplier.Preset preset)
        {
            var healthBar = enemyObject.GetComponent<WorldHealthBar>();
            if (healthBar != null)
            {
                switch (preset)
                {
                    case VisualPresetApplier.Preset.FastEnemy:
                        healthBar.Configure(new Vector2(0f, 0.58f), new Vector2(0.82f, 0.075f), true, new Color(1f, 0.85f, 0.2f, 0.95f));
                        break;
                    case VisualPresetApplier.Preset.TankEnemy:
                        healthBar.Configure(new Vector2(0f, 0.94f), new Vector2(1.3f, 0.1f), true, new Color(1f, 0.42f, 0.22f, 0.95f));
                        break;
                    default:
                        healthBar.Configure(new Vector2(0f, 0.78f), new Vector2(1.1f, 0.09f), true, new Color(0.22f, 0.95f, 0.68f, 0.95f));
                        break;
                }
            }

            var damageNumbers = enemyObject.GetComponent<DamageNumberEmitter>();
            if (damageNumbers != null)
            {
                damageNumbers.Configure(new Vector2(0f, preset == VisualPresetApplier.Preset.TankEnemy ? 1.05f : 0.68f), new Color(1f, 0.92f, 0.45f), preset == VisualPresetApplier.Preset.TankEnemy ? 4.8f : 4.2f);
            }
        }
    }
}
