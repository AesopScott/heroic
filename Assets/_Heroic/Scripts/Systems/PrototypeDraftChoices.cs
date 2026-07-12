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

                Boost("upgrade_movement_cloud_walk_speed", "Cloud Walk: Faster Steps", "Increase standard movement by 20% per tier."),
                Boost("upgrade_movement_cloud_walk_pickup", "Cloud Walk: Cloud Reach", "Increase pickup range by 50% per tier."),
                Boost("upgrade_movement_cloud_walk_knockback", "Cloud Walk: Rebuffing Mist", "Proc knockback against nearby enemies."),

                Movement("movement_blink", "Blink", "Equip short Arcane reposition."),
                Movement("movement_lunge", "Lunge", "Equip aggressive forward movement."),
                Movement("movement_teleport", "Teleport", "Equip long-range reposition."),
                Movement("movement_whirlwind", "Whirlwind", "Equip a damaging rush through enemies."),
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
    }
}
