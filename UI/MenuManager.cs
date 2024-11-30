namespace AF
{
    using System.Collections;
    using System.Linq;
    using AF.Ladders;
    using AF.Shops;
    using AF.UI.EquipmentMenu;
    using UnityEngine;

    public class MenuManager : MonoBehaviour
    {

        public bool canUseMenu = true;

        [Header("Visuals")]
        [HideInInspector] public bool hasPlayedFadeIn = false;

        [Header("Components")]
        public UIManager uIManager;
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        public CursorManager cursorManager;
        public UIDocumentCraftScreen craftScreen;
        public UIDocumentAlchemy uIDocumentAlchemy;
        public TitleScreenManager titleScreenManager;
        public UIDocumentBook uIDocumentBook;
        public UIDocumentGameOver uIDocumentGameOver;
        public UIDocumentShopMenu uIDocumentShopMenu;
        public UIDocumentCharacterCustomization uIDocumentCharacterCustomization;

        public PlayerManager playerManager;

        [Header("Flags")]
        public bool isMenuOpen;
        public bool isShowingModal = false;

        [Header("Menu Views")]
        public ViewEquipmentMenu viewEquipmentMenu;
        public ViewQuestsMenu viewQuestsMenu;
        public ViewSettings viewSettingsMenu;
        public ViewSettingsOptions viewSettingsOptions;
        public ViewSettingsControl viewSettingsControl;

        public enum MenuTab
        {
            EQUIPMENT,
            QUESTS,
            SETTINGS
        }

        public MenuTab menuTab = MenuTab.EQUIPMENT;


        [Header("Nested Edge Case")]
        public GameObject[] nestedMenus;

        public Texture2D screenshotBeforeOpeningMenu;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnMenuInput()
        {
            if (!CanUseMenu())
            {
                return;
            }

            hasPlayedFadeIn = false;

            var activeNestedMenus = nestedMenus.Select(x => x.GetComponent<INestedView>()).Where(nestedMenu => nestedMenu.IsActive());
            if (activeNestedMenus.Count() > 0)
            {
                foreach (var activeNestedMenu in activeNestedMenus)
                {
                    activeNestedMenu.Close();
                }

                return;
            }

            if (!isMenuOpen)
            {
                StartCoroutine(CaptureScreenshot());

                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }

        IEnumerator CaptureScreenshot()
        {
            // Wait until the end of the frame
            yield return new WaitForEndOfFrame();

            // Capture the screenshot as a texture
            Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
            if (screenshotTexture != null)
            {
                this.screenshotBeforeOpeningMenu = screenshotTexture;
            }
        }

        /// <summary>
        /// UnityEvent
        /// </summary>
        public void OnCancelInput()
        {
            if (!CanUseMenu() || isMenuOpen == false || isShowingModal)
            {
                return;
            }

            hasPlayedFadeIn = false;

            var activeNestedMenus = nestedMenus.Select(x => x.GetComponent<INestedView>()).Where(nestedMenu => nestedMenu.IsActive());
            if (activeNestedMenus.Count() > 0)
            {
                foreach (var activeNestedMenu in activeNestedMenus)
                {
                    activeNestedMenu.Close();
                }

                return;
            }

            if (isMenuOpen)
            {
                CloseMenu();
            }
        }

        public void SetMenuTab(MenuTab menuTab)
        {
            CloseMenuTabs();
            this.menuTab = menuTab;

            ActivateCurrentMenuTab();
        }

        void ActivateCurrentMenuTab()
        {
            if (menuTab == MenuTab.EQUIPMENT)
            {
                viewEquipmentMenu.gameObject.SetActive(true);
            }
            else if (menuTab == MenuTab.QUESTS)
            {
                viewQuestsMenu.gameObject.SetActive(true);
            }
            else if (menuTab == MenuTab.SETTINGS)
            {
                viewSettingsMenu.gameObject.SetActive(true);
            }
        }

        public void OpenMenu()
        {
            isMenuOpen = true;
            CloseMenuTabs();
            ActivateCurrentMenuTab();
            playerManager.playerComponentManager.DisablePlayerControl();
            playerManager.thirdPersonController.LockCameraPosition = true;
            cursorManager.ShowCursor();

            uIDocumentPlayerHUDV2.HideHUD();
        }

        public void CloseMenu()
        {
            isMenuOpen = false;
            CloseMenuTabs();
            playerManager.thirdPersonController.LockCameraPosition = false;
            playerManager.playerComponentManager.EnablePlayerControl();
            cursorManager.HideCursor();
            uIDocumentPlayerHUDV2.ShowHUD();
        }

        public void CloseMenuTabs()
        {
            viewEquipmentMenu.gameObject.SetActive(false);
            viewQuestsMenu.gameObject.SetActive(false);
            viewSettingsMenu.gameObject.SetActive(false);
            viewSettingsOptions.gameObject.SetActive(false);
            viewSettingsControl.gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnNextMenu()
        {
            if (!CanSwitchMenu())
            {
                return;
            }

            if (menuTab == MenuTab.EQUIPMENT)
            {
                SetMenuTab(MenuTab.QUESTS);
            }
            else if (menuTab == MenuTab.QUESTS)
            {
                SetMenuTab(MenuTab.SETTINGS);
            }
            else if (menuTab == MenuTab.SETTINGS)
            {
                SetMenuTab(MenuTab.EQUIPMENT);
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnPreviousMenu()
        {
            if (!CanSwitchMenu())
            {
                return;
            }

            if (menuTab == MenuTab.EQUIPMENT)
            {
                SetMenuTab(MenuTab.SETTINGS);
            }
            else if (menuTab == MenuTab.QUESTS)
            {
                SetMenuTab(MenuTab.EQUIPMENT);
            }
            else if (menuTab == MenuTab.SETTINGS)
            {
                SetMenuTab(MenuTab.QUESTS);
            }
        }

        bool CanSwitchMenu()
        {
            if (!isMenuOpen)
            {
                return false;
            }

            return true;
        }

        bool CanUseMenu()
        {
            if (!canUseMenu)
            {
                return false;
            }

            if (playerManager.IsBusy())
            {
                return false;
            }

            if (uIManager.displayedUis.Count > 0)
            {
                return false;
            }

            if (uIDocumentGameOver != null)
            {
                if (uIDocumentGameOver.isActiveAndEnabled)
                {
                    return false;
                }
            }

            if (titleScreenManager != null)
            {
                if (titleScreenManager.isActiveAndEnabled)
                {
                    return false;
                }
            }

            if (uIDocumentBook != null)
            {
                if (uIDocumentBook.isActiveAndEnabled)
                {
                    return false;
                }
            }

            if (craftScreen.isActiveAndEnabled)
            {
                return false;
            }

            if (uIDocumentShopMenu.isActiveAndEnabled)
            {
                return false;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                return false;
            }

            if (uIDocumentCharacterCustomization.isActiveAndEnabled)
            {
                return false;
            }

            return true;
        }
    }
}
