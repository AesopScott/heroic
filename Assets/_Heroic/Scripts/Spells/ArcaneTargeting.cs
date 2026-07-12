using Heroic.Enemies;
using UnityEngine;

namespace Heroic.Spells
{
    public static class ArcaneTargeting
    {
        public static EnemyController FindNearestEnemy(Vector3 origin, float maxRange)
        {
            EnemyController[] enemies = Object.FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
            EnemyController closest = null;
            float closestDistance = maxRange <= 0f ? float.MaxValue : maxRange;

            foreach (EnemyController enemy in enemies)
            {
                if (enemy == null)
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
