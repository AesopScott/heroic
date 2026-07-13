using UnityEngine;

namespace Heroic.World
{
    public readonly struct TerrainLevelProfile
    {
        public TerrainLevelProfile(
            int level,
            string name,
            int decorativeCount,
            int slowPatchCount,
            float slowMultiplier,
            int highGroundCount,
            int blockerCount,
            int largeBlockerCount)
        {
            Level = level;
            Name = name;
            DecorativeCount = decorativeCount;
            SlowPatchCount = slowPatchCount;
            SlowMultiplier = Mathf.Clamp(slowMultiplier, 0.2f, 1f);
            HighGroundCount = Mathf.Max(0, highGroundCount);
            BlockerCount = Mathf.Max(0, blockerCount);
            LargeBlockerCount = Mathf.Max(0, largeBlockerCount);
        }

        public int Level { get; }
        public string Name { get; }
        public int DecorativeCount { get; }
        public int SlowPatchCount { get; }
        public float SlowMultiplier { get; }
        public int HighGroundCount { get; }
        public int BlockerCount { get; }
        public int LargeBlockerCount { get; }

        public bool HasImpedingTerrain => SlowPatchCount > 0;
        public bool HasFightableTerrain => HighGroundCount > 0;
        public bool HasHardBlockers => BlockerCount + LargeBlockerCount > 0;

        public Vector2 SlowPatchSizeRange => new Vector2(
            Mathf.Lerp(2.2f, 4.8f, Mathf.InverseLerp(2f, 30f, Level)),
            Mathf.Lerp(3.8f, 8.5f, Mathf.InverseLerp(2f, 30f, Level)));

        public Vector2 HighGroundSizeRange => new Vector2(
            Mathf.Lerp(2.6f, 4.2f, Mathf.InverseLerp(10f, 30f, Level)),
            Mathf.Lerp(4.5f, 7.8f, Mathf.InverseLerp(10f, 30f, Level)));

        public Vector2 BlockerSizeRange => new Vector2(
            Mathf.Lerp(0.8f, 1.4f, Mathf.InverseLerp(16f, 30f, Level)),
            Mathf.Lerp(1.4f, 2.4f, Mathf.InverseLerp(16f, 30f, Level)));

        public float HighGroundDamageMultiplier => HasFightableTerrain ? 1.08f + Mathf.Min(0.22f, Level * 0.006f) : 1f;
        public float HighGroundRangeMultiplier => HasFightableTerrain ? 1.06f + Mathf.Min(0.18f, Level * 0.005f) : 1f;

        public static TerrainLevelProfile ForLevel(int level)
        {
            int index = Mathf.Clamp(level, 1, Profiles.Length) - 1;
            return Profiles[index];
        }

        public static readonly TerrainLevelProfile[] Profiles =
        {
            new TerrainLevelProfile(1, "Bare Dirt", 0, 0, 1f, 0, 0, 0),
            new TerrainLevelProfile(2, "Scuffed Dirt", 13, 2, 0.94f, 0, 0, 0),
            new TerrainLevelProfile(3, "Loose Furrows", 14, 3, 0.92f, 0, 0, 0),
            new TerrainLevelProfile(4, "Brush Patches", 15, 4, 0.90f, 0, 0, 0),
            new TerrainLevelProfile(5, "Mud Veins", 16, 5, 0.88f, 0, 0, 0),
            new TerrainLevelProfile(6, "Broken Track", 17, 6, 0.86f, 0, 0, 0),
            new TerrainLevelProfile(7, "Wet Lowland", 18, 7, 0.84f, 0, 0, 0),
            new TerrainLevelProfile(8, "Loose Stone", 19, 8, 0.82f, 0, 0, 0),
            new TerrainLevelProfile(9, "Heavy Bramble", 20, 9, 0.80f, 0, 0, 0),
            new TerrainLevelProfile(10, "Raised Dirt", 20, 9, 0.80f, 2, 0, 0),
            new TerrainLevelProfile(11, "Low Ridges", 21, 10, 0.78f, 3, 0, 0),
            new TerrainLevelProfile(12, "Stone Shelves", 22, 10, 0.76f, 3, 0, 0),
            new TerrainLevelProfile(13, "Ruin Footings", 23, 11, 0.74f, 4, 0, 0),
            new TerrainLevelProfile(14, "Fighting Ledges", 24, 11, 0.72f, 5, 0, 0),
            new TerrainLevelProfile(15, "Split Ground", 25, 12, 0.70f, 5, 0, 0),
            new TerrainLevelProfile(16, "First Boulders", 25, 12, 0.70f, 5, 4, 0),
            new TerrainLevelProfile(17, "Boulder Field", 26, 13, 0.68f, 5, 6, 0),
            new TerrainLevelProfile(18, "Collapsed Path", 27, 13, 0.66f, 6, 8, 1),
            new TerrainLevelProfile(19, "Ruined Wall", 28, 14, 0.64f, 6, 9, 1),
            new TerrainLevelProfile(20, "Choke Stones", 29, 14, 0.62f, 7, 10, 2),
            new TerrainLevelProfile(21, "Broken Causeway", 30, 15, 0.60f, 7, 11, 2),
            new TerrainLevelProfile(22, "Pillar Garden", 31, 15, 0.58f, 8, 12, 3),
            new TerrainLevelProfile(23, "High Ruins", 32, 16, 0.56f, 8, 13, 3),
            new TerrainLevelProfile(24, "Sunken Arena", 33, 16, 0.54f, 9, 14, 4),
            new TerrainLevelProfile(25, "Bramble Maze", 34, 17, 0.52f, 9, 15, 4),
            new TerrainLevelProfile(26, "Stone Maze", 35, 17, 0.50f, 10, 16, 5),
            new TerrainLevelProfile(27, "Broken Fortress", 36, 18, 0.48f, 10, 18, 5),
            new TerrainLevelProfile(28, "Shattered Pass", 37, 18, 0.46f, 11, 20, 6),
            new TerrainLevelProfile(29, "Obsidian Chokes", 38, 19, 0.44f, 11, 22, 7),
            new TerrainLevelProfile(30, "Ruined Labyrinth", 40, 20, 0.42f, 12, 24, 8)
        };
    }
}
