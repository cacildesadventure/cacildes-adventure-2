using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.Localization;

namespace AF.UI
{
    public class ViewMenu : MonoBehaviour
    {
        [HideInInspector] public VisualElement root;

        public const string EQUIPMENT_BUTTON = "EquipmentButton";
        public const string OBJECTIVES_BUTTON = "ObjectivesButton";
        public const string OPTIONS_BUTTON = "OptionsGameButton";

        // Button References
        private Button equipmentButton;
        private Button objectivesButton;
        private Button optionsButton;

        [Header("Components")]
        public MenuManager menuManager;
        public CursorManager cursorManager;
        public Soundbank soundbank;
        public FadeManager fadeManager;
        public SaveManager saveManager;
        public GameSession gameSession;

        [Header("Inputs")]
        public ActionButton previousMenu;
        public ActionButton nextMenu;
        public StarterAssetsInputs starterAssetsInputs;

        [Header("Menu Tab")]
        public MenuManager.MenuTab menuTab = MenuManager.MenuTab.EQUIPMENT;

        protected virtual void OnEnable()
        {
            SetupReferences();

            root.Q("Container").Q("PreviousButtonRoot").Add(previousMenu.GetKey(starterAssetsInputs));
            InitializeButtonStates();
            root.Q("Container").Q("NextButtonRoot").Add(nextMenu.GetKey(starterAssetsInputs));

            HighlightActiveButton();
        }

        private void Update()
        {
            if (!UnityEngine.Cursor.visible)
            {
                cursorManager.ShowCursor();
            }
        }

        private void SetupReferences()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            if (root == null || menuManager.hasPlayedFadeIn) return;

            menuManager.hasPlayedFadeIn = true;
            PlayMenuOpenAnimation();
        }

        private void PlayMenuOpenAnimation()
        {
            soundbank.PlaySound(soundbank.mainMenuOpen);

            DOTween.To(
                () => root.contentContainer.style.opacity.value,
                value => root.contentContainer.style.opacity = value,
                1f, // Target opacity
                0.25f // Duration
            );
        }

        private void InitializeButtonStates()
        {
            equipmentButton = root.Q<Button>(EQUIPMENT_BUTTON);
            objectivesButton = root.Q<Button>(OBJECTIVES_BUTTON);
            optionsButton = root.Q<Button>(OPTIONS_BUTTON);

            SetupButton(equipmentButton, MenuManager.MenuTab.EQUIPMENT);
            SetupButton(objectivesButton, MenuManager.MenuTab.QUESTS);
            SetupButton(optionsButton, MenuManager.MenuTab.SETTINGS);
        }

        private void SetupButton(Button button, MenuManager.MenuTab menuTab)
        {
            UIUtils.SetupButton(button, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);
                menuManager.SetMenuTab(menuTab);

                if (this.menuTab == menuTab)
                {
                    button.Focus();
                }
            }, soundbank);
        }

        private void HighlightActiveButton()
        {
            switch (menuManager.menuTab)
            {
                case MenuManager.MenuTab.EQUIPMENT:
                    MarkAsActive(equipmentButton);
                    break;
                case MenuManager.MenuTab.QUESTS:
                    MarkAsActive(objectivesButton);
                    break;
                case MenuManager.MenuTab.SETTINGS:
                    MarkAsActive(optionsButton);
                    break;
                default:
                    break;
            }
        }

        private void MarkAsActive(Button button)
        {
            UIUtils.PlayPopAnimation(button);

            button.AddToClassList("navbar-button-active");
        }
    }
}
