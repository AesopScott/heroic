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
                Skill("earth_stone_spike", "Stone Spike", "Unlock erupting terrain strikes."),
                Skill("earth_boulder_toss", "Boulder Toss", "Unlock a rolling disruptive boulder."),
                Skill("earth_earth_wall", "Earth Wall", "Unlock battlefield-shaping stone walls."),
                Skill("earth_quake", "Quake", "Unlock repeated ground disruption."),
                Skill("earth_mud_trap", "Mud Trap", "Unlock a slowing earth field."),
                Skill("mind_psychic_lance", "Psychic Lance", "Unlock precision mental disruption."),
                Skill("mind_fear_wave", "Fear Wave", "Unlock a cone that drives enemies away."),
                Skill("mind_illusion_clone", "Illusion Clone", "Unlock decoys that distract enemies."),
                Skill("mind_confuse", "Confuse", "Unlock enemy behavior disruption."),
                Skill("mind_mind_crush", "Mind Crush", "Unlock a psychic finisher."),
                Skill("blood_blood_bolt", "Blood Bolt", "Unlock draining blood strikes."),
                Skill("blood_sanguine_pact", "Sanguine Pact", "Unlock health-for-power sacrifice."),
                Skill("blood_blood_nova", "Blood Nova", "Unlock a draining blood burst."),
                Skill("blood_leech_bind", "Leech Bind", "Unlock sustained life drain."),
                Skill("blood_crimson_frenzy", "Crimson Frenzy", "Unlock risky blood overdrive."),
                Skill("poison_poison_dart", "Poison Dart", "Unlock venom projectiles."),
                Skill("poison_toxic_cloud", "Toxic Cloud", "Unlock lingering poison clouds."),
                Skill("poison_venom_trail", "Venom Trail", "Unlock a poisonous trail."),
                Skill("poison_infection", "Infection", "Unlock spreading poison."),
                Skill("poison_rot_bloom", "Rot Bloom", "Unlock toxic burst and decay."),

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

                Boost("upgrade_earth_stone_spike_more_spikes", "Stone Spike: More Spikes", "Add spike hits."),
                Boost("upgrade_earth_stone_spike_larger_spikes", "Stone Spike: Larger Spikes", "Increase spike damage."),
                Boost("upgrade_earth_stone_spike_ground_breaker", "Stone Spike: Ground Breaker", "Increase disruption duration."),
                Boost("upgrade_earth_boulder_toss_bigger_boulder", "Boulder Toss: Bigger Boulder", "Increase impact damage."),
                Boost("upgrade_earth_boulder_toss_more_bounce", "Boulder Toss: More Bounce", "Hit more targets."),
                Boost("upgrade_earth_boulder_toss_crushing_boulder", "Boulder Toss: Crushing Boulder", "Increase knockback and stun value."),
                Boost("upgrade_earth_earth_wall_longer_wall", "Earth Wall: Longer Wall", "Add wall segments."),
                Boost("upgrade_earth_earth_wall_taller_wall", "Earth Wall: Taller Wall", "Increase wall coverage."),
                Boost("upgrade_earth_earth_wall_harden_wall", "Earth Wall: Harden Wall", "Increase wall duration."),
                Boost("upgrade_earth_quake_larger_quake", "Quake: Larger Quake", "Increase quake radius."),
                Boost("upgrade_earth_quake_stronger_quake", "Quake: Stronger Quake", "Increase quake damage."),
                Boost("upgrade_earth_quake_repeated_quake", "Quake: Repeated Quake", "Add quake pulses."),
                Boost("upgrade_earth_mud_trap_bigger_trap", "Mud Trap: Bigger Trap", "Increase mud radius."),
                Boost("upgrade_earth_mud_trap_stickier_mud", "Mud Trap: Stickier Mud", "Strengthen slow."),
                Boost("upgrade_earth_mud_trap_heavy_mud", "Mud Trap: Heavy Mud", "Increase mud damage."),

                Boost("upgrade_mind_psychic_lance_more_damage", "Psychic Lance: More Damage", "Increase lance damage."),
                Boost("upgrade_mind_psychic_lance_longer_range", "Psychic Lance: Longer Range", "Increase lance range."),
                Boost("upgrade_mind_psychic_lance_mind_pierce", "Psychic Lance: Mind Pierce", "Widen the lance."),
                Boost("upgrade_mind_fear_wave_bigger_wave", "Fear Wave: Bigger Wave", "Widen the fear cone."),
                Boost("upgrade_mind_fear_wave_longer_fear", "Fear Wave: Longer Fear", "Increase fear duration."),
                Boost("upgrade_mind_fear_wave_stronger_panic", "Fear Wave: Stronger Panic", "Increase panic damage."),
                Boost("upgrade_mind_illusion_clone_more_clones", "Illusion Clone: More Clones", "Create more decoys."),
                Boost("upgrade_mind_illusion_clone_stronger_decoys", "Illusion Clone: Stronger Decoys", "Increase decoy duration."),
                Boost("upgrade_mind_illusion_clone_clone_burst", "Illusion Clone: Clone Burst", "Increase clone burst damage."),
                Boost("upgrade_mind_confuse_wider_effect", "Confuse: Wider Effect", "Increase confuse radius."),
                Boost("upgrade_mind_confuse_longer_confusion", "Confuse: Longer Confusion", "Increase confusion duration."),
                Boost("upgrade_mind_confuse_deeper_confusion", "Confuse: Deeper Confusion", "Increase confusion damage."),
                Boost("upgrade_mind_mind_crush_more_damage", "Mind Crush: More Damage", "Increase crush damage."),
                Boost("upgrade_mind_mind_crush_area_crush", "Mind Crush: Area Crush", "Increase crush radius."),
                Boost("upgrade_mind_mind_crush_execution_crush", "Mind Crush: Execution Crush", "Increase weakened-target damage."),

                Boost("upgrade_blood_blood_bolt_more_damage", "Blood Bolt: More Damage", "Increase bolt damage."),
                Boost("upgrade_blood_blood_bolt_lifesteal", "Blood Bolt: Lifesteal", "Increase healing from hits."),
                Boost("upgrade_blood_blood_bolt_splash_drain", "Blood Bolt: Splash Drain", "Increase splash drain radius."),
                Boost("upgrade_blood_sanguine_pact_more_power", "Sanguine Pact: More Power", "Increase sacrifice power."),
                Boost("upgrade_blood_sanguine_pact_more_healing", "Sanguine Pact: More Healing", "Increase recovery after sacrifice."),
                Boost("upgrade_blood_sanguine_pact_lower_cost", "Sanguine Pact: Lower Cost", "Reduce sacrifice health cost."),
                Boost("upgrade_blood_blood_nova_bigger_nova", "Blood Nova: Bigger Nova", "Increase nova radius."),
                Boost("upgrade_blood_blood_nova_stronger_nova", "Blood Nova: Stronger Nova", "Increase nova damage."),
                Boost("upgrade_blood_blood_nova_healing_nova", "Blood Nova: Healing Nova", "Increase nova healing."),
                Boost("upgrade_blood_leech_bind_longer_bind", "Leech Bind: Longer Bind", "Increase bind duration."),
                Boost("upgrade_blood_leech_bind_stronger_drain", "Leech Bind: Stronger Drain", "Increase drain healing."),
                Boost("upgrade_blood_leech_bind_multi_bind", "Leech Bind: Multi-Bind", "Link more enemies."),
                Boost("upgrade_blood_crimson_frenzy_faster_attacks", "Crimson Frenzy: Faster Attacks", "Increase frenzy duration."),
                Boost("upgrade_blood_crimson_frenzy_more_damage", "Crimson Frenzy: More Damage", "Increase frenzy power."),
                Boost("upgrade_blood_crimson_frenzy_low_health_power", "Crimson Frenzy: Low Health Power", "Increase risky sustain payoff."),

                Boost("upgrade_poison_poison_dart_more_darts", "Poison Dart: More Darts", "Add poison darts."),
                Boost("upgrade_poison_poison_dart_stronger_poison", "Poison Dart: Stronger Poison", "Increase poison damage."),
                Boost("upgrade_poison_poison_dart_spread_poison", "Poison Dart: Spread Poison", "Spread poison farther."),
                Boost("upgrade_poison_toxic_cloud_bigger_cloud", "Toxic Cloud: Bigger Cloud", "Increase cloud radius."),
                Boost("upgrade_poison_toxic_cloud_longer_cloud", "Toxic Cloud: Longer Cloud", "Increase cloud duration."),
                Boost("upgrade_poison_toxic_cloud_heavier_cloud", "Toxic Cloud: Heavier Cloud", "Increase cloud damage."),
                Boost("upgrade_poison_venom_trail_longer_trail", "Venom Trail: Longer Trail", "Increase trail duration."),
                Boost("upgrade_poison_venom_trail_stronger_trail", "Venom Trail: Stronger Trail", "Increase trail damage."),
                Boost("upgrade_poison_venom_trail_sticky_trail", "Venom Trail: Sticky Trail", "Slow enemies in trail."),
                Boost("upgrade_poison_infection_faster_spread", "Infection: Faster Spread", "Increase infection tick rate."),
                Boost("upgrade_poison_infection_stronger_infection", "Infection: Stronger Infection", "Increase infection damage."),
                Boost("upgrade_poison_infection_collapse", "Infection: Collapse", "Add poison death burst."),
                Boost("upgrade_poison_rot_bloom_bigger_bloom", "Rot Bloom: Bigger Bloom", "Increase bloom radius."),
                Boost("upgrade_poison_rot_bloom_more_bloom_damage", "Rot Bloom: More Bloom Damage", "Increase bloom damage."),
                Boost("upgrade_poison_rot_bloom_lingering_rot", "Rot Bloom: Lingering Rot", "Increase rot duration."),

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
