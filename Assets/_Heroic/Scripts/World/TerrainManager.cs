using Heroic.Player;
using Heroic.Visuals;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Heroic.World
{
    public class TerrainManager : MonoBehaviour
    {
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private Vector2 arenaSize = new Vector2(60f, 60f);
        [SerializeField] private int terrainSeed = 74013;
        [SerializeField] private bool randomizeRunSeed = true;
        [SerializeField] private float playerClearRadius = 4f;
        [SerializeField] private float edgePadding = 4f;
        [SerializeField] private int placementAttemptsPerFeature = 80;
        [SerializeField] private float featureSpacing = 0.75f;

        private readonly List<TerrainFeature> features = new List<TerrainFeature>(128);
        private readonly List<GameObject> featureObjects = new List<GameObject>(128);
        private int runSeed;
        private int currentLevel = -1;
        private bool subscribedToLevelChanges;

        public static TerrainManager Instance { get; private set; }
        public TerrainLevelProfile CurrentProfile { get; private set; }

        private void Awake()
        {
            Instance = this;
            runSeed = terrainSeed + (randomizeRunSeed ? UnityEngine.Random.Range(0, 1000000) : 0);
            ResolveReferences();
        }

        private void OnEnable()
        {
            ResolveReferences();
            SubscribeToLevelChanges();
        }

        private void Start()
        {
            ResolveReferences();
            SubscribeToLevelChanges();
            int level = playerExperience != null ? playerExperience.Level : 1;
            GenerateForLevel(level);
        }

        private void OnDisable()
        {
            if (playerExperience != null && subscribedToLevelChanges)
            {
                playerExperience.LevelChanged -= HandleLevelChanged;
            }

            subscribedToLevelChanges = false;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void GenerateForLevel(int level)
        {
            level = Mathf.Clamp(level, 1, TerrainLevelProfile.Profiles.Length);
            if (currentLevel == level && features.Count > 0)
            {
                return;
            }

            currentLevel = level;
            CurrentProfile = TerrainLevelProfile.ForLevel(level);
            ClearFeatures();

            System.Random random = new System.Random(runSeed + level * 9176);
            AddDecorations(CurrentProfile, random);
            AddSlowPatches(CurrentProfile, random);
            AddHighGround(CurrentProfile, random);
            AddBlockers(CurrentProfile, random);
        }

        public TerrainSample Sample(Vector2 worldPosition)
        {
            float movementMultiplier = 1f;
            float damageMultiplier = 1f;
            float rangeMultiplier = 1f;
            TerrainFeatureType dominantType = TerrainFeatureType.DecorativeRock;
            bool hasFeature = false;

            foreach (TerrainFeature feature in features)
            {
                if (!feature.Contains(worldPosition))
                {
                    continue;
                }

                hasFeature = true;
                dominantType = feature.Type;

                if (feature.BlocksMovement)
                {
                    return new TerrainSample(true, 0f, damageMultiplier, rangeMultiplier, feature.Type, true);
                }

                movementMultiplier = Mathf.Min(movementMultiplier, feature.MovementMultiplier);
                damageMultiplier = Mathf.Max(damageMultiplier, feature.DamageMultiplier);
                rangeMultiplier = Mathf.Max(rangeMultiplier, feature.RangeMultiplier);
            }

            return new TerrainSample(false, movementMultiplier, damageMultiplier, rangeMultiplier, dominantType, hasFeature);
        }

        public float MovementMultiplierAt(Vector2 worldPosition)
        {
            return Sample(worldPosition).MovementMultiplier;
        }

        public bool IsBlockedAt(Vector2 worldPosition, float actorRadius = 0.35f)
        {
            foreach (TerrainFeature feature in features)
            {
                if (feature.BlocksMovement && feature.IntersectsCircle(worldPosition, actorRadius))
                {
                    return true;
                }
            }

            return false;
        }

        public Vector2 FindNearestOpenPoint(Vector2 origin, Vector2 desired, float actorRadius, int steps = 12)
        {
            if (!IsBlockedAt(desired, actorRadius))
            {
                return desired;
            }

            Vector2 direction = desired - origin;
            for (int i = steps - 1; i >= 0; i--)
            {
                float percent = i / (float)steps;
                Vector2 candidate = origin + direction * percent;
                if (!IsBlockedAt(candidate, actorRadius))
                {
                    return candidate;
                }
            }

            return origin;
        }

        public string GetCurrentTerrainBonusSummary(Vector2 worldPosition)
        {
            TerrainSample sample = Sample(worldPosition);
            if (!sample.HasFeature || sample.Blocked)
            {
                return string.Empty;
            }

            if (sample.DamageMultiplier <= 1.01f && sample.RangeMultiplier <= 1.01f && sample.MovementMultiplier >= 0.99f)
            {
                return string.Empty;
            }

            return $"{FormatFeatureName(sample.Type)}: Move x{sample.MovementMultiplier:0.00}, Damage x{sample.DamageMultiplier:0.00}, Range x{sample.RangeMultiplier:0.00}";
        }

        private void ResolveReferences()
        {
            if (playerExperience == null)
            {
                playerExperience = FindAnyObjectByType<PlayerExperience>();
            }

            if (playerTarget == null && playerExperience != null)
            {
                playerTarget = playerExperience.transform;
            }
        }

        private void HandleLevelChanged(int level)
        {
            GenerateForLevel(level);
        }

        private void SubscribeToLevelChanges()
        {
            if (playerExperience == null || subscribedToLevelChanges)
            {
                return;
            }

            playerExperience.LevelChanged += HandleLevelChanged;
            subscribedToLevelChanges = true;
        }

        private void AddDecorations(TerrainLevelProfile profile, System.Random random)
        {
            for (int i = 0; i < profile.DecorativeCount; i++)
            {
                Vector2 size = RandomSize(random, 0.35f, 0.9f);
                TryPlaceFeature(random, TerrainFeatureType.DecorativeRock, size, 1f, false, 1f, 1f);
            }
        }

        private void AddSlowPatches(TerrainLevelProfile profile, System.Random random)
        {
            for (int i = 0; i < profile.SlowPatchCount; i++)
            {
                TerrainFeatureType type = SlowTypeForLevel(profile.Level, random);
                Vector2 range = profile.SlowPatchSizeRange;
                Vector2 size = RandomSize(random, range.x, range.y);
                float multiplier = Mathf.Clamp(profile.SlowMultiplier + RandomRange(random, -0.04f, 0.03f), 0.35f, 0.98f);
                TryPlaceFeature(random, type, size, multiplier, false, 1f, 1f);
            }
        }

        private void AddHighGround(TerrainLevelProfile profile, System.Random random)
        {
            for (int i = 0; i < profile.HighGroundCount; i++)
            {
                Vector2 range = profile.HighGroundSizeRange;
                Vector2 size = RandomSize(random, range.x, range.y);
                TerrainFeatureType type = profile.Level >= 18 && random.NextDouble() > 0.45 ? TerrainFeatureType.RuinedPlatform : TerrainFeatureType.HighGround;
                TryPlaceFeature(random, type, size, 0.95f, false, profile.HighGroundDamageMultiplier, profile.HighGroundRangeMultiplier);
            }
        }

        private void AddBlockers(TerrainLevelProfile profile, System.Random random)
        {
            Vector2 range = profile.BlockerSizeRange;
            for (int i = 0; i < profile.BlockerCount; i++)
            {
                TerrainFeatureType type = BlockerTypeForLevel(profile.Level, random);
                Vector2 size = RandomSize(random, range.x, range.y);
                TryPlaceFeature(random, type, size, 0f, true, 1f, 1f);
            }

            for (int i = 0; i < profile.LargeBlockerCount; i++)
            {
                TerrainFeatureType type = profile.Level >= 22 && random.NextDouble() > 0.55 ? TerrainFeatureType.Wall : TerrainFeatureType.Boulder;
                Vector2 size = RandomSize(random, range.y * 1.35f, range.y * 2.25f);
                TryPlaceFeature(random, type, size, 0f, true, 1f, 1f);
            }
        }

        private bool TryPlaceFeature(
            System.Random random,
            TerrainFeatureType type,
            Vector2 size,
            float movementMultiplier,
            bool blocksMovement,
            float damageMultiplier,
            float rangeMultiplier)
        {
            for (int attempt = 0; attempt < placementAttemptsPerFeature; attempt++)
            {
                Vector2 position = RandomPoint(random);
                if (!IsPlacementAllowed(position, size, blocksMovement))
                {
                    continue;
                }

                float rotation = RandomRange(random, 0f, 360f);
                TerrainFeature feature = new TerrainFeature(type, position, size, rotation, movementMultiplier, blocksMovement, damageMultiplier, rangeMultiplier);
                features.Add(feature);
                featureObjects.Add(CreateFeatureView(feature));
                return true;
            }

            return false;
        }

        private bool IsPlacementAllowed(Vector2 position, Vector2 size, bool blocksMovement)
        {
            if (playerTarget != null && Vector2.Distance(position, playerTarget.position) < playerClearRadius + size.magnitude * 0.25f)
            {
                return false;
            }

            float halfWidth = arenaSize.x * 0.5f - edgePadding;
            float halfHeight = arenaSize.y * 0.5f - edgePadding;
            if (Mathf.Abs(position.x) > halfWidth || Mathf.Abs(position.y) > halfHeight)
            {
                return false;
            }

            if (!blocksMovement)
            {
                return true;
            }

            foreach (TerrainFeature feature in features)
            {
                if (feature.BlocksMovement && Vector2.Distance(position, feature.Position) < (size.magnitude + feature.Size.magnitude) * 0.25f + featureSpacing)
                {
                    return false;
                }
            }

            return true;
        }

        private GameObject CreateFeatureView(TerrainFeature feature)
        {
            GameObject view = new GameObject("Terrain_" + feature.Type);
            view.transform.SetParent(transform);
            view.transform.position = new Vector3(feature.Position.x, feature.Position.y, 0.02f);
            view.transform.rotation = Quaternion.Euler(0f, 0f, feature.Rotation);
            view.transform.localScale = new Vector3(feature.Size.x, feature.Size.y, 1f);

            SpriteRenderer renderer = view.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteForFeature(feature.Type);
            renderer.color = ColorForFeature(feature.Type);
            renderer.sortingOrder = SortingOrderForFeature(feature.Type);
            return view;
        }

        private void ClearFeatures()
        {
            features.Clear();
            foreach (GameObject featureObject in featureObjects)
            {
                if (featureObject != null)
                {
                    Destroy(featureObject);
                }
            }

            featureObjects.Clear();
        }

        private Vector2 RandomPoint(System.Random random)
        {
            float halfWidth = arenaSize.x * 0.5f - edgePadding;
            float halfHeight = arenaSize.y * 0.5f - edgePadding;
            return new Vector2(RandomRange(random, -halfWidth, halfWidth), RandomRange(random, -halfHeight, halfHeight));
        }

        private static Vector2 RandomSize(System.Random random, float min, float max)
        {
            float major = RandomRange(random, min, max);
            float minor = RandomRange(random, min * 0.65f, major);
            return random.NextDouble() > 0.5 ? new Vector2(major, minor) : new Vector2(minor, major);
        }

        private static TerrainFeatureType SlowTypeForLevel(int level, System.Random random)
        {
            if (level >= 20 && random.NextDouble() > 0.65)
            {
                return TerrainFeatureType.LooseStone;
            }

            if (level >= 12 && random.NextDouble() > 0.6)
            {
                return TerrainFeatureType.ShallowWater;
            }

            if (level >= 5 && random.NextDouble() > 0.45)
            {
                return TerrainFeatureType.Mud;
            }

            return random.NextDouble() > 0.5 ? TerrainFeatureType.Brush : TerrainFeatureType.RoughDirt;
        }

        private static TerrainFeatureType BlockerTypeForLevel(int level, System.Random random)
        {
            if (level >= 24 && random.NextDouble() > 0.65)
            {
                return TerrainFeatureType.Wall;
            }

            if (level >= 21 && random.NextDouble() > 0.55)
            {
                return TerrainFeatureType.Pillar;
            }

            return TerrainFeatureType.Boulder;
        }

        private static Sprite SpriteForFeature(TerrainFeatureType type)
        {
            switch (type)
            {
                case TerrainFeatureType.DecorativeRock:
                case TerrainFeatureType.Boulder:
                case TerrainFeatureType.Pillar:
                    return ProceduralSpriteFactory.GetCircle(type.ToString(), Color.white, 64, 0.05f);
                case TerrainFeatureType.HighGround:
                case TerrainFeatureType.RuinedPlatform:
                    return ProceduralSpriteFactory.GetDiamond(type.ToString(), Color.white, 64);
                default:
                    return ProceduralSpriteFactory.GetCircle(type.ToString(), Color.white, 64, 0.18f);
            }
        }

        private static Color ColorForFeature(TerrainFeatureType type)
        {
            switch (type)
            {
                case TerrainFeatureType.DecorativeRock:
                    return new Color(0.25f, 0.22f, 0.18f, 0.42f);
                case TerrainFeatureType.RoughDirt:
                    return new Color(0.25f, 0.15f, 0.08f, 0.34f);
                case TerrainFeatureType.Brush:
                    return new Color(0.18f, 0.27f, 0.10f, 0.40f);
                case TerrainFeatureType.Mud:
                    return new Color(0.14f, 0.09f, 0.055f, 0.54f);
                case TerrainFeatureType.ShallowWater:
                    return new Color(0.12f, 0.25f, 0.30f, 0.46f);
                case TerrainFeatureType.LooseStone:
                    return new Color(0.34f, 0.32f, 0.28f, 0.46f);
                case TerrainFeatureType.HighGround:
                    return new Color(0.57f, 0.42f, 0.24f, 0.64f);
                case TerrainFeatureType.RuinedPlatform:
                    return new Color(0.48f, 0.46f, 0.40f, 0.72f);
                case TerrainFeatureType.Boulder:
                    return new Color(0.20f, 0.19f, 0.17f, 0.95f);
                case TerrainFeatureType.Wall:
                    return new Color(0.16f, 0.15f, 0.14f, 0.98f);
                case TerrainFeatureType.Pillar:
                    return new Color(0.24f, 0.23f, 0.21f, 0.98f);
                default:
                    return Color.white;
            }
        }

        private static int SortingOrderForFeature(TerrainFeatureType type)
        {
            switch (type)
            {
                case TerrainFeatureType.Boulder:
                case TerrainFeatureType.Wall:
                case TerrainFeatureType.Pillar:
                    return -8;
                case TerrainFeatureType.HighGround:
                case TerrainFeatureType.RuinedPlatform:
                    return -18;
                default:
                    return -40;
            }
        }

        private static string FormatFeatureName(TerrainFeatureType type)
        {
            string text = type.ToString();
            for (int i = text.Length - 1; i > 0; i--)
            {
                if (char.IsUpper(text[i]) && !char.IsWhiteSpace(text[i - 1]))
                {
                    text = text.Insert(i, " ");
                }
            }

            return text;
        }

        private static float RandomRange(System.Random random, float min, float max)
        {
            return Mathf.Lerp(min, max, (float)random.NextDouble());
        }

        private readonly struct TerrainFeature
        {
            public TerrainFeature(
                TerrainFeatureType type,
                Vector2 position,
                Vector2 size,
                float rotation,
                float movementMultiplier,
                bool blocksMovement,
                float damageMultiplier,
                float rangeMultiplier)
            {
                Type = type;
                Position = position;
                Size = size;
                Rotation = rotation;
                MovementMultiplier = movementMultiplier;
                BlocksMovement = blocksMovement;
                DamageMultiplier = damageMultiplier;
                RangeMultiplier = rangeMultiplier;
            }

            public TerrainFeatureType Type { get; }
            public Vector2 Position { get; }
            public Vector2 Size { get; }
            public float Rotation { get; }
            public float MovementMultiplier { get; }
            public bool BlocksMovement { get; }
            public float DamageMultiplier { get; }
            public float RangeMultiplier { get; }

            public bool Contains(Vector2 point)
            {
                Vector2 local = Rotate(point - Position, -Rotation);
                float halfX = Mathf.Max(0.01f, Size.x * 0.5f);
                float halfY = Mathf.Max(0.01f, Size.y * 0.5f);
                float normalized = (local.x * local.x) / (halfX * halfX) + (local.y * local.y) / (halfY * halfY);
                return normalized <= 1f;
            }

            public bool IntersectsCircle(Vector2 point, float radius)
            {
                Vector2 expandedSize = Size + Vector2.one * Mathf.Max(0f, radius * 2f);
                Vector2 local = Rotate(point - Position, -Rotation);
                float halfX = Mathf.Max(0.01f, expandedSize.x * 0.5f);
                float halfY = Mathf.Max(0.01f, expandedSize.y * 0.5f);
                float normalized = (local.x * local.x) / (halfX * halfX) + (local.y * local.y) / (halfY * halfY);
                return normalized <= 1f;
            }

            private static Vector2 Rotate(Vector2 point, float degrees)
            {
                float radians = degrees * Mathf.Deg2Rad;
                float sin = Mathf.Sin(radians);
                float cos = Mathf.Cos(radians);
                return new Vector2(point.x * cos - point.y * sin, point.x * sin + point.y * cos);
            }
        }
    }

    public readonly struct TerrainSample
    {
        public TerrainSample(
            bool blocked,
            float movementMultiplier,
            float damageMultiplier,
            float rangeMultiplier,
            TerrainFeatureType type,
            bool hasFeature)
        {
            Blocked = blocked;
            MovementMultiplier = movementMultiplier;
            DamageMultiplier = damageMultiplier;
            RangeMultiplier = rangeMultiplier;
            Type = type;
            HasFeature = hasFeature;
        }

        public bool Blocked { get; }
        public float MovementMultiplier { get; }
        public float DamageMultiplier { get; }
        public float RangeMultiplier { get; }
        public TerrainFeatureType Type { get; }
        public bool HasFeature { get; }
    }
}
