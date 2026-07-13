using Heroic.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Heroic.Systems
{
    public class MagicSystemController : MonoBehaviour
    {
        [SerializeField] private SpellStatModifier spellStats;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerController playerController;

        private bool componentBoostsEnabled;
        private bool sacrificeCastingEnabled;
        private bool rhythmCastingEnabled;
        private bool spellTensionEnabled;

        private float componentDamageBonus = 0.1f;
        private float componentRangeBonus = 0.08f;
        private float componentRecoveryBonus = 0.05f;

        private float sacrificeDamageBonus = 0.25f;
        private float sacrificeRecoveryBonus = 0.08f;
        private float sacrificeSpeedPenalty = 0.06f;
        private int sacrificeHealthCost = 18;

        private float rhythmDamageBonus = 0.08f;
        private float rhythmRangeBonus = 0.04f;
        private float rhythmRecoveryBonus = 0.16f;

        private float tensionDamageBonus = 0.22f;
        private float tensionRangeBonus = 0.1f;
        private float tensionRecoveryDebt = 0.12f;

        private readonly Dictionary<string, SynergyBonus> synergyBonuses = new Dictionary<string, SynergyBonus>();
        private int baseMaxHealth;
        private float baseMoveSpeed;

        private struct SynergyBonus
        {
            public float Damage;
            public float Range;
            public float Recovery;
            public float MoveSpeed;
            public int HealthRefund;
        }

        private void Awake()
        {
            spellStats ??= FindAnyObjectByType<SpellStatModifier>();
            playerHealth ??= FindAnyObjectByType<PlayerHealth>();
            playerController ??= FindAnyObjectByType<PlayerController>();

            if (playerHealth != null)
            {
                baseMaxHealth = playerHealth.MaxHealth;
            }

            if (playerController != null)
            {
                baseMoveSpeed = playerController.BaseMoveSpeed;
            }
        }

        public void EnableSystem(string systemId)
        {
            switch (systemId)
            {
                case "system_component_boosts":
                    componentBoostsEnabled = true;
                    break;
                case "system_sacrifice_casting":
                    sacrificeCastingEnabled = true;
                    break;
                case "system_rhythm_casting":
                    rhythmCastingEnabled = true;
                    break;
                case "system_spell_tension":
                    spellTensionEnabled = true;
                    break;
            }

            ApplySystemStats();
        }

        public void ApplyUpgrade(string choiceId, int tier)
        {
            if (SystemPairDefinitions.IsPairUpgrade(choiceId))
            {
                ApplySystemPairUpgrade(choiceId, tier);
                ApplySystemStats();
                return;
            }

            switch (choiceId)
            {
                case "upgrade_system_component_boosts_potent_components":
                    componentDamageBonus = Value(tier, 0.16f, 0.24f, 0.34f, 0.46f, 0.6f);
                    break;
                case "upgrade_system_component_boosts_extended_components":
                    componentRangeBonus = Value(tier, 0.13f, 0.2f, 0.29f, 0.4f, 0.54f);
                    break;
                case "upgrade_system_component_boosts_efficient_components":
                    componentRecoveryBonus = Value(tier, 0.1f, 0.16f, 0.23f, 0.32f, 0.44f);
                    break;
                case "upgrade_system_sacrifice_casting_deeper_sacrifice":
                    sacrificeDamageBonus = Value(tier, 0.34f, 0.48f, 0.66f, 0.88f, 1.15f);
                    break;
                case "upgrade_system_sacrifice_casting_quick_offering":
                    sacrificeRecoveryBonus = Value(tier, 0.14f, 0.22f, 0.32f, 0.44f, 0.6f);
                    break;
                case "upgrade_system_sacrifice_casting_controlled_cost":
                    sacrificeHealthCost = Value(tier, 16, 14, 12, 9, 6);
                    sacrificeSpeedPenalty = Value(tier, 0.05f, 0.04f, 0.03f, 0.02f, 0f);
                    break;
                case "upgrade_system_rhythm_casting_steady_rhythm":
                    rhythmRecoveryBonus = Value(tier, 0.24f, 0.34f, 0.46f, 0.6f, 0.78f);
                    break;
                case "upgrade_system_rhythm_casting_perfect_beat":
                    rhythmDamageBonus = Value(tier, 0.14f, 0.22f, 0.32f, 0.44f, 0.6f);
                    break;
                case "upgrade_system_rhythm_casting_clean_timing":
                    rhythmRangeBonus = Value(tier, 0.1f, 0.16f, 0.24f, 0.34f, 0.46f);
                    break;
                case "upgrade_system_spell_tension_charged_incantations":
                    tensionDamageBonus = Value(tier, 0.32f, 0.46f, 0.64f, 0.86f, 1.12f);
                    break;
                case "upgrade_system_spell_tension_longer_chants":
                    tensionRangeBonus = Value(tier, 0.18f, 0.28f, 0.4f, 0.54f, 0.72f);
                    break;
                case "upgrade_system_spell_tension_debt_control":
                    tensionRecoveryDebt = Value(tier, 0.1f, 0.08f, 0.05f, 0.02f, 0f);
                    break;
            }

            ApplySystemStats();
        }

        private void ApplySystemPairUpgrade(string choiceId, int tier)
        {
            if (choiceId.EndsWith("_amplify"))
            {
                SetSynergy(choiceId, tier, 0.05f, 0.09f, 0.14f, 0.2f, 0.28f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
                return;
            }

            if (choiceId.EndsWith("_extend"))
            {
                SetSynergy(choiceId, tier, 0f, 0f, 0f, 0f, 0f, 0.05f, 0.09f, 0.14f, 0.2f, 0.28f, 0f, 0f, 0f, 0f, 0f);
                return;
            }

            if (choiceId.EndsWith("_recover"))
            {
                SetSynergy(choiceId, tier, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.04f, 0.08f, 0.12f, 0.17f, 0.24f);
            }
        }

        private void ApplySystemStats()
        {
            float damage = 1f;
            float range = 1f;
            float recovery = 1f;
            float moveSpeed = 1f;
            int healthCost = 0;

            if (componentBoostsEnabled)
            {
                damage += componentDamageBonus;
                range += componentRangeBonus;
                recovery += componentRecoveryBonus;
            }

            if (sacrificeCastingEnabled)
            {
                damage += sacrificeDamageBonus;
                recovery += sacrificeRecoveryBonus;
                moveSpeed -= sacrificeSpeedPenalty;
                healthCost += sacrificeHealthCost;
            }

            if (rhythmCastingEnabled)
            {
                damage += rhythmDamageBonus;
                range += rhythmRangeBonus;
                recovery += rhythmRecoveryBonus;
            }

            if (spellTensionEnabled)
            {
                damage += tensionDamageBonus;
                range += tensionRangeBonus;
                recovery -= tensionRecoveryDebt;
            }

            foreach (SynergyBonus bonus in synergyBonuses.Values)
            {
                damage += bonus.Damage;
                range += bonus.Range;
                recovery += bonus.Recovery;
                moveSpeed += bonus.MoveSpeed;
                healthCost -= bonus.HealthRefund;
            }

            spellStats?.SetSystemMultipliers(damage, range, Mathf.Max(0.1f, recovery));

            if (playerHealth != null && baseMaxHealth > 0)
            {
                playerHealth.SetMaxHealth(Mathf.Max(1, baseMaxHealth - healthCost));
            }

            if (playerController != null && baseMoveSpeed > 0f)
            {
                playerController.SetBaseMoveSpeed(baseMoveSpeed * Mathf.Max(0.25f, moveSpeed));
            }
        }

        private void SetSynergy(
            string choiceId,
            int tier,
            float damageBasic,
            float damageAdvanced,
            float damageExpert,
            float damageMaster,
            float damageGrandmaster,
            float rangeBasic,
            float rangeAdvanced,
            float rangeExpert,
            float rangeMaster,
            float rangeGrandmaster,
            float recoveryBasic,
            float recoveryAdvanced,
            float recoveryExpert,
            float recoveryMaster,
            float recoveryGrandmaster,
            float moveSpeed = 0f,
            int healthBasic = 0,
            int healthAdvanced = 0,
            int healthExpert = 0,
            int healthMaster = 0,
            int healthGrandmaster = 0)
        {
            synergyBonuses[choiceId] = new SynergyBonus
            {
                Damage = Value(tier, damageBasic, damageAdvanced, damageExpert, damageMaster, damageGrandmaster),
                Range = Value(tier, rangeBasic, rangeAdvanced, rangeExpert, rangeMaster, rangeGrandmaster),
                Recovery = Value(tier, recoveryBasic, recoveryAdvanced, recoveryExpert, recoveryMaster, recoveryGrandmaster),
                MoveSpeed = moveSpeed,
                HealthRefund = Value(tier, healthBasic, healthAdvanced, healthExpert, healthMaster, healthGrandmaster)
            };
        }

        private static float Value(int tier, float basic, float advanced, float expert, float master, float grandmaster)
        {
            switch (Mathf.Clamp(tier, 1, 5))
            {
                case 1:
                    return basic;
                case 2:
                    return advanced;
                case 3:
                    return expert;
                case 4:
                    return master;
                default:
                    return grandmaster;
            }
        }

        private static int Value(int tier, int basic, int advanced, int expert, int master, int grandmaster)
        {
            switch (Mathf.Clamp(tier, 1, 5))
            {
                case 1:
                    return basic;
                case 2:
                    return advanced;
                case 3:
                    return expert;
                case 4:
                    return master;
                default:
                    return grandmaster;
            }
        }
    }
}
