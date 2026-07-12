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
        [SerializeField] private TerritoryCastingController territoryCasting;
        [SerializeField] private ArcaneUpgradeApplier arcaneUpgradeApplier;
        [SerializeField] private FireUpgradeApplier fireUpgradeApplier;
        [SerializeField] private ColdUpgradeApplier coldUpgradeApplier;
        [SerializeField] private LightningUpgradeApplier lightningUpgradeApplier;

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

            if (coldUpgradeApplier == null)
            {
                coldUpgradeApplier = GetComponent<ColdUpgradeApplier>();
            }

            if (lightningUpgradeApplier == null)
            {
                lightningUpgradeApplier = GetComponent<LightningUpgradeApplier>();
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

            if (choice.Id.StartsWith("upgrade_cold_"))
            {
                ApplyColdUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("upgrade_lightning_"))
            {
                ApplyLightningUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("upgrade_system_territory_casting"))
            {
                ApplyTerritoryCastingUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("upgrade_movement_cloud_walk"))
            {
                ApplyCloudWalkUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("upgrade_movement_whirlwind"))
            {
                ApplyWhirlwindUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("arcane_") || choice.Id.StartsWith("fire_") || choice.Id.StartsWith("cold_") || choice.Id.StartsWith("lightning_"))
            {
                buildState?.LearnSkill(choice.Id);
                spellCaster?.EnableSkill(choice.Id);
                return;
            }

            if (choice.Id == "system_territory_casting")
            {
                buildState?.LearnSkill(choice.Id);
                territoryCasting?.EnableTerritoryCasting();
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
            else if (choice.Id == "movement_cloud_walk")
            {
                EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId.CloudWalk);
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

        private void ApplyColdUpgrade(string choiceId)
        {
            string skillId = ResolveColdSkillId(choiceId);
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            spellCaster?.EnableSkill(skillId);
            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);
            coldUpgradeApplier?.Apply(choiceId, tier);
        }

        private void ApplyLightningUpgrade(string choiceId)
        {
            string skillId = ResolveLightningSkillId(choiceId);
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            spellCaster?.EnableSkill(skillId);
            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);
            lightningUpgradeApplier?.Apply(choiceId, tier);
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

        private string ResolveColdSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_cold_frost_ring"))
            {
                return "cold_frost_ring";
            }

            if (choiceId.StartsWith("upgrade_cold_ice_shard"))
            {
                return "cold_ice_shard";
            }

            if (choiceId.StartsWith("upgrade_cold_glacial_field"))
            {
                return "cold_glacial_field";
            }

            if (choiceId.StartsWith("upgrade_cold_crystal_prison"))
            {
                return "cold_crystal_prison";
            }

            if (choiceId.StartsWith("upgrade_cold_shatter_line"))
            {
                return "cold_shatter_line";
            }

            return "cold_unknown";
        }

        private string ResolveLightningSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_lightning_chain_bolt"))
            {
                return "lightning_chain_bolt";
            }

            if (choiceId.StartsWith("upgrade_lightning_static_field"))
            {
                return "lightning_static_field";
            }

            if (choiceId.StartsWith("upgrade_lightning_thunder_lance"))
            {
                return "lightning_thunder_lance";
            }

            if (choiceId.StartsWith("upgrade_lightning_spark_surge"))
            {
                return "lightning_spark_surge";
            }

            if (choiceId.StartsWith("upgrade_lightning_storm_call"))
            {
                return "lightning_storm_call";
            }

            return "lightning_unknown";
        }

        private void ApplyTerritoryCastingUpgrade(string choiceId)
        {
            const string skillId = "system_territory_casting";
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);
            if (choiceId == "upgrade_system_territory_casting_more_territories")
            {
                territoryCasting?.SetActiveZoneCount(Value(tier, 7, 8, 9, 10, 12));
            }
        }

        private int Value(int tier, int basic, int advanced, int expert, int master, int grandmaster)
        {
            switch (Mathf.Clamp(tier, 1, 5))
            {
                case 1:
                    return basic;
                case 2:
                    return advanced;
                case 3:
                    return expert;
                case 4:
                    return master;
                default:
                    return grandmaster;
            }
        }

        private void ApplyCloudWalkUpgrade(string choiceId)
        {
            const string skillId = "movement_cloud_walk";
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);

            if (choiceId == "upgrade_movement_cloud_walk_speed")
            {
                movementCaster?.SetCloudWalkStandardMovementTier(tier);
            }
            else if (choiceId == "upgrade_movement_cloud_walk_pickup")
            {
                movementCaster?.SetCloudWalkPickupRangeTier(tier);
            }
            else if (choiceId == "upgrade_movement_cloud_walk_knockback")
            {
                movementCaster?.SetCloudWalkKnockbackTier(tier);
            }
        }

        private void ApplyWhirlwindUpgrade(string choiceId)
        {
            const string skillId = "movement_whirlwind";
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);

            if (choiceId == "upgrade_movement_whirlwind_gale")
            {
                movementCaster?.SetWhirlwindGaleTier(tier);
            }
        }

        private void EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId skillId)
        {
            if (movementCaster == null)
            {
                return;
            }

            buildState?.LearnSkill(ResolveMovementSkillId(skillId));

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

        private string ResolveMovementSkillId(MovementCaster.MovementSkillId skillId)
        {
            switch (skillId)
            {
                case MovementCaster.MovementSkillId.Blink:
                    return "movement_blink";
                case MovementCaster.MovementSkillId.Lunge:
                    return "movement_lunge";
                case MovementCaster.MovementSkillId.Teleport:
                    return "movement_teleport";
                case MovementCaster.MovementSkillId.Whirlwind:
                    return "movement_whirlwind";
                case MovementCaster.MovementSkillId.CloudWalk:
                    return "movement_cloud_walk";
                default:
                    return string.Empty;
            }
        }
    }
}
