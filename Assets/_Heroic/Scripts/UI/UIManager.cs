using UnityEngine;
using Heroic.Core;

namespace Heroic.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuRoot;
        [SerializeField] private GameObject gameUiRoot;
        [SerializeField] private GameObject resultsRoot;
        [SerializeField] private GameObject levelUpDraftRoot;
        [SerializeField] private GameObject pauseRoot;
        [SerializeField] private RunManager runManager;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }
        }

        private void OnEnable()
        {
            if (runManager != null)
            {
                runManager.StateChanged += HandleStateChanged;
            }
        }

        private void OnDisable()
        {
            if (runManager != null)
            {
                runManager.StateChanged -= HandleStateChanged;
            }
        }

        public void ShowMainMenu()
        {
            SetOnlyActive(mainMenuRoot);
        }

        public void ShowGameUI()
        {
            SetOnlyActive(gameUiRoot);
        }

        public void ShowResults()
        {
            SetOnlyActive(resultsRoot);
        }

        public void ShowLevelUpDraft()
        {
            SetOnlyActive(levelUpDraftRoot);
        }

        public void ShowPause()
        {
            SetOnlyActive(pauseRoot);
        }

        private void HandleStateChanged(RunManager.RunState state)
        {
            switch (state)
            {
                case RunManager.RunState.MainMenu:
                    ShowMainMenu();
                    break;
                case RunManager.RunState.Playing:
                    ShowGameUI();
                    break;
                case RunManager.RunState.LevelUpDraft:
                    ShowLevelUpDraft();
                    break;
                case RunManager.RunState.Paused:
                    ShowPause();
                    break;
                case RunManager.RunState.Results:
                    ShowResults();
                    break;
            }
        }

        private void SetOnlyActive(GameObject activeRoot)
        {
            SetRootActive(mainMenuRoot, mainMenuRoot == activeRoot);
            SetRootActive(gameUiRoot, gameUiRoot == activeRoot);
            SetRootActive(resultsRoot, resultsRoot == activeRoot);
            SetRootActive(levelUpDraftRoot, levelUpDraftRoot == activeRoot);
            SetRootActive(pauseRoot, pauseRoot == activeRoot);
        }

        private void SetRootActive(GameObject root, bool active)
        {
            if (root != null)
            {
                root.SetActive(active);
            }
        }
    }
}
