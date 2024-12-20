namespace AF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Inventory;
    using GameAnalyticsSDK;
    using UnityEngine;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UIElements;

    public class UIBlacksmithUpgradeWeapon : MonoBehaviour
    {
        [Header("UI")]
        public VisualTreeAsset ingredientItem;
        public Sprite goldSprite;

        [Header("SFX")]
        public UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Components")]
        public NotificationManager notificationManager;
        public PlayerManager playerManager;
        public Soundbank soundbank;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Components")]
        public UIWeaponStatsContainer uIWeaponStatsContainer;

        void ClearPreviews(VisualElement root)
        {
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;

            uIWeaponStatsContainer.ClearPreviews(root);
        }

        public void DrawUI(Weapon selectedWeapon, VisualElement root)
        {
            ClearPreviews(root);

            PreviewWeaponUpgrade(selectedWeapon, root);

            UIUtils.PlayFadeInAnimation(root.Q<VisualElement>("WeaponUpgrade"), 0.2f);
        }

        void PreviewWeaponUpgrade(Weapon weapon, VisualElement root)
        {
            WeaponUpgradeLevel weaponUpgradeLevel = weapon.weaponDamage.GetWeaponUpgradeLevel(weapon.level);

            if (weaponUpgradeLevel == null)
            {
                return;
            }

            uIWeaponStatsContainer.PreviewWeaponUpgrade(weapon, root);

            // Requirements
            root.Q<VisualElement>("ItemInfo").Clear();

            foreach (var upgradeMaterial in weaponUpgradeLevel.upgradeMaterials)
            {
                UpgradeMaterial upgradeMaterialItem = upgradeMaterial.Key;
                int amountRequiredFoUpgrade = upgradeMaterial.Value;

                var ingredientItemEntry = ingredientItem.CloneTree();
                ingredientItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(upgradeMaterialItem.sprite);
                ingredientItemEntry.Q<Label>("Title").text = upgradeMaterialItem.GetName();

                var playerOwnedIngredient = inventoryDatabase.HasItem(upgradeMaterialItem)
                    ? inventoryDatabase.ownedItems[upgradeMaterialItem]
                    : null;

                var playerOwnedIngredientAmount = 0;
                if (playerOwnedIngredient != null)
                {
                    playerOwnedIngredientAmount = playerOwnedIngredient.amount;
                }
                ingredientItemEntry.Q<Label>("Amount").text = playerOwnedIngredientAmount + " / " + amountRequiredFoUpgrade;
                ingredientItemEntry.Q<Label>("Amount").style.opacity =
                    playerOwnedIngredient != null && playerOwnedIngredientAmount >= amountRequiredFoUpgrade ? 1 : 0.25f;

                root.Q<VisualElement>("ItemInfo").Add(ingredientItemEntry);
                root.Q<VisualElement>("ItemInfo").style.opacity = 1;
            }

            // Add Gold

            var goldItemEntry = ingredientItem.CloneTree();
            goldItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(goldSprite);
            goldItemEntry.Q<Label>("Title").text = LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Gold");

            goldItemEntry.Q<Label>("Amount").text = playerStatsDatabase.gold + " / " + weaponUpgradeLevel.goldCostForUpgrade;
            goldItemEntry.Q<Label>("Amount").style.opacity = playerStatsDatabase.gold >= weaponUpgradeLevel.goldCostForUpgrade ? 1 : 0.25f;

            root.Q<VisualElement>("ItemInfo").Add(goldItemEntry);
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = CraftingUtils.ShouldSkipUpgrade(weapon) ? 0 : 1;

            root.Q<Button>("UpgradeButton").RegisterCallback<ClickEvent>(ev =>
            {
                HandleWeaponUpgrade(weapon, root);
            });

            root.Q<Button>("UpgradeButton").SetEnabled(CanImproveWeapon(weapon));

            root.Q<Button>("UpgradeButton").style.display = CraftingUtils.ShouldSkipUpgrade(weapon) ? DisplayStyle.None : DisplayStyle.Flex;

            root.Q<Label>("WeaponFullyUpgradedLabel").style.display = CraftingUtils.IsFullyUpgraded(weapon) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        bool CanImproveWeapon(Weapon weapon) => CraftingUtils.CanImproveWeapon(inventoryDatabase, weapon, playerStatsDatabase.gold);

        void HandleWeaponUpgrade(Weapon wp, VisualElement root)
        {
            if (!CanImproveWeapon(wp))
            {
                return;
            }

            playerManager.playerAchievementsManager.achievementForUpgradingFirstWeapon.AwardAchievement();
            soundbank.PlaySound(soundbank.craftSuccess);
            notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Weapon improved!"), wp.sprite);

            LogAnalytic(AnalyticsUtils.OnUIButtonClick("UpgradeWeapon"), new() {
                { "weapon_upgraded", wp.name }
            });

            CraftingUtils.UpgradeWeapon(
                wp,
                (goldUsed) => uIDocumentPlayerGold.LoseGold(goldUsed),
                (upgradeMaterialUsed) => playerManager.playerInventory.RemoveItem(upgradeMaterialUsed.Key, upgradeMaterialUsed.Value)
            );

            DrawUI(wp, root);
        }

        void LogAnalytic(string eventName, Dictionary<string, object> values)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName, values);
        }
    }
}
