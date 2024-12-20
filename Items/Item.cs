
namespace AF
{
    using AYellowpaper.SerializedCollections;
    using UnityEngine;
    using UnityEngine.Localization;

    [CreateAssetMenu(menuName = "Items / Item / New Item")]
    public class Item : ScriptableObject
    {

        public string englishName;
        public string portugueseName;
        [TextAreaAttribute(minLines: 2, maxLines: 10)] public string englishDescription;
        [TextAreaAttribute(minLines: 2, maxLines: 10)] public string portugueseDescription;


        [Header("Localization")]
        public LocalizedString nameLocalized;
        public LocalizedString descriptionLocalized;
        public LocalizedString shortDescriptionLocalized;

        public Sprite sprite;

        public float value = 0;
        public bool isRenewable = false;
        [Tooltip("If we want to buy this item on a shop, this will override their value when trading with an NPC. E.g. Buying a boss weapon by trading a boss soul")]
        public SerializedDictionary<Item, int> tradingItemRequirements = new();

        public bool isUserCreatedItem = false;

        public string GetName()
        {
            if (nameLocalized != null && nameLocalized.IsEmpty == false)
            {
                return nameLocalized.GetLocalizedString();
            }

            return name;
        }


        public string GetDescription()
        {
            if (descriptionLocalized != null && descriptionLocalized.IsEmpty == false)
            {
                return descriptionLocalized.GetLocalizedString();
            }

            return "";
        }

        public string GetShortDescription()
        {
            if (shortDescriptionLocalized != null && shortDescriptionLocalized.IsEmpty == false)
            {
                return shortDescriptionLocalized.GetLocalizedString();
            }

            return "";
        }
    }
}
