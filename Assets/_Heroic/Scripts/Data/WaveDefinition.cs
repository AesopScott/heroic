using UnityEngine;

namespace Heroic.Data
{
    [CreateAssetMenu(menuName = "Heroic/Wave Definition", fileName = "WaveDefinition")]
    public class WaveDefinition : ScriptableObject
    {
        [System.Serializable]
        public class SpawnEntry
        {
            [SerializeField] private EnemyDefinition enemy;
            [SerializeField] private int weight = 1;

            public EnemyDefinition Enemy => enemy;
            public int Weight => Mathf.Max(0, weight);
        }

        [SerializeField] private int waveIndex = 1;
        [SerializeField] private float startsAtSeconds;
        [SerializeField] private float durationSeconds = 30f;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int minSpawnCount = 1;
        [SerializeField] private int maxSpawnCount = 1;
        [SerializeField] private SpawnEntry[] spawnEntries = new SpawnEntry[0];

        public int WaveIndex => waveIndex;
        public float StartsAtSeconds => startsAtSeconds;
        public float DurationSeconds => durationSeconds;
        public float SpawnInterval => spawnInterval;
        public int MinSpawnCount => Mathf.Max(1, minSpawnCount);
        public int MaxSpawnCount => Mathf.Max(MinSpawnCount, maxSpawnCount);
        public SpawnEntry[] SpawnEntries => spawnEntries;

        public bool IsActiveAt(float runTimeSeconds)
        {
            return runTimeSeconds >= startsAtSeconds && runTimeSeconds < startsAtSeconds + durationSeconds;
        }
    }
}
