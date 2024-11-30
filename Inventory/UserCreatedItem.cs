namespace AF
{
    using System.Linq;
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

        public string GetEffectPositiveDescription(StatusEffect positiveEffect)
        {
            float multiplier = GetPositiveIntensity(positiveEffect);

            return "+" + multiplier + "% " + positiveEffect.displayName.GetLocalizedString();
        }

        public string GetEffectNegativeDescription(StatusEffect negativeEffect)
        {
            float multiplier = GetNegativeIntensity(negativeEffect);

            return "+" + multiplier + "% " + negativeEffect.displayName.GetLocalizedString();
        }

        public float GetPositiveIntensity(StatusEffect effect)
        {
            int positiveIntensity = positiveEffects.Sum(x => x == effect ? 1 : 0);

            if (positiveIntensity <= 1)
            {
                return 12;
            }

            if (positiveIntensity <= 2)
            {
                return 50;
            }

            return 85;
        }

        public float GetNegativeIntensity(StatusEffect effect)
        {
            int negativeIntensity = negativeEffects.Sum(x => x == effect ? 1 : 0);

            if (negativeIntensity <= 1)
            {
                return 3;
            }

            if (negativeIntensity <= 2)
            {
                return 7;
            }

            return 15;
        }
    }
}
