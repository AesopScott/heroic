using UnityEngine;
using Heroic.Combat;
using Heroic.Core;
using Heroic.Data;
using Heroic.Player;
using Heroic.Visuals;

namespace Heroic.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyController enemyPrefab;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private RunManager runManager;
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private WaveDefinition[] waves = new WaveDefinition[0];
        [SerializeField] private float spawnRadius = 8f;
        [SerializeField] private float spawnRateMultiplier = 0.7f;
        [SerializeField] private int spawnRollCount = 5;
        [SerializeField] private float rollInterval = 3f;

        private float nextSpawnTime;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }

            if (playerExperience == null)
            {
                playerExperience = FindAnyObjectByType<PlayerExperience>();
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
            for (int i = 0; i < Mathf.Max(1, spawnRollCount); i++)
            {
                Vector2 location = GetSpawnOffset();
                SpawnRoll(activeWave, location);
            }

            nextSpawnTime = Time.time + GetCurrentSpawnInterval();
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
            rollInterval = Mathf.Max(0.1f, interval);
        }

        private void SpawnEnemy(EnemyDefinition enemyDefinition, Vector2 spawnOffset)
        {
            EnemyController prefab = ResolveEnemyPrefab(enemyDefinition);
            if (prefab == null)
            {
                return;
            }

            Vector3 spawnPosition = playerTarget.position + new Vector3(spawnOffset.x, spawnOffset.y, 0f);
            EnemyController enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
            enemy.SetTarget(playerTarget);
            ApplyDefinition(enemy, enemyDefinition);
        }

        private void SpawnRoll(WaveDefinition activeWave, Vector2 location)
        {
            int level = playerExperience != null ? playerExperience.Level : 1;
            SpawnByLevelTable(activeWave, level, location);
        }

        private void SpawnByLevelTable(WaveDefinition activeWave, int level, Vector2 location)
        {
            if (level <= 0)
            {
                return;
            }

            if (level >= 4)
            {
                TrySpawnLevelTable(activeWave, "enemy_crash_04", location, new[] { 50, 40, 8, 2 });
                TrySpawnLevelTable(activeWave, "enemy_thrower_01", location, new[] { 80, 18, 2, 0 });
                return;
            }

            string crashId = "enemy_crash_" + Mathf.Clamp(level, 1, 5).ToString("00");
            TrySpawnLevelTable(activeWave, crashId, location, new[] { 50, 40, 8, 2 });
        }

        private void TrySpawnLevelTable(WaveDefinition activeWave, string enemyId, Vector2 location, int[] chanceTable)
        {
            EnemyDefinition definition = FindEnemyDefinition(activeWave, enemyId);
            if (definition == null)
            {
                Debug.LogWarning($"EnemySpawner could not find enemy definition `{enemyId}` for player level table.");
                return;
            }

            int roll = Random.Range(0, 100);
            int spawnCount = RollSpawnCount(chanceTable, roll);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 offset = location + Random.insideUnitCircle * 0.75f;
                SpawnEnemy(definition, offset);
            }
        }

        private static int RollSpawnCount(int[] chanceTable, int roll)
        {
            int cumulative = 0;
            for (int i = 0; i < chanceTable.Length; i++)
            {
                cumulative += chanceTable[i];
                if (roll < cumulative)
                {
                    return i;
                }
            }

            return 0;
        }

        private Vector2 GetSpawnOffset()
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = Vector2.right;
            }

            return direction * spawnRadius;
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

        private float GetCurrentSpawnInterval()
        {
            float rateMultiplier = Mathf.Max(0.05f, spawnRateMultiplier);
            return Mathf.Max(0.1f, rollInterval / rateMultiplier);
        }

        private EnemyDefinition FindEnemyDefinition(WaveDefinition activeWave, string enemyId)
        {
            if (activeWave != null && activeWave.SpawnEntries != null)
            {
                foreach (WaveDefinition.SpawnEntry entry in activeWave.SpawnEntries)
                {
                    if (entry != null && entry.Enemy != null && string.Equals(entry.Enemy.Id, enemyId, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return entry.Enemy;
                    }
                }
            }

            foreach (WaveDefinition wave in waves)
            {
                if (wave == null || wave.SpawnEntries == null)
                {
                    continue;
                }

                foreach (WaveDefinition.SpawnEntry entry in wave.SpawnEntries)
                {
                    if (entry != null && entry.Enemy != null && string.Equals(entry.Enemy.Id, enemyId, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return entry.Enemy;
                    }
                }
            }

            return null;
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
            if (definition.VisualPreset == VisualPresetApplier.Preset.CrashLevel4 ||
                definition.VisualPreset == VisualPresetApplier.Preset.CrashLevel5 ||
                definition.VisualPreset == VisualPresetApplier.Preset.WallLevel1)
            {
                enemy.ConfigureContactBehavior(false, false);
            }
            else if (definition.VisualPreset == VisualPresetApplier.Preset.CrashLevel1 ||
                     definition.VisualPreset == VisualPresetApplier.Preset.CrashLevel2 ||
                     definition.VisualPreset == VisualPresetApplier.Preset.CrashLevel3)
            {
                enemy.ConfigureContactBehavior(true, true);
            }

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
