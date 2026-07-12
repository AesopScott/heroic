using UnityEngine;

namespace Heroic.Enemies
{
    public class EnemyDirector : MonoBehaviour
    {
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private float minimumSpawnInterval = 0.35f;
        [SerializeField] private float maximumSpawnInterval = 2f;

        private void Awake()
        {
            if (enemySpawner == null)
            {
                enemySpawner = FindAnyObjectByType<EnemySpawner>();
            }
        }

        public void SetDifficultyCurve(float normalizedRunTime)
        {
            if (enemySpawner == null)
            {
                return;
            }

            float interval = Mathf.Lerp(maximumSpawnInterval, minimumSpawnInterval, Mathf.Clamp01(normalizedRunTime));
            enemySpawner.SetBaseSpawnInterval(interval);
        }
    }
}
