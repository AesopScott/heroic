using UnityEngine;
using Heroic.Visuals;

namespace Heroic.Spells
{
    public class ArcaneOrbitCaster : MonoBehaviour
    {
        [SerializeField] private ArcaneOrbitOrb orbPrefab;
        [SerializeField] private int orbCount = 3;
        [SerializeField] private float radius = 1.4f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private int damage = 6;

        private bool spawned;
        private readonly System.Collections.Generic.List<ArcaneOrbitOrb> spawnedOrbs = new System.Collections.Generic.List<ArcaneOrbitOrb>();

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
                float angle = i * (360f / orbCount);
                ArcaneOrbitOrb orb = Instantiate(orbPrefab, transform.position, Quaternion.identity, transform);
                orb.Initialize(transform, angle, radius, rotationSpeed, damage);
                spawnedOrbs.Add(orb);
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
    }
}
