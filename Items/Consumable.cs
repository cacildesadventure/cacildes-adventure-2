namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Items / Item / New Consumable")]
    public class Consumable : Item
    {
        [Header("Options")]
        public bool shouldNotRemoveOnUse = false;
        public bool shouldHideEquipmentWhenConsuming = true;
        public bool isBossToken = false;
        public bool canBeConsumedForGold = false;

        [Header("Consume Effects")]
        public StatusEffect[] statusEffectsWhenConsumed;
        public float effectsDurationInSeconds = 6;

        [Header("User Created Item Options")]
        public bool isUserCreatedItem = false;
        public string createdItemThumbnailName;

        public string GetFormattedAppliedStatusEffects()
        {
            string result = "";

            foreach (var statusEffect in statusEffectsWhenConsumed)
            {
                if (statusEffect != null && statusEffect.GetName().Length > 0)
                {
                    result += $"{statusEffect.GetName()}\n";
                }
            }

            return result.TrimEnd();
        }

        public SerializedUserCreatedItem ConvertToSerializedUserCreatedItem()
        {
            return new SerializedUserCreatedItem
            {
                itemName = GetName(),
                itemThumbnailName = createdItemThumbnailName,
                effects = statusEffectsWhenConsumed.Select(element => element.name).ToArray(),
                value = (int)value,
            };
        }
    }
}
