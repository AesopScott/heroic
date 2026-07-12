using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Heroic.Core
{
    public class RunManager : MonoBehaviour
    {
        public enum RunState
        {
            MainMenu,
            Playing,
            LevelUpDraft,
            Paused,
            Results
        }

        [SerializeField] private RunState currentState = RunState.MainMenu;
        [SerializeField] private string gameSceneName = "Game";
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string resultsSceneName = "Results";
        [SerializeField] private bool useSceneTransitions;

        private float runStartedAt;
        private float finalRunTime;
        private bool runTimerActive;

        public event Action<RunState> StateChanged;
        public event Action<float> RunTimeChanged;
        public event Action<bool, float> RunEnded;

        public RunState CurrentState => currentState;
        public float RunTimeSeconds => runTimerActive ? Time.time - runStartedAt : finalRunTime;
        public bool WasVictory { get; private set; }

        public void StartRun()
        {
            runStartedAt = Time.time;
            finalRunTime = 0f;
            runTimerActive = true;
            WasVictory = false;
            SetState(RunState.Playing);
        }

        private void Update()
        {
            if (currentState == RunState.Playing && runTimerActive)
            {
                RunTimeChanged?.Invoke(RunTimeSeconds);
            }
        }

        public void EndRun()
        {
            EndRun(false);
        }

        public void EndRun(bool victory)
        {
            if (currentState == RunState.Results)
            {
                return;
            }

            WasVictory = victory;
            finalRunTime = RunTimeSeconds;
            runTimerActive = false;
            Time.timeScale = 1f;
            SetState(RunState.Results);
            RunEnded?.Invoke(victory, finalRunTime);

            if (useSceneTransitions && !string.IsNullOrEmpty(resultsSceneName))
            {
                SceneManager.LoadScene(resultsSceneName);
            }
        }

        public void PauseRun()
        {
            if (currentState != RunState.Playing)
            {
                return;
            }

            Time.timeScale = 0f;
            SetState(RunState.Paused);
        }

        public void ResumeRun()
        {
            if (currentState != RunState.Paused && currentState != RunState.LevelUpDraft)
            {
                return;
            }

            Time.timeScale = 1f;
            SetState(RunState.Playing);
        }

        public void OpenLevelUpDraft()
        {
            if (currentState != RunState.Playing)
            {
                return;
            }

            Time.timeScale = 0f;
            SetState(RunState.LevelUpDraft);
        }

        public void RestartRun()
        {
            Time.timeScale = 1f;
            if (useSceneTransitions && !string.IsNullOrEmpty(gameSceneName))
            {
                SceneManager.LoadScene(gameSceneName);
                return;
            }

            StartRun();
        }

        public void QuitToMainMenu()
        {
            Time.timeScale = 1f;
            if (useSceneTransitions && !string.IsNullOrEmpty(mainMenuSceneName))
            {
                SceneManager.LoadScene(mainMenuSceneName);
                return;
            }

            SetState(RunState.MainMenu);
        }

        private void SetState(RunState newState)
        {
            if (currentState == newState)
            {
                return;
            }

            currentState = newState;
            StateChanged?.Invoke(currentState);
        }
    }
}
