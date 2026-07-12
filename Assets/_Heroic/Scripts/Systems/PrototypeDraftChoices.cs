namespace Heroic.Systems
{
    public static class PrototypeDraftChoices
    {
        public static UpgradeManager.DraftChoice[] Create()
        {
            return new[]
            {
                Skill("arcane_arcane_blast", "Arcane Blast", "Unlock a targeted Arcane impact."),
                Skill("arcane_warp_pulse", "Warp Pulse", "Unlock a close Arcane control pulse."),
                Skill("arcane_spell_echo", "Spell Echo", "Unlock delayed Arcane repeat support."),
                Skill("arcane_arcane_orbit", "Arcane Orbit", "Unlock orbiting Arcane damage."),
                Skill("fire_fire_bolt", "Fire Bolt", "Unlock high-damage burning projectiles."),
                Skill("fire_flame_wave", "Flame Wave", "Unlock a sweeping cone of Fire damage."),
                Skill("fire_burning_ground", "Burning Ground", "Unlock lingering Fire zones."),
                Skill("cold_frost_ring", "Frost Ring", "Unlock an expanding cold ring that slows nearby enemies."),
                Skill("cold_ice_shard", "Ice Shard", "Unlock piercing cold projectiles."),
                Skill("cold_glacial_field", "Glacial Field", "Unlock lingering cold zones."),
                Skill("cold_crystal_prison", "Crystal Prison", "Unlock freezing traps."),
                Skill("cold_shatter_line", "Shatter Line", "Unlock a cold line attack that punishes chilled enemies."),
                Skill("lightning_chain_bolt", "Chain Bolt", "Unlock jumping Lightning damage."),
                Skill("lightning_static_field", "Static Field", "Unlock charged zones with stun pressure."),
                Skill("lightning_thunder_lance", "Thunder Lance", "Unlock a piercing Lightning line strike."),
                Skill("lightning_spark_surge", "Spark Surge", "Unlock rapid Lightning burst fire."),
                Skill("lightning_storm_call", "Storm Call", "Unlock repeated storm strikes."),

                System("system_territory_casting", "Territory Casting", "Reveal arena territories that boost damage, range, recovery, or all spell stats."),
                Boost("upgrade_system_territory_casting_more_territories", "Territory Casting: More Territories", "Keep more territory circles active at once."),

                Boost("upgrade_arcane_magic_missile_split_shot", "Magic Missile: Split Shot", "Add more missiles."),
                Boost("upgrade_arcane_magic_missile_seeking_shot", "Magic Missile: Seeking Shot", "Improve homing strength."),
                Boost("upgrade_arcane_magic_missile_arcane_pierce", "Magic Missile: Arcane Pierce", "Add pierce count."),

                Boost("upgrade_arcane_arcane_blast_power", "Arcane Blast: Power", "Increase blast damage."),
                Boost("upgrade_arcane_arcane_blast_reach", "Arcane Blast: Reach", "Increase blast range."),
                Boost("upgrade_arcane_arcane_blast_scatter", "Arcane Blast: Scatter", "Add secondary blasts."),

                Boost("upgrade_arcane_warp_pulse_push", "Warp Pulse: Push", "Strengthen push mode."),
                Boost("upgrade_arcane_warp_pulse_pull", "Warp Pulse: Pull", "Strengthen pull mode."),
                Boost("upgrade_arcane_warp_pulse_slow_warp", "Warp Pulse: Slow Warp", "Strengthen slow mode."),

                Boost("upgrade_arcane_spell_echo_repeat", "Spell Echo: Repeat", "Add echo repeats."),
                Boost("upgrade_arcane_spell_echo_amplify", "Spell Echo: Amplify", "Improve echo impact."),
                Boost("upgrade_arcane_spell_echo_chain_echo", "Spell Echo: Chain Echo", "Improve echo cadence."),

                Boost("upgrade_arcane_arcane_orbit_more_orbs", "Arcane Orbit: More Orbs", "Add orbiting projectiles."),
                Boost("upgrade_arcane_arcane_orbit_faster_orbs", "Arcane Orbit: Faster Orbs", "Increase orbit speed."),
                Boost("upgrade_arcane_arcane_orbit_larger_orbs", "Arcane Orbit: Larger Orbs", "Increase orbit radius and hit size."),

                Boost("upgrade_fire_fire_bolt_power", "Fire Bolt: Power", "Increase bolt damage."),
                Boost("upgrade_fire_fire_bolt_fork", "Fire Bolt: Fork", "Add more fire bolts."),
                Boost("upgrade_fire_fire_bolt_pierce", "Fire Bolt: Pierce", "Add pierce count."),

                Boost("upgrade_fire_flame_wave_heat", "Flame Wave: Heat", "Increase cone damage."),
                Boost("upgrade_fire_flame_wave_reach", "Flame Wave: Reach", "Extend cone range."),
                Boost("upgrade_fire_flame_wave_width", "Flame Wave: Width", "Widen the burn cone."),

                Boost("upgrade_fire_burning_ground_burn", "Burning Ground: Burn", "Increase damage per tick."),
                Boost("upgrade_fire_burning_ground_spread", "Burning Ground: Spread", "Increase burning area."),
                Boost("upgrade_fire_burning_ground_persist", "Burning Ground: Persist", "Increase zone duration."),

                Boost("upgrade_cold_frost_ring_wider_ring", "Frost Ring: Wider Ring", "Increase ring radius."),
                Boost("upgrade_cold_frost_ring_heavier_chill", "Frost Ring: Heavier Chill", "Strengthen the slow."),
                Boost("upgrade_cold_frost_ring_deep_freeze", "Frost Ring: Deep Freeze", "Increase freeze chance."),

                Boost("upgrade_cold_ice_shard_more_shards", "Ice Shard: More Shards", "Add more shards."),
                Boost("upgrade_cold_ice_shard_piercing_shards", "Ice Shard: Piercing Shards", "Add pierce count."),
                Boost("upgrade_cold_ice_shard_shatter_damage", "Ice Shard: Shatter Damage", "Deal bonus damage to controlled enemies."),

                Boost("upgrade_cold_glacial_field_wider_field", "Glacial Field: Wider Field", "Increase field radius."),
                Boost("upgrade_cold_glacial_field_longer_field", "Glacial Field: Longer Field", "Increase field duration."),
                Boost("upgrade_cold_glacial_field_deeper_chill", "Glacial Field: Deeper Chill", "Strengthen the slow."),

                Boost("upgrade_cold_crystal_prison_more_prisons", "Crystal Prison: More Prisons", "Create more traps."),
                Boost("upgrade_cold_crystal_prison_faster_trigger", "Crystal Prison: Faster Trigger", "Reduce trap arming delay."),
                Boost("upgrade_cold_crystal_prison_hard_lock", "Crystal Prison: Hard Lock", "Increase freeze duration."),

                Boost("upgrade_cold_shatter_line_wider_line", "Shatter Line: Wider Line", "Increase line width."),
                Boost("upgrade_cold_shatter_line_longer_line", "Shatter Line: Longer Line", "Increase line range."),
                Boost("upgrade_cold_shatter_line_brutal_shatter", "Shatter Line: Brutal Shatter", "Increase bonus damage to controlled enemies."),

                Boost("upgrade_lightning_chain_bolt_more_jumps", "Chain Bolt: More Jumps", "Add chain jumps."),
                Boost("upgrade_lightning_chain_bolt_higher_damage", "Chain Bolt: Higher Damage", "Increase damage per hit."),
                Boost("upgrade_lightning_chain_bolt_longer_chain", "Chain Bolt: Longer Chain", "Increase jump distance."),

                Boost("upgrade_lightning_static_field_bigger_field", "Static Field: Bigger Field", "Increase field radius."),
                Boost("upgrade_lightning_static_field_faster_ticks", "Static Field: Faster Ticks", "Increase tick rate."),
                Boost("upgrade_lightning_static_field_stun_chance", "Static Field: Stun Chance", "Increase stun chance."),

                Boost("upgrade_lightning_thunder_lance_piercing_lance", "Thunder Lance: Piercing Lance", "Hit more enemies in line."),
                Boost("upgrade_lightning_thunder_lance_wider_lance", "Thunder Lance: Wider Lance", "Increase line width."),
                Boost("upgrade_lightning_thunder_lance_critical_strike", "Thunder Lance: Critical Strike", "Deal bonus damage to isolated targets."),

                Boost("upgrade_lightning_spark_surge_more_sparks", "Spark Surge: More Sparks", "Add sparks to each burst."),
                Boost("upgrade_lightning_spark_surge_faster_surge", "Spark Surge: Faster Surge", "Shorten the burst window."),
                Boost("upgrade_lightning_spark_surge_target_spread", "Spark Surge: Target Spread", "Let sparks arc farther through clusters."),

                Boost("upgrade_lightning_storm_call_more_strikes", "Storm Call: More Strikes", "Add storm strikes."),
                Boost("upgrade_lightning_storm_call_faster_strikes", "Storm Call: Faster Strikes", "Reduce delay between strikes."),
                Boost("upgrade_lightning_storm_call_violent_storm", "Storm Call: Violent Storm", "Increase damage and stun chance."),

                Boost("upgrade_movement_cloud_walk_speed", "Cloud Walk: Faster Steps", "Increase standard movement by 20% per tier."),
                Boost("upgrade_movement_cloud_walk_pickup", "Cloud Walk: Cloud Reach", "Increase pickup range by 50% per tier."),
                Boost("upgrade_movement_cloud_walk_knockback", "Cloud Walk: Rebuffing Mist", "Proc knockback against nearby enemies."),
                Boost("upgrade_movement_whirlwind_gale", "Whirlwind: Gale Engine", "Increase spin movement speed and damage together."),

                Movement("movement_blink", "Blink", "Equip short Arcane reposition."),
                Movement("movement_lunge", "Lunge", "Equip aggressive forward movement."),
                Movement("movement_teleport", "Teleport", "Equip long-range reposition."),
                Movement("movement_whirlwind", "Whirlwind", "Spin through danger at 75% speed while damaging nearby enemies."),
                Movement("movement_cloud_walk", "Cloud Walk", "Gain 25% standard movement speed and burst across clouds.")
            };
        }

        private static UpgradeManager.DraftChoice Skill(string id, string name, string description)
        {
            return new UpgradeManager.DraftChoice(id, name, description, UpgradeManager.UpgradeCategory.Attack);
        }

        private static UpgradeManager.DraftChoice Boost(string id, string name, string description)
        {
            return new UpgradeManager.DraftChoice(id, name, description, UpgradeManager.UpgradeCategory.Boost);
        }

        private static UpgradeManager.DraftChoice Movement(string id, string name, string description)
        {
            return new UpgradeManager.DraftChoice(id, name, description, UpgradeManager.UpgradeCategory.Movement);
        }

        private static UpgradeManager.DraftChoice System(string id, string name, string description)
        {
            return new UpgradeManager.DraftChoice(id, name, description, UpgradeManager.UpgradeCategory.System);
        }
    }
}
