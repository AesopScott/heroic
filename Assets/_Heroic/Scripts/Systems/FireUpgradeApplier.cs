using Heroic.Spells;
using UnityEngine;

namespace Heroic.Systems
{
    public class FireUpgradeApplier : MonoBehaviour
    {
        [SerializeField] private FireBoltCaster fireBolt;
        [SerializeField] private FlameWaveCaster flameWave;
        [SerializeField] private BurningGroundCaster burningGround;

        public bool Apply(string choiceId, int tier)
        {
            int clampedTier = Mathf.Clamp(tier, 1, 5);

            switch (choiceId)
            {
                case "upgrade_fire_fire_bolt_power":
                    fireBolt?.SetDamage(Value(clampedTier, 28, 42, 60, 86, 125));
                    return true;
                case "upgrade_fire_fire_bolt_fork":
                    fireBolt?.SetProjectileCount(Value(clampedTier, 2, 3, 4, 5, 7));
                    return true;
                case "upgrade_fire_fire_bolt_pierce":
                    fireBolt?.SetPierceCount(Value(clampedTier, 1, 2, 3, 5, 8));
                    return true;

                case "upgrade_fire_flame_wave_heat":
                    flameWave?.SetDamage(Value(clampedTier, 38, 54, 78, 112, 160));
                    return true;
                case "upgrade_fire_flame_wave_reach":
                    flameWave?.SetRange(Value(clampedTier, 6f, 7.2f, 8.6f, 10.2f, 12.5f));
                    return true;
                case "upgrade_fire_flame_wave_width":
                    flameWave?.SetWidth(Value(clampedTier, 2.8f, 3.4f, 4.1f, 5f, 6.2f));
                    return true;

                case "upgrade_fire_burning_ground_burn":
                    burningGround?.SetDamagePerTick(Value(clampedTier, 12, 18, 26, 38, 56));
                    return true;
                case "upgrade_fire_burning_ground_spread":
                    burningGround?.SetRadius(Value(clampedTier, 1.8f, 2.2f, 2.7f, 3.3f, 4.1f));
                    return true;
                case "upgrade_fire_burning_ground_persist":
                    burningGround?.SetDuration(Value(clampedTier, 3.8f, 4.6f, 5.5f, 6.8f, 8.5f));
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
