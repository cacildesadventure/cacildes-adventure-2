using System.Linq;
using AF.UI;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace AF
{
    public class ViewSettings : ViewMenu
    {
        [Header("UI Documents")]
        public ViewSettingsOptions viewSettingsOptions;
        public ViewSettingsControl viewSettingsControl;
        public ViewSettingsLoad viewSettingsLoad;
        VisualElement optionsContainer;

        Button saveButton, loadButton, controlsButton, optionsButton, returnToTitleScreenButton, exitButton, newGamePlusButton;

        [Header("Quest To Allow New Game Plus")]
        public QuestParent questParentToAllowNewGamePlus;
        public int[] rangeOfQuestToAllowNewGamePlus;


        [Header("Footer")]
        public MenuFooter menuFooter;

        public ActionButton confirmButton, exitMenuButton;

        [Header("Localization")]
        public LocalizedString saveTooltip;
        public LocalizedString optionsTooltip;
        public LocalizedString controlTooltip;
        public LocalizedString loadTooltip;
        public LocalizedString returnToTitleScreenTooltip;
        public LocalizedString exitTooltip;

        [Header("Databases")]
        public GameSettings gameSettings;

        protected override void OnEnable()
        {
            base.OnEnable();

            SetupRefs();
        }

        private void OnDisable()
        {
            gameSettings.SavePreferences();
        }

        void LogAnalytic(string eventName)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName);
        }
        void SetupRefs()
        {
            //            viewComponent_GameSettings.SetupRefs(root);

            UIUtils.PlayPopAnimation(root.Q<VisualElement>("Swords"), new Vector3(0.8f, 0.8f, 0.8f), 1f);

            optionsContainer = root.Q<VisualElement>("OptionsMenu");
            optionsContainer.style.display = DisplayStyle.Flex;

            SetupSaveButton();
            SetupLoadGameButton();
            SetupGameplayButton();
            SetupControlButton();
            SetupNewGamePlusButton();

            SetupReturnToTitleScreenButton();
            SetupExitGameButton();

            menuFooter.SetupReferences();
            SetupFooterActions();

            saveButton.Focus();
        }

        void SetupFooterActions()
        {
            menuFooter.GetFooterActionsContainer().Add(confirmButton.GetKey(starterAssetsInputs));
            menuFooter.GetFooterActionsContainer().Add(exitMenuButton.GetKey(starterAssetsInputs));
        }

        void SetupSaveButton()
        {
            saveButton = root.Q<Button>("SaveGame");
            saveButton.SetEnabled(saveManager.CanSave());

            UIUtils.SetupButton(saveButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);
                saveManager.SaveGameData(menuManager.screenshotBeforeOpeningMenu);
            },
            () =>
            {
                menuFooter.DisplayTooltip(saveTooltip.GetLocalizedString());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);
        }

        void SetupControlButton()
        {
            controlsButton = root.Q<Button>("Controls");

            UIUtils.SetupButton(controlsButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);

                viewSettingsControl.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            },
            () =>
            {
                menuFooter.DisplayTooltip(controlTooltip.GetLocalizedString());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);
        }
        void SetupLoadGameButton()
        {
            loadButton = root.Q<Button>("LoadGame");

            UIUtils.SetupButton(loadButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);

                viewSettingsLoad.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            },
            () =>
            {
                menuFooter.DisplayTooltip(loadTooltip.GetLocalizedString());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);
        }

        void SetupGameplayButton()
        {
            optionsButton = root.Q<Button>("Gameplay");

            UIUtils.SetupButton(optionsButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);

                viewSettingsOptions.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            },
            () =>
            {
                menuFooter.DisplayTooltip(optionsTooltip.GetLocalizedString());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);
        }

        void SetupNewGamePlusButton()
        {
            newGamePlusButton = root.Q<Button>("NewGamePlus");
            root.Q<Label>("CurrentNewGameCounter").text = " " + gameSession.currentGameIteration;

            if (questParentToAllowNewGamePlus != null && rangeOfQuestToAllowNewGamePlus.Contains(questParentToAllowNewGamePlus.questProgress))
            {
                UIUtils.SetupButton(newGamePlusButton, () =>
                {
                    LogAnalytic(AnalyticsUtils.OnUIButtonClick(newGamePlusButton.name));

                    soundbank.PlaySound(soundbank.uiHover);

                    fadeManager.FadeIn(1f, () =>
                    {
                        saveManager.ResetGameStateForNewGamePlusAndReturnToTitleScreen();
                    });

                }, soundbank);
                newGamePlusButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                newGamePlusButton.style.display = DisplayStyle.None;
            }
        }

        void SetupReturnToTitleScreenButton()
        {
            returnToTitleScreenButton = root.Q<Button>("ReturnToTitleScreen");
            UIUtils.SetupButton(returnToTitleScreenButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);

                fadeManager.FadeIn(1f, () =>
                {
                    saveManager.ResetGameStateAndReturnToTitleScreen(false);
                });
            },
            () =>
            {
                menuFooter.DisplayTooltip(returnToTitleScreenTooltip.GetLocalizedString());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);
        }

        void SetupExitGameButton()
        {
            exitButton = root.Q<Button>("ExitGame");
            UIUtils.SetupButton(exitButton, () =>
            {
                soundbank.PlaySound(soundbank.uiHover);

                fadeManager.FadeIn(1f, () =>
                {
                    Application.Quit();
                });
            },
            () =>
            {
                menuFooter.DisplayTooltip(exitTooltip.GetLocalizedString());
            },
            () =>
            {
                menuFooter.HideTooltip();
            }, true, soundbank);
        }
    }
}
