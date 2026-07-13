using Heroic.Systems;
using UnityEngine;

namespace Heroic.UI
{
    public static class SkillVisualRegistry
    {
        public readonly struct Visual
        {
            public Visual(string artKey, string glyph, Color color)
            {
                ArtKey = artKey;
                Glyph = glyph;
                Color = color;
            }

            public string ArtKey { get; }
            public string Glyph { get; }
            public Color Color { get; }
        }

        public static Visual ForCategory(UpgradeManager.UpgradeCategory category)
        {
            switch (category)
            {
                case UpgradeManager.UpgradeCategory.Attack:
                    return new Visual("icon.category.attack", "ATK", Hex("FF6A2A"));
                case UpgradeManager.UpgradeCategory.Defense:
                    return new Visual("icon.category.defense", "DEF", Hex("7CE3FF"));
                case UpgradeManager.UpgradeCategory.Movement:
                    return new Visual("icon.category.movement", "MOV", Hex("88F7B0"));
                case UpgradeManager.UpgradeCategory.System:
                    return new Visual("icon.category.system", "SYS", Hex("C8C3FF"));
                case UpgradeManager.UpgradeCategory.Boost:
                    return new Visual("icon.category.boost", "UPG", Hex("FFD45A"));
                default:
                    return new Visual("icon.category.boost", "UP", Hex("FFD45A"));
            }
        }

        public static Visual ForChoice(string choiceId, UpgradeManager.UpgradeCategory category)
        {
            string id = choiceId.ToLowerInvariant();
            string skillKey = ResolveSkillKey(id);
            if (!string.IsNullOrEmpty(skillKey))
            {
                return SkillVisual(skillKey);
            }

            string systemKey = ResolveSystemKey(id);
            if (!string.IsNullOrEmpty(systemKey))
            {
                return SystemVisual(systemKey);
            }

            string movementKey = ResolveMovementKey(id);
            if (!string.IsNullOrEmpty(movementKey))
            {
                return MovementVisual(movementKey);
            }

            if (category == UpgradeManager.UpgradeCategory.System || id.StartsWith("system_") || id.StartsWith("upgrade_system_"))
            {
                return new Visual("icon.system.paired_synergy", "SYS", Hex("C8C3FF"));
            }

            return SchoolVisual(ResolveSchoolKey(id));
        }

        public static Color ForTier(int tier)
        {
            switch (Mathf.Clamp(tier, 1, 5))
            {
                case 1:
                    return Hex("9BA3AA");
                case 2:
                    return Hex("54D36B");
                case 3:
                    return Hex("4FA3FF");
                case 4:
                    return Hex("B066FF");
                default:
                    return Hex("FFD45A");
            }
        }

        public static string ResolveBaseId(string choiceId)
        {
            string id = choiceId.ToLowerInvariant();
            if (!id.StartsWith("upgrade_"))
            {
                return id;
            }

            if (id.StartsWith("upgrade_arcane_magic_missile")) return "arcane_magic_missile";
            if (id.StartsWith("upgrade_arcane_arcane_blast")) return "arcane_arcane_blast";
            if (id.StartsWith("upgrade_arcane_warp_pulse")) return "arcane_warp_pulse";
            if (id.StartsWith("upgrade_arcane_spell_echo")) return "arcane_spell_echo";
            if (id.StartsWith("upgrade_arcane_arcane_orbit")) return "arcane_arcane_orbit";
            if (id.StartsWith("upgrade_fire_fire_bolt")) return "fire_fire_bolt";
            if (id.StartsWith("upgrade_fire_flame_wave")) return "fire_flame_wave";
            if (id.StartsWith("upgrade_fire_burning_ground")) return "fire_burning_ground";
            if (id.StartsWith("upgrade_cold_frost_ring")) return "cold_frost_ring";
            if (id.StartsWith("upgrade_cold_ice_shard")) return "cold_ice_shard";
            if (id.StartsWith("upgrade_cold_glacial_field")) return "cold_glacial_field";
            if (id.StartsWith("upgrade_cold_crystal_prison")) return "cold_crystal_prison";
            if (id.StartsWith("upgrade_cold_shatter_line")) return "cold_shatter_line";
            if (id.StartsWith("upgrade_lightning_chain_bolt")) return "lightning_chain_bolt";
            if (id.StartsWith("upgrade_lightning_static_field")) return "lightning_static_field";
            if (id.StartsWith("upgrade_lightning_thunder_lance")) return "lightning_thunder_lance";
            if (id.StartsWith("upgrade_lightning_spark_surge")) return "lightning_spark_surge";
            if (id.StartsWith("upgrade_lightning_storm_call")) return "lightning_storm_call";
            if (id.StartsWith("upgrade_system_territory_casting")) return "system_territory_casting";
            if (id.StartsWith("upgrade_system_component")) return "system_component_boosts";
            if (id.StartsWith("upgrade_system_sacrifice")) return "system_sacrifice_casting";
            if (id.StartsWith("upgrade_system_echo") || id.StartsWith("upgrade_system_incantation")) return "system_echo_casting";
            if (id.StartsWith("upgrade_system_spell_weaving")) return "system_spell_weaving";
            if (id.StartsWith("upgrade_system_runic_magic")) return "system_runic_magic";
            if (id.StartsWith("upgrade_system_rhythm")) return "system_rhythm_casting";
            if (id.StartsWith("upgrade_system_spell_tension")) return "system_spell_tension";
            if (id.StartsWith("upgrade_movement_cloud_walk")) return "movement_cloud_walk";
            if (id.StartsWith("upgrade_movement_whirlwind")) return "movement_whirlwind";
            if (id.StartsWith("upgrade_movement_blink")) return "movement_blink";
            if (id.StartsWith("upgrade_movement_lunge")) return "movement_lunge";
            if (id.StartsWith("upgrade_movement_teleport")) return "movement_teleport";

            return id;
        }

        public static Color TextColorFor(Color background)
        {
            return IsBright(background) ? new Color(0.03f, 0.04f, 0.05f) : Color.white;
        }

        public static Color Darken(Color color, float multiplier)
        {
            return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, 0.96f);
        }

        private static Visual SkillVisual(string skillKey)
        {
            switch (skillKey)
            {
                case "magic_missile": return new Visual("icon.skill.magic_missile", "MM", Hex("78D7FF"));
                case "arcane_blast": return new Visual("icon.skill.arcane_blast", "AB", Hex("78D7FF"));
                case "warp_pulse": return new Visual("icon.skill.warp_pulse", "WP", Hex("78D7FF"));
                case "spell_echo": return new Visual("icon.skill.spell_echo", "SE", Hex("78D7FF"));
                case "arcane_orbit": return new Visual("icon.skill.arcane_orbit", "AO", Hex("78D7FF"));
                case "fire_bolt": return new Visual("icon.skill.fire_bolt", "FB", Hex("FF6A2A"));
                case "flame_wave": return new Visual("icon.skill.flame_wave", "FW", Hex("FF6A2A"));
                case "burning_ground": return new Visual("icon.skill.burning_ground", "BG", Hex("FF6A2A"));
                case "frost_ring": return new Visual("icon.skill.frost_ring", "FR", Hex("7FE7FF"));
                case "ice_shard": return new Visual("icon.skill.ice_shard", "IS", Hex("7FE7FF"));
                case "glacial_field": return new Visual("icon.skill.glacial_field", "GF", Hex("7FE7FF"));
                case "crystal_prison": return new Visual("icon.skill.crystal_prison", "CP", Hex("7FE7FF"));
                case "shatter_line": return new Visual("icon.skill.shatter_line", "SL", Hex("7FE7FF"));
                case "chain_bolt": return new Visual("icon.skill.chain_bolt", "CB", Hex("F5E84B"));
                case "static_field": return new Visual("icon.skill.static_field", "SF", Hex("F5E84B"));
                case "thunder_lance": return new Visual("icon.skill.thunder_lance", "TL", Hex("F5E84B"));
                case "spark_surge": return new Visual("icon.skill.spark_surge", "SS", Hex("F5E84B"));
                case "storm_call": return new Visual("icon.skill.storm_call", "SC", Hex("F5E84B"));
                default: return new Visual("icon.category.attack", "??", Hex("78D7FF"));
            }
        }

        private static Visual SystemVisual(string systemKey)
        {
            switch (systemKey)
            {
                case "territory_casting": return new Visual("icon.system.territory_casting", "TC", Hex("C8C3FF"));
                case "component_magic": return new Visual("icon.system.component_magic", "CM", Hex("6EF1C8"));
                case "sacrificial_casting": return new Visual("icon.system.sacrificial_casting", "SC", Hex("D43A4E"));
                case "incantation_casting": return new Visual("icon.system.incantation_casting", "IC", Hex("B7E7FF"));
                case "spell_weaving": return new Visual("icon.system.spell_weaving", "SW", Hex("EAA8FF"));
                case "runic_magic": return new Visual("icon.system.runic_magic", "RM", Hex("D6B36A"));
                case "rhythm_casting": return new Visual("icon.system.rhythm_casting", "RC", Hex("F8D56A"));
                case "spell_tension": return new Visual("icon.system.spell_tension", "ST", Hex("F06F86"));
                default: return new Visual("icon.system.paired_synergy", "PX", Color.white);
            }
        }

        private static Visual MovementVisual(string movementKey)
        {
            switch (movementKey)
            {
                case "blink": return new Visual("icon.movement.blink", "BL", Hex("88F7B0"));
                case "lunge": return new Visual("icon.movement.lunge", "LG", Hex("88F7B0"));
                case "teleport": return new Visual("icon.movement.teleport", "TP", Hex("88F7B0"));
                case "whirlwind": return new Visual("icon.movement.whirlwind", "WH", Hex("88F7B0"));
                case "cloud_walk": return new Visual("icon.movement.cloud_walk", "CW", Hex("88F7B0"));
                default: return new Visual("icon.category.movement", "MV", Hex("88F7B0"));
            }
        }

        private static Visual SchoolVisual(string schoolKey)
        {
            switch (schoolKey)
            {
                case "fire": return new Visual("icon.school.fire", "FIR", Hex("FF6A2A"));
                case "cold": return new Visual("icon.school.cold", "CLD", Hex("7FE7FF"));
                case "lightning": return new Visual("icon.school.lightning", "LTN", Hex("F5E84B"));
                case "earth": return new Visual("icon.school.earth", "ERT", Hex("A8743D"));
                case "mind": return new Visual("icon.school.mind", "MND", Hex("D889FF"));
                case "blood": return new Visual("icon.school.blood", "BLD", Hex("C0263E"));
                case "poison": return new Visual("icon.school.poison", "PSN", Hex("76D94E"));
                default: return new Visual("icon.school.arcane", "ARC", Hex("78D7FF"));
            }
        }

        private static string ResolveSkillKey(string id)
        {
            string baseId = ResolveBaseId(id);
            if (baseId.Contains("magic_missile")) return "magic_missile";
            if (baseId.Contains("arcane_blast")) return "arcane_blast";
            if (baseId.Contains("warp_pulse")) return "warp_pulse";
            if (baseId.Contains("spell_echo")) return "spell_echo";
            if (baseId.Contains("arcane_orbit")) return "arcane_orbit";
            if (baseId.Contains("fire_bolt")) return "fire_bolt";
            if (baseId.Contains("flame_wave")) return "flame_wave";
            if (baseId.Contains("burning_ground")) return "burning_ground";
            if (baseId.Contains("frost_ring")) return "frost_ring";
            if (baseId.Contains("ice_shard")) return "ice_shard";
            if (baseId.Contains("glacial_field")) return "glacial_field";
            if (baseId.Contains("crystal_prison")) return "crystal_prison";
            if (baseId.Contains("shatter_line")) return "shatter_line";
            if (baseId.Contains("chain_bolt")) return "chain_bolt";
            if (baseId.Contains("static_field")) return "static_field";
            if (baseId.Contains("thunder_lance")) return "thunder_lance";
            if (baseId.Contains("spark_surge")) return "spark_surge";
            if (baseId.Contains("storm_call")) return "storm_call";
            return string.Empty;
        }

        private static string ResolveSystemKey(string id)
        {
            string baseId = ResolveBaseId(id);
            if (baseId.Contains("territory_casting")) return "territory_casting";
            if (baseId.Contains("component")) return "component_magic";
            if (baseId.Contains("sacrifice")) return "sacrificial_casting";
            if (baseId.Contains("echo_casting") || baseId.Contains("incantation")) return "incantation_casting";
            if (baseId.Contains("spell_weaving")) return "spell_weaving";
            if (baseId.Contains("runic_magic")) return "runic_magic";
            if (baseId.Contains("rhythm_casting")) return "rhythm_casting";
            if (baseId.Contains("spell_tension")) return "spell_tension";
            return string.Empty;
        }

        private static string ResolveMovementKey(string id)
        {
            string baseId = ResolveBaseId(id);
            if (baseId.Contains("blink")) return "blink";
            if (baseId.Contains("lunge")) return "lunge";
            if (baseId.Contains("teleport")) return "teleport";
            if (baseId.Contains("whirlwind")) return "whirlwind";
            if (baseId.Contains("cloud_walk")) return "cloud_walk";
            return string.Empty;
        }

        private static string ResolveSchoolKey(string id)
        {
            if (id.Contains("fire_")) return "fire";
            if (id.Contains("cold_")) return "cold";
            if (id.Contains("lightning_")) return "lightning";
            if (id.Contains("earth_")) return "earth";
            if (id.Contains("mind_")) return "mind";
            if (id.Contains("blood_")) return "blood";
            if (id.Contains("poison_")) return "poison";
            return "arcane";
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString("#" + value, out Color color);
            return color;
        }

        private static bool IsBright(Color color)
        {
            return color.r * 0.299f + color.g * 0.587f + color.b * 0.114f > 0.62f;
        }
    }
}
