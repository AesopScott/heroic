using System.Collections.Generic;

namespace Heroic.Systems
{
    public static class SystemPairDefinitions
    {
        public readonly struct PairDefinition
        {
            public readonly string PairId;
            public readonly string DisplayName;
            public readonly string FirstSystemId;
            public readonly string SecondSystemId;

            public PairDefinition(string pairId, string displayName, string firstSystemId, string secondSystemId)
            {
                PairId = pairId;
                DisplayName = displayName;
                FirstSystemId = firstSystemId;
                SecondSystemId = secondSystemId;
            }
        }

        public static readonly PairDefinition[] Pairs =
        {
            new PairDefinition("system_pair_territorial_components", "Territorial Components", "system_territory_casting", "system_component_boosts"),
            new PairDefinition("system_pair_blood_territory", "Blood Territory", "system_territory_casting", "system_sacrifice_casting"),
            new PairDefinition("system_pair_inscribed_territory", "Inscribed Territory", "system_territory_casting", "system_echo_casting"),
            new PairDefinition("system_pair_woven_territory", "Woven Territory", "system_territory_casting", "system_spell_weaving"),
            new PairDefinition("system_pair_runemarked_territory", "Runemarked Territory", "system_territory_casting", "system_runic_magic"),
            new PairDefinition("system_pair_blood_reagents", "Blood Reagents", "system_component_boosts", "system_sacrifice_casting"),
            new PairDefinition("system_pair_chanted_components", "Chanted Components", "system_component_boosts", "system_echo_casting"),
            new PairDefinition("system_pair_woven_reagents", "Woven Reagents", "system_component_boosts", "system_spell_weaving"),
            new PairDefinition("system_pair_runic_components", "Runic Components", "system_component_boosts", "system_runic_magic"),
            new PairDefinition("system_pair_blood_incantations", "Blood Incantations", "system_sacrifice_casting", "system_echo_casting"),
            new PairDefinition("system_pair_sacrificial_weave", "Sacrificial Weave", "system_sacrifice_casting", "system_spell_weaving"),
            new PairDefinition("system_pair_blood_runes", "Blood Runes", "system_sacrifice_casting", "system_runic_magic"),
            new PairDefinition("system_pair_woven_incantations", "Woven Incantations", "system_echo_casting", "system_spell_weaving"),
            new PairDefinition("system_pair_runic_incantations", "Runic Incantations", "system_echo_casting", "system_runic_magic"),
            new PairDefinition("system_pair_woven_runes", "Woven Runes", "system_spell_weaving", "system_runic_magic")
        };

        public static bool IsPairUpgrade(string choiceId)
        {
            return !string.IsNullOrEmpty(choiceId) && choiceId.StartsWith("upgrade_system_pair_");
        }

        public static string ResolvePairId(string choiceId)
        {
            if (string.IsNullOrEmpty(choiceId))
            {
                return string.Empty;
            }

            foreach (PairDefinition pair in Pairs)
            {
                string prefix = "upgrade_" + pair.PairId + "_";
                if (choiceId.StartsWith(prefix))
                {
                    return pair.PairId;
                }
            }

            return string.Empty;
        }

        public static bool TryGetPrerequisites(string choiceId, out string firstSystemId, out string secondSystemId)
        {
            string pairId = ResolvePairId(choiceId);
            foreach (PairDefinition pair in Pairs)
            {
                if (pair.PairId == pairId)
                {
                    firstSystemId = pair.FirstSystemId;
                    secondSystemId = pair.SecondSystemId;
                    return true;
                }
            }

            firstSystemId = string.Empty;
            secondSystemId = string.Empty;
            return false;
        }

        public static void AddActivePairs(IReadOnlyList<string> learnedSkillIds, List<string> destination)
        {
            if (learnedSkillIds == null || destination == null)
            {
                return;
            }

            foreach (PairDefinition pair in Pairs)
            {
                if (Contains(learnedSkillIds, pair.FirstSystemId) && Contains(learnedSkillIds, pair.SecondSystemId) && !destination.Contains(pair.PairId))
                {
                    destination.Add(pair.PairId);
                }
            }
        }

        private static bool Contains(IReadOnlyList<string> values, string value)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
