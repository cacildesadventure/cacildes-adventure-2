using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

namespace AF.Inventory
{
    [CreateAssetMenu(fileName = "Inventory Database", menuName = "System/New Inventory Database", order = 0)]
    public class InventoryDatabase : ScriptableObject
    {

        [Header("Inventory")]
        [SerializedDictionary("Item", "Quantity")]
        public SerializedDictionary<Item, ItemAmount> ownedItems = new();

        public SerializedDictionary<Item, ItemAmount> defaultItems = new();
        public SerializedDictionary<string, List<UserCreatedItem>> userCreatedItems = new();

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;


#if UNITY_EDITOR
        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }
#endif
        public void Clear()
        {
            ownedItems.Clear();
        }

        public void SetDefaultItems()
        {
            ownedItems.Clear();

            foreach (var defaultItem in defaultItems)
            {
                ownedItems.Add(defaultItem.Key, defaultItem.Value);

                if (defaultItem.Key is Armor armor)
                {
                    equipmentDatabase.EquipArmor(armor);
                }
                else if (defaultItem.Key is Legwear legwear)
                {
                    equipmentDatabase.EquipLegwear(legwear);
                }
            }
        }

        public void ReplenishItems()
        {
            foreach (var item in ownedItems)
            {
                if (item.Value.usages > 0)
                {
                    item.Value.amount += item.Value.usages;
                    item.Value.usages = 0;
                }
            }
        }

        public void AddUserCreatedItem(UserCreatedItem userCreatedItem)
        {
            if (userCreatedItems.ContainsKey(userCreatedItem.itemName))
            {
                userCreatedItems[userCreatedItem.itemName].Add(userCreatedItem);
            }
            else
            {
                userCreatedItems.Add(userCreatedItem.itemName, new() { userCreatedItem });
            }

            // Look up to a entry in owned items with the same user created item name
            Item matchedEntry = ownedItems.FirstOrDefault(ownedItem => ownedItem.Key.name == userCreatedItem.itemName).Key;

            if (matchedEntry != null)
            {
                AddItem(matchedEntry, 1);
                return;
            }

            Consumable consumable = CreateInstance<Consumable>();
            consumable.value = userCreatedItem.value;
            consumable.sprite = userCreatedItem.itemThumbnail;
            consumable.name = userCreatedItem.itemName;
            consumable.isUserCreatedItem = true;

            var statusEffectsWhenConsumed = new List<StatusEffect>();
            statusEffectsWhenConsumed.AddRange(userCreatedItem.positiveEffects);
            statusEffectsWhenConsumed.AddRange(userCreatedItem.negativeEffects);
            consumable.statusEffectsWhenConsumed = statusEffectsWhenConsumed.ToArray();

            AddItem(consumable, 1);
        }

        public void RemoveUserCreatedItem(Item item)
        {
            if (!userCreatedItems.ContainsKey(item.name))
            {
                return;
            }

            if (userCreatedItems[item.name].Count <= 1)
            {
                userCreatedItems.Remove(item.name);
            }
            else
            {
                userCreatedItems[item.name].RemoveRange(0, 1);
            }
        }

        public bool IsUserCreatedItem(Item item)
        {
            return userCreatedItems.ContainsKey(item.name);
        }

        public void AddItem(Item itemToAdd)
        {
            AddItem(itemToAdd, 1);
        }

        public void AddItem(Item itemToAdd, int quantity)
        {
            if (HasItem(itemToAdd))
            {
                ownedItems[itemToAdd].amount += quantity;
            }
            else
            {
                ownedItems.Add(itemToAdd, new ItemAmount() { amount = quantity, usages = 0 });
            }
        }

        public void RemoveItem(Item itemToAdd)
        {
            RemoveItem(itemToAdd, 1);
        }

        public void RemoveItem(Item itemToRemove, int quantity)
        {
            if (!ownedItems.ContainsKey(itemToRemove))
            {
                return;
            }

            if (ownedItems[itemToRemove].amount <= 1)
            {
                // If not reusable item
                if (itemToRemove.isRenewable)
                {
                    ownedItems[itemToRemove].amount = 0;
                    ownedItems[itemToRemove].usages++;
                }
                else
                {
                    UnequipItemToRemove(itemToRemove);

                    // Remove item 
                    ownedItems.Remove(itemToRemove);
                }
            }
            else
            {
                ownedItems[itemToRemove].amount -= quantity;

                if (itemToRemove.isRenewable)
                {
                    ownedItems[itemToRemove].usages++;
                }
            }
        }

        void UnequipItemToRemove(Item item)
        {
            equipmentDatabase.UnequipItem(item);
        }

        public int GetItemAmount(Item itemToFind)
        {
            if (!ownedItems.ContainsKey(itemToFind))
            {
                return -1;
            }

            return this.ownedItems[itemToFind].amount;
        }

        public bool HasItem(Item itemToFind)
        {
            return this.ownedItems.ContainsKey(itemToFind);
        }

        public int GetWeaponsCount()
        {
            return ownedItems.Count(x => x.Key is Weapon);
        }

        public int GetSpellsCount()
        {
            return ownedItems.Count(x => x.Key is Spell);
        }

        public Dictionary<CraftingMaterial, ItemAmount> GetIngredients()
        {
            // Return an empty dictionary if there are no owned items
            if (ownedItems.Count <= 0)
            {
                return new Dictionary<CraftingMaterial, ItemAmount>();
            }

            // Filter the owned items where the key is of type CraftingMaterial and return as a dictionary
            return ownedItems
                .Where(x => x.Key is CraftingMaterial)
                .ToDictionary(x => (CraftingMaterial)x.Key, x => x.Value);
        }
    }
}
