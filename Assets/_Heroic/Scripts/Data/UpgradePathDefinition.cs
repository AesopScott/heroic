using UnityEngine;

namespace Heroic.Data
{
    [CreateAssetMenu(menuName = "Heroic/Upgrade Path Definition", fileName = "UpgradePathDefinition")]
    public class UpgradePathDefinition : ScriptableObject
    {
        public enum UpgradeTier
        {
            Basic = 1,
            Advanced = 2,
            Expert = 3,
            Master = 4,
            Grandmaster = 5
        }

        [System.Serializable]
        public class TierValue
        {
            [SerializeField] private UpgradeTier tier = UpgradeTier.Basic;
            [SerializeField] private float damageMultiplier = 1f;
            [SerializeField] private float cooldownMultiplier = 1f;
            [SerializeField] private float rangeMultiplier = 1f;
            [SerializeField] private float radiusMultiplier = 1f;
            [SerializeField] private int addedProjectiles;
            [SerializeField] private float procChanceBonus;
            [TextArea] [SerializeField] private string description;

            public UpgradeTier Tier => tier;
            public float DamageMultiplier => damageMultiplier;
            public float CooldownMultiplier => cooldownMultiplier;
            public float RangeMultiplier => rangeMultiplier;
            public float RadiusMultiplier => radiusMultiplier;
            public int AddedProjectiles => addedProjectiles;
            public float ProcChanceBonus => procChanceBonus;
            public string Description => description;
        }

        [SerializeField] private string pathName;
        [TextArea] [SerializeField] private string summary;
        [SerializeField] private TierValue[] tiers = new TierValue[5];

        public string PathName => pathName;
        public string Summary => summary;
        public TierValue[] Tiers => tiers;
    }
}
