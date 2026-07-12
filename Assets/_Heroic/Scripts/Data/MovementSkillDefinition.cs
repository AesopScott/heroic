using UnityEngine;

namespace Heroic.Data
{
    [CreateAssetMenu(menuName = "Heroic/Movement Skill Definition", fileName = "MovementSkillDefinition")]
    public class MovementSkillDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string skillName;
        [SerializeField] private string role;
        [SerializeField] private string cooldown;
        [SerializeField] private string range;
        [SerializeField] private float baseCooldownSeconds = 6f;
        [SerializeField] private float baseRangeUnits = 3f;
        [SerializeField] private int baseDamage;
        [SerializeField] private UpgradePathDefinition[] upgradePaths = new UpgradePathDefinition[3];
        [TextArea] [SerializeField] private string description;

        public string Id => id;
        public string SkillName => skillName;
        public string Role => role;
        public string Cooldown => cooldown;
        public string Range => range;
        public float BaseCooldownSeconds => baseCooldownSeconds;
        public float BaseRangeUnits => baseRangeUnits;
        public int BaseDamage => baseDamage;
        public UpgradePathDefinition[] UpgradePaths => upgradePaths;
        public string Description => description;
    }
}
