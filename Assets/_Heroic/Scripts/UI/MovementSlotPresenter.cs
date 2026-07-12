using Heroic.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroic.UI
{
    public class MovementSlotPresenter : MonoBehaviour
    {
        [SerializeField] private MovementCaster movementCaster;
        [SerializeField] private int slotIndex;
        [SerializeField] private TMP_Text skillNameText;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private Image cooldownFill;

        private void Update()
        {
            if (movementCaster == null)
            {
                return;
            }

            MovementCaster.MovementSkillId skill = movementCaster.GetEquippedSkill(slotIndex);
            float remaining = movementCaster.GetRemainingCooldown(slotIndex);
            float cooldown = movementCaster.GetCooldown(slotIndex);

            if (skillNameText != null)
            {
                skillNameText.text = skill == MovementCaster.MovementSkillId.None ? "-" : skill.ToString();
            }

            if (cooldownText != null)
            {
                cooldownText.text = remaining > 0f ? remaining.ToString("0.0") : string.Empty;
            }

            if (cooldownFill != null)
            {
                cooldownFill.fillAmount = cooldown > 0f ? Mathf.Clamp01(remaining / cooldown) : 0f;
            }
        }
    }
}
