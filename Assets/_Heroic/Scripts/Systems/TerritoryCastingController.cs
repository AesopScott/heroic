using Heroic.Visuals;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Heroic.Systems
{
    public class TerritoryCastingController : MonoBehaviour
    {
        private enum TerritoryKind
        {
            Damage,
            Range,
            Recovery,
            Confluence
        }

        private class TerritoryZone
        {
            public TerritoryKind Kind;
            public Vector2 Position;
            public float Radius;
            public float NextSpawnAt;
            public float ExpiresAt;
            public GameObject Visual;

            public bool IsActive => Visual != null;
        }

        [SerializeField] private float zoneRadius = 2.45f;
        [SerializeField] private float zoneLifetime = 10f;
        [SerializeField] private float initialSpawnSpacing = 2f;
        [SerializeField] private int activeZoneCount = 6;
        [SerializeField] private int maximumActiveZoneCount = 12;
        [SerializeField] private float screenEdgePadding = 1f;
        [SerializeField] private float minimumDistanceBetweenZones = 2.8f;
        [SerializeField] private float damageBoost = 1.35f;
        [SerializeField] private float rangeBoost = 1.35f;
        [SerializeField] private float recoveryBoost = 1.35f;
        [SerializeField] private float confluenceBoost = 1.2f;

        private readonly List<TerritoryZone> zones = new List<TerritoryZone>();
        private SpellStatModifier spellStats;
        private bool enabledTerritory;
        private float activeDamageMultiplier = 1f;
        private float activeRangeMultiplier = 1f;
        private float activeRecoveryMultiplier = 1f;

        public bool HasActiveTerritoryBonus => enabledTerritory && (activeDamageMultiplier > 1.01f || activeRangeMultiplier > 1.01f || activeRecoveryMultiplier > 1.01f);
        public string ActiveBonusSummary
        {
            get
            {
                if (!HasActiveTerritoryBonus)
                {
                    return "- None";
                }

                string summary = string.Empty;
                AppendBonus(ref summary, "Territory Damage", activeDamageMultiplier);
                AppendBonus(ref summary, "Territory Range", activeRangeMultiplier);
                AppendBonus(ref summary, "Territory Recovery", activeRecoveryMultiplier);
                return summary;
            }
        }

        private void Awake()
        {
            spellStats = GetComponent<SpellStatModifier>();
        }

        private void Update()
        {
            if (!enabledTerritory || spellStats == null)
            {
                return;
            }

            RemoveExpiredZones();
            FillActiveZones();
            ApplyCurrentTerritory();
        }

        public void EnableTerritoryCasting()
        {
            if (enabledTerritory)
            {
                return;
            }

            enabledTerritory = true;
            EnsureZoneSlots();
            ApplyCurrentTerritory();
        }

        public void SetActiveZoneCount(int count)
        {
            activeZoneCount = Mathf.Clamp(count, 6, maximumActiveZoneCount);
            if (enabledTerritory)
            {
                EnsureZoneSlots();
            }
        }

        public void SetZoneRadius(float radius)
        {
            zoneRadius = Mathf.Max(0.5f, radius);
            for (int i = 0; i < zones.Count; i++)
            {
                zones[i].Radius = zoneRadius;
            }
        }

        public void SetBoostMultipliers(float damage, float range, float recovery, float confluence)
        {
            damageBoost = Mathf.Max(1f, damage);
            rangeBoost = Mathf.Max(1f, range);
            recoveryBoost = Mathf.Max(1f, recovery);
            confluenceBoost = Mathf.Max(1f, confluence);
            ApplyCurrentTerritory();
        }

        private void RemoveExpiredZones()
        {
            for (int i = 0; i < zones.Count; i++)
            {
                TerritoryZone zone = zones[i];
                if (!zone.IsActive || Time.time < zone.ExpiresAt)
                {
                    continue;
                }

                Destroy(zone.Visual);
                zone.Visual = null;
                zone.NextSpawnAt = Time.time;
            }
        }

        private void FillActiveZones()
        {
            EnsureZoneSlots();
            for (int i = 0; i < zones.Count; i++)
            {
                TerritoryZone zone = zones[i];
                if (!zone.IsActive && Time.time >= zone.NextSpawnAt)
                {
                    SpawnZone(zone);
                }
            }
        }

        private void EnsureZoneSlots()
        {
            int desiredCount = Mathf.Clamp(activeZoneCount, 6, maximumActiveZoneCount);
            int startingCount = zones.Count;
            while (zones.Count < desiredCount)
            {
                int index = zones.Count;
                float spawnDelay = (index - startingCount) * initialSpawnSpacing;
                zones.Add(new TerritoryZone
                {
                    Kind = KindForSlot(index),
                    Radius = zoneRadius,
                    NextSpawnAt = Time.time + spawnDelay
                });
            }
        }

        private void SpawnZone(TerritoryZone zone)
        {
            zone.Position = PickRandomScreenPosition();
            zone.Radius = zoneRadius;
            zone.ExpiresAt = Time.time + zoneLifetime;
            zone.Visual = CreateZoneVisual(zone.Kind, zone.Position);
        }

        private Vector2 PickRandomScreenPosition()
        {
            for (int attempt = 0; attempt < 24; attempt++)
            {
                Vector2 candidate = RandomScreenPoint();
                if (IsFarEnoughFromZones(candidate))
                {
                    return candidate;
                }
            }

            return RandomScreenPoint();
        }

        private Vector2 RandomScreenPoint()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                camera = FindAnyObjectByType<Camera>();
            }

            if (camera == null)
            {
                return (Vector2)transform.position + Random.insideUnitCircle * 8f;
            }

            float depth = Mathf.Abs(camera.transform.position.z - transform.position.z);
            Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
            Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, depth));
            float padding = Mathf.Max(screenEdgePadding, zoneRadius);
            float minX = Mathf.Min(bottomLeft.x, topRight.x) + padding;
            float maxX = Mathf.Max(bottomLeft.x, topRight.x) - padding;
            float minY = Mathf.Min(bottomLeft.y, topRight.y) + padding;
            float maxY = Mathf.Max(bottomLeft.y, topRight.y) - padding;

            if (minX > maxX)
            {
                minX = maxX = transform.position.x;
            }

            if (minY > maxY)
            {
                minY = maxY = transform.position.y;
            }

            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }

        private bool IsFarEnoughFromZones(Vector2 candidate)
        {
            for (int i = 0; i < zones.Count; i++)
            {
                if (zones[i].IsActive && Vector2.Distance(candidate, zones[i].Position) < minimumDistanceBetweenZones)
                {
                    return false;
                }
            }

            return true;
        }

        private static TerritoryKind KindForSlot(int index)
        {
            switch (index % 6)
            {
                case 0:
                    return TerritoryKind.Damage;
                case 1:
                    return TerritoryKind.Range;
                case 2:
                    return TerritoryKind.Recovery;
                case 3:
                    return TerritoryKind.Confluence;
                case 4:
                    return TerritoryKind.Damage;
                default:
                    return TerritoryKind.Range;
            }
        }

        private GameObject CreateZoneVisual(TerritoryKind kind, Vector2 position)
        {
            Color color = ColorFor(kind);
            GameObject root = new GameObject("Territory_" + kind);
            root.transform.position = position;

            GameObject fillObject = new GameObject("Fill");
            fillObject.transform.SetParent(root.transform, false);
            SpriteRenderer fill = fillObject.AddComponent<SpriteRenderer>();
            Color fillColor = color;
            fillColor.a = 0.14f;
            fill.sprite = ProceduralSpriteFactory.GetCircle("territory_" + kind + "_fill", fillColor, 128, 0.1f);
            fill.sortingOrder = -18;
            fillObject.transform.localScale = Vector3.one * (zoneRadius * 2f);

            GameObject ringObject = new GameObject("Ring");
            ringObject.transform.SetParent(root.transform, false);
            SpriteRenderer ring = ringObject.AddComponent<SpriteRenderer>();
            ring.sprite = ProceduralSpriteFactory.GetRing("territory_" + kind + "_ring", color, 128, 0.08f, 0.03f);
            ring.sortingOrder = -17;
            ringObject.transform.localScale = Vector3.one * (zoneRadius * 2f);

            GameObject labelObject = new GameObject("BonusLabel");
            labelObject.transform.SetParent(root.transform, false);
            TextMeshPro label = labelObject.AddComponent<TextMeshPro>();
            label.text = LabelFor(kind);
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = 1.25f;
            label.color = Color.white;
            label.enableWordWrapping = false;
            label.rectTransform.sizeDelta = new Vector2(zoneRadius * 1.65f, 0.8f);
            labelObject.transform.localPosition = new Vector3(0f, -0.08f, 0f);
            MeshRenderer labelRenderer = label.GetComponent<MeshRenderer>();
            if (labelRenderer != null)
            {
                labelRenderer.sortingOrder = -16;
            }

            return root;
        }

        private void ApplyCurrentTerritory()
        {
            float damage = 1f;
            float range = 1f;
            float recovery = 1f;
            Vector2 playerPosition = transform.position;

            for (int i = 0; i < zones.Count; i++)
            {
                TerritoryZone zone = zones[i];
                if (zone.Visual == null || Vector2.Distance(playerPosition, zone.Position) > zone.Radius)
                {
                    continue;
                }

                if (zone.Kind == TerritoryKind.Damage)
                {
                    damage = Mathf.Max(damage, damageBoost);
                }
                else if (zone.Kind == TerritoryKind.Range)
                {
                    range = Mathf.Max(range, rangeBoost);
                }
                else if (zone.Kind == TerritoryKind.Recovery)
                {
                    recovery = Mathf.Max(recovery, recoveryBoost);
                }
                else
                {
                    damage = Mathf.Max(damage, confluenceBoost);
                    range = Mathf.Max(range, confluenceBoost);
                    recovery = Mathf.Max(recovery, confluenceBoost);
                }
            }

            activeDamageMultiplier = damage;
            activeRangeMultiplier = range;
            activeRecoveryMultiplier = recovery;
            spellStats.SetTerritoryMultipliers(damage, range, recovery);
        }

        private static void AppendBonus(ref string summary, string label, float multiplier)
        {
            if (multiplier <= 1.01f)
            {
                return;
            }

            if (!string.IsNullOrEmpty(summary))
            {
                summary += "\n";
            }

            summary += "- " + label + " x" + multiplier.ToString("0.00");
        }

        private string LabelFor(TerritoryKind kind)
        {
            switch (kind)
            {
                case TerritoryKind.Damage:
                    return "+" + Percent(damageBoost) + "% DMG";
                case TerritoryKind.Range:
                    return "+" + Percent(rangeBoost) + "% RNG";
                case TerritoryKind.Recovery:
                    return "+" + Percent(recoveryBoost) + "% REC";
                default:
                    return "+" + Percent(confluenceBoost) + "% ALL";
            }
        }

        private static int Percent(float multiplier)
        {
            return Mathf.RoundToInt((multiplier - 1f) * 100f);
        }

        private static Color ColorFor(TerritoryKind kind)
        {
            switch (kind)
            {
                case TerritoryKind.Damage:
                    return new Color(1f, 0.38f, 0.16f, 0.68f);
                case TerritoryKind.Range:
                    return new Color(0.35f, 0.78f, 1f, 0.68f);
                case TerritoryKind.Recovery:
                    return new Color(0.55f, 1f, 0.55f, 0.68f);
                default:
                    return new Color(1f, 0.86f, 0.28f, 0.72f);
            }
        }
    }
}
