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
        [SerializeField] private EarthAbilityCaster stoneSpikeCaster;
        [SerializeField] private EarthAbilityCaster boulderTossCaster;
        [SerializeField] private EarthAbilityCaster earthWallCaster;
        [SerializeField] private EarthAbilityCaster quakeCaster;
        [SerializeField] private EarthAbilityCaster mudTrapCaster;
        [SerializeField] private MindAbilityCaster psychicLanceCaster;
        [SerializeField] private MindAbilityCaster fearWaveCaster;
        [SerializeField] private MindAbilityCaster illusionCloneCaster;
        [SerializeField] private MindAbilityCaster confuseCaster;
        [SerializeField] private MindAbilityCaster mindCrushCaster;
        [SerializeField] private BloodAbilityCaster bloodBoltCaster;
        [SerializeField] private BloodAbilityCaster sanguinePactCaster;
        [SerializeField] private BloodAbilityCaster bloodNovaCaster;
        [SerializeField] private BloodAbilityCaster leechBindCaster;
        [SerializeField] private BloodAbilityCaster crimsonFrenzyCaster;
        [SerializeField] private PoisonAbilityCaster poisonDartCaster;
        [SerializeField] private PoisonAbilityCaster toxicCloudCaster;
        [SerializeField] private PoisonAbilityCaster venomTrailCaster;
        [SerializeField] private PoisonAbilityCaster infectionCaster;
        [SerializeField] private PoisonAbilityCaster rotBloomCaster;
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
            SetOptionalCasterEnabled(stoneSpikeCaster, false);
            SetOptionalCasterEnabled(boulderTossCaster, false);
            SetOptionalCasterEnabled(earthWallCaster, false);
            SetOptionalCasterEnabled(quakeCaster, false);
            SetOptionalCasterEnabled(mudTrapCaster, false);
            SetOptionalCasterEnabled(psychicLanceCaster, false);
            SetOptionalCasterEnabled(fearWaveCaster, false);
            SetOptionalCasterEnabled(illusionCloneCaster, false);
            SetOptionalCasterEnabled(confuseCaster, false);
            SetOptionalCasterEnabled(mindCrushCaster, false);
            SetOptionalCasterEnabled(bloodBoltCaster, false);
            SetOptionalCasterEnabled(sanguinePactCaster, false);
            SetOptionalCasterEnabled(bloodNovaCaster, false);
            SetOptionalCasterEnabled(leechBindCaster, false);
            SetOptionalCasterEnabled(crimsonFrenzyCaster, false);
            SetOptionalCasterEnabled(poisonDartCaster, false);
            SetOptionalCasterEnabled(toxicCloudCaster, false);
            SetOptionalCasterEnabled(venomTrailCaster, false);
            SetOptionalCasterEnabled(infectionCaster, false);
            SetOptionalCasterEnabled(rotBloomCaster, false);
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
                case "earth_stone_spike":
                    SetOptionalCasterEnabled(stoneSpikeCaster, true);
                    break;
                case "earth_boulder_toss":
                    SetOptionalCasterEnabled(boulderTossCaster, true);
                    break;
                case "earth_earth_wall":
                    SetOptionalCasterEnabled(earthWallCaster, true);
                    break;
                case "earth_quake":
                    SetOptionalCasterEnabled(quakeCaster, true);
                    break;
                case "earth_mud_trap":
                    SetOptionalCasterEnabled(mudTrapCaster, true);
                    break;
                case "mind_psychic_lance":
                    SetOptionalCasterEnabled(psychicLanceCaster, true);
                    break;
                case "mind_fear_wave":
                    SetOptionalCasterEnabled(fearWaveCaster, true);
                    break;
                case "mind_illusion_clone":
                    SetOptionalCasterEnabled(illusionCloneCaster, true);
                    break;
                case "mind_confuse":
                    SetOptionalCasterEnabled(confuseCaster, true);
                    break;
                case "mind_mind_crush":
                    SetOptionalCasterEnabled(mindCrushCaster, true);
                    break;
                case "blood_blood_bolt":
                    SetOptionalCasterEnabled(bloodBoltCaster, true);
                    break;
                case "blood_sanguine_pact":
                    SetOptionalCasterEnabled(sanguinePactCaster, true);
                    break;
                case "blood_blood_nova":
                    SetOptionalCasterEnabled(bloodNovaCaster, true);
                    break;
                case "blood_leech_bind":
                    SetOptionalCasterEnabled(leechBindCaster, true);
                    break;
                case "blood_crimson_frenzy":
                    SetOptionalCasterEnabled(crimsonFrenzyCaster, true);
                    break;
                case "poison_poison_dart":
                    SetOptionalCasterEnabled(poisonDartCaster, true);
                    break;
                case "poison_toxic_cloud":
                    SetOptionalCasterEnabled(toxicCloudCaster, true);
                    break;
                case "poison_venom_trail":
                    SetOptionalCasterEnabled(venomTrailCaster, true);
                    break;
                case "poison_infection":
                    SetOptionalCasterEnabled(infectionCaster, true);
                    break;
                case "poison_rot_bloom":
                    SetOptionalCasterEnabled(rotBloomCaster, true);
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
