namespace AF
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class UserCreatedItem
    {
        public string itemName;
        public string itemThumbnailName = "";
        public Sprite itemThumbnail;
        public StatusEffect[] positiveEffects;
        public StatusEffect[] negativeEffects;
        public int value = 0;


        public Consumable GenerateItem()
        {
            Consumable consumable = ScriptableObject.CreateInstance<Consumable>();
            consumable.value = value;
            consumable.sprite = itemThumbnail;
            consumable.name = itemName;
            consumable.isUserCreatedItem = true;

            var statusEffectsWhenConsumed = new List<StatusEffect>();
            statusEffectsWhenConsumed.AddRange(positiveEffects);
            statusEffectsWhenConsumed.AddRange(negativeEffects);
            consumable.statusEffectsWhenConsumed = statusEffectsWhenConsumed.ToArray();

            consumable.createdItemThumbnailName = itemThumbnailName;

            return consumable;
        }
    }
}
