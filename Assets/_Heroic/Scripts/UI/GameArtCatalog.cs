using System.Collections.Generic;

namespace Heroic.UI
{
    public static class GameArtCatalog
    {
        public readonly struct ArtArtifactDefinition
        {
            public ArtArtifactDefinition(string key, string displayName, string artifactType, string placeholderGlyph, string hexColor, string requiredFor)
            {
                Key = key;
                DisplayName = displayName;
                ArtifactType = artifactType;
                PlaceholderGlyph = placeholderGlyph;
                HexColor = hexColor;
                RequiredFor = requiredFor;
            }

            public string Key { get; }
            public string DisplayName { get; }
            public string ArtifactType { get; }
            public string PlaceholderGlyph { get; }
            public string HexColor { get; }
            public string RequiredFor { get; }
        }

        public static IReadOnlyList<ArtArtifactDefinition> All => Artifacts;

        private static readonly ArtArtifactDefinition[] Artifacts =
        {
            new ArtArtifactDefinition("icon.category.attack", "Attack Category Icon", "UI Icon", "ATK", "FF6A2A", "Draft cards and spellbook filters"),
            new ArtArtifactDefinition("icon.category.defense", "Defense Category Icon", "UI Icon", "DEF", "7CE3FF", "Draft cards and future defense lane"),
            new ArtArtifactDefinition("icon.category.movement", "Movement Category Icon", "UI Icon", "MOV", "88F7B0", "Draft cards and movement slot UI"),
            new ArtArtifactDefinition("icon.category.system", "System Category Icon", "UI Icon", "SYS", "C8C3FF", "Draft cards and spell system UI"),
            new ArtArtifactDefinition("icon.category.boost", "Boost Category Icon", "UI Icon", "UPG", "FFD45A", "Draft cards and upgrade tiers"),

            new ArtArtifactDefinition("icon.school.arcane", "Arcane School Icon", "UI Icon", "ARC", "78D7FF", "Arcane cards, spellbook, spell proc feedback"),
            new ArtArtifactDefinition("icon.school.fire", "Fire School Icon", "UI Icon", "FIR", "FF6A2A", "Fire cards, burn status, fire spell VFX"),
            new ArtArtifactDefinition("icon.school.cold", "Cold School Icon", "UI Icon", "CLD", "7FE7FF", "Cold cards, slow/freeze status, cold spell VFX"),
            new ArtArtifactDefinition("icon.school.lightning", "Lightning School Icon", "UI Icon", "LTN", "F5E84B", "Lightning cards, stun marker, chain spell VFX"),
            new ArtArtifactDefinition("icon.school.earth", "Earth School Icon", "UI Icon", "ERT", "A8743D", "Earth cards, knockdown marker, terrain previews"),
            new ArtArtifactDefinition("icon.school.mind", "Mind School Icon", "UI Icon", "MND", "D889FF", "Mind cards, fear/confuse status"),
            new ArtArtifactDefinition("icon.school.blood", "Blood School Icon", "UI Icon", "BLD", "C0263E", "Blood cards, bleed/drain status"),
            new ArtArtifactDefinition("icon.school.poison", "Poison School Icon", "UI Icon", "PSN", "76D94E", "Poison cards, contagious/disabled status"),

            new ArtArtifactDefinition("icon.system.territory_casting", "Territory Casting Icon", "UI Icon", "TC", "C8C3FF", "System cards, learned system list, active territory UI"),
            new ArtArtifactDefinition("icon.system.component_magic", "Component Magic Icon", "UI Icon", "CM", "6EF1C8", "System cards, component stack UI, component pickups"),
            new ArtArtifactDefinition("icon.system.sacrificial_casting", "Sacrificial Casting Icon", "UI Icon", "SC", "D43A4E", "System cards, sacrifice penalty UI"),
            new ArtArtifactDefinition("icon.system.incantation_casting", "Incantation Casting Icon", "UI Icon", "IC", "B7E7FF", "System cards, proc feedback UI"),
            new ArtArtifactDefinition("icon.system.spell_weaving", "Spell Weaving Icon", "UI Icon", "SW", "EAA8FF", "System cards, woven element tooltip rows"),
            new ArtArtifactDefinition("icon.system.runic_magic", "Runic Magic Icon", "UI Icon", "RM", "D6B36A", "System cards, rune trap UI"),
            new ArtArtifactDefinition("icon.system.rhythm_casting", "Rhythm Casting Icon", "UI Icon", "RC", "F8D56A", "Timing meter if retained"),
            new ArtArtifactDefinition("icon.system.spell_tension", "Spell Tension Icon", "UI Icon", "ST", "F06F86", "Charge/debt meter if retained"),
            new ArtArtifactDefinition("icon.system.paired_synergy", "Paired System Synergy Icon", "UI Icon", "PX", "FFFFFF", "Paired system badges and upgrade cards"),

            new ArtArtifactDefinition("icon.movement.blink", "Blink Icon", "UI Icon", "BL", "88F7B0", "Movement cards and slot 1-3 UI"),
            new ArtArtifactDefinition("icon.movement.lunge", "Lunge Icon", "UI Icon", "LG", "88F7B0", "Movement cards and slot 1-3 UI"),
            new ArtArtifactDefinition("icon.movement.teleport", "Teleport Icon", "UI Icon", "TP", "88F7B0", "Movement cards and slot 1-3 UI"),
            new ArtArtifactDefinition("icon.movement.whirlwind", "Whirlwind Icon", "UI Icon", "WH", "88F7B0", "Movement cards and slot 1-3 UI"),
            new ArtArtifactDefinition("icon.movement.cloud_walk", "Cloud Walk Icon", "UI Icon", "CW", "88F7B0", "Movement cards and slot 1-3 UI"),

            new ArtArtifactDefinition("icon.skill.magic_missile", "Magic Missile Icon", "UI Icon", "MM", "78D7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.arcane_blast", "Arcane Blast Icon", "UI Icon", "AB", "78D7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.warp_pulse", "Warp Pulse Icon", "UI Icon", "WP", "78D7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.spell_echo", "Spell Echo Icon", "UI Icon", "SE", "78D7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.arcane_orbit", "Arcane Orbit Icon", "UI Icon", "AO", "78D7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.fire_bolt", "Fire Bolt Icon", "UI Icon", "FB", "FF6A2A", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.flame_wave", "Flame Wave Icon", "UI Icon", "FW", "FF6A2A", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.burning_ground", "Burning Ground Icon", "UI Icon", "BG", "FF6A2A", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.frost_ring", "Frost Ring Icon", "UI Icon", "FR", "7FE7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.ice_shard", "Ice Shard Icon", "UI Icon", "IS", "7FE7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.glacial_field", "Glacial Field Icon", "UI Icon", "GF", "7FE7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.crystal_prison", "Crystal Prison Icon", "UI Icon", "CP", "7FE7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.shatter_line", "Shatter Line Icon", "UI Icon", "SL", "7FE7FF", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.chain_bolt", "Chain Bolt Icon", "UI Icon", "CB", "F5E84B", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.static_field", "Static Field Icon", "UI Icon", "SF", "F5E84B", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.thunder_lance", "Thunder Lance Icon", "UI Icon", "TL", "F5E84B", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.spark_surge", "Spark Surge Icon", "UI Icon", "SS", "F5E84B", "Ability cards and spellbook"),
            new ArtArtifactDefinition("icon.skill.storm_call", "Storm Call Icon", "UI Icon", "SC", "F5E84B", "Ability cards and spellbook"),

            new ArtArtifactDefinition("marker.status.burn", "Burn Status Marker", "Status Marker", "BRN", "FF6A2A", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.slow", "Slow Status Marker", "Status Marker", "SLW", "7FE7FF", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.freeze", "Freeze Status Marker", "Status Marker", "FRZ", "B9F4FF", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.stun", "Stun Status Marker", "Status Marker", "STN", "F5E84B", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.knockdown", "Knockdown Status Marker", "Status Marker", "KDN", "A8743D", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.fear", "Fear Status Marker", "Status Marker", "FEA", "D889FF", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.confuse", "Confuse Status Marker", "Status Marker", "CNF", "D889FF", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.bleed", "Bleed Status Marker", "Status Marker", "BLD", "C0263E", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.drain", "Drain Status Marker", "Status Marker", "DRN", "C0263E", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.poison", "Poison Status Marker", "Status Marker", "PSN", "76D94E", "Enemy status feedback"),
            new ArtArtifactDefinition("marker.status.disabled", "Disabled Status Marker", "Status Marker", "DIS", "76D94E", "Enemy status feedback"),

            new ArtArtifactDefinition("enemy.crash", "Crash Enemy Shape", "Enemy Sprite", "CR", "9BA3AA", "Crash family"),
            new ArtArtifactDefinition("enemy.shooter", "Shooter Enemy Shape", "Enemy Sprite", "SH", "9BA3AA", "Shooter family"),
            new ArtArtifactDefinition("enemy.caster", "Caster Enemy Shape", "Enemy Sprite", "CA", "C8C3FF", "Caster families"),
            new ArtArtifactDefinition("enemy.fast", "Fast Enemy Shape", "Enemy Sprite", "FM", "88F7B0", "Skitter, Gale Hound, Blink Stalker, Burrower"),
            new ArtArtifactDefinition("enemy.defensive", "Defensive Enemy Shape", "Enemy Sprite", "DF", "A8743D", "Bulwark, Ward Shell, Shield Totem"),
            new ArtArtifactDefinition("enemy.swarm", "Swarm Enemy Shape", "Enemy Sprite", "SW", "76D94E", "Splitter, Page Swarm, Venom Brood"),
            new ArtArtifactDefinition("enemy.boss", "Boss Enemy Shape", "Enemy Sprite", "BO", "FFD45A", "Mini-bosses, bosses, master boss"),

            new ArtArtifactDefinition("world.territory.damage", "Damage Territory Circle", "World VFX", "DMG", "FF6A2A", "Territory Casting"),
            new ArtArtifactDefinition("world.territory.range", "Range Territory Circle", "World VFX", "RNG", "7FE7FF", "Territory Casting"),
            new ArtArtifactDefinition("world.territory.recovery", "Recovery Territory Circle", "World VFX", "REC", "88F7B0", "Territory Casting"),
            new ArtArtifactDefinition("world.territory.confluence", "Confluence Territory Circle", "World VFX", "ALL", "C8C3FF", "Territory Casting"),
            new ArtArtifactDefinition("world.component.damage", "Damage Component Pickup", "Pickup Sprite", "CD", "FF6A2A", "Component Magic"),
            new ArtArtifactDefinition("world.component.range", "Range Component Pickup", "Pickup Sprite", "CR", "7FE7FF", "Component Magic"),
            new ArtArtifactDefinition("world.component.recovery", "Recovery Component Pickup", "Pickup Sprite", "CC", "88F7B0", "Component Magic"),
            new ArtArtifactDefinition("world.rune.generic", "Generic Rune Trap", "World VFX", "RN", "D6B36A", "Runic Magic"),
            new ArtArtifactDefinition("world.sacrifice.flash", "Sacrifice Cast Flash", "World VFX", "SX", "D43A4E", "Sacrificial Casting"),
            new ArtArtifactDefinition("world.rhythm.beat", "Rhythm Beat Pulse", "World/UI VFX", "BT", "F8D56A", "Rhythm Casting"),
            new ArtArtifactDefinition("world.tension.charge", "Spell Tension Charge", "World/UI VFX", "CH", "F06F86", "Spell Tension")
        };
    }
}
