using Heroic.Player;
using Heroic.Spells;
using UnityEngine;

namespace Heroic.Systems
{
    public class UpgradeChoiceApplier : MonoBehaviour
    {
        [SerializeField] private UpgradeManager upgradeManager;
        [SerializeField] private RunBuildState buildState;
        [SerializeField] private SpellCaster spellCaster;
        [SerializeField] private MovementCaster movementCaster;
        [SerializeField] private ArcaneUpgradeApplier arcaneUpgradeApplier;

        private void Awake()
        {
            if (upgradeManager == null)
            {
                upgradeManager = GetComponent<UpgradeManager>();
            }

            if (arcaneUpgradeApplier == null)
            {
                arcaneUpgradeApplier = GetComponent<ArcaneUpgradeApplier>();
            }
        }

        private void OnEnable()
        {
            if (upgradeManager != null)
            {
                upgradeManager.ChoiceApplied += ApplyChoice;
            }
        }

        private void OnDisable()
        {
            if (upgradeManager != null)
            {
                upgradeManager.ChoiceApplied -= ApplyChoice;
            }
        }

        private void ApplyChoice(UpgradeManager.DraftChoice choice)
        {
            if (choice == null)
            {
                return;
            }

            if (choice.Id.StartsWith("upgrade_arcane_"))
            {
                ApplyArcaneUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("arcane_"))
            {
                buildState?.LearnSkill(choice.Id);
                spellCaster?.EnableSkill(choice.Id);
                return;
            }

            if (choice.Id == "movement_blink")
            {
                EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId.Blink);
            }
            else if (choice.Id == "movement_lunge")
            {
                EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId.Lunge);
            }
            else if (choice.Id == "movement_teleport")
            {
                EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId.Teleport);
            }
        }

        private void ApplyArcaneUpgrade(string choiceId)
        {
            string skillId = ResolveArcaneSkillId(choiceId);
            buildState?.LearnSkill(skillId);
            spellCaster?.EnableSkill(skillId);
            buildState?.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState != null ? buildState.GetSkillPathTier(skillId, choiceId) : 1;
            arcaneUpgradeApplier?.Apply(choiceId, tier);
        }

        private string ResolveArcaneSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_arcane_magic_missile"))
            {
                return "arcane_magic_missile";
            }

            if (choiceId.StartsWith("upgrade_arcane_arcane_blast"))
            {
                return "arcane_arcane_blast";
            }

            if (choiceId.StartsWith("upgrade_arcane_warp_pulse"))
            {
                return "arcane_warp_pulse";
            }

            if (choiceId.StartsWith("upgrade_arcane_spell_echo"))
            {
                return "arcane_spell_echo";
            }

            if (choiceId.StartsWith("upgrade_arcane_arcane_orbit"))
            {
                return "arcane_arcane_orbit";
            }

            return "arcane_unknown";
        }

        private void EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId skillId)
        {
            if (movementCaster == null)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                if (movementCaster.GetEquippedSkill(i) == skillId)
                {
                    return;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (movementCaster.GetEquippedSkill(i) == MovementCaster.MovementSkillId.None)
                {
                    movementCaster.EquipMovementSkill(i, skillId);
                    buildState?.EquipMovementSkill(i, skillId);
                    return;
                }
            }

            movementCaster.EquipMovementSkill(0, skillId);
            buildState?.EquipMovementSkill(0, skillId);
        }
    }
}
