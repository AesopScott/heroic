using UnityEngine;

namespace Heroic.Core
{
    public class DemoSafetyHotkeys : MonoBehaviour
    {
        [SerializeField] private RunManager runManager;
        [SerializeField] private bool enabledForPrototype = true;
        [SerializeField] private KeyCode forceDefeatKey = KeyCode.F8;
        [SerializeField] private KeyCode forceVictoryKey = KeyCode.F9;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }
        }

        private void Update()
        {
            if (!enabledForPrototype || runManager == null || !CanForceRunEnd())
            {
                return;
            }

            if (Input.GetKeyDown(forceDefeatKey))
            {
                runManager.EndRun(false);
            }
            else if (Input.GetKeyDown(forceVictoryKey))
            {
                runManager.EndRun(true);
            }
        }

        private bool CanForceRunEnd()
        {
            return runManager.CurrentState == RunManager.RunState.Playing
                || runManager.CurrentState == RunManager.RunState.LevelUpDraft
                || runManager.CurrentState == RunManager.RunState.Paused;
        }
    }
}
