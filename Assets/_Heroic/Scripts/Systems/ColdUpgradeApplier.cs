using Heroic.Spells;
using UnityEngine;

namespace Heroic.Systems
{
    public class ColdUpgradeApplier : MonoBehaviour
    {
        [SerializeField] private FrostRingCaster frostRing;
        [SerializeField] private IceShardCaster iceShard;
        [SerializeField] private GlacialFieldCaster glacialField;
        [SerializeField] private CrystalPrisonCaster crystalPrison;
        [SerializeField] private ShatterLineCaster shatterLine;

        public bool Apply(string choiceId, int tier)
        {
            int clampedTier = Mathf.Clamp(tier, 1, 5);

            switch (choiceId)
            {
                case "upgrade_cold_frost_ring_wider_ring":
                    frostRing?.SetRadius(Value(clampedTier, 3.9f, 4.5f, 5.2f, 6f, 7f));
                    return true;
                case "upgrade_cold_frost_ring_heavier_chill":
                    frostRing?.SetSlowMultiplier(Value(clampedTier, 0.55f, 0.48f, 0.4f, 0.32f, 0.24f));
                    return true;
                case "upgrade_cold_frost_ring_deep_freeze":
                    frostRing?.SetFreezeChance(Value(clampedTier, 0.12f, 0.18f, 0.25f, 0.34f, 0.45f));
                    return true;

                case "upgrade_cold_ice_shard_more_shards":
                    iceShard?.SetProjectileCount(Value(clampedTier, 2, 3, 4, 5, 7));
                    return true;
                case "upgrade_cold_ice_shard_piercing_shards":
                    iceShard?.SetPierceCount(Value(clampedTier, 1, 2, 3, 5, 8));
                    return true;
                case "upgrade_cold_ice_shard_shatter_damage":
                    iceShard?.SetControlledDamageMultiplier(Value(clampedTier, 1.25f, 1.45f, 1.7f, 2f, 2.4f));
                    return true;

                case "upgrade_cold_glacial_field_wider_field":
                    glacialField?.SetRadius(Value(clampedTier, 2.2f, 2.7f, 3.3f, 4f, 4.8f));
                    return true;
                case "upgrade_cold_glacial_field_longer_field":
                    glacialField?.SetDuration(Value(clampedTier, 4f, 4.8f, 5.8f, 7f, 8.5f));
                    return true;
                case "upgrade_cold_glacial_field_deeper_chill":
                    glacialField?.SetSlowMultiplier(Value(clampedTier, 0.48f, 0.4f, 0.33f, 0.26f, 0.2f));
                    return true;

                case "upgrade_cold_crystal_prison_more_prisons":
                    crystalPrison?.SetPrisonCount(Value(clampedTier, 2, 3, 4, 5, 7));
                    return true;
                case "upgrade_cold_crystal_prison_faster_trigger":
                    crystalPrison?.SetTriggerDelay(Value(clampedTier, 0.28f, 0.22f, 0.16f, 0.11f, 0.07f));
                    return true;
                case "upgrade_cold_crystal_prison_hard_lock":
                    crystalPrison?.SetFreezeDuration(Value(clampedTier, 1.35f, 1.7f, 2.1f, 2.6f, 3.2f));
                    return true;

                case "upgrade_cold_shatter_line_wider_line":
                    shatterLine?.SetWidth(Value(clampedTier, 1.35f, 1.65f, 2f, 2.4f, 2.9f));
                    return true;
                case "upgrade_cold_shatter_line_longer_line":
                    shatterLine?.SetRange(Value(clampedTier, 7.5f, 8.8f, 10.2f, 12f, 14f));
                    return true;
                case "upgrade_cold_shatter_line_brutal_shatter":
                    shatterLine?.SetControlledDamageMultiplier(Value(clampedTier, 1.55f, 1.85f, 2.2f, 2.65f, 3.2f));
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
