using UnityEngine;

namespace Heroic.Data
{
    [CreateAssetMenu(menuName = "Heroic/Skill Definition", fileName = "SkillDefinition")]
    public class SkillDefinition : ScriptableObject
    {
        public enum SkillCategory
        {
            Attack,
            Defense,
            System
        }

        public enum CastStyle
        {
            Instant,
            Fast,
            Standard,
            Slow
        }

        public enum TargetingStyle
        {
            NearestEnemy,
            Directional,
            SelfCentered,
            TargetedArea,
            Orbit
        }

        public enum AreaShape
        {
            Projectile,
            Circle,
            Cone,
            Line,
            Ring,
            Orbit
        }

        [SerializeField] private string id;
        [SerializeField] private string skillName;
        [SerializeField] private string role;
        [TextArea] [SerializeField] private string baseDescription;
        [SerializeField] private SkillCategory category = SkillCategory.Attack;
        [SerializeField] private CastStyle castStyle = CastStyle.Instant;
        [SerializeField] private TargetingStyle targetingStyle = TargetingStyle.NearestEnemy;
        [SerializeField] private AreaShape areaShape = AreaShape.Projectile;
        [SerializeField] private string behaviorKey;
        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private float baseCooldown = 1f;
        [SerializeField] private float baseRange = 8f;
        [SerializeField] private float baseRadius = 0.5f;
        [SerializeField] private int baseDamage = 1;
        [SerializeField] private int baseProjectileCount = 1;
        [SerializeField] private float baseProcChance;
        [SerializeField] private UpgradePathDefinition[] upgradePaths = new UpgradePathDefinition[3];

        public string Id => id;
        public string SkillName => skillName;
        public string Role => role;
        public string BaseDescription => baseDescription;
        public SkillCategory Category => category;
        public CastStyle SkillCastStyle => castStyle;
        public TargetingStyle Targeting => targetingStyle;
        public AreaShape Shape => areaShape;
        public string BehaviorKey => behaviorKey;
        public GameObject EffectPrefab => effectPrefab;
        public float BaseCooldown => baseCooldown;
        public float BaseRange => baseRange;
        public float BaseRadius => baseRadius;
        public int BaseDamage => baseDamage;
        public int BaseProjectileCount => baseProjectileCount;
        public float BaseProcChance => baseProcChance;
        public UpgradePathDefinition[] UpgradePaths => upgradePaths;
    }
}
