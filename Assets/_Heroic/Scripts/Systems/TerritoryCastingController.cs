using Heroic.Visuals;
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

        private struct TerritoryZone
        {
            public TerritoryKind Kind;
            public Vector2 Position;
            public float Radius;
            public GameObject Visual;
        }

        [SerializeField] private float zoneRadius = 2.45f;
        [SerializeField] private float damageBoost = 1.35f;
        [SerializeField] private float rangeBoost = 1.35f;
        [SerializeField] private float recoveryBoost = 1.35f;
        [SerializeField] private float confluenceBoost = 1.2f;

        private readonly TerritoryZone[] zones = new TerritoryZone[6];
        private SpellStatModifier spellStats;
        private bool enabledTerritory;

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

            ApplyCurrentTerritory();
        }

        public void EnableTerritoryCasting()
        {
            if (enabledTerritory)
            {
                return;
            }

            enabledTerritory = true;
            CreateZones();
            ApplyCurrentTerritory();
        }

        private void CreateZones()
        {
            Vector2 origin = transform.position;
            SetZone(0, TerritoryKind.Damage, origin + new Vector2(-5.5f, 3.25f));
            SetZone(1, TerritoryKind.Range, origin + new Vector2(5.5f, 3.25f));
            SetZone(2, TerritoryKind.Recovery, origin + new Vector2(-5.5f, -3.25f));
            SetZone(3, TerritoryKind.Confluence, origin + new Vector2(5.5f, -3.25f));
            SetZone(4, TerritoryKind.Damage, origin + new Vector2(0f, 6.1f));
            SetZone(5, TerritoryKind.Range, origin + new Vector2(0f, -6.1f));
        }

        private void SetZone(int index, TerritoryKind kind, Vector2 position)
        {
            zones[index] = new TerritoryZone
            {
                Kind = kind,
                Position = position,
                Radius = zoneRadius,
                Visual = CreateZoneVisual(kind, position)
            };
        }

        private GameObject CreateZoneVisual(TerritoryKind kind, Vector2 position)
        {
            Color color = ColorFor(kind);
            GameObject root = new GameObject("Territory_" + kind);
            root.transform.position = position;

            SpriteRenderer fill = root.AddComponent<SpriteRenderer>();
            Color fillColor = color;
            fillColor.a = 0.14f;
            fill.sprite = ProceduralSpriteFactory.GetCircle("territory_" + kind + "_fill", fillColor, 128, 0.1f);
            fill.sortingOrder = -18;
            root.transform.localScale = Vector3.one * (zoneRadius * 2f);

            GameObject ringObject = new GameObject("Ring");
            ringObject.transform.SetParent(root.transform, false);
            SpriteRenderer ring = ringObject.AddComponent<SpriteRenderer>();
            ring.sprite = ProceduralSpriteFactory.GetRing("territory_" + kind + "_ring", color, 128, 0.08f, 0.03f);
            ring.sortingOrder = -17;
            return root;
        }

        private void ApplyCurrentTerritory()
        {
            float damage = 1f;
            float range = 1f;
            float recovery = 1f;
            Vector2 playerPosition = transform.position;

            for (int i = 0; i < zones.Length; i++)
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

            spellStats.SetTerritoryMultipliers(damage, range, recovery);
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
