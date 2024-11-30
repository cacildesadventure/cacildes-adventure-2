namespace AF
{
    using System.Collections;
    using System.Linq;
    using AF.Inventory;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIAlchemyResultingItem : MonoBehaviour
    {
        [Header("UI Components")]
        public UIDocumentAlchemy uIDocumentAlchemy;

        [Header("Databases")]
        public RecipesDatabase recipesDatabase;
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Prefabs")]
        public VisualTreeAsset createdItemEffectEntry;

        VisualElement GetRoot()
        {
            return uIDocumentAlchemy.uIDocument.rootVisualElement;
        }
        private void OnEnable()
        {
            DrawUI();
        }

        public void DrawUI()
        {
            if (uIDocumentAlchemy.selectedCreatedItem == null)
            {
                Clear();
            }
            else
            {
                DrawItem(uIDocumentAlchemy.selectedCreatedItem);
            }
        }

        void Clear()
        {
            GetRoot().Q<Label>("ResultingItemLabel").style.display = DisplayStyle.None;
            GetRoot().Q<Label>("ItemName").text = "";
            GetRoot().Q<VisualElement>("ItemIcon").style.display = DisplayStyle.None;
            GetRoot().Q<VisualElement>("PotionEffectsContainer").Clear();
            GetRoot().Q<VisualElement>("PotionEffectsContainer").style.display = DisplayStyle.None;

            GetRoot().Q<Button>("CraftUserCreatedItemButton").style.display = DisplayStyle.Flex;
            GetRoot().Q<Button>("CraftUserCreatedItemButton").SetEnabled(false);
        }

        void DrawItem(UserCreatedItem userCreatedItem)
        {
            GetRoot().Q<Label>("ResultingItemLabel").style.display = DisplayStyle.Flex;
            GetRoot().Q<Label>("ItemName").text = userCreatedItem.itemName;
            GetRoot().Q<VisualElement>("ItemIcon").style.backgroundImage = new StyleBackground(userCreatedItem.itemThumbnail);
            GetRoot().Q<VisualElement>("ItemIcon").style.display = DisplayStyle.Flex;

            GetRoot().Q<VisualElement>("PotionEffectsContainer").Clear();
            GetRoot().Q<VisualElement>("PotionEffectsContainer").style.display = DisplayStyle.Flex;

            GetRoot().Q<Button>("CraftUserCreatedItemButton").style.display = DisplayStyle.Flex;
            GetRoot().Q<Button>("CraftUserCreatedItemButton").SetEnabled(true);

            foreach (var positiveEffect in AlchemyUtils.CombineEffects(userCreatedItem.positiveEffects.Select(x => x.GetName()).ToArray()))
            {
                VisualElement createdItemEffect = createdItemEffectEntry.Instantiate();

                createdItemEffect.Q<VisualElement>("PositiveIndicator").style.display = DisplayStyle.Flex;
                createdItemEffect.Q<VisualElement>("NegativeIndicator").style.display = DisplayStyle.None;
                createdItemEffect.Q<Label>("Effect").text = positiveEffect;
                GetRoot().Q<VisualElement>("PotionEffectsContainer").Add(createdItemEffect);
            }

            foreach (var negativeEffect in AlchemyUtils.CombineEffects(userCreatedItem.negativeEffects.Select(x => x.GetName()).ToArray()))
            {
                VisualElement createdItemEffect = createdItemEffectEntry.Instantiate();
                createdItemEffect.Q<VisualElement>("PositiveIndicator").style.display = DisplayStyle.None;
                createdItemEffect.Q<VisualElement>("NegativeIndicator").style.display = DisplayStyle.Flex;
                createdItemEffect.Q<Label>("Effect").text = negativeEffect;
                GetRoot().Q<VisualElement>("PotionEffectsContainer").Add(createdItemEffect);
            }
        }

    }
}
