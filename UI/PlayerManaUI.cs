namespace AF
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class PlayerManaUI : MonoBehaviour
    {
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        VisualElement manaContainer;
        VisualElement manaFill;

        [Header("Graphic Settings")]
        public Color manaOriginalColor;
        public float manaContainerBaseWidth = 150;
        float _containerMultiplierPerLevel = 10f;

        [Header("Components")]
        public PlayerManager playerManager;
        bool isSetup = false;

        void Start()
        {
            playerManager.health.onHealthChanged.AddListener(UpdateUI);
            UpdateUI();
        }

        private void OnEnable()
        {
            if (!isSetup)
            {
                SetupRefs();
            }
        }

        void SetupRefs()
        {
            manaContainer = uIDocumentPlayerHUDV2.uIDocument.rootVisualElement.Q<VisualElement>("Mana");
            manaFill = manaContainer.Q<VisualElement>("Fill");
            isSetup = true;
        }

        void UpdateUI()
        {
            if (!isSetup)
            {
                SetupRefs();
            }

            manaContainer.style.width = manaContainerBaseWidth + ((
                playerManager.statsBonusController.GetCurrentIntelligence()) * _containerMultiplierPerLevel);

            this.manaFill.style.width = new Length(playerManager.manaManager.GetCurrentManaPercentage(), LengthUnit.Percent);
        }


        public void DisplayInsufficientMana()
        {
            UIUtils.DisplayInsufficientBarBackgroundColor(manaOriginalColor, manaFill);
        }

    }
}
