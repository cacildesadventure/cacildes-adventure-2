namespace AF
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Inventory;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIBlacksmithWeaponsList : MonoBehaviour
    {

        [Header("UI")]
        public VisualTreeAsset recipeItem;

        [Header("Components")]
        public Soundbank soundbank;
        public UIDocumentCraftScreen uIDocumentCraftScreen;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        // Last scroll position
        int lastScrollElementIndex = -1;
        Weapon selectedWeapon;

        public void ClearPreviews(VisualElement root)
        {
            selectedWeapon = null;
        }

        public void DrawUI(VisualElement root, Action onClose)
        {
            PopulateScrollView(root, onClose);
        }

        void PopulateScrollView(VisualElement root, Action onClose)
        {
            var scrollView = root.Q<ScrollView>();
            scrollView.Clear();

            Button exitButton = new()
            {
                text = Glossary.IsPortuguese() ? "Regressar" : "Return"
            };
            exitButton.AddToClassList("primary-button");
            UIUtils.SetupButton(exitButton, () =>
            {
                onClose();
            }, soundbank);

            scrollView.Add(exitButton);

            PopulateWeaponsScrollView(root, onClose);

            if (lastScrollElementIndex == -1)
            {
                scrollView.ScrollTo(exitButton);
                exitButton.Focus();
            }
            else
            {
                StartCoroutine(GiveFocusCoroutine(root));
            }
        }

        IEnumerator GiveFocusCoroutine(VisualElement root)
        {
            yield return new WaitForSeconds(0);
            GiveFocus(root);
        }

        void GiveFocus(VisualElement root)
        {
            UIUtils.ScrollToLastPosition(
                lastScrollElementIndex,
                root.Q<ScrollView>(),
                () =>
                {
                    lastScrollElementIndex = -1;
                }
            );
        }

        void PopulateWeaponsScrollView(VisualElement root, Action onClose)
        {
            var scrollView = root.Q<ScrollView>();

            int i = 0;
            foreach (var itemEntry in GetWeaponsList())
            {
                int currentIndex = i;

                Weapon wp = itemEntry.Key as Weapon;

                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<VisualElement>("ItemIcon").style.backgroundImage = new StyleBackground(wp.sprite);
                scrollItem.Q<Label>("ItemName").text = GetWeaponName(wp);
                scrollItem.Q<VisualElement>("RemoveIngredient").style.display = DisplayStyle.None;
                scrollItem.Q<VisualElement>("AddIngredient").style.display = DisplayStyle.None;

                var craftBtn = scrollItem.Q<Button>("CraftButtonItem");

                // craftBtn.style.opacity = CraftingUtils.CanImproveWeapon(inventoryDatabase, wp, playerStatsDatabase.gold) ? 1f : 0.25f;

                if (selectedWeapon == wp)
                {
                    scrollItem.Q<Button>("CraftButtonItem").AddToClassList("blacksmith-craft-button-active");
                }

                scrollItem.Q<Button>("CraftButtonItem").AddToClassList("blacksmith-craft-button");

                UIUtils.SetupButton(craftBtn, () =>
                {
                    lastScrollElementIndex = currentIndex;

                    SelectWeapon(wp);

                    DrawUI(root, onClose);
                },
                () =>
                {
                    scrollView.ScrollTo(craftBtn);
                },
                () =>
                {
                },
                true,
                soundbank);

                scrollView.Add(craftBtn);

                i++;
            }
        }

        void SelectWeapon(Weapon weapon)
        {
            uIDocumentCraftScreen.OnSelectedWeapon(weapon);
            selectedWeapon = weapon;
        }

        Dictionary<Item, ItemAmount> GetWeaponsList()
        {
            if (uIDocumentCraftScreen.blacksmithAction == BlacksmithAction.UPGRADE)
            {
                return inventoryDatabase.ownedItems
                    .Where(itemEntry => itemEntry.Key is Weapon wp && wp.canBeUpgraded)
                    .ToDictionary(item => item.Key, item => item.Value);
            }

            return new();
        }

        string GetWeaponName(Weapon wp) => $"{wp.GetName()} +{wp.level}";

    }
}
