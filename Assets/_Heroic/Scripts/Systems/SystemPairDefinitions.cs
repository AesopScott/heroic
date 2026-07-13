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
            new PairDefinition("system_pair_territory_components", "Territory + Components", "system_territory_casting", "system_component_boosts"),
            new PairDefinition("system_pair_territory_sacrifice", "Territory + Sacrifice", "system_territory_casting", "system_sacrifice_casting"),
            new PairDefinition("system_pair_territory_rhythm", "Territory + Rhythm", "system_territory_casting", "system_rhythm_casting"),
            new PairDefinition("system_pair_territory_tension", "Territory + Tension", "system_territory_casting", "system_spell_tension"),
            new PairDefinition("system_pair_components_sacrifice", "Components + Sacrifice", "system_component_boosts", "system_sacrifice_casting"),
            new PairDefinition("system_pair_components_rhythm", "Components + Rhythm", "system_component_boosts", "system_rhythm_casting"),
            new PairDefinition("system_pair_components_tension", "Components + Tension", "system_component_boosts", "system_spell_tension"),
            new PairDefinition("system_pair_sacrifice_rhythm", "Sacrifice + Rhythm", "system_sacrifice_casting", "system_rhythm_casting"),
            new PairDefinition("system_pair_sacrifice_tension", "Sacrifice + Tension", "system_sacrifice_casting", "system_spell_tension"),
            new PairDefinition("system_pair_rhythm_tension", "Rhythm + Tension", "system_rhythm_casting", "system_spell_tension")
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
