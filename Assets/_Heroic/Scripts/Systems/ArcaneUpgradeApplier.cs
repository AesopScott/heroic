using Heroic.Spells;
using UnityEngine;

namespace Heroic.Systems
{
    public class ArcaneUpgradeApplier : MonoBehaviour
    {
        [SerializeField] private MagicMissileCaster magicMissile;
        [SerializeField] private ArcaneBlastCaster arcaneBlast;
        [SerializeField] private WarpPulseCaster warpPulse;
        [SerializeField] private SpellEchoCaster spellEcho;
        [SerializeField] private ArcaneOrbitCaster arcaneOrbit;
        [SerializeField] private ArcaneUtilityCaster forceField;
        [SerializeField] private ArcaneUtilityCaster timeWarp;
        [SerializeField] private ArcaneUtilityCaster haste;

        public bool Apply(string choiceId, int tier)
        {
            int clampedTier = Mathf.Clamp(tier, 1, 5);

            switch (choiceId)
            {
                case "upgrade_arcane_magic_missile_split_shot":
                    magicMissile?.SetProjectileCount(Value(clampedTier, 2, 3, 4, 5, 7));
                    return true;
                case "upgrade_arcane_magic_missile_seeking_shot":
                    magicMissile?.SetHomingStrength(Value(clampedTier, 2f, 3.5f, 5f, 7f, 10f));
                    return true;
                case "upgrade_arcane_magic_missile_arcane_pierce":
                    magicMissile?.SetPierceCount(Value(clampedTier, 1, 2, 3, 5, 8));
                    return true;

                case "upgrade_arcane_arcane_blast_power":
                    arcaneBlast?.SetDamage(Value(clampedTier, 30, 45, 65, 90, 130));
                    return true;
                case "upgrade_arcane_arcane_blast_reach":
                    arcaneBlast?.SetRange(Value(clampedTier, 9f, 11f, 13f, 16f, 20f));
                    return true;
                case "upgrade_arcane_arcane_blast_scatter":
                    arcaneBlast?.SetScatterCount(Value(clampedTier, 1, 2, 3, 4, 6));
                    return true;

                case "upgrade_arcane_warp_pulse_push":
                    warpPulse?.SetMode(WarpPulseCaster.WarpMode.Push);
                    warpPulse?.SetDisplacementDistance(Value(clampedTier, 2f, 2.75f, 3.5f, 4.5f, 6f));
                    return true;
                case "upgrade_arcane_warp_pulse_pull":
                    warpPulse?.SetMode(WarpPulseCaster.WarpMode.Pull);
                    warpPulse?.SetDisplacementDistance(Value(clampedTier, 2f, 2.75f, 3.5f, 4.5f, 6f));
                    return true;
                case "upgrade_arcane_warp_pulse_slow_warp":
                    warpPulse?.SetMode(WarpPulseCaster.WarpMode.Slow);
                    warpPulse?.SetSlow(Value(clampedTier, 0.75f, 0.65f, 0.55f, 0.4f, 0.25f), Value(clampedTier, 2f, 2.5f, 3f, 4f, 5.5f));
                    return true;

                case "upgrade_arcane_spell_echo_repeat":
                    spellEcho?.SetEchoEnabled(true);
                    spellEcho?.SetEchoCount(Value(clampedTier, 1, 2, 2, 3, 4));
                    return true;
                case "upgrade_arcane_spell_echo_amplify":
                    spellEcho?.SetEchoEnabled(true);
                    magicMissile?.SetDamage(Value(clampedTier, 12, 15, 18, 22, 28));
                    arcaneBlast?.SetDamage(Value(clampedTier, 25, 35, 50, 75, 110));
                    warpPulse?.SetDamage(Value(clampedTier, 8, 12, 16, 24, 35));
                    return true;
                case "upgrade_arcane_spell_echo_chain_echo":
                    spellEcho?.SetEchoEnabled(true);
                    spellEcho?.SetEchoDelay(Value(clampedTier, 0.32f, 0.28f, 0.24f, 0.18f, 0.12f));
                    return true;

                case "upgrade_arcane_arcane_orbit_more_orbs":
                    arcaneOrbit?.SetOrbCount(Value(clampedTier, 4, 5, 6, 8, 10));
                    return true;
                case "upgrade_arcane_arcane_orbit_faster_orbs":
                    arcaneOrbit?.SetRotationSpeed(Value(clampedTier, 220f, 270f, 330f, 420f, 540f));
                    return true;
                case "upgrade_arcane_arcane_orbit_larger_orbs":
                    arcaneOrbit?.SetRadius(Value(clampedTier, 1.6f, 1.9f, 2.2f, 2.6f, 3.1f));
                    return true;

                case "upgrade_arcane_force_field_stronger_field":
                    forceField?.SetDamage(Value(clampedTier, 18, 26, 36, 50, 70));
                    return true;
                case "upgrade_arcane_force_field_wider_field":
                    forceField?.SetRadius(Value(clampedTier, 1.7f, 2.1f, 2.6f, 3.2f, 4f));
                    return true;
                case "upgrade_arcane_force_field_quick_field":
                    forceField?.SetCastInterval(Value(clampedTier, 3.5f, 3f, 2.5f, 2f, 1.5f));
                    return true;

                case "upgrade_arcane_time_warp_deeper_warp":
                    timeWarp?.SetSlowMultiplier(Value(clampedTier, 0.4f, 0.34f, 0.28f, 0.22f, 0.16f));
                    return true;
                case "upgrade_arcane_time_warp_longer_warp":
                    timeWarp?.SetDuration(Value(clampedTier, 3f, 3.6f, 4.3f, 5.2f, 6.5f));
                    return true;
                case "upgrade_arcane_time_warp_wider_warp":
                    timeWarp?.SetRadius(Value(clampedTier, 1.8f, 2.2f, 2.7f, 3.3f, 4.1f));
                    return true;

                case "upgrade_arcane_haste_faster_haste":
                    haste?.SetSpeedMultiplier(Value(clampedTier, 1.55f, 1.7f, 1.85f, 2.05f, 2.3f));
                    return true;
                case "upgrade_arcane_haste_longer_haste":
                    haste?.SetDuration(Value(clampedTier, 3f, 3.8f, 4.8f, 6f, 7.5f));
                    return true;
                case "upgrade_arcane_haste_quick_haste":
                    haste?.SetCastInterval(Value(clampedTier, 5.5f, 4.8f, 4.1f, 3.4f, 2.6f));
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
