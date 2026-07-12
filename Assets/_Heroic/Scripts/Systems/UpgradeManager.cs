using UnityEngine;
using System;
using System.Collections.Generic;
using Heroic.Core;
using Heroic.Player;

namespace Heroic.Systems
{
    public class UpgradeManager : MonoBehaviour
    {
        public enum UpgradeCategory
        {
            Attack,
            Defense,
            System,
            Boost,
            Movement
        }

        public enum AbilityType
        {
            Unspecified,
            Projectile,
            AreaOfEffectCaster,
            AreaOfEffectEnemy,
            AreaOfEffectGround,
            Augmentation,
            Cone,
            Line,
            NearestEnemy
        }

        [Serializable]
        public class DraftChoice
        {
            [SerializeField] private string id;
            [SerializeField] private string displayName;
            [TextArea] [SerializeField] private string description;
            [SerializeField] private UpgradeCategory category;
            [SerializeField] private AbilityType abilityType = AbilityType.Unspecified;

            public string Id => id;
            public string DisplayName => displayName;
            public string Description => description;
            public UpgradeCategory Category => category;
            public AbilityType Type => abilityType;

            public DraftChoice()
            {
            }

            public DraftChoice(string id, string displayName, string description, UpgradeCategory category)
                : this(id, displayName, description, category, AbilityType.Unspecified)
            {
            }

            public DraftChoice(string id, string displayName, string description, UpgradeCategory category, AbilityType abilityType)
            {
                this.id = id;
                this.displayName = displayName;
                this.description = description;
                this.category = category;
                this.abilityType = abilityType;
            }
        }

        [SerializeField] private DraftChoice[] draftPool = new DraftChoice[0];
        [SerializeField] private bool usePrototypeDraftPoolWhenEmpty = true;
        [SerializeField] private int minimumChoices = 3;
        [SerializeField] private int maximumChoices = 5;
        [SerializeField] private int movementChoicesOnMovementDraft = 3;
        [SerializeField] private RunManager runManager;
        [SerializeField] private RunBuildState buildState;

        private readonly List<DraftChoice> currentChoices = new List<DraftChoice>();

        public event Action<IReadOnlyList<DraftChoice>, bool> DraftOpened;
        public event Action<DraftChoice> ChoiceApplied;
        public event Action DraftClosed;

        public bool IsDraftOpen { get; private set; }
        public bool CurrentDraftIncludesMovement { get; private set; }
        public IReadOnlyList<DraftChoice> CurrentChoices => currentChoices;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }

            if (buildState == null)
            {
                buildState = FindAnyObjectByType<RunBuildState>();
            }

            if (usePrototypeDraftPoolWhenEmpty && (draftPool == null || draftPool.Length == 0))
            {
                draftPool = PrototypeDraftChoices.Create();
            }
        }

        public void OpenDraft(int playerLevel, bool includeMovementChoice)
        {
            IsDraftOpen = true;
            CurrentDraftIncludesMovement = includeMovementChoice;
            BuildChoices(playerLevel);
            runManager?.OpenLevelUpDraft();
            DraftOpened?.Invoke(currentChoices, includeMovementChoice);
        }

        public void OpenDraft()
        {
            OpenDraft(1, false);
        }

        public void ApplyChoice(string choiceId)
        {
            DraftChoice selected = currentChoices.Find(choice => choice.Id == choiceId);
            if (selected == null)
            {
                return;
            }

            ChoiceApplied?.Invoke(selected);
            RemoveChoicesInLane(selected);

            if (currentChoices.Count == 0)
            {
                CloseDraft();
                return;
            }

            DraftOpened?.Invoke(currentChoices, CurrentDraftIncludesMovement);
        }

        public void CloseDraft()
        {
            IsDraftOpen = false;
            CurrentDraftIncludesMovement = false;
            currentChoices.Clear();
            DraftClosed?.Invoke();
            runManager?.ResumeRun();
        }

        private void BuildChoices(int playerLevel)
        {
            currentChoices.Clear();
            bool includeMovementChoice = playerLevel % 2 == 0;
            bool includeSystemChoice = playerLevel % 3 == 0;

            List<DraftChoice> movementEligible = new List<DraftChoice>();
            List<DraftChoice> abilityEligible = new List<DraftChoice>();
            List<DraftChoice> systemEligible = new List<DraftChoice>();
            foreach (DraftChoice choice in draftPool)
            {
                if (choice == null)
                {
                    continue;
                }

                if (IsChoiceEligible(choice, includeMovementChoice, includeSystemChoice))
                {
                    if (choice.Category == UpgradeCategory.Movement || IsMovementBoost(choice))
                    {
                        movementEligible.Add(choice);
                    }
                    else if (choice.Category == UpgradeCategory.Attack || choice.Category == UpgradeCategory.Defense || choice.Category == UpgradeCategory.Boost)
                    {
                        abilityEligible.Add(choice);
                    }
                    else
                    {
                        systemEligible.Add(choice);
                    }
                }
            }

            int eligibleCount = movementEligible.Count + abilityEligible.Count + systemEligible.Count;
            int lowerChoiceCount = Mathf.Clamp(minimumChoices, 1, Mathf.Max(1, maximumChoices));
            int upperChoiceCount = Mathf.Max(lowerChoiceCount, maximumChoices);
            int desiredMovementChoices = includeMovementChoice ? Mathf.Min(Mathf.Max(1, movementChoicesOnMovementDraft), movementEligible.Count) : 0;
            int targetCount = Mathf.Min(UnityEngine.Random.Range(lowerChoiceCount, upperChoiceCount + 1), eligibleCount);
            targetCount = Mathf.Min(eligibleCount, Mathf.Max(targetCount, desiredMovementChoices));

            while (movementEligible.Count > 0 && currentChoices.Count < desiredMovementChoices)
            {
                AddRandomChoice(movementEligible);
            }

            int reservedSystemSlots = systemEligible.Count > 0 && currentChoices.Count < targetCount ? 1 : 0;
            while (abilityEligible.Count > 0 && currentChoices.Count < targetCount - reservedSystemSlots)
            {
                AddRandomChoice(abilityEligible);
            }

            while (systemEligible.Count > 0 && currentChoices.Count < targetCount)
            {
                AddRandomChoice(systemEligible);
            }

            while (abilityEligible.Count > 0 && currentChoices.Count < targetCount)
            {
                AddRandomChoice(abilityEligible);
            }

            while (movementEligible.Count > 0 && currentChoices.Count < targetCount)
            {
                AddRandomChoice(movementEligible);
            }
        }

        private void AddRandomChoice(List<DraftChoice> choices)
        {
            int index = UnityEngine.Random.Range(0, choices.Count);
            currentChoices.Add(choices[index]);
            choices.RemoveAt(index);
        }

        private void RemoveChoicesInLane(DraftChoice selected)
        {
            ChoiceLane selectedLane = ResolveLane(selected);
            currentChoices.RemoveAll(choice => ResolveLane(choice) == selectedLane);
        }

        private enum ChoiceLane
        {
            Ability,
            Movement,
            System
        }

        private static ChoiceLane ResolveLane(DraftChoice choice)
        {
            if (choice.Category == UpgradeCategory.Movement)
            {
                return ChoiceLane.Movement;
            }

            if (choice.Category == UpgradeCategory.System)
            {
                return ChoiceLane.System;
            }

            return ChoiceLane.Ability;
        }

        private bool IsChoiceEligible(DraftChoice choice, bool includeMovementChoice, bool includeSystemChoice)
        {
            if (choice.Category == UpgradeCategory.Movement)
            {
                return includeMovementChoice && !IsMovementEquipped(choice.Id);
            }

            if (choice.Category == UpgradeCategory.Boost)
            {
                if (IsMovementBoost(choice) && !includeMovementChoice)
                {
                    return false;
                }

                if (IsSystemSynergyBoost(choice))
                {
                    string[] prerequisites = ResolveSystemSynergyPrerequisites(choice.Id);
                    string synergyId = ResolveBoostedSkillId(choice.Id);
                    return prerequisites.Length == 2
                        && buildState != null
                        && buildState.HasSkill(prerequisites[0])
                        && buildState.HasSkill(prerequisites[1])
                        && buildState.GetSkillPathTier(synergyId, choice.Id) < 5;
                }

                string boostedSkillId = ResolveBoostedSkillId(choice.Id);
                if (string.IsNullOrEmpty(boostedSkillId) || buildState == null || !buildState.HasSkill(boostedSkillId))
                {
                    return false;
                }

                return buildState.GetSkillPathTier(boostedSkillId, choice.Id) < 5;
            }

            if (choice.Category == UpgradeCategory.System)
            {
                return includeSystemChoice && (buildState == null || !buildState.HasSkill(choice.Id));
            }

            if (choice.Category == UpgradeCategory.Attack)
            {
                return buildState == null || !buildState.HasSkill(choice.Id);
            }

            return true;
        }

        private static bool IsMovementBoost(DraftChoice choice)
        {
            return choice.Category == UpgradeCategory.Boost && ResolveBoostedSkillId(choice.Id).StartsWith("movement_");
        }

        private static bool IsSystemSynergyBoost(DraftChoice choice)
        {
            return choice.Category == UpgradeCategory.Boost && choice.Id.StartsWith("upgrade_system_synergy_");
        }

        private bool IsMovementEquipped(string choiceId)
        {
            if (buildState == null || buildState.EquippedMovementSkills == null)
            {
                return false;
            }

            MovementCaster.MovementSkillId skillId = ResolveMovementSkillId(choiceId);
            if (skillId == MovementCaster.MovementSkillId.None)
            {
                return false;
            }

            foreach (MovementCaster.MovementSkillId equippedSkill in buildState.EquippedMovementSkills)
            {
                if (equippedSkill == skillId)
                {
                    return true;
                }
            }

            return false;
        }

        private static MovementCaster.MovementSkillId ResolveMovementSkillId(string choiceId)
        {
            switch (choiceId)
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

        private static string ResolveBoostedSkillId(string choiceId)
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
    }
}
