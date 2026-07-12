using Heroic.Systems;
using UnityEngine;

namespace Heroic.UI
{
    public static class SkillIconRegistry
    {
        private const string IconRoot = "SkillIcons/";

        public static Sprite GetIcon(string skillId)
        {
            if (string.IsNullOrEmpty(skillId))
            {
                return Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            }

            Sprite sprite = Resources.Load<Sprite>(IconRoot + skillId);
            return sprite != null ? sprite : Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        }

        public static string ResolveSkillId(UpgradeManager.DraftChoice choice)
        {
            if (choice == null)
            {
                return string.Empty;
            }

            return ResolveSkillId(choice.Id);
        }

        public static string ResolveSkillId(string choiceId)
        {
            if (string.IsNullOrEmpty(choiceId))
            {
                return string.Empty;
            }

            if (!choiceId.StartsWith("upgrade_"))
            {
                return choiceId;
            }

            string id = choiceId.ToLowerInvariant();
            string[] knownPrefixes =
            {
                "arcane_magic_missile",
                "arcane_arcane_blast",
                "arcane_warp_pulse",
                "arcane_spell_echo",
                "arcane_arcane_orbit",
                "arcane_force_field",
                "arcane_time_warp",
                "arcane_haste",
                "fire_fire_bolt",
                "fire_flame_wave",
                "fire_burning_ground",
                "fire_flame_shield",
                "fire_flame_wall",
                "fire_incinerate",
                "fire_inferno",
                "fire_phoenix",
                "cold_frost_ring",
                "cold_ice_shard",
                "cold_glacial_field",
                "cold_crystal_prison",
                "cold_shatter_line",
                "cold_blizzard",
                "cold_frostbite",
                "cold_cryostasis",
                "lightning_chain_bolt",
                "lightning_static_field",
                "lightning_thunder_lance",
                "lightning_spark_surge",
                "lightning_storm_call",
                "lightning_ball_lightning",
                "lightning_conductor",
                "lightning_fork",
                "earth_stone_spike",
                "earth_boulder_toss",
                "earth_earth_wall",
                "earth_quake",
                "earth_mud_trap",
                "earth_quicksand",
                "earth_brambles",
                "earth_swarm",
                "mind_psychic_lance",
                "mind_fear_wave",
                "mind_illusion_clone",
                "mind_confuse",
                "mind_mind_crush",
                "mind_telekinesis",
                "mind_push",
                "mind_mass_charm",
                "blood_blood_bolt",
                "blood_sanguine_pact",
                "blood_blood_nova",
                "blood_leech_bind",
                "blood_crimson_frenzy",
                "blood_drain_life",
                "blood_blood_boil",
                "blood_exsanguinate",
                "poison_poison_dart",
                "poison_toxic_cloud",
                "poison_venom_trail",
                "poison_infection",
                "poison_rot_bloom",
                "poison_disintegrate",
                "poison_poison_cloud",
                "poison_disease",
                "system_territory_casting",
                "system_component_boosts",
                "system_sacrifice_casting",
                "system_rhythm_casting",
                "system_spell_tension",
                "system_echo_casting",
                "system_spell_weaving",
                "system_runic_magic",
                "movement_blink",
                "movement_lunge",
                "movement_teleport",
                "movement_whirlwind",
                "movement_cloud_walk",
                "movement_invisibility",
                "movement_stoneskin",
                "movement_tunnel",
                "movement_flight"
            };

            for (int i = 0; i < knownPrefixes.Length; i++)
            {
                string prefix = "upgrade_" + knownPrefixes[i];
                if (id.StartsWith(prefix))
                {
                    return knownPrefixes[i];
                }
            }

            return choiceId.Replace("upgrade_", string.Empty);
        }

        public static Color GetColor(string skillId)
        {
            string id = (skillId ?? string.Empty).ToLowerInvariant();
            if (id.StartsWith("movement_"))
            {
                return Hex("88F7B0");
            }

            if (id.StartsWith("system_"))
            {
                return Hex("C8C3FF");
            }

            if (id.StartsWith("fire_"))
            {
                return Hex("FF6A2A");
            }

            if (id.StartsWith("cold_"))
            {
                return Hex("7FE7FF");
            }

            if (id.StartsWith("lightning_"))
            {
                return Hex("F5E84B");
            }

            if (id.StartsWith("earth_"))
            {
                return Hex("A8743D");
            }

            if (id.StartsWith("mind_"))
            {
                return Hex("D889FF");
            }

            if (id.StartsWith("blood_"))
            {
                return Hex("C0263E");
            }

            if (id.StartsWith("poison_"))
            {
                return Hex("76D94E");
            }

            return Hex("78D7FF");
        }

        public static string GetElementName(string skillId)
        {
            string id = (skillId ?? string.Empty).ToLowerInvariant();
            if (id.StartsWith("movement_")) return "Movement";
            if (id.StartsWith("system_")) return "System";
            if (id.StartsWith("arcane_")) return "Arcane";
            if (id.StartsWith("fire_")) return "Fire";
            if (id.StartsWith("cold_")) return "Cold";
            if (id.StartsWith("lightning_")) return "Lightning";
            if (id.StartsWith("earth_")) return "Earth";
            if (id.StartsWith("mind_")) return "Mind";
            if (id.StartsWith("blood_")) return "Blood";
            if (id.StartsWith("poison_")) return "Poison";
            return "Skill";
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString("#" + value, out Color color);
            return color;
        }
    }
}
