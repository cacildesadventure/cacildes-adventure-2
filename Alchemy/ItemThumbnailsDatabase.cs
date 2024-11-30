
namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Created Item Thumbnail", menuName = "Alchemy / New Created Item Thumbnail", order = 0)]
    public class CreatedItemThumbnail : ScriptableObject
    {
        public CreatedItemCategory createdItemCategory;

        public StatusEffect statusEffect;

        public Sprite thumbnail;
    }
}
