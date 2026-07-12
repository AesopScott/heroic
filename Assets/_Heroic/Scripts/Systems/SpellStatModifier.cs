using UnityEngine;

namespace Heroic.Systems
{
    public class SpellStatModifier : MonoBehaviour
    {
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float rangeMultiplier = 1f;
        [SerializeField] private float recoveryMultiplier = 1f;

        public float DamageMultiplier => damageMultiplier;
        public float RangeMultiplier => rangeMultiplier;
        public float RecoveryMultiplier => recoveryMultiplier;

        public void SetTerritoryMultipliers(float damage, float range, float recovery)
        {
            damageMultiplier = Mathf.Max(0.05f, damage);
            rangeMultiplier = Mathf.Max(0.05f, range);
            recoveryMultiplier = Mathf.Max(0.05f, recovery);
        }

        public int ModifyDamage(int baseDamage)
        {
            return Mathf.Max(0, Mathf.RoundToInt(baseDamage * damageMultiplier));
        }

        public float ModifyRange(float baseRange)
        {
            return Mathf.Max(0f, baseRange * rangeMultiplier);
        }

        public float ModifyCooldown(float baseCooldown)
        {
            return Mathf.Max(0.05f, baseCooldown / recoveryMultiplier);
        }
    }
}
