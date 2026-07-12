using UnityEngine;

namespace Heroic.Data
{
    [CreateAssetMenu(menuName = "Heroic/Skill Database", fileName = "SkillDatabase")]
    public class SkillDatabase : ScriptableObject
    {
        [SerializeField] private SkillDefinition[] skills = new SkillDefinition[0];

        public SkillDefinition[] Skills => skills;
    }
}
