using Heroic.Enemies;
using Heroic.Player;
using Heroic.Spells;
using Heroic.Systems;
using TMPro;
using UnityEngine;

namespace Heroic.Core
{
    public class InvestorShowcaseMode : MonoBehaviour
    {
        [SerializeField] private bool enabledForPrototype = true;
        [SerializeField] private SpellCaster spellCaster;
        [SerializeField] private ArcaneUpgradeApplier arcaneUpgradeApplier;
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
            spellCaster?.EnableSkill("arcane_arcane_blast");
            spellCaster?.EnableSkill("arcane_warp_pulse");
            spellCaster?.EnableSkill("arcane_spell_echo");
            spellCaster?.EnableSkill("arcane_arcane_orbit");

            arcaneUpgradeApplier?.Apply("upgrade_arcane_magic_missile_seeking_shot", 2);
            arcaneUpgradeApplier?.Apply("upgrade_arcane_arcane_blast_scatter", 2);
            arcaneUpgradeApplier?.Apply("upgrade_arcane_arcane_orbit_more_orbs", 1);

            if (movementCaster != null)
            {
                movementCaster.EquipMovementSkill(0, MovementCaster.MovementSkillId.Blink);
                movementCaster.EquipMovementSkill(1, MovementCaster.MovementSkillId.Lunge);
                movementCaster.EquipMovementSkill(2, MovementCaster.MovementSkillId.Teleport);
            }
        }

        private void ApplyShowcasePacing()
        {
            playerExperience?.ConfigureLeveling(5, 1.18f);
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
