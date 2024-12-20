namespace AF
{
    using AF.Inventory;
    using AF.Music;
    using GameAnalyticsSDK;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentCraftScreen : MonoBehaviour
    {
        [Header("SFX")]
        public AudioClip sfxOnEnterMenu;

        [Header("UI Components")]
        public UIDocument uIDocument;
        [HideInInspector] public VisualElement root;
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;

        [Header("Components")]
        public PlayerManager playerManager;
        public CursorManager cursorManager;
        public BGMManager bgmManager;
        public Soundbank soundbank;
        public StarterAssetsInputs starterAssetsInputs;

        [HideInInspector] public bool returnToBonfire = false;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public BlacksmithAction blacksmithAction = BlacksmithAction.UPGRADE;

        [Header("Blacksmith Components")]
        public UIBlacksmithWeaponsList uIBlacksmithWeaponsList;
        public UIBlacksmithUpgradeWeapon uIBlacksmithUpgradeWeapon;

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

            SetupTabButtons();

            DrawUI();
        }

        void SetupTabButtons()
        {
            UIUtils.SetupButton(root.Q<Button>("UpgradesButton"), () =>
            {
                this.blacksmithAction = BlacksmithAction.UPGRADE;
                DrawUI();
            }, soundbank);
            UIUtils.SetupButton(root.Q<Button>("GemstonesButton"), () =>
            {
                this.blacksmithAction = BlacksmithAction.CUSTOMIZE_GEMSTONE;
                DrawUI();
            }, soundbank);
            UIUtils.SetupButton(root.Q<Button>("SharpeningButton"), () =>
            {
                this.blacksmithAction = BlacksmithAction.SHARPEN;
                DrawUI();
            }, soundbank);
        }

        void ClearRoot()
        {
            root.Q<VisualElement>("WeaponUpgrade").style.opacity = 0;
        }

        void DrawUI()
        {
            ClearRoot();

            SetupTabs();

            uIBlacksmithWeaponsList.DrawUI(root, Close);
        }

        void SetupTabs()
        {
            Label tabDescriptionInfo = root.Q<Label>("TabInfoDescription");
            tabDescriptionInfo.text = "";
            DisableTabs();

            string activeButtonId = "";

            if (blacksmithAction == BlacksmithAction.CUSTOMIZE_GEMSTONE)
            {
                activeButtonId = "GemstonesButton";
                tabDescriptionInfo.text = Glossary.IsPortuguese()
                    ? "Personaliza os atributos da tua arma e o tipo de dano usando gemas especiais."
                    : "Customize your weapon's attributes and damage type by slotting special gemstones.";
            }
            else if (blacksmithAction == BlacksmithAction.SHARPEN)
            {
                activeButtonId = "SharpeningButton";
                tabDescriptionInfo.text = Glossary.IsPortuguese()
                    ? "Afia a tua arma com óleo, aumentando temporariamente o seu dano por 1000 ataques garantidos."
                    : "Sharpen your weapon with oil to temporarily boost its damage for 1000 guaranteed hits.";
            }
            else
            {
                activeButtonId = "UpgradesButton";
                tabDescriptionInfo.text = Glossary.IsPortuguese()
                    ? "Aumenta permanentemente o dano da tua arma ao usar materiais de melhoria."
                    : "Permanently increase your weapon's damage by using upgrade materials.";
            }

            root.Q<Button>(activeButtonId).AddToClassList("blacksmith-tab-button-active");
        }

        void DisableTabs()
        {
            root.Q<Button>("UpgradesButton").RemoveFromClassList("blacksmith-tab-button-active");
            root.Q<Button>("GemstonesButton").RemoveFromClassList("blacksmith-tab-button-active");
            root.Q<Button>("SharpeningButton").RemoveFromClassList("blacksmith-tab-button-active");
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenBlacksmithMenu()
        {
            LogAnalytic(AnalyticsUtils.OnUIButtonClick("Blacksmith"));

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
            if (returnToBonfire)
            {
                returnToBonfire = false;

                uIDocumentBonfireMenu.gameObject.SetActive(true);
                cursorManager.ShowCursor();
                this.gameObject.SetActive(false);
                return;
            }

            playerManager.playerComponentManager.EnableComponents();
            playerManager.playerComponentManager.EnableCharacterController();

            this.gameObject.SetActive(false);
            cursorManager.HideCursor();
        }

        void LogAnalytic(string eventName)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName);
        }

        public void OnSelectedWeapon(Weapon weapon)
        {
            if (blacksmithAction == BlacksmithAction.UPGRADE)
            {
                uIBlacksmithUpgradeWeapon.DrawUI(weapon, root);
            }
        }

        private void OnDisable()
        {
            uIBlacksmithWeaponsList.ClearPreviews(root);
        }
    }
}
