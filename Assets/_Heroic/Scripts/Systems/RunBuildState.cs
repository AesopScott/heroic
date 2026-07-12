using Heroic.Player;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Heroic.Systems
{
    public class RunBuildState : MonoBehaviour
    {
        [Serializable]
        public class SkillUpgradeState
        {
            [SerializeField] private string skillId;
            [SerializeField] private string upgradePathId;
            [SerializeField] private int tier;

            public string SkillId => skillId;
            public string UpgradePathId => upgradePathId;
            public int Tier => tier;

            public SkillUpgradeState(string skillId, string upgradePathId, int tier)
            {
                this.skillId = skillId;
                this.upgradePathId = upgradePathId;
                this.tier = tier;
            }

            public void SetTier(int newTier)
            {
                tier = Mathf.Clamp(newTier, 0, 5);
            }
        }

        [SerializeField] private List<string> learnedSkillIds = new List<string>();
        [SerializeField] private List<SkillUpgradeState> skillUpgrades = new List<SkillUpgradeState>();
        [SerializeField] private MovementCaster.MovementSkillId[] equippedMovementSkills =
        {
            MovementCaster.MovementSkillId.Blink,
            MovementCaster.MovementSkillId.Lunge,
            MovementCaster.MovementSkillId.Teleport
        };

        public IReadOnlyList<string> LearnedSkillIds => learnedSkillIds;
        public IReadOnlyList<SkillUpgradeState> SkillUpgrades => skillUpgrades;
        public IReadOnlyList<MovementCaster.MovementSkillId> EquippedMovementSkills => equippedMovementSkills;

        public bool HasSkill(string skillId)
        {
            return learnedSkillIds.Contains(skillId);
        }

        public void LearnSkill(string skillId)
        {
            if (string.IsNullOrEmpty(skillId) || learnedSkillIds.Contains(skillId))
            {
                return;
            }

            learnedSkillIds.Add(skillId);
        }

        public void EquipMovementSkill(int slotIndex, MovementCaster.MovementSkillId skillId)
        {
            if (slotIndex < 0 || slotIndex >= equippedMovementSkills.Length)
            {
                return;
            }

            equippedMovementSkills[slotIndex] = skillId;
        }

        public void UpgradeSkillPath(string skillId, string upgradePathId)
        {
            SkillUpgradeState upgrade = skillUpgrades.Find(state =>
                state.SkillId == skillId && state.UpgradePathId == upgradePathId);

            if (upgrade == null)
            {
                skillUpgrades.Add(new SkillUpgradeState(skillId, upgradePathId, 1));
                return;
            }

            upgrade.SetTier(upgrade.Tier + 1);
        }

        public int GetSkillPathTier(string skillId, string upgradePathId)
        {
            SkillUpgradeState upgrade = skillUpgrades.Find(state =>
                state.SkillId == skillId && state.UpgradePathId == upgradePathId);

            return upgrade == null ? 0 : upgrade.Tier;
        }
    }
}
