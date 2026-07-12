using UnityEngine;

namespace Heroic.Combat
{
    public class StatusEffect : MonoBehaviour
    {
        [SerializeField] private float duration = 1f;

        private void Start()
        {
            Destroy(gameObject, duration);
        }
    }
}
