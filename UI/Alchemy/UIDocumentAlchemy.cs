namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Music;
    using GameAnalyticsSDK;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UIElements;

    public class UIDocumentAlchemy : MonoBehaviour
    {

        [Header("SFX")]
        public AudioClip sfxOnEnterMenu;

        [Header("UI Components")]
        public UIDocument uIDocument;
        [HideInInspector] public VisualElement root;

        public UIAlchemyIngredients uIAlchemyIngredients;
        public UIAlchemyResultingItem uIAlchemyResultingItem;


        [Header("Components")]
        public UIManager uIManager;
        public NotificationManager notificationManager;
        public PlayerManager playerManager;
        public CursorManager cursorManager;
        public BGMManager bgmManager;
        public Soundbank soundbank;
        public StarterAssetsInputs starterAssetsInputs;

        CreatedItemThumbnail[] createdItemThumbnails;

        [Header("Created Item Preview")]
        public List<CraftingMaterial> selectedIngredients = new();
        public UserCreatedItem selectedCreatedItem;

        public int maxAllowedIngredients = 2;

        [Header("Events")]
        public UnityEvent onAlchemyStart;
        public UnityEvent onAlchemyEnd;

        private void Awake()
        {
            this.gameObject.SetActive(false);

            starterAssetsInputs.onMenuEvent.AddListener(OnClose);
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            bgmManager.PlaySound(sfxOnEnterMenu, null);
            cursorManager.ShowCursor();
            playerManager.uIDocumentPlayerHUDV2.HideHUD();

            uIManager.displayedUis.Add(this.gameObject);

            selectedIngredients.Clear();
            selectedCreatedItem = null;

            root.Q<Label>("SlotIndicator").text = Glossary.COMBINE_UP_TO_X_INGREDIENTS_TO_CREATE_A_UNIQUE_ITEM(maxAllowedIngredients);

            UIUtils.SetupButton(root.Q<Button>("CraftUserCreatedItemButton"), OnCraft, soundbank);

            UpdateUI();

            onAlchemyStart?.Invoke();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenAlchemyMenu()
        {
            LogAnalytic(AnalyticsUtils.OnUIButtonClick("Alchemy"));

            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            Close();
        }

        public void Close()
        {
            playerManager.playerComponentManager.EnableComponents();
            playerManager.playerComponentManager.EnableCharacterController();

            this.gameObject.SetActive(false);
            cursorManager.HideCursor();

            uIManager.displayedUis.Remove(this.gameObject);
            playerManager.uIDocumentPlayerHUDV2.ShowHUD();

            onAlchemyEnd?.Invoke();
        }

        public void UpdateUI()
        {
            uIAlchemyIngredients.DrawUI();
            uIAlchemyResultingItem.DrawUI();
        }

        public void SelectIngredient(CraftingMaterial ingredient, bool hasEnough)
        {
            if (selectedIngredients.Count < maxAllowedIngredients && hasEnough)
            {
                // Add the ingredient if the limit is not reached
                this.selectedIngredients.Add(ingredient);
            }

            RegenerateItem();
        }

        public void UnselectIngredient(CraftingMaterial ingredient)
        {
            if (this.selectedIngredients.Contains(ingredient))
            {
                this.selectedIngredients.Remove(ingredient);
            }

            RegenerateItem();
        }


        void RegenerateItem()
        {
            // Generate the resulting item with the selected ingredients
            selectedCreatedItem = AlchemyUtils.GenerateResult(
                selectedIngredients.ToArray(),
                GetCreatedItemThumbnails(),
                playerManager.statsBonusController.GetCurrentIntelligence()
            );

            // Update the UI after ingredient selection or removal
            UpdateUI();
        }

        public void OnCraft()
        {
            if (selectedCreatedItem == null)
            {
                return;
            }

            playerManager.playerAchievementsManager.achievementForBrewingFirstPotion.AwardAchievement();

            LogAnalytic(AnalyticsUtils.OnUIButtonClick("CraftItem"), new() {
                { "item_created", selectedCreatedItem.itemName }
            });

            soundbank.PlaySound(soundbank.craftSuccess);

            playerManager.playerInventory.AddUserCreatedItem(selectedCreatedItem);

            notificationManager.ShowNotification(
                LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Received") + " x" + selectedCreatedItem.itemName, selectedCreatedItem.itemThumbnail);

            List<CraftingMaterial> ingredientsToRemove = new();
            foreach (CraftingMaterial ingredient in selectedIngredients)
            {
                playerManager.playerInventory.RemoveItem(ingredient, 1);

                // If has not enough to continue crafting, remove selection
                if (playerManager.playerInventory.inventoryDatabase.GetItemAmount(ingredient) < selectedIngredients.Where(x => x == ingredient)?.Count())
                {
                    ingredientsToRemove.Add(ingredient);
                }
            }

            foreach (CraftingMaterial ingredientToRemove in ingredientsToRemove)
            {
                selectedIngredients.RemoveAll((item) => item == ingredientToRemove);
            }

            RegenerateItem();
        }

        void LogAnalytic(string eventName)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName);
        }

        void LogAnalytic(string eventName, Dictionary<string, object> values)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName, values);
        }

        CreatedItemThumbnail[] GetCreatedItemThumbnails()
        {
            if (createdItemThumbnails == null || createdItemThumbnails.Length <= 0)
            {
                createdItemThumbnails = Resources.LoadAll<CreatedItemThumbnail>("Created Item Thumbnails");
            }

            return createdItemThumbnails;
        }

    }
}
