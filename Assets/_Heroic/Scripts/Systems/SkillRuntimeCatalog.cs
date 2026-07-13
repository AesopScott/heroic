using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Heroic.Systems
{
    public readonly struct SkillRuntimeStats
    {
        public SkillRuntimeStats(float cooldown, float range, int damage, string effect, string baseSpec = "", string gameplay = "")
        {
            Cooldown = cooldown;
            Range = range;
            Damage = damage;
            Effect = effect;
            BaseSpec = baseSpec;
            Gameplay = gameplay;
        }

        public float Cooldown { get; }
        public float Range { get; }
        public int Damage { get; }
        public string Effect { get; }
        public string BaseSpec { get; }
        public string Gameplay { get; }
    }

    public static class SkillRuntimeCatalog
    {
        private const string AbilitiesCurrentResourcePath = "Reference/abilities-current";
        private static Dictionary<string, SkillRuntimeStats> stats;

        public static SkillRuntimeStats Get(string skillId)
        {
            EnsureLoaded();
            return !string.IsNullOrEmpty(skillId) && stats.TryGetValue(skillId, out SkillRuntimeStats value)
                ? value
                : new SkillRuntimeStats(0f, 0f, 0, "Missing from abilities-current", "Not found in canonical ability reference.", string.Empty);
        }

        private static void EnsureLoaded()
        {
            if (stats != null)
            {
                return;
            }

            stats = new Dictionary<string, SkillRuntimeStats>();
            TextAsset source = Resources.Load<TextAsset>(AbilitiesCurrentResourcePath);
            if (source != null && !string.IsNullOrWhiteSpace(source.text))
            {
                ParseAbilitiesCurrent(source.text);
            }
        }

        private static void ParseAbilitiesCurrent(string markdown)
        {
            string lane = string.Empty;
            string[] lines = markdown.Replace("\r\n", "\n").Split('\n');
            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (line.StartsWith("## "))
                {
                    lane = ResolveLane(line);
                    continue;
                }

                if (string.IsNullOrEmpty(lane) || !line.StartsWith("|") || line.Contains("---"))
                {
                    continue;
                }

                string[] cells = SplitMarkdownRow(line);
                if (cells.Length < 7 || cells[0] == "Skill" || cells[0] == "System")
                {
                    continue;
                }

                string id = ResolveId(lane, cells[0]);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                string baseSpec = CleanCell(cells[3]);
                string gameplay = CleanCell(cells[4]);
                string effect = string.IsNullOrWhiteSpace(gameplay) ? CleanCell(cells[1]) : gameplay;
                stats[id] = new SkillRuntimeStats(ParseCooldown(baseSpec), ParseRange(baseSpec), ParseDamage(baseSpec), effect, baseSpec, gameplay);
            }
        }

        private static string[] SplitMarkdownRow(string line)
        {
            string trimmed = line.Trim().Trim('|');
            string[] cells = trimmed.Split('|');
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = CleanCell(cells[i]);
            }

            return cells;
        }

        private static string ResolveLane(string heading)
        {
            if (heading.Contains("Arcane Abilities")) return "arcane";
            if (heading.Contains("Fire Abilities")) return "fire";
            if (heading.Contains("Cold Abilities")) return "cold";
            if (heading.Contains("Lightning Abilities")) return "lightning";
            if (heading.Contains("Earth Abilities")) return "earth";
            if (heading.Contains("Mind Abilities")) return "mind";
            if (heading.Contains("Blood Abilities")) return "blood";
            if (heading.Contains("Poison Abilities")) return "poison";
            if (heading.Contains("Movement Skills")) return "movement";
            if (heading.Contains("Spell Systems")) return "system";
            return string.Empty;
        }

        private static string ResolveId(string lane, string displayName)
        {
            string normalized = Normalize(displayName);
            if (lane == "system")
            {
                switch (normalized)
                {
                    case "component_magic": return "system_component_boosts";
                    case "sacrificial_casting": return "system_sacrifice_casting";
                    case "territory_casting": return "system_territory_casting";
                    default: return "system_" + normalized;
                }
            }

            return lane + "_" + normalized;
        }

        private static string Normalize(string value)
        {
            string normalized = Regex.Replace(value.ToLowerInvariant(), @"[^a-z0-9]+", "_").Trim('_');
            return normalized == "quake" ? "quake" : normalized;
        }

        private static float ParseCooldown(string baseSpec)
        {
            Match match = Regex.Match(baseSpec, @"(?<![A-Za-z])([0-9]+(?:\.[0-9]+)?)s cooldown");
            return match.Success && float.TryParse(match.Groups[1].Value, out float cooldown) ? cooldown : 0f;
        }

        private static float ParseRange(string baseSpec)
        {
            Match match = Regex.Match(baseSpec, @"range ([0-9]+(?:\.[0-9]+)?)");
            if (!match.Success)
            {
                match = Regex.Match(baseSpec, @"radius ([0-9]+(?:\.[0-9]+)?)");
            }

            return match.Success && float.TryParse(match.Groups[1].Value, out float range) ? range : 0f;
        }

        private static int ParseDamage(string baseSpec)
        {
            Match match = Regex.Match(baseSpec, @"([0-9]+)\s+(?:[A-Za-z -]+?\s)?damage");
            return match.Success && int.TryParse(match.Groups[1].Value, out int damage) ? damage : 0;
        }

        private static string CleanCell(string value)
        {
            return Regex.Replace(value.Trim(), @"\s+", " ");
        }
    }
}
