using System.Collections.Generic;

namespace Heroic.UI
{
    public static class SystemInterfaceCatalog
    {
        public readonly struct InterfaceDefinition
        {
            public InterfaceDefinition(string systemId, string displayName, string requiredInterface, string currentStatus)
            {
                SystemId = systemId;
                DisplayName = displayName;
                RequiredInterface = requiredInterface;
                CurrentStatus = currentStatus;
            }

            public string SystemId { get; }
            public string DisplayName { get; }
            public string RequiredInterface { get; }
            public string CurrentStatus { get; }
        }

        public static IReadOnlyList<InterfaceDefinition> All => Definitions;

        public static InterfaceDefinition ForSystem(string systemId)
        {
            foreach (InterfaceDefinition definition in Definitions)
            {
                if (definition.SystemId == systemId)
                {
                    return definition;
                }
            }

            return new InterfaceDefinition(systemId, FormatUnknownSystem(systemId), "Build summary entry", "Interface shell only");
        }

        private static readonly InterfaceDefinition[] Definitions =
        {
            new InterfaceDefinition(
                "system_territory_casting",
                "Territory Casting",
                "World zones, labels, and active bonus list",
                "Implemented world interface; tuning needed"),
            new InterfaceDefinition(
                "system_component_boosts",
                "Component Magic",
                "Ground pickups, stack tracker, duration timers, total boost display",
                "Art/UI shell defined; mechanics pending"),
            new InterfaceDefinition(
                "system_sacrifice_casting",
                "Sacrificial Casting",
                "Penalty tracker, gained power display, proc warnings",
                "Art/UI shell defined; mechanics pending"),
            new InterfaceDefinition(
                "system_echo_casting",
                "Incantation Casting",
                "Double-cast, damage surge, and recovery proc feedback",
                "Art/UI shell defined; mechanics pending"),
            new InterfaceDefinition(
                "system_spell_weaving",
                "Spell Weaving",
                "Woven element tooltip rows and secondary color VFX",
                "Art/UI shell defined; mechanics pending"),
            new InterfaceDefinition(
                "system_runic_magic",
                "Runic Magic",
                "Ground rune glyphs, trigger radius, lifetime, rune-enabled tooltip flag",
                "Art/UI shell defined; mechanics pending"),
            new InterfaceDefinition(
                "system_rhythm_casting",
                "Rhythm Casting",
                "Beat meter, timing window, streak/perfect/miss feedback",
                "Legacy shell; needs keep/cut decision"),
            new InterfaceDefinition(
                "system_spell_tension",
                "Spell Tension",
                "Hold meter, debt meter, backlash warning",
                "Legacy shell; needs keep/cut decision")
        };

        private static string FormatUnknownSystem(string systemId)
        {
            if (string.IsNullOrWhiteSpace(systemId))
            {
                return "Unknown System";
            }

            string text = systemId.StartsWith("system_") ? systemId.Substring("system_".Length) : systemId;
            string[] parts = text.Split('_');
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    parts[i] = char.ToUpperInvariant(parts[i][0]) + (parts[i].Length > 1 ? parts[i].Substring(1) : string.Empty);
                }
            }

            return string.Join(" ", parts);
        }
    }
}
