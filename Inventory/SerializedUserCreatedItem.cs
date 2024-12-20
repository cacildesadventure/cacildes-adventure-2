namespace AF
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class SerializedUserCreatedItem
    {
        public string itemName;
        public string itemThumbnailName = "";
        public string[] positiveEffectNames;
        public string[] negativeEffectNames;
        public int value = 0;

        public UserCreatedItem GenerateItem()
        {
            List<StatusEffect> positiveEffects = new();
            foreach (var positiveEffectName in positiveEffectNames)
            {
                StatusEffect positiveStatusEffect = Resources.Load<StatusEffect>("Status/Consumables/" + positiveEffectName);
                if (positiveStatusEffect != null)
                {
                    positiveEffects.Add(positiveStatusEffect);
                }
            }

            List<StatusEffect> negativeEffects = new();
            foreach (var negativeEffectName in negativeEffectNames)
            {
                StatusEffect negativeStatusEffect = Resources.Load<StatusEffect>("Status/Negative Status/" + negativeEffectName);
                if (negativeStatusEffect != null)
                {
                    negativeEffects.Add(negativeStatusEffect);
                }
            }

            CreatedItemThumbnail createdItemThumbnail = Resources.Load<CreatedItemThumbnail>("Created Item Thumbnails/" + itemThumbnailName);

            return new UserCreatedItem
            {
                itemName = itemName,
                itemThumbnail = createdItemThumbnail?.thumbnail,
                itemThumbnailName = createdItemThumbnail?.name,
                positiveEffects = positiveEffects.ToArray(),
                negativeEffects = negativeEffects.ToArray(),
                value = value,
            };
        }

    }
}
