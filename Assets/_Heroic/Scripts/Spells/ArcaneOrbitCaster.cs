using UnityEngine;
using Heroic.Visuals;
using Heroic.Systems;

namespace Heroic.Spells
{
    public class ArcaneOrbitCaster : MonoBehaviour
    {
        [SerializeField] private ArcaneOrbitOrb orbPrefab;
        [SerializeField] private int orbCount = 3;
        [SerializeField] private float radius = 1.4f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private int damage = 6;
        [SerializeField] private float respawnCooldown = 3f;

        private bool spawned;
        private readonly System.Collections.Generic.List<ArcaneOrbitOrb> spawnedOrbs = new System.Collections.Generic.List<ArcaneOrbitOrb>();
        private SpellStatModifier spellStats;
        private float nextOrbRespawnTime;

        private void Awake()
        {
            spellStats = GetComponent<SpellStatModifier>();
        }

        private void Update()
        {
            if (!spawned)
            {
                return;
            }

            RemoveDestroyedOrbs();
            for (int i = 0; i < spawnedOrbs.Count; i++)
            {
                ArcaneOrbitOrb orb = spawnedOrbs[i];
                if (orb != null)
                {
                    orb.SetDamage(ModifiedDamage(damage));
                    orb.SetRadius(ModifiedRange(radius));
                }
            }

            RestoreMissingOrb();
        }

        public void SpawnOrbs()
        {
            if (spawned || orbPrefab == null)
            {
                return;
            }

            spawned = true;
            ClearOrbs();
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.45f, 0.82f, 1f, 0.32f), radius * 1.25f, 0.24f);
            for (int i = 0; i < orbCount; i++)
            {
                SpawnOrbAtIndex(i);
            }
        }

        public void SetOrbCount(int value)
        {
            orbCount = Mathf.Max(1, value);
            spawned = false;
            SpawnOrbs();
        }

        public void SetRotationSpeed(float value)
        {
            rotationSpeed = Mathf.Max(0f, value);
            foreach (ArcaneOrbitOrb orb in spawnedOrbs)
            {
                if (orb != null)
                {
                    orb.SetRotationSpeed(rotationSpeed);
                }
            }
        }

        public void SetRadius(float value)
        {
            radius = Mathf.Max(0.1f, value);
            foreach (ArcaneOrbitOrb orb in spawnedOrbs)
            {
                if (orb != null)
                {
                    orb.SetRadius(radius);
                }
            }
        }

        public void SetDamage(int value)
        {
            damage = Mathf.Max(0, value);
            foreach (ArcaneOrbitOrb orb in spawnedOrbs)
            {
                if (orb != null)
                {
                    orb.SetDamage(damage);
                }
            }
        }

        private void ClearOrbs()
        {
            for (int i = spawnedOrbs.Count - 1; i >= 0; i--)
            {
                if (spawnedOrbs[i] != null)
                {
                    Destroy(spawnedOrbs[i].gameObject);
                }
            }

            spawnedOrbs.Clear();
        }

        private void RestoreMissingOrb()
        {
            if (orbPrefab == null || spawnedOrbs.Count >= orbCount)
            {
                nextOrbRespawnTime = 0f;
                return;
            }

            if (nextOrbRespawnTime <= 0f)
            {
                nextOrbRespawnTime = Time.time + respawnCooldown;
                return;
            }

            if (Time.time < nextOrbRespawnTime)
            {
                return;
            }

            SpawnOrbAtIndex(spawnedOrbs.Count);
            nextOrbRespawnTime = spawnedOrbs.Count < orbCount ? Time.time + respawnCooldown : 0f;
        }

        private void SpawnOrbAtIndex(int index)
        {
            float angle = index * (360f / Mathf.Max(1, orbCount));
            ArcaneOrbitOrb orb = Instantiate(orbPrefab, transform.position, Quaternion.identity, transform);
            orb.Initialize(transform, angle, ModifiedRange(radius), rotationSpeed, ModifiedDamage(damage));
            spawnedOrbs.Add(orb);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.45f, 0.82f, 1f, 0.22f), ModifiedRange(radius), 0.14f);
        }

        private void RemoveDestroyedOrbs()
        {
            for (int i = spawnedOrbs.Count - 1; i >= 0; i--)
            {
                if (spawnedOrbs[i] == null)
                {
                    spawnedOrbs.RemoveAt(i);
                }
            }
        }

        private int ModifiedDamage(int value)
        {
            return spellStats != null ? spellStats.ModifyDamage(value) : value;
        }

        private float ModifiedRange(float value)
        {
            return spellStats != null ? spellStats.ModifyRange(value) : value;
        }
    }
}
