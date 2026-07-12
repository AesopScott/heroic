using UnityEngine;

namespace Heroic.Data
{
    [CreateAssetMenu(menuName = "Heroic/Movement Database", fileName = "MovementDatabase")]
    public class MovementDatabase : ScriptableObject
    {
        [SerializeField] private MovementSkillDefinition[] movementSkills = new MovementSkillDefinition[0];

        public MovementSkillDefinition[] MovementSkills => movementSkills;
    }
}
