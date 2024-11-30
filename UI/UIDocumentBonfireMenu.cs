namespace AF
{
    using System.Collections;
    using AF.Bonfires;
    using AF.UI;
    using UnityEngine;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UIElements;

    public class UIDocumentBonfireMenu : MonoBehaviour
    {
        [Header("UI Components")]
        public UIDocument uIDocument;
        public UIDocumentLevelUp uiDocumentLevelUp;
        public UIDocumentBonfireTravel uiDocumentTravel;

        [Header("Components")]
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public Soundbank soundbank;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("References")]
        public GameSession gameSession;

        [Header("Footer")]
        public MenuFooter menuFooter;

        public ActionButton confirmButton, exitMenuButton;
        public StarterAssetsInputs starterAssetsInputs;

        [Header("UI Elements")]
        VisualElement root;
        Button levelUpButton, passTimeButton, exitBonfireButton, travelButton;
        Label bonfireName, bonfireNameLabelUI, canLevelUpIndicator, currentGoldAndRequiredLabel, goldAndRequiredForNextLevel;

        // Flags
        bool isPassingTime = false;
        float originalDaySpeed = 0f;
        bool hasEnoughForLevellingUp = false;

        Bonfire currentBonfire;
        bool canEscape = false;

        private void Start()
        {
            originalDaySpeed = gameSession.daySpeed;
            gameObject.SetActive(false);
        }

        void SetupRefs()
        {
            this.root = uIDocument.rootVisualElement;

            bonfireName = root.Q<Label>("BonfireName");
            bonfireNameLabelUI = root.Q<Label>("BonfireNameLabel");
            canLevelUpIndicator = root.Q<Label>("CanLevelUpIndicator");
            currentGoldAndRequiredLabel = root.Q<Label>("CurrentGoldAndRequiredLabel");
            goldAndRequiredForNextLevel = root.Q<Label>("GoldAndRequiredFornextLevel");

            levelUpButton = root.Q<Button>("LevelUpButton");
            passTimeButton = root.Q<Button>("PassTimeButton");
            exitBonfireButton = root.Q<Button>("LeaveButton");
            travelButton = root.Q<Button>("TravelButton");


            menuFooter.SetupReferences();
            SetupFooterActions();
        }

        private void OnEnable()
        {
            canEscape = false;
            SetupRefs();
            DrawUI();
            Invoke(nameof(ResetCanEscapeFlag), 0.5f);
            playerManager.uIDocumentPlayerHUDV2.HideHUD();
        }

        void ResetCanEscapeFlag()
        {
            canEscape = true;
        }

        public void SetCurrentBonfire(Bonfire bonfire)
        {
            this.currentBonfire = bonfire;
        }

        public void DrawUI()
        {
            cursorManager.ShowCursor();
            isPassingTime = false;

            if (currentBonfire != null)
            {
                bonfireName.text = currentBonfire.GetBonfireName();
            }

            hasEnoughForLevellingUp = playerStatsDatabase.gold >= playerManager.playerLevelManager.GetRequiredExperienceForNextLevel();

            SetupButtons();
        }

        void SetupButtons()
        {
            canLevelUpIndicator.text = hasEnoughForLevellingUp ? " *" : "";

            SetButtonTexts();
            RegisterButtonCallbacks();

            travelButton.style.display = (currentBonfire != null && currentBonfire.canUseTravelToOtherMaps) ? DisplayStyle.Flex : DisplayStyle.None;

            exitBonfireButton.Focus();
        }

        void SetButtonTexts()
        {
            goldAndRequiredForNextLevel.text = $"{playerStatsDatabase.gold} / {playerManager.playerLevelManager.GetRequiredExperienceForNextLevel()}";
        }

        void RegisterButtonCallbacks()
        {
            UIUtils.SetupButton(exitBonfireButton, () =>
            {
                ExitBonfire();
            },
            () =>
            {
                menuFooter.DisplayTooltip(GetReturnTooltip());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);

            UIUtils.SetupButton(levelUpButton, () =>
            {
                uiDocumentLevelUp.gameObject.SetActive(true);
                gameObject.SetActive(false);
            },
            () =>
            {
                menuFooter.DisplayTooltip(GetLevelupTooltip());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);

            UIUtils.SetupButton(passTimeButton, () =>
            {
                if (!isPassingTime)
                    StartCoroutine(MoveTime());
            },
            () =>
            {
                menuFooter.DisplayTooltip(GetWait1HourTooltip());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);

            UIUtils.SetupButton(travelButton, () =>
            {
                uiDocumentTravel.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            },
            () =>
            {
                menuFooter.DisplayTooltip(GetTravelTooltip());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled == false || canEscape == false)
            {
                return;
            }

            if (uiDocumentLevelUp.isActiveAndEnabled == false
                && uiDocumentTravel.isActiveAndEnabled == false
                )
            {
                ExitBonfire();
            }
        }

        void ExitBonfire()
        {
            cursorManager.HideCursor();
            gameSession.daySpeed = originalDaySpeed;
            currentBonfire.ExitBonfire();
            currentBonfire = null;
            playerManager.uIDocumentPlayerHUDV2.ShowHUD();
        }

        IEnumerator MoveTime()
        {
            if (!isPassingTime)
            {
                isPassingTime = true;
                var originalDaySpeed = gameSession.daySpeed;
                var targetHour = Mathf.Floor(gameSession.timeOfDay) + 1;

                if (targetHour > 23)
                {
                    gameSession.timeOfDay = 0;
                    targetHour = 0;
                }

                yield return null;
                gameSession.daySpeed = 2;

                yield return new WaitUntil(() => Mathf.FloorToInt(gameSession.timeOfDay) == Mathf.FloorToInt(targetHour));

                gameSession.daySpeed = originalDaySpeed;
                isPassingTime = false;
            }
        }


        void SetupFooterActions()
        {
            menuFooter.GetFooterActionsContainer().Add(confirmButton.GetKey(starterAssetsInputs));
            menuFooter.GetFooterActionsContainer().Add(exitMenuButton.GetKey(starterAssetsInputs));
        }

        string GetReturnTooltip()
        {
            if (LocalizationSettings.SelectedLocale.Identifier.Code == "pt")
            {
                return "Sair da fogueira e continua a tua aventura";
            }

            return "Exit bonfire and resume your adventure";
        }
        string GetLevelupTooltip()
        {
            if (LocalizationSettings.SelectedLocale.Identifier.Code == "pt")
            {
                return "Usa o teu ouro para subir de nível";
            }

            return "Use your gold to level up";
        }
        string GetWait1HourTooltip()
        {
            if (LocalizationSettings.SelectedLocale.Identifier.Code == "pt")
            {
                return "Esperar uma hora";
            }

            return "Wait one hour";
        }

        string GetTravelTooltip()
        {
            if (LocalizationSettings.SelectedLocale.Identifier.Code == "pt")
            {
                return "Viajar para outra fogueira";
            }

            return "Fast-travel to another bonfire";
        }


    }
}
