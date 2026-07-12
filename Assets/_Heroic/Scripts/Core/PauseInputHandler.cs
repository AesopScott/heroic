using UnityEngine;

namespace Heroic.Core
{
    public class PauseInputHandler : MonoBehaviour
    {
        [SerializeField] private RunManager runManager;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape) || runManager == null)
            {
                return;
            }

            if (runManager.CurrentState == RunManager.RunState.Playing)
            {
                runManager.PauseRun();
            }
            else if (runManager.CurrentState == RunManager.RunState.Paused)
            {
                runManager.ResumeRun();
            }
        }
    }
}
