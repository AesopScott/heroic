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
        [SerializeField] private FireUpgradeApplier fireUpgradeApplier;

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

            if (fireUpgradeApplier == null)
            {
                fireUpgradeApplier = GetComponent<FireUpgradeApplier>();
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

            if (choice.Id.StartsWith("upgrade_fire_"))
            {
                ApplyFireUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("arcane_") || choice.Id.StartsWith("fire_"))
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
            else if (choice.Id == "movement_whirlwind")
            {
                EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId.Whirlwind);
            }
        }

        private void ApplyArcaneUpgrade(string choiceId)
        {
            string skillId = ResolveArcaneSkillId(choiceId);
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            spellCaster?.EnableSkill(skillId);
            buildState?.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState != null ? buildState.GetSkillPathTier(skillId, choiceId) : 1;
            arcaneUpgradeApplier?.Apply(choiceId, tier);
        }

        private void ApplyFireUpgrade(string choiceId)
        {
            string skillId = ResolveFireSkillId(choiceId);
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            spellCaster?.EnableSkill(skillId);
            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);
            fireUpgradeApplier?.Apply(choiceId, tier);
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

        private string ResolveFireSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_fire_fire_bolt"))
            {
                return "fire_fire_bolt";
            }

            if (choiceId.StartsWith("upgrade_fire_flame_wave"))
            {
                return "fire_flame_wave";
            }

            if (choiceId.StartsWith("upgrade_fire_burning_ground"))
            {
                return "fire_burning_ground";
            }

            return "fire_unknown";
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
