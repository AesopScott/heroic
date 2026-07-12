using Heroic.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Heroic.UI
{
    public class DraftPresenter : MonoBehaviour
    {
        [SerializeField] private UpgradeManager upgradeManager;
        [SerializeField] private Button[] choiceButtons = new Button[0];
        [SerializeField] private TMP_Text[] choiceLabels = new TMP_Text[0];
        [SerializeField] private TMP_Text headerText;

        private IReadOnlyList<UpgradeManager.DraftChoice> currentChoices;

        private void Awake()
        {
            if (upgradeManager == null)
            {
                upgradeManager = FindAnyObjectByType<UpgradeManager>();
            }
        }

        private void OnEnable()
        {
            if (upgradeManager != null)
            {
                upgradeManager.DraftOpened += HandleDraftOpened;
                upgradeManager.DraftClosed += HandleDraftClosed;
            }
        }

        private void OnDisable()
        {
            if (upgradeManager != null)
            {
                upgradeManager.DraftOpened -= HandleDraftOpened;
                upgradeManager.DraftClosed -= HandleDraftClosed;
            }
        }

        private void HandleDraftOpened(IReadOnlyList<UpgradeManager.DraftChoice> choices, bool includesMovement)
        {
            currentChoices = choices;

            if (headerText != null)
            {
                headerText.text = includesMovement ? "Choose an upgrade or movement" : "Choose an upgrade";
            }

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (choiceButtons[i] == null)
                {
                    continue;
                }

                bool hasChoice = choices != null && i < choices.Count;
                choiceButtons[i].gameObject.SetActive(hasChoice);

                if (!hasChoice)
                {
                    continue;
                }

                int capturedIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => SelectChoice(capturedIndex));

                if (i < choiceLabels.Length && choiceLabels[i] != null)
                {
                    choiceLabels[i].text = FormatChoiceLabel(choices[i]);
                }
            }
        }

        private void HandleDraftClosed()
        {
            currentChoices = null;
        }

        private void SelectChoice(int index)
        {
            if (upgradeManager == null || currentChoices == null || index < 0 || index >= currentChoices.Count)
            {
                return;
            }

            upgradeManager.ApplyChoice(currentChoices[index].Id);
        }

        private static string FormatChoiceLabel(UpgradeManager.DraftChoice choice)
        {
            string category = choice.Category.ToString().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(choice.Description))
            {
                return $"<size=82%><color=#87C8FF>{category}</color></size>\n<b>{choice.DisplayName}</b>";
            }

            return $"<size=82%><color=#87C8FF>{category}</color></size>\n<b>{choice.DisplayName}</b>\n<size=86%><color=#C7E6F5>{choice.Description}</color></size>";
        }
    }
}
