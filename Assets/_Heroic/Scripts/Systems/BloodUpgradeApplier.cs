using Heroic.Spells;
using UnityEngine;

namespace Heroic.Systems
{
    public class BloodUpgradeApplier : MonoBehaviour
    {
        [SerializeField] private BloodAbilityCaster bloodBolt;
        [SerializeField] private BloodAbilityCaster sanguinePact;
        [SerializeField] private BloodAbilityCaster bloodNova;
        [SerializeField] private BloodAbilityCaster leechBind;
        [SerializeField] private BloodAbilityCaster crimsonFrenzy;

        public bool Apply(string choiceId, int tier)
        {
            int t = Mathf.Clamp(tier, 1, 5);
            switch (choiceId)
            {
                case "upgrade_blood_blood_bolt_more_damage": bloodBolt?.SetDamage(Value(t, 28, 40, 56, 78, 108)); return true;
                case "upgrade_blood_blood_bolt_lifesteal": bloodBolt?.SetLifestealMultiplier(Value(t, 0.35f, 0.48f, 0.62f, 0.8f, 1f)); return true;
                case "upgrade_blood_blood_bolt_splash_drain": bloodBolt?.SetRadius(Value(t, 2f, 2.5f, 3f, 3.7f, 4.5f)); return true;
                case "upgrade_blood_sanguine_pact_more_power": sanguinePact?.SetPowerMultiplier(Value(t, 1.35f, 1.55f, 1.8f, 2.1f, 2.5f)); return true;
                case "upgrade_blood_sanguine_pact_more_healing": sanguinePact?.SetLifestealMultiplier(Value(t, 0.7f, 0.95f, 1.25f, 1.6f, 2f)); return true;
                case "upgrade_blood_sanguine_pact_lower_cost": sanguinePact?.SetSacrificeCost(Value(t, 10, 8, 6, 4, 2)); return true;
                case "upgrade_blood_blood_nova_bigger_nova": bloodNova?.SetRadius(Value(t, 2.2f, 2.8f, 3.5f, 4.3f, 5.3f)); return true;
                case "upgrade_blood_blood_nova_stronger_nova": bloodNova?.SetDamage(Value(t, 30, 42, 58, 80, 110)); return true;
                case "upgrade_blood_blood_nova_healing_nova": bloodNova?.SetLifestealMultiplier(Value(t, 0.35f, 0.48f, 0.65f, 0.85f, 1.1f)); return true;
                case "upgrade_blood_leech_bind_longer_bind": leechBind?.SetDuration(Value(t, 4f, 5.2f, 6.6f, 8.2f, 10f)); return true;
                case "upgrade_blood_leech_bind_stronger_drain": leechBind?.SetLifestealMultiplier(Value(t, 0.45f, 0.62f, 0.85f, 1.1f, 1.4f)); return true;
                case "upgrade_blood_leech_bind_multi_bind": leechBind?.SetCount(Value(t, 2, 3, 4, 5, 7)); return true;
                case "upgrade_blood_crimson_frenzy_faster_attacks": crimsonFrenzy?.SetDuration(Value(t, 4f, 5.5f, 7f, 9f, 12f)); return true;
                case "upgrade_blood_crimson_frenzy_more_damage": crimsonFrenzy?.SetPowerMultiplier(Value(t, 1.35f, 1.55f, 1.8f, 2.15f, 2.6f)); return true;
                case "upgrade_blood_crimson_frenzy_low_health_power": crimsonFrenzy?.SetLifestealMultiplier(Value(t, 0.3f, 0.45f, 0.65f, 0.9f, 1.2f)); return true;
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
