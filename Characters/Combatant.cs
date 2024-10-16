
namespace AF
{
    using AF.Inventory;
    using AYellowpaper.SerializedCollections;
    using UnityEngine;
    using UnityEngine.Localization;

    [CreateAssetMenu(menuName = "NPCs / Characters / New Combatant")]
    public class Combatant : ScriptableObject
    {
        [Header("Name")]
        public LocalizedString enemyName;

        [Header("Health")]
        public int health = 400;

        [Header("Poise")]
        public int poise = 1;

        [Header("Posture")]
        public int posture = 100;

        [Header("Backstab")]
        public bool allowBackstabs = true;


        [Header("Loot")]
        [SerializedDictionary("Item", "Chance To Get")]
        public SerializedDictionary<Item, LootItemAmount> lootTable;

        [Header("Experience")]
        public int gold = 400;

        public enum InheritOption
        {
            INHERIT,
            OVERRIDE,
            MERGE,
        }
    }
}
