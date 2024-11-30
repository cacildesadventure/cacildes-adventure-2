
namespace AF
{
    using UnityEngine;
    using UnityEngine.Localization;

    [CreateAssetMenu(fileName = "New Created Item Category", menuName = "Alchemy / New Created Item Category", order = 0)]
    public class CreatedItemCategory : ScriptableObject
    {
        public LocalizedString category;
        public int weight = 1;

        public string GetCategory()
        {
            if (category.IsEmpty)
            {
                return "";
            }

            return category.GetLocalizedString();
        }
    }
}
