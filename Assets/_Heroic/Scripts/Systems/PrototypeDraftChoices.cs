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

                Movement("movement_blink", "Blink", "Equip short Arcane reposition."),
                Movement("movement_lunge", "Lunge", "Equip aggressive forward movement."),
                Movement("movement_teleport", "Teleport", "Equip long-range reposition.")
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
