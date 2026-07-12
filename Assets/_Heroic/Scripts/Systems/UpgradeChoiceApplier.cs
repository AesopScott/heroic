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
        [SerializeField] private MagicSystemController magicSystemController;
        [SerializeField] private ArcaneUpgradeApplier arcaneUpgradeApplier;
        [SerializeField] private FireUpgradeApplier fireUpgradeApplier;
        [SerializeField] private ColdUpgradeApplier coldUpgradeApplier;
        [SerializeField] private LightningUpgradeApplier lightningUpgradeApplier;
        [SerializeField] private EarthUpgradeApplier earthUpgradeApplier;
        [SerializeField] private MindUpgradeApplier mindUpgradeApplier;
        [SerializeField] private BloodUpgradeApplier bloodUpgradeApplier;
        [SerializeField] private PoisonUpgradeApplier poisonUpgradeApplier;

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

            if (earthUpgradeApplier == null)
            {
                earthUpgradeApplier = GetComponent<EarthUpgradeApplier>();
            }

            if (mindUpgradeApplier == null)
            {
                mindUpgradeApplier = GetComponent<MindUpgradeApplier>();
            }

            if (bloodUpgradeApplier == null)
            {
                bloodUpgradeApplier = GetComponent<BloodUpgradeApplier>();
            }

            if (poisonUpgradeApplier == null)
            {
                poisonUpgradeApplier = GetComponent<PoisonUpgradeApplier>();
            }

            if (magicSystemController == null)
            {
                magicSystemController = FindAnyObjectByType<MagicSystemController>();
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

            if (choice.Id.StartsWith("upgrade_earth_"))
            {
                ApplyEarthUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("upgrade_mind_"))
            {
                ApplyMindUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("upgrade_blood_"))
            {
                ApplyBloodUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("upgrade_poison_"))
            {
                ApplyPoisonUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("upgrade_system_"))
            {
                ApplySystemUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("upgrade_movement_"))
            {
                ApplyMovementUpgrade(choice.Id);
                return;
            }

            if (choice.Id.StartsWith("arcane_") || choice.Id.StartsWith("fire_") || choice.Id.StartsWith("cold_") || choice.Id.StartsWith("lightning_") || choice.Id.StartsWith("earth_") || choice.Id.StartsWith("mind_") || choice.Id.StartsWith("blood_") || choice.Id.StartsWith("poison_"))
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

            if (choice.Id.StartsWith("system_"))
            {
                buildState?.LearnSkill(choice.Id);
                magicSystemController?.EnableSystem(choice.Id);
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
            else if (choice.Id == "movement_invisibility")
            {
                EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId.Invisibility);
            }
            else if (choice.Id == "movement_stoneskin")
            {
                EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId.Stoneskin);
            }
            else if (choice.Id == "movement_tunnel")
            {
                EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId.Tunnel);
            }
            else if (choice.Id == "movement_flight")
            {
                EquipFirstOpenMovementSlot(MovementCaster.MovementSkillId.Flight);
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

        private void ApplyEarthUpgrade(string choiceId)
        {
            string skillId = ResolveEarthSkillId(choiceId);
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            spellCaster?.EnableSkill(skillId);
            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);
            earthUpgradeApplier?.Apply(choiceId, tier);
        }

        private void ApplyMindUpgrade(string choiceId)
        {
            string skillId = ResolveMindSkillId(choiceId);
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            spellCaster?.EnableSkill(skillId);
            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);
            mindUpgradeApplier?.Apply(choiceId, tier);
        }

        private void ApplyBloodUpgrade(string choiceId)
        {
            string skillId = ResolveBloodSkillId(choiceId);
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            spellCaster?.EnableSkill(skillId);
            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);
            bloodUpgradeApplier?.Apply(choiceId, tier);
        }

        private void ApplyPoisonUpgrade(string choiceId)
        {
            string skillId = ResolvePoisonSkillId(choiceId);
            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            spellCaster?.EnableSkill(skillId);
            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);
            poisonUpgradeApplier?.Apply(choiceId, tier);
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

            if (choiceId.StartsWith("upgrade_arcane_force_field"))
            {
                return "arcane_force_field";
            }

            if (choiceId.StartsWith("upgrade_arcane_time_warp"))
            {
                return "arcane_time_warp";
            }

            if (choiceId.StartsWith("upgrade_arcane_haste"))
            {
                return "arcane_haste";
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

            if (choiceId.StartsWith("upgrade_fire_flame_shield"))
            {
                return "fire_flame_shield";
            }

            if (choiceId.StartsWith("upgrade_fire_flame_wall"))
            {
                return "fire_flame_wall";
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

        private string ResolveEarthSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_earth_stone_spike"))
            {
                return "earth_stone_spike";
            }

            if (choiceId.StartsWith("upgrade_earth_boulder_toss"))
            {
                return "earth_boulder_toss";
            }

            if (choiceId.StartsWith("upgrade_earth_earth_wall"))
            {
                return "earth_earth_wall";
            }

            if (choiceId.StartsWith("upgrade_earth_quake"))
            {
                return "earth_quake";
            }

            if (choiceId.StartsWith("upgrade_earth_mud_trap"))
            {
                return "earth_mud_trap";
            }

            return "earth_unknown";
        }

        private string ResolveMindSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_mind_psychic_lance"))
            {
                return "mind_psychic_lance";
            }

            if (choiceId.StartsWith("upgrade_mind_fear_wave"))
            {
                return "mind_fear_wave";
            }

            if (choiceId.StartsWith("upgrade_mind_illusion_clone"))
            {
                return "mind_illusion_clone";
            }

            if (choiceId.StartsWith("upgrade_mind_confuse"))
            {
                return "mind_confuse";
            }

            if (choiceId.StartsWith("upgrade_mind_mind_crush"))
            {
                return "mind_mind_crush";
            }

            return "mind_unknown";
        }

        private string ResolveBloodSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_blood_blood_bolt"))
            {
                return "blood_blood_bolt";
            }

            if (choiceId.StartsWith("upgrade_blood_sanguine_pact"))
            {
                return "blood_sanguine_pact";
            }

            if (choiceId.StartsWith("upgrade_blood_blood_nova"))
            {
                return "blood_blood_nova";
            }

            if (choiceId.StartsWith("upgrade_blood_leech_bind"))
            {
                return "blood_leech_bind";
            }

            if (choiceId.StartsWith("upgrade_blood_crimson_frenzy"))
            {
                return "blood_crimson_frenzy";
            }

            return "blood_unknown";
        }

        private string ResolvePoisonSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_poison_poison_dart"))
            {
                return "poison_poison_dart";
            }

            if (choiceId.StartsWith("upgrade_poison_toxic_cloud"))
            {
                return "poison_toxic_cloud";
            }

            if (choiceId.StartsWith("upgrade_poison_venom_trail"))
            {
                return "poison_venom_trail";
            }

            if (choiceId.StartsWith("upgrade_poison_infection"))
            {
                return "poison_infection";
            }

            if (choiceId.StartsWith("upgrade_poison_rot_bloom"))
            {
                return "poison_rot_bloom";
            }

            return "poison_unknown";
        }

        private void ApplySystemUpgrade(string choiceId)
        {
            string skillId = ResolveSystemSkillId(choiceId);
            if (IsSystemSynergyUpgrade(choiceId))
            {
                string[] prerequisites = ResolveSystemSynergyPrerequisites(choiceId);
                if (buildState == null || prerequisites.Length != 2 || !buildState.HasSkill(prerequisites[0]) || !buildState.HasSkill(prerequisites[1]))
                {
                    return;
                }

                buildState.UpgradeSkillPath(skillId, choiceId);
                int synergyTier = buildState.GetSkillPathTier(skillId, choiceId);
                magicSystemController?.ApplyUpgrade(choiceId, synergyTier);
                return;
            }

            if (buildState == null || !buildState.HasSkill(skillId))
            {
                return;
            }

            buildState.UpgradeSkillPath(skillId, choiceId);
            int tier = buildState.GetSkillPathTier(skillId, choiceId);
            if (skillId == "system_territory_casting")
            {
                if (choiceId == "upgrade_system_territory_casting_more_territories")
                {
                    territoryCasting?.SetActiveZoneCount(Value(tier, 7, 8, 9, 10, 12));
                }
                else if (choiceId == "upgrade_system_territory_casting_larger_territories")
                {
                    territoryCasting?.SetZoneRadius(Value(tier, 2.8f, 3.15f, 3.55f, 4f, 4.6f));
                }
                else if (choiceId == "upgrade_system_territory_casting_stronger_territories")
                {
                    float standard = Value(tier, 1.42f, 1.5f, 1.6f, 1.72f, 1.88f);
                    float confluence = Value(tier, 1.25f, 1.32f, 1.4f, 1.5f, 1.62f);
                    territoryCasting?.SetBoostMultipliers(standard, standard, standard, confluence);
                }
                return;
            }

            magicSystemController?.ApplyUpgrade(choiceId, tier);
        }

        private static string ResolveSystemSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_system_territory_casting"))
            {
                return "system_territory_casting";
            }

            if (choiceId.StartsWith("upgrade_system_synergy_"))
            {
                return choiceId.Replace("upgrade_", string.Empty);
            }

            if (choiceId.StartsWith("upgrade_system_component_boosts"))
            {
                return "system_component_boosts";
            }

            if (choiceId.StartsWith("upgrade_system_sacrifice_casting"))
            {
                return "system_sacrifice_casting";
            }

            if (choiceId.StartsWith("upgrade_system_rhythm_casting"))
            {
                return "system_rhythm_casting";
            }

            if (choiceId.StartsWith("upgrade_system_spell_tension"))
            {
                return "system_spell_tension";
            }

            return string.Empty;
        }

        private static bool IsSystemSynergyUpgrade(string choiceId)
        {
            return choiceId.StartsWith("upgrade_system_synergy_");
        }

        private static string[] ResolveSystemSynergyPrerequisites(string choiceId)
        {
            switch (choiceId)
            {
                case "upgrade_system_synergy_territory_components":
                    return new[] { "system_territory_casting", "system_component_boosts" };
                case "upgrade_system_synergy_territory_sacrifice":
                    return new[] { "system_territory_casting", "system_sacrifice_casting" };
                case "upgrade_system_synergy_territory_rhythm":
                    return new[] { "system_territory_casting", "system_rhythm_casting" };
                case "upgrade_system_synergy_territory_tension":
                    return new[] { "system_territory_casting", "system_spell_tension" };
                case "upgrade_system_synergy_components_sacrifice":
                    return new[] { "system_component_boosts", "system_sacrifice_casting" };
                case "upgrade_system_synergy_components_rhythm":
                    return new[] { "system_component_boosts", "system_rhythm_casting" };
                case "upgrade_system_synergy_components_tension":
                    return new[] { "system_component_boosts", "system_spell_tension" };
                case "upgrade_system_synergy_sacrifice_rhythm":
                    return new[] { "system_sacrifice_casting", "system_rhythm_casting" };
                case "upgrade_system_synergy_sacrifice_tension":
                    return new[] { "system_sacrifice_casting", "system_spell_tension" };
                case "upgrade_system_synergy_rhythm_tension":
                    return new[] { "system_rhythm_casting", "system_spell_tension" };
                default:
                    return new string[0];
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

        private float Value(int tier, float basic, float advanced, float expert, float master, float grandmaster)
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

        private void ApplyMovementUpgrade(string choiceId)
        {
            string skillId = ResolveMovementUpgradeSkillId(choiceId);
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
            else if (choiceId == "upgrade_movement_whirlwind_gale")
            {
                movementCaster?.SetWhirlwindGaleTier(tier);
            }
            else if (choiceId == "upgrade_movement_invisibility_longer_fade")
            {
                movementCaster?.SetInvisibilityDurationTier(tier);
            }
            else if (choiceId == "upgrade_movement_invisibility_swift_fade")
            {
                movementCaster?.SetInvisibilitySpeedTier(tier);
            }
            else if (choiceId == "upgrade_movement_invisibility_exit_burst")
            {
                movementCaster?.SetInvisibilityExitDamageTier(tier);
            }
            else if (choiceId == "upgrade_movement_stoneskin_longer_skin")
            {
                movementCaster?.SetStoneskinDurationTier(tier);
            }
            else if (choiceId == "upgrade_movement_stoneskin_lighter_skin")
            {
                movementCaster?.SetStoneskinSpeedTier(tier);
            }
            else if (choiceId == "upgrade_movement_stoneskin_thorn_skin")
            {
                movementCaster?.SetStoneskinPulseDamageTier(tier);
            }
            else if (choiceId == "upgrade_movement_tunnel_deeper_tunnel")
            {
                movementCaster?.SetTunnelDurationTier(tier);
            }
            else if (choiceId == "upgrade_movement_tunnel_eruption")
            {
                movementCaster?.SetTunnelEruptionRadiusTier(tier);
            }
            else if (choiceId == "upgrade_movement_flight_swift_flight")
            {
                movementCaster?.SetFlightDurationTier(tier);
            }
            else if (choiceId == "upgrade_movement_flight_landing_gust")
            {
                movementCaster?.SetFlightLandingRadiusTier(tier);
                movementCaster?.SetMovementDamageTier(MovementCaster.MovementSkillId.Flight, tier);
            }
            else if (choiceId.Contains("_longer_") || choiceId.EndsWith("_longer_blink") || choiceId.EndsWith("_longer_lunge") || choiceId.EndsWith("_longer_teleport") || choiceId.EndsWith("_longer_tunnel"))
            {
                movementCaster?.SetMovementRangeTier(ResolveMovementSkillEnum(skillId), tier);
            }
            else if (choiceId.Contains("_quick_"))
            {
                movementCaster?.SetMovementCooldownTier(ResolveMovementSkillEnum(skillId), tier);
            }
            else if (choiceId.EndsWith("_arc_flash") || choiceId.EndsWith("_heavy_impact") || choiceId.EndsWith("_arrival_burst"))
            {
                movementCaster?.SetMovementDamageTier(ResolveMovementSkillEnum(skillId), tier);
            }
            else if (choiceId == "upgrade_movement_whirlwind_wider_spin")
            {
                movementCaster?.SetWhirlwindRadiusTier(tier);
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
                case MovementCaster.MovementSkillId.Invisibility:
                    return "movement_invisibility";
                case MovementCaster.MovementSkillId.Stoneskin:
                    return "movement_stoneskin";
                case MovementCaster.MovementSkillId.Tunnel:
                    return "movement_tunnel";
                case MovementCaster.MovementSkillId.Flight:
                    return "movement_flight";
                default:
                    return string.Empty;
            }
        }

        private string ResolveMovementUpgradeSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_movement_blink"))
            {
                return "movement_blink";
            }

            if (choiceId.StartsWith("upgrade_movement_lunge"))
            {
                return "movement_lunge";
            }

            if (choiceId.StartsWith("upgrade_movement_teleport"))
            {
                return "movement_teleport";
            }

            if (choiceId.StartsWith("upgrade_movement_whirlwind"))
            {
                return "movement_whirlwind";
            }

            if (choiceId.StartsWith("upgrade_movement_cloud_walk"))
            {
                return "movement_cloud_walk";
            }

            if (choiceId.StartsWith("upgrade_movement_invisibility"))
            {
                return "movement_invisibility";
            }

            if (choiceId.StartsWith("upgrade_movement_stoneskin"))
            {
                return "movement_stoneskin";
            }

            if (choiceId.StartsWith("upgrade_movement_tunnel"))
            {
                return "movement_tunnel";
            }

            if (choiceId.StartsWith("upgrade_movement_flight"))
            {
                return "movement_flight";
            }

            return string.Empty;
        }

        private MovementCaster.MovementSkillId ResolveMovementSkillEnum(string skillId)
        {
            switch (skillId)
            {
                case "movement_blink":
                    return MovementCaster.MovementSkillId.Blink;
                case "movement_lunge":
                    return MovementCaster.MovementSkillId.Lunge;
                case "movement_teleport":
                    return MovementCaster.MovementSkillId.Teleport;
                case "movement_whirlwind":
                    return MovementCaster.MovementSkillId.Whirlwind;
                case "movement_cloud_walk":
                    return MovementCaster.MovementSkillId.CloudWalk;
                case "movement_invisibility":
                    return MovementCaster.MovementSkillId.Invisibility;
                case "movement_stoneskin":
                    return MovementCaster.MovementSkillId.Stoneskin;
                case "movement_tunnel":
                    return MovementCaster.MovementSkillId.Tunnel;
                case "movement_flight":
                    return MovementCaster.MovementSkillId.Flight;
                default:
                    return MovementCaster.MovementSkillId.None;
            }
        }
    }
}
