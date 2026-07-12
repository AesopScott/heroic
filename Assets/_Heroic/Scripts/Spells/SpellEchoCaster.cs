using UnityEngine;
using System;
using System.Collections;
using Heroic.Visuals;

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
                TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.72f, 0.92f, 1f, 0.24f), 1.05f + i * 0.12f, 0.16f);
                castAction.Invoke();
            }
        }

        public void SetEchoEnabled(bool enabled)
        {
            echoEnabled = enabled;
            if (echoEnabled)
            {
                TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.58f, 0.85f, 1f, 0.36f), 1.25f, 0.22f);
            }
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
