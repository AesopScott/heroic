using Heroic.Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace Heroic.Spells
{
    public static class LightningTargeting
    {
        public static EnemyController FindNearestEnemy(Vector2 origin, float maxRange, List<EnemyController> excluded = null)
        {
            EnemyController[] enemies = Object.FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
            EnemyController closest = null;
            float closestDistance = maxRange <= 0f ? float.MaxValue : maxRange;

            foreach (EnemyController enemy in enemies)
            {
                if (enemy == null || (excluded != null && excluded.Contains(enemy)))
                {
                    continue;
                }

                float distance = Vector2.Distance(origin, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = enemy;
                }
            }

            return closest;
        }
    }
}
