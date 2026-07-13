using System.Collections.Generic;
using Heroic.Player;
using UnityEngine;

namespace Heroic.World
{
    public class DynamicTerrainGrid : MonoBehaviour
    {
        private enum TileRole
        {
            Base,
            Decorative,
            Slow,
            HighGround,
            Blocker,
            LargeBlocker
        }

        [SerializeField] private Texture2D[] terrainSheets = new Texture2D[0];
        [SerializeField] private Texture2D[] dirtTextures = new Texture2D[0];
        [SerializeField] private Texture2D[] decorativeTextures = new Texture2D[0];
        [SerializeField] private Texture2D[] mudTextures = new Texture2D[0];
        [SerializeField] private Texture2D[] waterTextures = new Texture2D[0];
        [SerializeField] private Texture2D[] looseStoneTextures = new Texture2D[0];
        [SerializeField] private Texture2D[] highGroundTextures = new Texture2D[0];
        [SerializeField] private Texture2D[] blockerTextures = new Texture2D[0];
        [SerializeField] private Transform playerReference;
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private Vector2 worldSize = new Vector2(60f, 60f);
        [SerializeField] private float tileWorldSize = 4f;
        [SerializeField] private float playerSafeRadius = 7f;
        [SerializeField] private float blockerSpacing = 5f;
        [SerializeField] private int runSeed = 73129;
        [SerializeField] private int terrainLayer = 8;
        [SerializeField] private int baseSortingOrder = -99;
        [SerializeField] private int featureSortingOrder = -96;
        [SerializeField] private int blockerSortingOrder = -94;
        [SerializeField] private float baseTileOverlap = 0.28f;

        private readonly List<Sprite> sheetSprites = new();
        private readonly List<Sprite> baseSprites = new();
        private readonly List<Sprite> decorativeSprites = new();
        private readonly List<Sprite> slowSprites = new();
        private readonly List<Sprite> highGroundSprites = new();
        private readonly List<Sprite> blockerSprites = new();
        private readonly List<Vector2> hardBlockerCenters = new();
        private System.Random random;

        private void Awake()
        {
            if (playerExperience == null)
            {
                playerExperience = FindAnyObjectByType<PlayerExperience>();
            }

            if (playerReference == null && playerExperience != null)
            {
                playerReference = playerExperience.transform;
            }
        }

        private void OnEnable()
        {
            if (playerExperience != null)
            {
                playerExperience.LevelChanged += HandleLevelChanged;
            }
        }

        private void OnDisable()
        {
            if (playerExperience != null)
            {
                playerExperience.LevelChanged -= HandleLevelChanged;
            }
        }

        private void Start()
        {
            GenerateForCurrentLevel();
        }

        public bool IsSpawnLocationBlocked(Vector2 position, float radius)
        {
            int layerMask = 1 << terrainLayer;
            return Physics2D.OverlapCircle(position, Mathf.Max(0.1f, radius), layerMask) != null;
        }

        public void GenerateForCurrentLevel()
        {
            int level = playerExperience != null ? playerExperience.Level : 1;
            Generate(TerrainLevelProfile.ForLevel(level));
        }

        private void HandleLevelChanged(int level)
        {
            Generate(TerrainLevelProfile.ForLevel(level));
        }

        private void Generate(TerrainLevelProfile profile)
        {
            ClearChildren();
            sheetSprites.Clear();
            baseSprites.Clear();
            decorativeSprites.Clear();
            slowSprites.Clear();
            highGroundSprites.Clear();
            blockerSprites.Clear();
            hardBlockerCenters.Clear();
            random = new System.Random(runSeed + profile.Level * 4099);

            if (profile.Level <= 9)
            {
                return;
            }

            BuildTypedSpriteCache();
            if (sheetSprites.Count == 0)
            {
                BuildSpriteCache(SelectTerrainSheet(profile.Level), profile.Level);
            }

            if (sheetSprites.Count == 0)
            {
                return;
            }

            int columns = Mathf.CeilToInt(worldSize.x / tileWorldSize);
            int rows = Mathf.CeilToInt(worldSize.y / tileWorldSize);
            Vector2 origin = new Vector2(-columns * tileWorldSize * 0.5f + tileWorldSize * 0.5f, -rows * tileWorldSize * 0.5f + tileWorldSize * 0.5f);
            HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Vector2 cellCenter = origin + new Vector2(x * tileWorldSize, y * tileWorldSize);
                    CreateTile($"Ground_{x:00}_{y:00}", cellCenter, TileRole.Base, Vector2Int.one, false);
                }
            }

            PlaceFeatures(profile.DecorativeCount, TileRole.Decorative, columns, rows, origin, occupied, Vector2Int.one, false);
            PlaceFeatures(profile.SlowPatchCount, TileRole.Slow, columns, rows, origin, occupied, Vector2Int.one, false);
            PlaceFeatures(profile.HighGroundCount, TileRole.HighGround, columns, rows, origin, occupied, Vector2Int.one, false);

            if (profile.HasHardBlockers)
            {
                PlaceFeatures(profile.LargeBlockerCount, TileRole.LargeBlocker, columns, rows, origin, occupied, new Vector2Int(2, 2), true);
                PlaceFeatures(profile.BlockerCount, TileRole.Blocker, columns, rows, origin, occupied, Vector2Int.one, true);
            }
        }

        private Texture2D SelectTerrainSheet(int level)
        {
            if (terrainSheets == null || terrainSheets.Length == 0)
            {
                return null;
            }

            int band = Mathf.Clamp((Mathf.Clamp(level, 1, 30) - 1) / 6, 0, terrainSheets.Length - 1);
            return terrainSheets[band] != null ? terrainSheets[band] : terrainSheets[0];
        }

        private void BuildTypedSpriteCache()
        {
            AddSpritesFromTextures(baseSprites, dirtTextures);
            AddSpritesFromTextures(decorativeSprites, decorativeTextures);
            AddSpritesFromTextures(slowSprites, mudTextures);
            AddSpritesFromTextures(slowSprites, waterTextures);
            AddSpritesFromTextures(slowSprites, looseStoneTextures);
            AddSpritesFromTextures(highGroundSprites, highGroundTextures);
            AddSpritesFromTextures(blockerSprites, blockerTextures);

            sheetSprites.AddRange(baseSprites);
            sheetSprites.AddRange(decorativeSprites);
            sheetSprites.AddRange(slowSprites);
            sheetSprites.AddRange(highGroundSprites);
            sheetSprites.AddRange(blockerSprites);
        }

        private void AddSpritesFromTextures(List<Sprite> target, Texture2D[] textures)
        {
            if (textures == null)
            {
                return;
            }

            foreach (Texture2D texture in textures)
            {
                if (texture == null)
                {
                    continue;
                }

                target.Add(CreateTerrainSprite(texture, new Rect(0f, 0f, texture.width, texture.height), Mathf.Max(texture.width, texture.height)));
            }
        }

        private void BuildSpriteCache(Texture2D texture, int level)
        {
            if (texture == null)
            {
                return;
            }

            if (texture.width == 1024 && texture.height == 1536)
            {
                if (level <= 1)
                {
                    BuildLevelOneDirtOnlyCache(texture);
                }
                else
                {
                    BuildTerrainOneSpriteCache(texture);
                }

                return;
            }

            int columns = 3;
            int cellSize = texture.width / columns;
            int rows = Mathf.Max(1, Mathf.RoundToInt(texture.height / (float)cellSize));
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    int y = texture.height - (row + 1) * cellSize;
                    if (y < 0)
                    {
                        continue;
                    }

                    Rect rect = new Rect(column * cellSize, y, cellSize, cellSize);
                    Sprite sprite = CreateTerrainSprite(texture, rect, cellSize);
                    RegisterGenericSprite(row, column, sprite);
                }
            }
        }

        private void BuildLevelOneDirtOnlyCache(Texture2D texture)
        {
            const int cropSize = 256;
            const int pixelsPerUnit = 256;
            const int dirtX = 341;
            const int dirtY = 1195;

            Add(
                baseSprites,
                CreateTerrainSprite(texture, new Rect(dirtX + 26, dirtY + 28, cropSize, cropSize), pixelsPerUnit),
                CreateTerrainSprite(texture, new Rect(dirtX + 52, dirtY + 52, cropSize, cropSize), pixelsPerUnit),
                CreateTerrainSprite(texture, new Rect(dirtX + 18, dirtY + 76, cropSize, cropSize), pixelsPerUnit),
                CreateTerrainSprite(texture, new Rect(dirtX + 70, dirtY + 20, cropSize, cropSize), pixelsPerUnit));

            sheetSprites.AddRange(baseSprites);
        }

        private void BuildTerrainOneSpriteCache(Texture2D texture)
        {
            const int cellSize = 341;

            Sprite rockGrass = CreateTerrainSprite(texture, new Rect(0, 1195, cellSize, cellSize), cellSize);
            Sprite packedDirt = CreateTerrainSprite(texture, new Rect(341, 1195, cellSize, cellSize), cellSize);
            Sprite brush = CreateTerrainSprite(texture, new Rect(682, 1195, cellSize, cellSize), cellSize);
            Sprite mud = CreateTerrainSprite(texture, new Rect(0, 854, cellSize, cellSize), cellSize);
            Sprite water = CreateTerrainSprite(texture, new Rect(341, 854, cellSize, cellSize), cellSize);
            Sprite looseStone = CreateTerrainSprite(texture, new Rect(682, 854, cellSize, cellSize), cellSize);
            Sprite raisedDirt = CreateTerrainSprite(texture, new Rect(0, 513, cellSize, cellSize), cellSize);
            Sprite stoneFloor = CreateTerrainSprite(texture, new Rect(341, 513, cellSize, cellSize), cellSize);
            Sprite boulder = CreateTerrainSprite(texture, new Rect(682, 513, cellSize, cellSize), cellSize);
            Sprite wallLeft = CreateTerrainSprite(texture, new Rect(0, 172, cellSize, cellSize), cellSize);
            Sprite pillar = CreateTerrainSprite(texture, new Rect(341, 172, cellSize, cellSize), cellSize);
            Sprite wallRight = CreateTerrainSprite(texture, new Rect(682, 172, cellSize, cellSize), cellSize);
            Sprite roughDirtLeft = CreateTerrainSprite(texture, new Rect(0, 0, 512, 172), cellSize);
            Sprite roughDirtRight = CreateTerrainSprite(texture, new Rect(512, 0, 512, 172), cellSize);

            Add(
                sheetSprites,
                rockGrass,
                packedDirt,
                brush,
                mud,
                water,
                looseStone,
                raisedDirt,
                stoneFloor,
                boulder,
                wallLeft,
                pillar,
                wallRight,
                roughDirtLeft,
                roughDirtRight);
            Add(baseSprites, packedDirt, roughDirtLeft, roughDirtRight);
            Add(decorativeSprites, rockGrass, brush, raisedDirt);
            Add(slowSprites, mud, brush, water);
            Add(highGroundSprites, raisedDirt, stoneFloor);
            Add(blockerSprites, looseStone, boulder, wallLeft, pillar, wallRight);
        }

        private void RegisterGenericSprite(int row, int column, Sprite sprite)
        {
            sheetSprites.Add(sprite);

            int index = row * 3 + column;
            if (index == 1 || index >= 12)
            {
                baseSprites.Add(sprite);
            }
            else if (index == 2 || index == 3 || index == 4)
            {
                slowSprites.Add(sprite);
            }
            else if (index == 6 || index == 7)
            {
                highGroundSprites.Add(sprite);
            }
            else if (index == 8 || index == 9 || index == 10 || index == 11)
            {
                blockerSprites.Add(sprite);
            }
            else
            {
                decorativeSprites.Add(sprite);
            }
        }

        private Sprite CreateTerrainSprite(Texture2D texture, Rect rect, float pixelsPerUnit)
        {
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }

        private static void Add(List<Sprite> target, params Sprite[] sprites)
        {
            foreach (Sprite sprite in sprites)
            {
                if (sprite != null)
                {
                    target.Add(sprite);
                }
            }
        }

        private void PlaceFeatures(int count, TileRole role, int columns, int rows, Vector2 origin, HashSet<Vector2Int> occupied, Vector2Int footprint, bool hardBlocker)
        {
            int placed = 0;
            int attempts = 0;
            int maxAttempts = Mathf.Max(120, count * 40);
            while (placed < count && attempts < maxAttempts)
            {
                attempts++;
                int x = random.Next(0, Mathf.Max(1, columns - footprint.x + 1));
                int y = random.Next(0, Mathf.Max(1, rows - footprint.y + 1));
                Vector2Int cell = new Vector2Int(x, y);
                if (!CanPlace(cell, footprint, columns, rows, origin, occupied, hardBlocker))
                {
                    continue;
                }

                Vector2 center = origin + new Vector2((x + (footprint.x - 1) * 0.5f) * tileWorldSize, (y + (footprint.y - 1) * 0.5f) * tileWorldSize);
                GameObject tile = CreateTile($"{role}_{placed:00}", center, role, footprint, hardBlocker);
                if (hardBlocker)
                {
                    BoxCollider2D collider = tile.AddComponent<BoxCollider2D>();
                    collider.size = Vector2.one * tileWorldSize * 0.86f;
                    if (footprint.x > 1 || footprint.y > 1)
                    {
                        collider.size = new Vector2(tileWorldSize * footprint.x * 0.82f, tileWorldSize * footprint.y * 0.82f);
                    }

                    hardBlockerCenters.Add(center);
                }

                for (int fy = 0; fy < footprint.y; fy++)
                {
                    for (int fx = 0; fx < footprint.x; fx++)
                    {
                        occupied.Add(new Vector2Int(x + fx, y + fy));
                    }
                }

                placed++;
            }
        }

        private bool CanPlace(Vector2Int cell, Vector2Int footprint, int columns, int rows, Vector2 origin, HashSet<Vector2Int> occupied, bool hardBlocker)
        {
            if (cell.x < 0 || cell.y < 0 || cell.x + footprint.x > columns || cell.y + footprint.y > rows)
            {
                return false;
            }

            Vector2 center = origin + new Vector2((cell.x + (footprint.x - 1) * 0.5f) * tileWorldSize, (cell.y + (footprint.y - 1) * 0.5f) * tileWorldSize);
            if (playerReference != null && Vector2.Distance(center, playerReference.position) < playerSafeRadius)
            {
                return false;
            }

            for (int y = 0; y < footprint.y; y++)
            {
                for (int x = 0; x < footprint.x; x++)
                {
                    if (occupied.Contains(new Vector2Int(cell.x + x, cell.y + y)))
                    {
                        return false;
                    }
                }
            }

            if (!hardBlocker)
            {
                return true;
            }

            foreach (Vector2 blockerCenter in hardBlockerCenters)
            {
                if (Vector2.Distance(center, blockerCenter) < blockerSpacing)
                {
                    return false;
                }
            }

            return true;
        }

        private GameObject CreateTile(string tileName, Vector2 position, TileRole role, Vector2Int footprint, bool hardBlocker)
        {
            GameObject tile = new GameObject(tileName);
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = position;
            tile.layer = hardBlocker ? terrainLayer : gameObject.layer;

            SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
            renderer.sprite = PickSprite(role);
            renderer.sortingOrder = hardBlocker ? blockerSortingOrder : role == TileRole.Base ? baseSortingOrder : featureSortingOrder;
            renderer.color = GetRoleTint(role);
            FitRendererToFootprint(renderer, role, footprint);

            int quarterTurns = random.Next(0, 4);
            tile.transform.localRotation = Quaternion.Euler(0f, 0f, quarterTurns * 90f);
            if (random.NextDouble() > 0.5)
            {
                Vector3 scale = tile.transform.localScale;
                scale.x *= -1f;
                tile.transform.localScale = scale;
            }

            return tile;
        }

        private Sprite PickSprite(TileRole role)
        {
            List<Sprite> candidates = role switch
            {
                TileRole.Decorative => decorativeSprites,
                TileRole.Slow => slowSprites,
                TileRole.HighGround => highGroundSprites,
                TileRole.Blocker => blockerSprites,
                TileRole.LargeBlocker => blockerSprites,
                _ => baseSprites,
            };

            if (candidates.Count == 0)
            {
                candidates.AddRange(sheetSprites);
            }

            return candidates[random.Next(0, candidates.Count)];
        }

        private static Color GetRoleTint(TileRole role)
        {
            return role switch
            {
                TileRole.Slow => new Color(0.82f, 0.94f, 1f, 0.88f),
                TileRole.HighGround => new Color(1f, 0.96f, 0.78f, 0.94f),
                TileRole.Decorative => new Color(1f, 1f, 1f, 0.9f),
                _ => Color.white,
            };
        }

        private void FitRendererToFootprint(SpriteRenderer renderer, TileRole role, Vector2Int footprint)
        {
            if (renderer.sprite == null)
            {
                return;
            }

            Vector2 spriteSize = renderer.sprite.bounds.size;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            {
                return;
            }

            float overlap = role == TileRole.Base ? Mathf.Max(0f, baseTileOverlap) : 0f;
            float targetWidth = tileWorldSize * footprint.x + overlap;
            float targetHeight = tileWorldSize * footprint.y + overlap;
            renderer.transform.localScale = new Vector3(targetWidth / spriteSize.x, targetHeight / spriteSize.y, 1f);
        }

        private void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
