
namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Inventory;
    using AYellowpaper.SerializedCollections;
    using UnityEngine;
    using UnityEngine.Localization;

    public class CharacterLoot : MonoBehaviour
    {
        [Header("Loot")]
        [SerializedDictionary("Item", "Chance To Get")]
        public SerializedDictionary<Item, LootItemAmount> lootTable;
        public Combatant.InheritOption inheritOption = Combatant.InheritOption.INHERIT;

        [Header("Experience")]
        public int bonusGold = 0;

        [Header("Components")]
        public CharacterManager lootOwner;

        // Scene References
        private PlayerManager playerManager;
        private NotificationManager notificationManager;
        private Soundbank soundbank;
        private UIDocumentPlayerGold uIDocumentPlayerGold;
        private UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;

        [Header("Localization")]
        public LocalizedString found; // "Found: "

        public void GiveLoot()
        {
            StartCoroutine(GiveLoot_Coroutine());
        }

        private IEnumerator GiveLoot_Coroutine()
        {
            int goldToReceive = lootOwner.combatant.gold + bonusGold;

            yield return new WaitForSeconds(1f);

            goldToReceive = CalculateGoldToReceive(goldToReceive);

            // Handle Loot
            GetLoot();

            yield return new WaitForSeconds(0.2f);

            GetUIDocumentPlayerGold().AddGold(goldToReceive);
        }

        private int CalculateGoldToReceive(int goldToReceive)
        {
            if (GetPlayerManager().statsBonusController != null)
            {
                var statsBonus = GetPlayerManager().statsBonusController;
                var additionalCoinPercentage = statsBonus.additionalCoinPercentage;

                if (additionalCoinPercentage != 0)
                {
                    goldToReceive += (int)Mathf.Ceil(goldToReceive * additionalCoinPercentage / 100);
                }

                if (statsBonus.ShouldDoubleCoinFromFallenEnemy())
                {
                    goldToReceive *= 2;
                }
            }

            return goldToReceive;
        }

        private void GetLoot()
        {
            var itemsToReceive = new SerializedDictionary<Item, ItemAmount>();
            bool hasPlayedFanfare = false;

            var finalLootTable = GetFinalLootTable();

            foreach (var dropEntry in finalLootTable)
            {
                if (ShouldSkipItem(dropEntry)) continue;

                if (Random.Range(0, 100f) <= dropEntry.Value.chanceToGet)
                {
                    if (!hasPlayedFanfare)
                    {
                        GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);
                        hasPlayedFanfare = true;
                    }

                    itemsToReceive.Add(dropEntry.Key, dropEntry.Value);
                }
            }

            HandleLootReceived(itemsToReceive);
        }

        private bool ShouldSkipItem(KeyValuePair<Item, LootItemAmount> dropEntry)
        {
            if (dropEntry.Value.ignoreIfPlayerOwns && playerManager.playerInventory.inventoryDatabase.HasItem(dropEntry.Key))
            {
                return true;
            }

            if (dropEntry.Key is Card card &&
                playerManager.playerInventory.inventoryDatabase.GetItemAmount(dropEntry.Key) >= card.maximumCardsAllowedInInventory)
            {
                return true;
            }

            return false;
        }

        private SerializedDictionary<Item, LootItemAmount> GetFinalLootTable()
        {
            var finalLootTable = new SerializedDictionary<Item, LootItemAmount>();

            switch (inheritOption)
            {
                case Combatant.InheritOption.INHERIT:
                    finalLootTable = lootOwner?.characterLoot?.lootTable ?? lootTable;
                    break;

                case Combatant.InheritOption.OVERRIDE:
                    finalLootTable = lootTable;
                    break;

                case Combatant.InheritOption.MERGE:
                    // Merge the current class loot table with the inherited one from the loot owner
                    if (lootOwner?.characterLoot != null)
                    {
                        finalLootTable = MergeLootTables(lootOwner.characterLoot.lootTable, lootTable);
                    }
                    else
                    {
                        finalLootTable = lootTable;
                    }
                    break;
            }

            return finalLootTable;
        }

        private SerializedDictionary<Item, LootItemAmount> MergeLootTables(SerializedDictionary<Item, LootItemAmount> ownerTable, SerializedDictionary<Item, LootItemAmount> classTable)
        {
            var mergedLootTable = new SerializedDictionary<Item, LootItemAmount>(ownerTable);

            foreach (var entry in classTable)
            {
                if (mergedLootTable.ContainsKey(entry.Key))
                {
                    // Optionally, combine the amounts or chances if the same item exists in both
                    mergedLootTable[entry.Key].chanceToGet = Mathf.Max(mergedLootTable[entry.Key].chanceToGet, entry.Value.chanceToGet);
                    mergedLootTable[entry.Key].amount += entry.Value.amount;
                }
                else
                {
                    mergedLootTable.Add(entry.Key, entry.Value);
                }
            }

            return mergedLootTable;
        }

        private void HandleLootReceived(SerializedDictionary<Item, ItemAmount> itemsToReceive)
        {
            bool isBoss = lootOwner.characterBossController.IsBoss();

            var itemsToDisplay = new List<UIDocumentReceivedItemPrompt.ItemsReceived>();

            foreach (var item in itemsToReceive)
            {
                GetPlayerManager().playerInventory.AddItem(item.Key, item.Value.amount);

                var receivedItem = new UIDocumentReceivedItemPrompt.ItemsReceived
                {
                    itemName = item.Key.nameLocalized.GetLocalizedString(),
                    quantity = item.Value.amount,
                    sprite = item.Key.sprite,
                };

                if (isBoss && GetUIDocumentReceivedItemPrompt() != null)
                {
                    itemsToDisplay.Add(receivedItem);
                }
                else
                {
                    GetNotificationManager().ShowNotification(found.GetLocalizedString() + " " + item.Key.GetName(), item.Key.sprite);
                }
            }

            if (isBoss && itemsToDisplay.Count > 0)
            {
                GetUIDocumentReceivedItemPrompt().gameObject.SetActive(true);
                GetUIDocumentReceivedItemPrompt().DisplayItemsReceived(itemsToDisplay);
            }
        }

        // Lazy initialization helper methods
        private PlayerManager GetPlayerManager()
        {
            return playerManager ??= FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
        }

        private Soundbank GetSoundbank()
        {
            return soundbank ??= FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
        }

        private NotificationManager GetNotificationManager()
        {
            return notificationManager ??= FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
        }

        private UIDocumentPlayerGold GetUIDocumentPlayerGold()
        {
            return uIDocumentPlayerGold ??= FindAnyObjectByType<UIDocumentPlayerGold>(FindObjectsInactive.Include);
        }

        private UIDocumentReceivedItemPrompt GetUIDocumentReceivedItemPrompt()
        {
            return uIDocumentReceivedItemPrompt ??= FindAnyObjectByType<UIDocumentReceivedItemPrompt>(FindObjectsInactive.Include);
        }
    }
}
