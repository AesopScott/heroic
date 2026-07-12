using UnityEngine;

namespace Heroic.Data
{
    [CreateAssetMenu(menuName = "Heroic/Magic School Definition", fileName = "MagicSchoolDefinition")]
    public class MagicSchoolDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string schoolName;
        [SerializeField] private string role;
        [SerializeField] private string baseDamage;
        [SerializeField] private string castStyle;
        [SerializeField] private string areaShape;
        [SerializeField] private string range;
        [SerializeField] private string cooldown;
        [SerializeField] private string proc1;
        [SerializeField] private string proc2;
        [TextArea] [SerializeField] private string valueProposition;

        public string Id => id;
        public string SchoolName => schoolName;
        public string Role => role;
        public string BaseDamage => baseDamage;
        public string CastStyle => castStyle;
        public string AreaShape => areaShape;
        public string Range => range;
        public string Cooldown => cooldown;
        public string Proc1 => proc1;
        public string Proc2 => proc2;
        public string ValueProposition => valueProposition;
    }
}
