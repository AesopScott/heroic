using UnityEngine;
using System;
using System.Collections.Generic;
using Heroic.Core;

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

            if (usePrototypeDraftPoolWhenEmpty && (draftPool == null || draftPool.Length == 0))
            {
                draftPool = PrototypeDraftChoices.Create();
            }
        }

        public void OpenDraft(int playerLevel, bool includeMovementChoice)
        {
            IsDraftOpen = true;
            CurrentDraftIncludesMovement = includeMovementChoice;
            BuildChoices(includeMovementChoice);
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

        private void BuildChoices(bool includeMovementChoice)
        {
            currentChoices.Clear();

            List<DraftChoice> eligible = new List<DraftChoice>();
            foreach (DraftChoice choice in draftPool)
            {
                if (choice == null)
                {
                    continue;
                }

                if (choice.Category == UpgradeCategory.Movement && !includeMovementChoice)
                {
                    continue;
                }

                eligible.Add(choice);
            }

            int lowerChoiceCount = Mathf.Clamp(minimumChoices, 1, Mathf.Max(1, maximumChoices));
            int upperChoiceCount = Mathf.Max(lowerChoiceCount, maximumChoices);
            int targetCount = Mathf.Min(UnityEngine.Random.Range(lowerChoiceCount, upperChoiceCount + 1), eligible.Count);
            while (eligible.Count > 0 && currentChoices.Count < targetCount)
            {
                int index = UnityEngine.Random.Range(0, eligible.Count);
                currentChoices.Add(eligible[index]);
                eligible.RemoveAt(index);
            }
        }
    }
}
