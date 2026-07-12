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
        [SerializeField] private FireBoltCaster fireBoltCaster;
        [SerializeField] private FlameWaveCaster flameWaveCaster;
        [SerializeField] private BurningGroundCaster burningGroundCaster;
        [SerializeField] private FrostRingCaster frostRingCaster;
        [SerializeField] private IceShardCaster iceShardCaster;
        [SerializeField] private GlacialFieldCaster glacialFieldCaster;
        [SerializeField] private CrystalPrisonCaster crystalPrisonCaster;
        [SerializeField] private ShatterLineCaster shatterLineCaster;
        [SerializeField] private ChainBoltCaster chainBoltCaster;
        [SerializeField] private StaticFieldCaster staticFieldCaster;
        [SerializeField] private ThunderLanceCaster thunderLanceCaster;
        [SerializeField] private SparkSurgeCaster sparkSurgeCaster;
        [SerializeField] private StormCallCaster stormCallCaster;
        [SerializeField] private bool startWithMagicMissile = true;

        private void Start()
        {
            SetOptionalCasterEnabled(magicMissileCaster, startWithMagicMissile);
            SetOptionalCasterEnabled(arcaneBlastCaster, false);
            SetOptionalCasterEnabled(warpPulseCaster, false);
            SetOptionalCasterEnabled(arcaneOrbitCaster, false);
            SetOptionalCasterEnabled(fireBoltCaster, false);
            SetOptionalCasterEnabled(flameWaveCaster, false);
            SetOptionalCasterEnabled(burningGroundCaster, false);
            SetOptionalCasterEnabled(frostRingCaster, false);
            SetOptionalCasterEnabled(iceShardCaster, false);
            SetOptionalCasterEnabled(glacialFieldCaster, false);
            SetOptionalCasterEnabled(crystalPrisonCaster, false);
            SetOptionalCasterEnabled(shatterLineCaster, false);
            SetOptionalCasterEnabled(chainBoltCaster, false);
            SetOptionalCasterEnabled(staticFieldCaster, false);
            SetOptionalCasterEnabled(thunderLanceCaster, false);
            SetOptionalCasterEnabled(sparkSurgeCaster, false);
            SetOptionalCasterEnabled(stormCallCaster, false);
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
                    spellEchoCaster?.SetEchoEnabled(true);
                    break;
                case "arcane_arcane_orbit":
                    SetOptionalCasterEnabled(arcaneOrbitCaster, true);
                    arcaneOrbitCaster?.SpawnOrbs();
                    break;
                case "fire_fire_bolt":
                    SetOptionalCasterEnabled(fireBoltCaster, true);
                    break;
                case "fire_flame_wave":
                    SetOptionalCasterEnabled(flameWaveCaster, true);
                    break;
                case "fire_burning_ground":
                    SetOptionalCasterEnabled(burningGroundCaster, true);
                    break;
                case "cold_frost_ring":
                    SetOptionalCasterEnabled(frostRingCaster, true);
                    break;
                case "cold_ice_shard":
                    SetOptionalCasterEnabled(iceShardCaster, true);
                    break;
                case "cold_glacial_field":
                    SetOptionalCasterEnabled(glacialFieldCaster, true);
                    break;
                case "cold_crystal_prison":
                    SetOptionalCasterEnabled(crystalPrisonCaster, true);
                    break;
                case "cold_shatter_line":
                    SetOptionalCasterEnabled(shatterLineCaster, true);
                    break;
                case "lightning_chain_bolt":
                    SetOptionalCasterEnabled(chainBoltCaster, true);
                    break;
                case "lightning_static_field":
                    SetOptionalCasterEnabled(staticFieldCaster, true);
                    break;
                case "lightning_thunder_lance":
                    SetOptionalCasterEnabled(thunderLanceCaster, true);
                    break;
                case "lightning_spark_surge":
                    SetOptionalCasterEnabled(sparkSurgeCaster, true);
                    break;
                case "lightning_storm_call":
                    SetOptionalCasterEnabled(stormCallCaster, true);
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
