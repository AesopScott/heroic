using Heroic.Spells;
using UnityEngine;

namespace Heroic.Systems
{
    public class PoisonUpgradeApplier : MonoBehaviour
    {
        [SerializeField] private PoisonAbilityCaster poisonDart;
        [SerializeField] private PoisonAbilityCaster toxicCloud;
        [SerializeField] private PoisonAbilityCaster venomTrail;
        [SerializeField] private PoisonAbilityCaster infection;
        [SerializeField] private PoisonAbilityCaster rotBloom;

        public bool Apply(string choiceId, int tier)
        {
            int t = Mathf.Clamp(tier, 1, 5);
            switch (choiceId)
            {
                case "upgrade_poison_poison_dart_more_darts": poisonDart?.SetCount(Value(t, 2, 3, 4, 5, 7)); return true;
                case "upgrade_poison_poison_dart_stronger_poison": poisonDart?.SetDamage(Value(t, 14, 20, 28, 40, 58)); return true;
                case "upgrade_poison_poison_dart_spread_poison": poisonDart?.SetSpreadRadius(Value(t, 2.5f, 3.3f, 4.2f, 5.4f, 7f)); return true;
                case "upgrade_poison_toxic_cloud_bigger_cloud": toxicCloud?.SetRadius(Value(t, 2.4f, 3f, 3.8f, 4.8f, 6f)); return true;
                case "upgrade_poison_toxic_cloud_longer_cloud": toxicCloud?.SetDuration(Value(t, 5f, 6.4f, 8f, 10f, 13f)); return true;
                case "upgrade_poison_toxic_cloud_heavier_cloud": toxicCloud?.SetDamage(Value(t, 14, 20, 28, 40, 58)); return true;
                case "upgrade_poison_venom_trail_longer_trail": venomTrail?.SetDuration(Value(t, 5f, 6.5f, 8.5f, 11f, 14f)); return true;
                case "upgrade_poison_venom_trail_stronger_trail": venomTrail?.SetDamage(Value(t, 13, 18, 26, 36, 52)); return true;
                case "upgrade_poison_venom_trail_sticky_trail": venomTrail?.SetSlowMultiplier(Value(t, 0.75f, 0.65f, 0.55f, 0.42f, 0.3f)); return true;
                case "upgrade_poison_infection_faster_spread": infection?.SetTickInterval(Value(t, 0.45f, 0.36f, 0.28f, 0.22f, 0.16f)); return true;
                case "upgrade_poison_infection_stronger_infection": infection?.SetDamage(Value(t, 14, 21, 31, 45, 66)); return true;
                case "upgrade_poison_infection_collapse": infection?.SetBurstDamage(Value(t, 16, 26, 40, 62, 90)); return true;
                case "upgrade_poison_rot_bloom_bigger_bloom": rotBloom?.SetRadius(Value(t, 2.4f, 3f, 3.8f, 4.8f, 6f)); return true;
                case "upgrade_poison_rot_bloom_more_bloom_damage": rotBloom?.SetDamage(Value(t, 16, 24, 36, 52, 76)); return true;
                case "upgrade_poison_rot_bloom_lingering_rot": rotBloom?.SetDuration(Value(t, 5f, 6.5f, 8.5f, 11f, 14f)); return true;
                default: return false;
            }
        }

        private int Value(int tier, int basic, int advanced, int expert, int master, int grandmaster)
        {
            switch (tier) { case 1: return basic; case 2: return advanced; case 3: return expert; case 4: return master; default: return grandmaster; }
        }

        private float Value(int tier, float basic, float advanced, float expert, float master, float grandmaster)
        {
            switch (tier) { case 1: return basic; case 2: return advanced; case 3: return expert; case 4: return master; default: return grandmaster; }
        }
    }
}
