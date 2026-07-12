using Heroic.Spells;
using UnityEngine;

namespace Heroic.Systems
{
    public class EarthUpgradeApplier : MonoBehaviour
    {
        [SerializeField] private EarthAbilityCaster stoneSpike;
        [SerializeField] private EarthAbilityCaster boulderToss;
        [SerializeField] private EarthAbilityCaster earthWall;
        [SerializeField] private EarthAbilityCaster quake;
        [SerializeField] private EarthAbilityCaster mudTrap;

        public bool Apply(string choiceId, int tier)
        {
            int t = Mathf.Clamp(tier, 1, 5);
            switch (choiceId)
            {
                case "upgrade_earth_stone_spike_more_spikes": stoneSpike?.SetCount(Value(t, 4, 5, 6, 8, 10)); return true;
                case "upgrade_earth_stone_spike_larger_spikes": stoneSpike?.SetDamage(Value(t, 30, 42, 58, 78, 105)); return true;
                case "upgrade_earth_stone_spike_ground_breaker": stoneSpike?.SetStunDuration(Value(t, 0.45f, 0.6f, 0.8f, 1.05f, 1.35f)); return true;
                case "upgrade_earth_boulder_toss_bigger_boulder": boulderToss?.SetDamage(Value(t, 34, 48, 66, 90, 120)); return true;
                case "upgrade_earth_boulder_toss_more_bounce": boulderToss?.SetCount(Value(t, 2, 3, 4, 5, 7)); return true;
                case "upgrade_earth_boulder_toss_crushing_boulder": boulderToss?.SetBonusDamageMultiplier(Value(t, 1.25f, 1.45f, 1.7f, 2f, 2.4f)); return true;
                case "upgrade_earth_earth_wall_longer_wall": earthWall?.SetCount(Value(t, 4, 5, 6, 8, 10)); return true;
                case "upgrade_earth_earth_wall_taller_wall": earthWall?.SetRadius(Value(t, 1.9f, 2.2f, 2.6f, 3.1f, 3.7f)); return true;
                case "upgrade_earth_earth_wall_harden_wall": earthWall?.SetDuration(Value(t, 4f, 5f, 6.5f, 8f, 10f)); return true;
                case "upgrade_earth_quake_larger_quake": quake?.SetRadius(Value(t, 2.2f, 2.8f, 3.5f, 4.3f, 5.3f)); return true;
                case "upgrade_earth_quake_stronger_quake": quake?.SetDamage(Value(t, 32, 44, 60, 82, 110)); return true;
                case "upgrade_earth_quake_repeated_quake": quake?.SetCount(Value(t, 2, 3, 4, 5, 7)); return true;
                case "upgrade_earth_mud_trap_bigger_trap": mudTrap?.SetRadius(Value(t, 2.2f, 2.8f, 3.5f, 4.3f, 5.2f)); return true;
                case "upgrade_earth_mud_trap_stickier_mud": mudTrap?.SetSlowMultiplier(Value(t, 0.48f, 0.4f, 0.32f, 0.25f, 0.18f)); return true;
                case "upgrade_earth_mud_trap_heavy_mud": mudTrap?.SetBonusDamageMultiplier(Value(t, 1.25f, 1.45f, 1.7f, 2f, 2.4f)); return true;
                default: return false;
            }
        }

        private int Value(int tier, int basic, int advanced, int expert, int master, int grandmaster)
        {
            switch (tier) { case 1: return basic; case 2: return advanced; case 3: return expert; case 4: return master; default: return grandmaster; }
        }

        private float Value(int tier, float basic, float advanced, float expert, float master, float grandmaster)
        {
            switch (tier) { case 1: return basic; case 2: return advanced; case 3: return expert; case 4: return master; default: return grandmaster; }
        }
    }
}
