using Heroic.Enemies;
using Heroic.Player;
using Heroic.Spells;
using TMPro;
using UnityEngine;

namespace Heroic.Core
{
    public class InvestorShowcaseMode : MonoBehaviour
    {
        [SerializeField] private bool enabledForPrototype = false;
        [SerializeField] private SpellCaster spellCaster;
        [SerializeField] private MovementCaster movementCaster;
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private BossSpawner bossSpawner;
        [SerializeField] private TMP_Text showcaseLabel;

        private void Start()
        {
            if (!enabledForPrototype)
            {
                return;
            }

            ApplyShowcaseLoadout();
            ApplyShowcasePacing();
            UpdateShowcaseLabel();
        }

        private void ApplyShowcaseLoadout()
        {
            spellCaster?.EnableSkill("arcane_magic_missile");

            // Movement skills are earned through level-up choices, not granted at run start.
        }

        private void ApplyShowcasePacing()
        {
            playerExperience?.ConfigureLeveling(15, 1.18f);
            bossSpawner?.SetSpawnAtSeconds(120f);
        }

        private void UpdateShowcaseLabel()
        {
            if (showcaseLabel != null)
            {
                showcaseLabel.text = "Heroic 1.0 Showcase";
            }
        }
    }
}
