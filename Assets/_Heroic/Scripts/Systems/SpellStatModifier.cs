using UnityEngine;

namespace Heroic.Systems
{
    public class SpellStatModifier : MonoBehaviour
    {
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float rangeMultiplier = 1f;
        [SerializeField] private float recoveryMultiplier = 1f;

        private float territoryDamageMultiplier = 1f;
        private float territoryRangeMultiplier = 1f;
        private float territoryRecoveryMultiplier = 1f;
        private float systemDamageMultiplier = 1f;
        private float systemRangeMultiplier = 1f;
        private float systemRecoveryMultiplier = 1f;

        public float DamageMultiplier => damageMultiplier;
        public float RangeMultiplier => rangeMultiplier;
        public float RecoveryMultiplier => recoveryMultiplier;

        public void SetTerritoryMultipliers(float damage, float range, float recovery)
        {
            territoryDamageMultiplier = Mathf.Max(0.05f, damage);
            territoryRangeMultiplier = Mathf.Max(0.05f, range);
            territoryRecoveryMultiplier = Mathf.Max(0.05f, recovery);
            RecalculateMultipliers();
        }

        public void SetSystemMultipliers(float damage, float range, float recovery)
        {
            systemDamageMultiplier = Mathf.Max(0.05f, damage);
            systemRangeMultiplier = Mathf.Max(0.05f, range);
            systemRecoveryMultiplier = Mathf.Max(0.05f, recovery);
            RecalculateMultipliers();
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

        private void RecalculateMultipliers()
        {
            damageMultiplier = territoryDamageMultiplier * systemDamageMultiplier;
            rangeMultiplier = territoryRangeMultiplier * systemRangeMultiplier;
            recoveryMultiplier = territoryRecoveryMultiplier * systemRecoveryMultiplier;
        }
    }
}
