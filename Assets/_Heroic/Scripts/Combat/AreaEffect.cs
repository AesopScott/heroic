using UnityEngine;

namespace Heroic.Combat
{
    public class AreaEffect : MonoBehaviour
    {
        [SerializeField] private float duration = 2f;

        private void Start()
        {
            Destroy(gameObject, duration);
        }
    }
}
