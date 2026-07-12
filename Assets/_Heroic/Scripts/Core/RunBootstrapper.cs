using UnityEngine;

namespace Heroic.Core
{
    public class RunBootstrapper : MonoBehaviour
    {
        [SerializeField] private RunManager runManager;
        [SerializeField] private bool startRunOnSceneStart = true;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }
        }

        private void Start()
        {
            if (startRunOnSceneStart)
            {
                runManager?.StartRun();
            }
        }
    }
}
