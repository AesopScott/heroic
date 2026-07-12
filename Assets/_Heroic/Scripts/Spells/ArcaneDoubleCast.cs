using UnityEngine;
using System;
using System.Collections;

namespace Heroic.Spells
{
    public class ArcaneDoubleCast : MonoBehaviour
    {
        [SerializeField] private float chance = 0.15f;
        [SerializeField] private float delay = 0.12f;

        public bool TrySchedule(Action castAction)
        {
            if (castAction == null || UnityEngine.Random.value > chance)
            {
                return false;
            }

            StartCoroutine(DoubleCastRoutine(castAction));
            return true;
        }

        private IEnumerator DoubleCastRoutine(Action castAction)
        {
            yield return new WaitForSeconds(delay);
            castAction.Invoke();
        }
    }
}
