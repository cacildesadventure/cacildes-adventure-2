namespace AF
{
    using UnityEngine;

    public class CraftingMaterial : Item
    {

        [Header("Ingredient Category (Optional)")]
        public CreatedItemCategory createdItemCategory;

        public StatusEffect[] positiveEffects;
        public StatusEffect[] negativeEffects;

        [Tooltip("If not compatible, will create dung bombs")]
        public CraftingMaterial[] compatibleIngredients;

        [Header("Requirements")]
        [Tooltip("Minimum ingredients to produce the effects")] public int minimumAmount = 2;

    }
}
