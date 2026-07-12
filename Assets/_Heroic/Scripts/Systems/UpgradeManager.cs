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

        [Serializable]
        public class DraftChoice
        {
            [SerializeField] private string id;
            [SerializeField] private string displayName;
            [TextArea] [SerializeField] private string description;
            [SerializeField] private UpgradeCategory category;

            public string Id => id;
            public string DisplayName => displayName;
            public string Description => description;
            public UpgradeCategory Category => category;

            public DraftChoice()
            {
            }

            public DraftChoice(string id, string displayName, string description, UpgradeCategory category)
            {
                this.id = id;
                this.displayName = displayName;
                this.description = description;
                this.category = category;
            }
        }

        [SerializeField] private DraftChoice[] draftPool = new DraftChoice[0];
        [SerializeField] private bool usePrototypeDraftPoolWhenEmpty = true;
        [SerializeField] private int minimumChoices = 3;
        [SerializeField] private int maximumChoices = 5;
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
            CloseDraft();
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
                    if (choice.Category == UpgradeCategory.Movement)
                    {
                        movementEligible.Add(choice);
                    }
                    else if (choice.Category == UpgradeCategory.Attack || choice.Category == UpgradeCategory.Defense)
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
            int targetCount = Mathf.Min(UnityEngine.Random.Range(lowerChoiceCount, upperChoiceCount + 1), eligibleCount);

            if (includeMovementChoice && movementEligible.Count > 0 && targetCount > 0)
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

        private bool IsChoiceEligible(DraftChoice choice, bool includeMovementChoice, bool includeSystemChoice)
        {
            if (choice.Category == UpgradeCategory.Movement)
            {
                return includeMovementChoice && !IsMovementEquipped(choice.Id);
            }

            if (choice.Category == UpgradeCategory.Boost)
            {
                if (!includeSystemChoice)
                {
                    return false;
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
                return includeSystemChoice;
            }

            if (choice.Category == UpgradeCategory.Attack)
            {
                return buildState == null || !buildState.HasSkill(choice.Id);
            }

            return true;
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

            if (choiceId.StartsWith("upgrade_movement_cloud_walk"))
            {
                return "movement_cloud_walk";
            }

            return string.Empty;
        }
    }
}
