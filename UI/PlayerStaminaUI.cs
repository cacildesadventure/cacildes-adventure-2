namespace AF
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class PlayerStaminaUI : MonoBehaviour
    {
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        VisualElement container;
        VisualElement fill;

        [Header("Graphic Settings")]
        public float containerBaseWidth = 150;
        float _containerMultiplierPerLevel = 10f;
        public Color staminaOriginalColor;

        [Header("Components")]
        public PlayerManager playerManager;
        bool isSetup = false;

        void Start()
        {
            playerManager.staminaStatManager.onStaminaChanged.AddListener(UpdateUI);
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
            container = uIDocumentPlayerHUDV2.uIDocument.rootVisualElement.Q<VisualElement>("Stamina");
            fill = container.Q<VisualElement>("Fill");
            isSetup = true;
        }

        void UpdateUI()
        {
            if (!isSetup)
            {
                SetupRefs();
            }

            container.style.width = containerBaseWidth + ((
                playerManager.statsBonusController.GetCurrentEndurance()) * _containerMultiplierPerLevel);

            this.fill.style.width = new Length(playerManager.staminaStatManager.GetCurrentStaminaPercentage(), LengthUnit.Percent);
        }

        public void DisplayInsufficientStamina()
        {
            UIUtils.DisplayInsufficientBarBackgroundColor(staminaOriginalColor, fill);
        }


    }
}
