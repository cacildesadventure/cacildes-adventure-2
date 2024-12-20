namespace AF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class AlchemyUtils
    {
        public static UserCreatedItem GenerateResult(CraftingMaterial[] ingredients, CreatedItemThumbnail[] createdItemThumbnails, int playerIntelligence)
        {
            if (!AreIngredientsIncompatible(ingredients)) return null;

            StatusEffect[] positiveEffects = ExtractEffects(ingredients, positive: true);
            StatusEffect[] negativeEffects = ExtractEffects(ingredients, positive: false);

            if (positiveEffects.Length <= 0 && negativeEffects.Length <= 0)
            {
                return null;
            }

            string itemName = GenerateItemName(ingredients);
            if (string.IsNullOrEmpty(itemName)) return null;

            var generatedThumbnail = GenerateItemThumbnail(ingredients, createdItemThumbnails);
            int value = GetItemValue(ingredients, playerIntelligence);

            return new UserCreatedItem
            {
                itemName = itemName,
                itemThumbnail = generatedThumbnail?.thumbnail,
                itemThumbnailName = generatedThumbnail?.name,
                positiveEffects = positiveEffects,
                negativeEffects = negativeEffects,
                value = value,
            };
        }

        static StatusEffect[] ExtractEffects(CraftingMaterial[] ingredients, bool positive)
        {
            var effects = ingredients
                .Where(ingredient => ingredients.Count(x => x == ingredient) >= ingredient.minimumAmount)
                .SelectMany(ingredient => positive ? ingredient.positiveEffects : ingredient.negativeEffects);

            return effects.ToArray();
        }

        public static bool AreIngredientsIncompatible(CraftingMaterial[] ingredients) =>
            ingredients.All(ingredient => ingredient.compatibleIngredients.Length == 0 ||
                ingredient.compatibleIngredients.All(compatible => ingredients.Contains(compatible)));

        static string GenerateItemName(CraftingMaterial[] ingredients)
        {
            string baseName = GetBaseName(ingredients);
            var positiveEffects = ExtractEffects(ingredients, positive: true).Select(x => x.GetName()).ToArray();
            var negativeEffects = ExtractEffects(ingredients, positive: false).Select(x => x.GetName()).ToArray();

            string positiveText = string.Join(" " + Glossary.AND() + " ", CombineEffects(positiveEffects));
            string negativeText = string.Join(" " + Glossary.AND() + " ", CombineEffects(negativeEffects));

            return baseName + positiveText + (negativeText.Length > 0 ? Glossary.AND() + " " + negativeText : "");
        }

        static string GetBaseName(CraftingMaterial[] ingredients)
        {
            string potency = ""; //ingredients.Length == 1 ? Glossary.SMALL() + " " : ingredients.Length > 2 ? Glossary.GREAT() + " " : "";
            var category = ingredients.Select(ing => ing.createdItemCategory).Where(cat => cat != null).OrderByDescending(cat => cat.weight).FirstOrDefault();

            return category == null ? "" : potency + category.GetCategory() + " - ";
        }

        public static List<string> CombineEffects(string[] effects)
        {
            var effectDictionary = new Dictionary<string, int?>();
            var digitPositions = new Dictionary<string, List<(int start, int length)>>();

            foreach (var effect in effects)
            {
                var match = Regex.Match(effect, @"(\d+)");

                if (match.Success)
                {
                    int value = int.Parse(match.Value);
                    if (effectDictionary.ContainsKey(effect))
                    {
                        effectDictionary[effect] += value;
                    }
                    else
                    {
                        effectDictionary[effect] = value;
                        digitPositions[effect] = new List<(int, int)> { (match.Index, match.Length) };
                    }
                }
                else
                {
                    // Handle non-numeric effects
                    if (!effectDictionary.ContainsKey(effect))
                    {
                        effectDictionary[effect] = null; // Null indicates no numeric value
                    }
                }
            }

            return effectDictionary.Select(entry =>
            {
                var effectText = entry.Key;

                if (entry.Value.HasValue)
                {
                    // For numeric effects, insert the aggregated value
                    var pos = digitPositions[effectText][0];
                    return effectText.Remove(pos.start, pos.length).Insert(pos.start, entry.Value.Value.ToString());
                }

                // For non-numeric effects, return the effect as-is
                return effectText;

            }).ToList();
        }

        static CreatedItemThumbnail GenerateItemThumbnail(CraftingMaterial[] ingredients, CreatedItemThumbnail[] thumbnails)
        {
            var itemCategory = ingredients.Select(ing => ing.createdItemCategory).FirstOrDefault(cat => cat != null);
            var categoryThumbnails = thumbnails.Where(th => th.createdItemCategory == itemCategory).ToArray();

            var firstStatusEffect = ingredients.SelectMany(ing => ing.positiveEffects.Concat(ing.negativeEffects)).FirstOrDefault();
            return categoryThumbnails.FirstOrDefault(th => th.statusEffect == firstStatusEffect) ?? categoryThumbnails.FirstOrDefault() ?? thumbnails.FirstOrDefault(th => th.createdItemCategory == null);
        }

        static int GetItemValue(CraftingMaterial[] ingredients, int playerIntelligence)
        {
            int baseValue = ingredients.Sum(ing => (int)ing.value * ingredients.Length);
            return (int)(baseValue + baseValue * (1 - (10 / (10 + (float)playerIntelligence))));
        }
    }
}
