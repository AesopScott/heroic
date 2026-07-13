using UnityEngine;

namespace Heroic.World
{
    public readonly struct TerrainLevelProfile
    {
        public TerrainLevelProfile(
            int level,
            int decorativeCount,
            int slowPatchCount,
            float slowMultiplier,
            int highGroundCount,
            int blockerCount,
            int largeBlockerCount)
        {
            Level = Mathf.Clamp(level, 1, 30);
            DecorativeCount = Mathf.Max(0, decorativeCount);
            SlowPatchCount = Mathf.Max(0, slowPatchCount);
            SlowMultiplier = Mathf.Clamp01(slowMultiplier);
            HighGroundCount = Mathf.Max(0, highGroundCount);
            BlockerCount = Mathf.Max(0, blockerCount);
            LargeBlockerCount = Mathf.Max(0, largeBlockerCount);
        }

        public int Level { get; }
        public int DecorativeCount { get; }
        public int SlowPatchCount { get; }
        public float SlowMultiplier { get; }
        public int HighGroundCount { get; }
        public int BlockerCount { get; }
        public int LargeBlockerCount { get; }
        public bool HasHardBlockers => BlockerCount > 0 || LargeBlockerCount > 0;

        public static TerrainLevelProfile ForLevel(int level)
        {
            int clamped = Mathf.Clamp(level, 1, 30);
            return clamped switch
            {
                1 => new TerrainLevelProfile(1, 12, 0, 1.00f, 0, 0, 0),
                2 => new TerrainLevelProfile(2, 13, 2, 0.94f, 0, 0, 0),
                3 => new TerrainLevelProfile(3, 14, 3, 0.92f, 0, 0, 0),
                4 => new TerrainLevelProfile(4, 15, 4, 0.90f, 0, 0, 0),
                5 => new TerrainLevelProfile(5, 16, 5, 0.88f, 0, 0, 0),
                6 => new TerrainLevelProfile(6, 17, 6, 0.86f, 0, 0, 0),
                7 => new TerrainLevelProfile(7, 18, 7, 0.84f, 0, 0, 0),
                8 => new TerrainLevelProfile(8, 19, 8, 0.82f, 0, 0, 0),
                9 => new TerrainLevelProfile(9, 20, 9, 0.80f, 0, 0, 0),
                10 => new TerrainLevelProfile(10, 20, 9, 0.80f, 2, 0, 0),
                11 => new TerrainLevelProfile(11, 21, 10, 0.78f, 3, 0, 0),
                12 => new TerrainLevelProfile(12, 22, 10, 0.76f, 3, 0, 0),
                13 => new TerrainLevelProfile(13, 23, 11, 0.74f, 4, 0, 0),
                14 => new TerrainLevelProfile(14, 24, 11, 0.72f, 5, 0, 0),
                15 => new TerrainLevelProfile(15, 25, 12, 0.70f, 5, 0, 0),
                16 => new TerrainLevelProfile(16, 25, 12, 0.70f, 5, 4, 0),
                17 => new TerrainLevelProfile(17, 26, 13, 0.68f, 5, 6, 0),
                18 => new TerrainLevelProfile(18, 27, 13, 0.66f, 6, 8, 1),
                19 => new TerrainLevelProfile(19, 28, 14, 0.64f, 6, 9, 1),
                20 => new TerrainLevelProfile(20, 29, 14, 0.62f, 7, 10, 2),
                21 => new TerrainLevelProfile(21, 30, 15, 0.60f, 7, 11, 2),
                22 => new TerrainLevelProfile(22, 31, 15, 0.58f, 8, 12, 3),
                23 => new TerrainLevelProfile(23, 32, 16, 0.56f, 8, 13, 3),
                24 => new TerrainLevelProfile(24, 33, 16, 0.54f, 9, 14, 4),
                25 => new TerrainLevelProfile(25, 34, 17, 0.52f, 9, 15, 4),
                26 => new TerrainLevelProfile(26, 35, 17, 0.50f, 10, 16, 5),
                27 => new TerrainLevelProfile(27, 36, 18, 0.48f, 10, 18, 5),
                28 => new TerrainLevelProfile(28, 37, 18, 0.46f, 11, 20, 6),
                29 => new TerrainLevelProfile(29, 38, 19, 0.44f, 11, 22, 7),
                _ => new TerrainLevelProfile(30, 40, 20, 0.42f, 12, 24, 8),
            };
        }
    }
}
