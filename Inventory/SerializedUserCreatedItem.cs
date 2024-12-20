namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [System.Serializable]
    public class SerializedUserCreatedItem
    {
        public string itemName;
        public string itemThumbnailName = "";
        public string[] effects;
        public int value = 0;

        public Consumable GenerateConsumable()
        {

            StatusEffect[] allStatusEffects = Resources.LoadAll<StatusEffect>("Status");

            List<StatusEffect> effectsToAdd = new();

            foreach (var effect in effects)
            {
                StatusEffect statusEffectInstance = allStatusEffects.FirstOrDefault(x => x.name == effect);

                if (statusEffectInstance != null)
                {
                    effectsToAdd.Add(statusEffectInstance);
                }
            }

            CreatedItemThumbnail createdItemThumbnail = Resources.Load<CreatedItemThumbnail>("Created Item Thumbnails/" + itemThumbnailName);

            return new()
            {
                name = itemName,
                createdItemThumbnailName = createdItemThumbnail?.name,
                sprite = createdItemThumbnail?.thumbnail,
                statusEffectsWhenConsumed = effectsToAdd.ToArray(),
                value = value,
                isUserCreatedItem = true
            };
        }
    }
}
