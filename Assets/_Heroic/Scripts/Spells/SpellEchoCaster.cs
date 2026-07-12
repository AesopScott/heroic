using UnityEngine;
using System;
using System.Collections;

namespace Heroic.Spells
{
    public class SpellEchoCaster : MonoBehaviour
    {
        [SerializeField] private int echoCount = 1;
        [SerializeField] private float echoDelay = 0.35f;
        [SerializeField] private bool echoEnabled;

        public void Echo(Action castAction)
        {
            if (!echoEnabled || castAction == null || echoCount <= 0)
            {
                return;
            }

            StartCoroutine(EchoRoutine(castAction));
        }

        private IEnumerator EchoRoutine(Action castAction)
        {
            for (int i = 0; i < echoCount; i++)
            {
                yield return new WaitForSeconds(echoDelay);
                castAction.Invoke();
            }
        }

        public void SetEchoEnabled(bool enabled)
        {
            echoEnabled = enabled;
        }

        public void SetEchoCount(int value)
        {
            echoCount = Mathf.Max(0, value);
        }

        public void SetEchoDelay(float value)
        {
            echoDelay = Mathf.Max(0.05f, value);
        }
    }
}
