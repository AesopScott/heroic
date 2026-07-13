using Heroic.Systems;

namespace Heroic.UI
{
    public static class SkillTooltipText
    {
        public static string TitleFor(string skillId)
        {
            if (string.IsNullOrWhiteSpace(skillId))
            {
                return "Skill";
            }

            string id = skillId;
            if (id.StartsWith("system_pair_"))
            {
                id = id.Substring("system_pair_".Length);
            }
            else if (id.StartsWith("system_"))
            {
                id = id.Substring("system_".Length);
            }
            else if (id.StartsWith("movement_"))
            {
                id = id.Substring("movement_".Length);
            }
            else
            {
                int separator = id.IndexOf('_');
                if (separator >= 0 && separator < id.Length - 1)
                {
                    id = id.Substring(separator + 1);
                }
            }

            return ToTitleCase(id.Replace('_', ' '));
        }

        public static string BodyFor(string skillId, string extra = "")
        {
            SkillRuntimeStats stats = SkillRuntimeCatalog.Get(skillId);
            string body = $"Base Spec: {stats.BaseSpec}\nGameplay: {stats.Effect}";
            if (!string.IsNullOrWhiteSpace(extra))
            {
                body += "\n" + extra;
            }

            return body;
        }

        public static string UpgradeBody(string upgradePathId, int tier)
        {
            return $"Upgrade: {TitleFor(upgradePathId.Replace("upgrade_", string.Empty))}\nTier: {tier}";
        }

        private static string ToTitleCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Skill";
            }

            string[] words = value.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(words[i]))
                {
                    continue;
                }

                words[i] = char.ToUpperInvariant(words[i][0]) + (words[i].Length > 1 ? words[i].Substring(1) : string.Empty);
            }

            return string.Join(" ", words);
        }
    }
}
