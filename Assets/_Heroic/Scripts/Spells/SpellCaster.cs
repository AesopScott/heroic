using UnityEngine;

namespace Heroic.Spells
{
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private MagicMissileCaster magicMissileCaster;
        [SerializeField] private ArcaneBlastCaster arcaneBlastCaster;
        [SerializeField] private WarpPulseCaster warpPulseCaster;
        [SerializeField] private SpellEchoCaster spellEchoCaster;
        [SerializeField] private ArcaneOrbitCaster arcaneOrbitCaster;
        [SerializeField] private bool startWithMagicMissile = true;

        private void Start()
        {
            SetOptionalCasterEnabled(magicMissileCaster, startWithMagicMissile);
            SetOptionalCasterEnabled(arcaneBlastCaster, false);
            SetOptionalCasterEnabled(warpPulseCaster, false);
            SetOptionalCasterEnabled(arcaneOrbitCaster, false);
        }

        public void CastPrimarySpell()
        {
            if (magicMissileCaster != null)
            {
                magicMissileCaster.enabled = true;
            }
        }

        public void CastSkill(string skillId)
        {
            EnableSkill(skillId);
        }

        public void EnableSkill(string skillId)
        {
            switch (skillId)
            {
                case "arcane_magic_missile":
                    SetOptionalCasterEnabled(magicMissileCaster, true);
                    break;
                case "arcane_arcane_blast":
                    SetOptionalCasterEnabled(arcaneBlastCaster, true);
                    break;
                case "arcane_warp_pulse":
                    SetOptionalCasterEnabled(warpPulseCaster, true);
                    break;
                case "arcane_spell_echo":
                    SetOptionalCasterEnabled(spellEchoCaster, true);
                    break;
                case "arcane_arcane_orbit":
                    SetOptionalCasterEnabled(arcaneOrbitCaster, true);
                    arcaneOrbitCaster?.SpawnOrbs();
                    break;
            }
        }

        private void SetOptionalCasterEnabled(MonoBehaviour caster, bool isEnabled)
        {
            if (caster != null)
            {
                caster.enabled = isEnabled;
            }
        }
    }
}
