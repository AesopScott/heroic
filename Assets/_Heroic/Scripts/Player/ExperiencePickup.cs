using UnityEngine;
using System;

namespace Heroic.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class ExperiencePickup : MonoBehaviour
    {
        [SerializeField] private int experienceValue = 1;
        [SerializeField] private float magnetRange = 7f;
        [SerializeField] private float magnetSpeed = 11f;

        private Transform target;
        private PlayerPickupMagnet targetMagnet;

        public event Action<ExperiencePickup> Collected;

        public void SetExperienceValue(int value)
        {
            experienceValue = Mathf.Max(1, value);
        }

        private void Update()
        {
            if (target == null)
            {
                FindTarget();
            }

            if (target == null)
            {
                return;
            }

            float distance = Vector2.Distance(transform.position, target.position);
            float activeMagnetRange = targetMagnet != null ? targetMagnet.PickupRange : magnetRange;
            if (distance <= activeMagnetRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, magnetSpeed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var playerExperience = other.GetComponent<PlayerExperience>();
            if (playerExperience == null)
            {
                return;
            }

            playerExperience.AddExperience(experienceValue);
            Collected?.Invoke(this);
            Destroy(gameObject);
        }

        private void FindTarget()
        {
            PlayerExperience playerExperience = FindAnyObjectByType<PlayerExperience>();
            if (playerExperience != null)
            {
                target = playerExperience.transform;
                targetMagnet = playerExperience.GetComponent<PlayerPickupMagnet>();
            }
        }
    }
}
