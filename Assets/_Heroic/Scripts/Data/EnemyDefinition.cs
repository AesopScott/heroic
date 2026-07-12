using Heroic.Visuals;
using UnityEngine;

namespace Heroic.Data
{
    [CreateAssetMenu(menuName = "Heroic/Enemy Definition", fileName = "EnemyDefinition")]
    public class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string enemyName;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int maxHealth = 1;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private int contactDamage = 1;
        [SerializeField] private int experienceValue = 1;
        [SerializeField] private VisualPresetApplier.Preset visualPreset = VisualPresetApplier.Preset.BasicEnemy;
        [SerializeField] private bool boss;
        [TextArea] [SerializeField] private string description;

        public string Id => id;
        public string EnemyName => enemyName;
        public GameObject Prefab => prefab;
        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public int ContactDamage => contactDamage;
        public int ExperienceValue => experienceValue;
        public VisualPresetApplier.Preset VisualPreset => visualPreset;
        public bool IsBoss => boss;
        public string Description => description;
    }
}
