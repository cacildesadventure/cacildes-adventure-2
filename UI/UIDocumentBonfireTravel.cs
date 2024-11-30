namespace AF
{
    using System.Collections.Generic;
    using AF.Bonfires;
    using AF.UI;
    using UnityEngine;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UIElements;


    [System.Serializable]
    public class BonfireLocation
    {
        public Location location;
        public string bonfireId;
        public string spawnGameObjectNameRef;
    }

    public class UIDocumentBonfireTravel : MonoBehaviour
    {
        public List<BonfireLocation> bonfireLocations = new();

        [Header("Components")]
        public Soundbank soundbank;
        public CursorManager cursorManager;
        public TeleportManager teleportManager;

        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualTreeAsset travelOptionAsset;
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;
        [Header("Footer")]
        public MenuFooter menuFooter;

        public ActionButton confirmButton, exitMenuButton;
        public StarterAssetsInputs starterAssetsInputs;
        [Header("Databases")]
        public BonfiresDatabase bonfiresDatabase;

        // Internal
        VisualElement root;

        // Last scroll position
        int lastScrollElementIndex = -1;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled)
            {
                Close();
            }
        }

        void Close()
        {
            uIDocumentBonfireMenu.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            root.Q<ScrollView>().Clear();
            root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 0;


            menuFooter.SetupReferences();
            SetupFooterActions();

            // The exit button
            var exitOption = travelOptionAsset.CloneTree();
            exitOption.Q<Button>().text = GetReturnLabel();

            UIUtils.SetupButton(exitOption.Q<Button>(), () =>
            {
                Close();
            },
            () =>
            {
                {
                    root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 0;
                }
            },
            () =>
            {
                root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 0;
            },
            true,
            soundbank);

            root.Q<ScrollView>().Add(exitOption);

            // Add callbacks
            foreach (var location in bonfireLocations)
            {
                if (bonfiresDatabase.unlockedBonfires.Contains(location.bonfireId))
                {
                    var clonedBonfireOption = travelOptionAsset.CloneTree();
                    clonedBonfireOption.Q<Button>().text = location.location.GetLocationDisplayName();

                    UIUtils.SetupButton(clonedBonfireOption.Q<Button>(), () =>
                    {
                        teleportManager.Teleport(location.location.name, location.spawnGameObjectNameRef);
                    },
                    () =>
                    {
                        {
                            root.Q<IMGUIContainer>("BonfireIcon").style.backgroundImage = new StyleBackground(location.location.locationThumbnail);
                            root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 1;
                            root.Q<ScrollView>().ScrollTo(clonedBonfireOption);
                        }
                    },
                    () =>
                    {
                        root.Q<IMGUIContainer>("BonfireIcon").style.opacity = 0;
                    },
                    true,
                    soundbank);


                    root.Q<ScrollView>().Add(clonedBonfireOption);
                }

            }

            cursorManager.ShowCursor();

            if (lastScrollElementIndex == -1)
            {
                root.Q<ScrollView>().ScrollTo(exitOption);
            }
            else
            {
                Invoke(nameof(GiveFocus), 0f);
            }
        }
        void GiveFocus()
        {
            UIUtils.ScrollToLastPosition(
                lastScrollElementIndex,
                root.Q<ScrollView>(),
                () =>
                {
                    lastScrollElementIndex = -1;
                }
            );
        }
        void SetupFooterActions()
        {
            menuFooter.GetFooterActionsContainer().Add(confirmButton.GetKey(starterAssetsInputs));
            menuFooter.GetFooterActionsContainer().Add(exitMenuButton.GetKey(starterAssetsInputs));
        }

        string GetReturnLabel()
        {
            if (LocalizationSettings.SelectedLocale.Identifier.Code == "pt")
            {
                return "Regressar";
            }

            return "Return";
        }
    }
}
