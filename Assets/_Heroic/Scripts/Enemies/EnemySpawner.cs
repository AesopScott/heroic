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
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private float spawnRateMultiplier = 0.35f;
        [SerializeField] private float packSpacing = 0.9f;

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
            int spawnCount = GetCurrentSpawnCount(activeWave);
            Vector2 packCenter = GetSpawnOffset();
            Vector2 packSide = new Vector2(-packCenter.y, packCenter.x).normalized;
            for (int i = 0; i < spawnCount; i++)
            {
                float centeredIndex = i - ((spawnCount - 1) * 0.5f);
                Vector2 packOffset = packCenter + (packSide * centeredIndex * packSpacing);
                SpawnEnemy(ChooseCrashDefinition(activeWave), packOffset);
            }

            SpawnSupplementalShooters(activeWave, packCenter, packSide, spawnCount);

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

        private void SpawnSupplementalShooters(WaveDefinition activeWave, Vector2 packCenter, Vector2 packSide, int crashSpawnCount)
        {
            if (playerExperience == null || playerExperience.Level < 4)
            {
                return;
            }

            EnemyDefinition shooterDefinition = FindEnemyDefinition(activeWave, "enemy_shooter_01");
            if (shooterDefinition == null)
            {
                return;
            }

            int shooterCount = Random.Range(0, 2);
            for (int i = 0; i < shooterCount; i++)
            {
                float side = Random.value < 0.5f ? -1f : 1f;
                float spacingIndex = (crashSpawnCount * 0.5f) + 1f + i;
                Vector2 shooterOffset = packCenter + packSide * side * spacingIndex * packSpacing;
                SpawnEnemy(shooterDefinition, shooterOffset);
            }
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

        private float GetCurrentSpawnInterval(WaveDefinition activeWave)
        {
            float rateMultiplier = Mathf.Max(0.05f, spawnRateMultiplier);
            if (activeWave == null)
            {
                return Mathf.Max(0.1f, spawnInterval / rateMultiplier);
            }

            return Mathf.Max(0.1f, activeWave.SpawnInterval / rateMultiplier);
        }

        private int GetCurrentSpawnCount(WaveDefinition activeWave)
        {
            if (activeWave == null)
            {
                return 1;
            }

            int minSpawnCount = activeWave.MinSpawnCount;
            int maxSpawnCount = activeWave.MaxSpawnCount;
            if (activeWave.WaveIndex == 1 && playerExperience != null)
            {
                minSpawnCount = 1;
                maxSpawnCount = playerExperience.Level >= 2 ? 2 : 1;
            }

            return Random.Range(minSpawnCount, maxSpawnCount + 1);
        }

        private EnemyDefinition ChooseCrashDefinition(WaveDefinition activeWave)
        {
            int level = playerExperience != null ? playerExperience.Level : 1;
            int crashLevel = Mathf.Clamp(level, 1, 4);
            EnemyDefinition crashDefinition = FindEnemyDefinition(activeWave, "enemy_crash_" + crashLevel.ToString("00"));
            if (crashDefinition != null)
            {
                return crashDefinition;
            }

            return ChooseEnemy(activeWave);
        }

        private EnemyDefinition FindEnemyDefinition(WaveDefinition activeWave, string enemyId)
        {
            if (activeWave == null || activeWave.SpawnEntries == null)
            {
                return null;
            }

            foreach (WaveDefinition.SpawnEntry entry in activeWave.SpawnEntries)
            {
                if (entry != null && entry.Enemy != null && entry.Enemy.Id == enemyId)
                {
                    return entry.Enemy;
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
            if (definition.VisualPreset == VisualPresetApplier.Preset.CrashLevel4)
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
