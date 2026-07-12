using Heroic.Spells;
using UnityEngine;

namespace Heroic.Systems
{
    public class LightningUpgradeApplier : MonoBehaviour
    {
        [SerializeField] private ChainBoltCaster chainBolt;
        [SerializeField] private StaticFieldCaster staticField;
        [SerializeField] private ThunderLanceCaster thunderLance;
        [SerializeField] private SparkSurgeCaster sparkSurge;
        [SerializeField] private StormCallCaster stormCall;

        public bool Apply(string choiceId, int tier)
        {
            int clampedTier = Mathf.Clamp(tier, 1, 5);

            switch (choiceId)
            {
                case "upgrade_lightning_chain_bolt_more_jumps":
                    chainBolt?.SetJumpCount(Value(clampedTier, 4, 5, 6, 8, 10));
                    return true;
                case "upgrade_lightning_chain_bolt_higher_damage":
                    chainBolt?.SetDamage(Value(clampedTier, 30, 40, 54, 72, 96));
                    return true;
                case "upgrade_lightning_chain_bolt_longer_chain":
                    chainBolt?.SetChainRange(Value(clampedTier, 5.5f, 6.8f, 8.2f, 10f, 12.5f));
                    return true;

                case "upgrade_lightning_static_field_bigger_field":
                    staticField?.SetRadius(Value(clampedTier, 2.2f, 2.7f, 3.3f, 4f, 4.8f));
                    return true;
                case "upgrade_lightning_static_field_faster_ticks":
                    staticField?.SetTickInterval(Value(clampedTier, 0.38f, 0.32f, 0.26f, 0.2f, 0.15f));
                    return true;
                case "upgrade_lightning_static_field_stun_chance":
                    staticField?.SetStunChance(Value(clampedTier, 0.12f, 0.18f, 0.25f, 0.34f, 0.45f));
                    return true;

                case "upgrade_lightning_thunder_lance_piercing_lance":
                    thunderLance?.SetPierceCount(Value(clampedTier, 4, 5, 7, 9, 12));
                    return true;
                case "upgrade_lightning_thunder_lance_wider_lance":
                    thunderLance?.SetWidth(Value(clampedTier, 0.95f, 1.2f, 1.5f, 1.9f, 2.4f));
                    return true;
                case "upgrade_lightning_thunder_lance_critical_strike":
                    thunderLance?.SetIsolatedDamageMultiplier(Value(clampedTier, 1.3f, 1.55f, 1.85f, 2.2f, 2.7f));
                    return true;

                case "upgrade_lightning_spark_surge_more_sparks":
                    sparkSurge?.SetSparkCount(Value(clampedTier, 5, 6, 8, 10, 13));
                    return true;
                case "upgrade_lightning_spark_surge_faster_surge":
                    sparkSurge?.SetSparkDelay(Value(clampedTier, 0.1f, 0.08f, 0.065f, 0.05f, 0.035f));
                    return true;
                case "upgrade_lightning_spark_surge_target_spread":
                    sparkSurge?.SetTargetSpreadRadius(Value(clampedTier, 4.5f, 5.8f, 7.2f, 9f, 11f));
                    return true;

                case "upgrade_lightning_storm_call_more_strikes":
                    stormCall?.SetStrikeCount(Value(clampedTier, 5, 6, 8, 10, 13));
                    return true;
                case "upgrade_lightning_storm_call_faster_strikes":
                    stormCall?.SetStrikeDelay(Value(clampedTier, 0.34f, 0.28f, 0.22f, 0.16f, 0.1f));
                    return true;
                case "upgrade_lightning_storm_call_violent_storm":
                    stormCall?.SetDamage(Value(clampedTier, 38, 50, 68, 92, 125));
                    stormCall?.SetStunChance(Value(clampedTier, 0.22f, 0.3f, 0.4f, 0.52f, 0.68f));
                    return true;
                default:
                    return false;
            }
        }

        private int Value(int tier, int basic, int advanced, int expert, int master, int grandmaster)
        {
            switch (tier)
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

        private float Value(int tier, float basic, float advanced, float expert, float master, float grandmaster)
        {
            switch (tier)
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
