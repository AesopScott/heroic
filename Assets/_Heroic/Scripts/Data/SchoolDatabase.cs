using UnityEngine;

namespace Heroic.Data
{
    [CreateAssetMenu(menuName = "Heroic/School Database", fileName = "SchoolDatabase")]
    public class SchoolDatabase : ScriptableObject
    {
        [SerializeField] private MagicSchoolDefinition[] schools = new MagicSchoolDefinition[0];

        public MagicSchoolDefinition[] Schools => schools;
    }
}
