using Heroic.Spells;
using UnityEngine;

namespace Heroic.Systems
{
    public class MindUpgradeApplier : MonoBehaviour
    {
        [SerializeField] private MindAbilityCaster psychicLance;
        [SerializeField] private MindAbilityCaster fearWave;
        [SerializeField] private MindAbilityCaster illusionClone;
        [SerializeField] private MindAbilityCaster confuse;
        [SerializeField] private MindAbilityCaster mindCrush;

        public bool Apply(string choiceId, int tier)
        {
            int t = Mathf.Clamp(tier, 1, 5);
            switch (choiceId)
            {
                case "upgrade_mind_psychic_lance_more_damage": psychicLance?.SetDamage(Value(t, 26, 38, 54, 76, 105)); return true;
                case "upgrade_mind_psychic_lance_longer_range": psychicLance?.SetRange(Value(t, 7f, 8.5f, 10f, 12f, 14.5f)); return true;
                case "upgrade_mind_psychic_lance_mind_pierce": psychicLance?.SetWidth(Value(t, 1.5f, 1.9f, 2.4f, 3f, 3.8f)); return true;
                case "upgrade_mind_fear_wave_bigger_wave": fearWave?.SetWidth(Value(t, 1.8f, 2.3f, 2.9f, 3.6f, 4.5f)); return true;
                case "upgrade_mind_fear_wave_longer_fear": fearWave?.SetDuration(Value(t, 2.5f, 3.2f, 4f, 5f, 6.2f)); return true;
                case "upgrade_mind_fear_wave_stronger_panic": fearWave?.SetDamage(Value(t, 20, 28, 38, 52, 70)); return true;
                case "upgrade_mind_illusion_clone_more_clones": illusionClone?.SetCount(Value(t, 2, 3, 4, 5, 7)); return true;
                case "upgrade_mind_illusion_clone_stronger_decoys": illusionClone?.SetDuration(Value(t, 3f, 4f, 5.2f, 6.8f, 8.5f)); return true;
                case "upgrade_mind_illusion_clone_clone_burst": illusionClone?.SetDamage(Value(t, 22, 32, 45, 62, 86)); return true;
                case "upgrade_mind_confuse_wider_effect": confuse?.SetRadius(Value(t, 2f, 2.5f, 3.1f, 3.8f, 4.7f)); return true;
                case "upgrade_mind_confuse_longer_confusion": confuse?.SetDuration(Value(t, 2.6f, 3.4f, 4.4f, 5.7f, 7f)); return true;
                case "upgrade_mind_confuse_deeper_confusion": confuse?.SetDamage(Value(t, 22, 30, 42, 58, 80)); return true;
                case "upgrade_mind_mind_crush_more_damage": mindCrush?.SetDamage(Value(t, 32, 46, 64, 88, 120)); return true;
                case "upgrade_mind_mind_crush_area_crush": mindCrush?.SetRadius(Value(t, 1.9f, 2.4f, 3f, 3.7f, 4.5f)); return true;
                case "upgrade_mind_mind_crush_execution_crush": mindCrush?.SetExecutionMultiplier(Value(t, 1.45f, 1.8f, 2.2f, 2.7f, 3.3f)); return true;
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
