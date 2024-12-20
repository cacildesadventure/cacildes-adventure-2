using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using AF.Ladders;
using AF.StatusEffects;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class PlayerInventory : MonoBehaviour
    {
        public Consumable currentConsumedItem;


        [Header("UI Components")]
        public NotificationManager notificationManager;
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        public UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;

        [Header("Flags")]
        public bool isConsumingItem = false;

        [Header("Events")]
        public UnityEvent onResetState;

        [Header("Ashes Edge Case")]
        public bool disableAshesUsage = false;
        public Item ashes;
        public UnityEvent onDisabledAshes;

        public void ResetStates()
        {
            isConsumingItem = false;
            onResetState?.Invoke();
        }

        public void ReplenishItems()
        {
            inventoryDatabase.ReplenishItems();

            uIDocumentPlayerHUDV2.equipmentHUD.UpdateUI();
        }

        void HandleItemAchievements(Item item)
        {
            if (item is Weapon)
            {
                int numberOfWeapons = inventoryDatabase.GetWeaponsCount();

                if (numberOfWeapons <= 0)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringFirstWeapon.AwardAchievement();
                }
                else if (numberOfWeapons == 10)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringTenWeapons.AwardAchievement();
                }
            }
            else if (item is Spell)
            {
                int numberOfSpells = inventoryDatabase.GetSpellsCount();

                if (numberOfSpells <= 0)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringFirstSpell.AwardAchievement();
                }
            }
        }

        void LogAnalytic(string eventName)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName);
        }

        public void AddItem(Item item, int quantity)
        {

            if (item is Weapon weapon)
            {
                if (weapon.tradingItemRequirements != null && weapon.tradingItemRequirements.Count > 0)
                {
                    // Special Weapon Found
                    LogAnalytic(AnalyticsUtils.OnBossWeaponAcquired(weapon.name));
                }
            }
            else if (item is Armor armor)
            {
                LogAnalytic(AnalyticsUtils.OnArmorAcquired(armor.name));
            }
            else if (item is Spell spell)
            {
                LogAnalytic(AnalyticsUtils.OnSpellAcquired(spell.name));
            }

            HandleItemAchievements(item);

            inventoryDatabase.AddItem(item, quantity);

            uIDocumentPlayerHUDV2.equipmentHUD.UpdateUI();
        }

        public void RemoveItem(Item item, int quantity)
        {
            inventoryDatabase.RemoveItem(item, quantity);

            uIDocumentPlayerHUDV2.equipmentHUD.UpdateUI();
        }

        public void AddUserCreatedItem(UserCreatedItem userCreatedItem)
        {
            inventoryDatabase.AddUserCreatedItem(userCreatedItem);
            uIDocumentPlayerHUDV2.equipmentHUD.UpdateUI();
        }

        bool CanConsumeItem(Consumable consumable)
        {
            if (isConsumingItem)
            {
                return false;
            }

            if (consumable.isRenewable && inventoryDatabase.GetItemAmount(consumable) <= 0)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Consumable depleted"),
                    notificationManager.notEnoughSpells);

                return false;
            }

            if (playerManager.playerCombatController.isCombatting)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Can't consume item at this time."),
                    notificationManager.systemError);

                return false;
            }


            if (playerManager.thirdPersonController.isSwimming)
            {

                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            if (playerManager.characterPosture.isStunned)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Can't consume item at this time."),
                    notificationManager.systemError);

                return false;
            }

            if (playerManager.dodgeController.isDodging)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            if (!playerManager.thirdPersonController.Grounded)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            if (playerManager.isBusy)
            {
                return false;
            }

            if (playerStatsDatabase.currentHealth <= 0)
            {
                return false;
            }

            if (disableAshesUsage && consumable == ashes)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            return true;
        }

        public void PrepareItemForConsuming(Consumable consumable)
        {
            if (!CanConsumeItem(consumable))
            {
                return;
            }

            this.currentConsumedItem = consumable;

            if (consumable.shouldHideEquipmentWhenConsuming)
            {
                playerManager.playerWeaponsManager.HideEquipment();
            }

            if (consumable.isBossToken || consumable.canBeConsumedForGold)
            {
                uIDocumentPlayerGold.AddGold((int)consumable.value);
            }

            if (consumable is not Card)
            {
                isConsumingItem = true;
                foreach (StatusEffect statusEffect in currentConsumedItem.statusEffectsWhenConsumed)
                {
                    playerManager.statusController.statusEffectInstances.FirstOrDefault(x => x.Key == statusEffect).Value?.onConsumeStart?.Invoke();
                }

                playerManager.playerComponentManager.DisableCharacterController();
                playerManager.playerComponentManager.DisableComponents();
            }
            else
            {
                isConsumingItem = playerManager.playerCardManager.StartCardUse(currentConsumedItem as Card);
            }
        }

        public void FinishItemConsumption()
        {
            if (currentConsumedItem == null)
            {
                return;
            }

            if (currentConsumedItem is not Card)
            {
                playerManager.playerComponentManager.EnableCharacterController();
                playerManager.playerComponentManager.EnableComponents();
            }
            else
            {
                playerManager.playerCardManager.EndCurrentCardUse();
            }

            if (currentConsumedItem.shouldNotRemoveOnUse == false)
            {
                if (playerManager.statsBonusController.chanceToNotLoseItemUponConsumption && Random.Range(0f, 1f) > 0.8f)
                {
                    notificationManager.ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Consumable depleted"),
                        notificationManager.notEnoughSpells);


                    notificationManager.ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "The item has been preserved for future use.")
                    );
                }
                else
                {
                    playerManager.playerInventory.RemoveItem(currentConsumedItem, 1);
                }
            }

            foreach (StatusEffect statusEffect in currentConsumedItem.statusEffectsWhenConsumed)
            {
                // For positive effects, we override the status effect resistance to be the duration of the consumable effect
                playerManager.statusController.statusEffectResistances[statusEffect] = currentConsumedItem.effectsDurationInSeconds;

                playerManager.statusController.InflictStatusEffect(statusEffect, currentConsumedItem.effectsDurationInSeconds, true);
            }

            currentConsumedItem = null;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void AllowAshes()
        {
            disableAshesUsage = false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisableAshes()
        {
            onDisabledAshes?.Invoke();
            disableAshesUsage = true;
        }
    }
}
