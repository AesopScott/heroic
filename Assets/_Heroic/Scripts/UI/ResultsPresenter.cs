using Heroic.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heroic.UI
{
    public class ResultsPresenter : MonoBehaviour
    {
        [SerializeField] private RunManager runManager;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text summaryText;
        [SerializeField] private bool useSceneTransitionsForButtons = true;
        [SerializeField] private string gameSceneName = "Game";
        [SerializeField] private string mainMenuSceneName = "MainMenu";

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
                runManager.RunEnded += HandleRunEnded;
            }
        }

        private void OnDisable()
        {
            if (runManager != null)
            {
                runManager.RunEnded -= HandleRunEnded;
            }
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            if (useSceneTransitionsForButtons && !string.IsNullOrWhiteSpace(gameSceneName))
            {
                SceneManager.LoadScene(gameSceneName);
                return;
            }

            runManager?.RestartRun();
        }

        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            if (useSceneTransitionsForButtons && !string.IsNullOrWhiteSpace(mainMenuSceneName))
            {
                SceneManager.LoadScene(mainMenuSceneName);
                return;
            }

            runManager?.QuitToMainMenu();
        }

        private void HandleRunEnded(bool victory, float seconds)
        {
            if (resultText != null)
            {
                resultText.text = victory ? "ARCANE WARDEN DEFEATED" : "SPELLBOOK SHATTERED";
            }

            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(seconds / 60f);
                int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
                timeText.text = $"Run Time  {minutes:00}:{remainingSeconds:00}";
            }

            if (summaryText != null)
            {
                summaryText.text = victory
                    ? "The living spellbook survived the arena and ended the Warden's pressure."
                    : "The Warden overwhelmed the spellbook. Choose a sharper build and run it back.";
            }
        }
    }
}
