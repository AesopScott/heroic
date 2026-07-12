using UnityEngine;

namespace Heroic.Player
{
    public class PlayerPickupMagnet : MonoBehaviour
    {
        [SerializeField] private float pickupRange = 20f;

        public float PickupRange => pickupRange;

        public void SetPickupRange(float value)
        {
            pickupRange = Mathf.Max(0f, value);
        }
    }
}
