
namespace AF
{
    using System.Collections.Generic;
    using AF.UI;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentReceivedItemPrompt : MonoBehaviour
    {
        [System.Serializable]
        public class ItemsReceived
        {
            public string itemName = "Item Name";
            public int quantity = 0;
            public Sprite sprite = null;
        }

        [Header("Components")]
        public StarterAssetsInputs starterAssetsInputs;
        public ActionButton interactButton;

        VisualElement root;
        VisualElement actionButtonContainer;

        [Header("UI Components")]
        UIDocument uiDocument => GetComponent<UIDocument>();
        public VisualTreeAsset receivedItemPrefab;

        VisualElement rootPanel, cardContainer;

        bool isPopupActive = false;

        [Header("Sounds")]
        public Soundbank soundbank;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        void SetupUI()
        {
            isPopupActive = false;

            root = uiDocument.rootVisualElement;
            actionButtonContainer = root.Q<VisualElement>("ActionButtonContainer");

            // Clean up action button in case gamepad / keyboard input changes
            actionButtonContainer.Clear();
            var resultKey = interactButton.GetKey(starterAssetsInputs);
            actionButtonContainer.Add(resultKey);


            rootPanel = root.Q<VisualElement>("ReceivedItemsContainer");
            rootPanel.Clear();
        }

        public void DisplayItemsReceived(List<ItemsReceived> itemsReceived)
        {
            SetupUI();

            foreach (var itemUIEntry in itemsReceived)
            {
                var clone = receivedItemPrefab.CloneTree();

                clone.Q<VisualElement>("ActionButtons").Q<Label>("ItemName").text = itemUIEntry.itemName;
                clone.Q<Label>("ItemQuantity").text = itemUIEntry.quantity.ToString();
                clone.Q<IMGUIContainer>("ItemSprite").style.backgroundImage = new StyleBackground(itemUIEntry.sprite);

                rootPanel.Add(clone);
            }

            // Force the flag to be activated in the next frame so that OnInteract doesn't overlap with another OnInteract event of the same frame (i.e. Opening a chest)
            Invoke(nameof(SetIsPopupActiveToTrue), 0f);
        }

        void SetIsPopupActiveToTrue()
        {
            isPopupActive = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnInteract()
        {
            if (isPopupActive)
            {
                isPopupActive = false;

                gameObject.SetActive(false);

                soundbank.PlaySound(soundbank.uiDecision);
            }
        }

    }
}
