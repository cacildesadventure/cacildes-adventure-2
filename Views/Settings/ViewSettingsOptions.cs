
namespace AF
{
    using System;
    using AF.UI;
    using UnityEngine;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UIElements;

    public class ViewSettingsOptions : ViewMenu, INestedView
    {
        [Header("Components")]
        public GameSettings gameSettings;
        public PlayerCamera playerCamera;

        [Header("UI Documents")]
        public ViewSettings viewSettings;

        [Header("Footer")]
        public MenuFooter menuFooter;
        public ActionButton returnToSettingsButton;

        [Header("Localization")]

        public LocalizedString CameraSensitivityLabel; // Camera Sensitivity ({0})
        public LocalizedString CameraDistanceLabel; // Camera Distance To Player ({0})
        public LocalizedString MusicVolumeLabel; // Music Volume ({0})

        enum ViewMenu { Gameplay, Language, Graphics, Audio }

        ViewMenu viewMenu = ViewMenu.Gameplay;

        protected override void OnEnable()
        {
            base.OnEnable();

            SetupRefs();
        }

        void SetupRefs()
        {
            UIUtils.SetupButton(
            root.Q<Button>("ExitButton"), () =>
            {
                Close();
            }, soundbank);


            SetupViewMenus();

            SetupGameplaySettings();
            SetupLanguageSettings();
            SetupGraphicsSettings();
            SetupAudioSettings();

            menuFooter.SetupReferences();
            SetupFooterActions();

            UpdateUI();
        }

        void UpdateUI()
        {
            Button gameplayButton = root.Q<Button>("Gameplay");
            gameplayButton.RemoveFromClassList("navbar-button-active");
            Button languageButton = root.Q<Button>("Language");
            languageButton.RemoveFromClassList("navbar-button-active");
            Button graphicsButton = root.Q<Button>("Graphics");
            graphicsButton.RemoveFromClassList("navbar-button-active");
            Button audioButton = root.Q<Button>("Audio");
            audioButton.RemoveFromClassList("navbar-button-active");

            VisualElement gameplayOptions = root.Q<VisualElement>("GameplayOptions");
            gameplayOptions.style.display = DisplayStyle.None;
            VisualElement languageOptions = root.Q<VisualElement>("LanguageOptions");
            languageOptions.style.display = DisplayStyle.None;
            VisualElement graphicsOptions = root.Q<VisualElement>("GraphicsOptions");
            graphicsOptions.style.display = DisplayStyle.None;
            VisualElement audioOptions = root.Q<VisualElement>("AudioOptions");
            audioOptions.style.display = DisplayStyle.None;

            if (viewMenu == ViewMenu.Gameplay)
            {
                gameplayOptions.style.display = DisplayStyle.Flex;
                gameplayButton.AddToClassList("navbar-button-active");
            }
            else if (viewMenu == ViewMenu.Language)
            {
                languageOptions.style.display = DisplayStyle.Flex;
                languageButton.AddToClassList("navbar-button-active");
            }
            else if (viewMenu == ViewMenu.Graphics)
            {
                graphicsOptions.style.display = DisplayStyle.Flex;
                graphicsButton.AddToClassList("navbar-button-active");
            }
            else if (viewMenu == ViewMenu.Audio)
            {
                audioOptions.style.display = DisplayStyle.Flex;
                audioButton.AddToClassList("navbar-button-active");
            }
        }

        void SetupViewMenus()
        {
            Button gameplayButton = root.Q<Button>("Gameplay");
            UIUtils.SetupButton(gameplayButton, () =>
            {
                viewMenu = ViewMenu.Gameplay;
                UpdateUI();
            }, () => { }, () => { }, false, soundbank);

            Button languageButton = root.Q<Button>("Language");
            UIUtils.SetupButton(languageButton, () =>
            {
                viewMenu = ViewMenu.Language;
                UpdateUI();
            }, () => { }, () => { }, false, soundbank);

            Button graphicsButton = root.Q<Button>("Graphics");
            UIUtils.SetupButton(graphicsButton, () =>
            {
                viewMenu = ViewMenu.Graphics;
                UpdateUI();
            }, () => { }, () => { }, false, soundbank);

            Button audioButton = root.Q<Button>("Audio");
            UIUtils.SetupButton(audioButton, () =>
            {
                viewMenu = ViewMenu.Audio;
                UpdateUI();
            }, () => { }, () => { }, false, soundbank);
        }

        void SetupGameplaySettings()
        {
            SetupCameraSensitivitySetting();
            SetupCameraDistanceSetting();
            SetupDisplayControlsSetting();
        }

        void SetupLanguageSettings()
        {
            SetupLanguageSetting();
        }

        void SetupGraphicsSettings()
        {
            SetupGraphicsQualitySetting();
        }

        void SetupAudioSettings()
        {
            SetupMusicVolumeSetting();
        }

        void SetupLanguageSetting()
        {
            RadioButtonGroup gameLanguageOptions = root.Q<RadioButtonGroup>("GameLanguage");
            gameLanguageOptions.value = LocalizationSettings.SelectedLocale.Identifier == "en" ? 0 : 1;
            gameLanguageOptions.Focus();

            gameLanguageOptions.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue == 0)
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
                }
                else
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("pt");
                }
            });
        }

        void SetupCameraSensitivitySetting()
        {
            Slider cameraSensitivity = root.Q<Slider>("CameraSensitivity");

            cameraSensitivity.RegisterValueChangedCallback(ev =>
            {
                gameSettings.cameraSensitivity = ev.newValue;
                cameraSensitivity.label = string.Format(CameraSensitivityLabel.GetLocalizedString(), ev.newValue);
            });
            cameraSensitivity.lowValue = gameSettings.minimumCameraSensitivity;
            cameraSensitivity.highValue = gameSettings.maximumCameraSensitivity;
            cameraSensitivity.value = gameSettings.cameraSensitivity;
            cameraSensitivity.label = string.Format(CameraSensitivityLabel.GetLocalizedString(), gameSettings.cameraSensitivity);
        }

        void SetupCameraDistanceSetting()
        {
            Slider cameraDistanceToPlayer = root.Q<Slider>("CameraDistanceToPlayer");
            cameraDistanceToPlayer.RegisterValueChangedCallback(ev =>
           {
               gameSettings.cameraDistance = ev.newValue;
               cameraDistanceToPlayer.label = string.Format(CameraDistanceLabel.GetLocalizedString(), ev.newValue);

               playerCamera.UpdateCameraDistance();
           });
            cameraDistanceToPlayer.lowValue = gameSettings.minimumCameraDistance;
            cameraDistanceToPlayer.highValue = gameSettings.maximumCameraDistance;
            cameraDistanceToPlayer.value = gameSettings.cameraDistance;
            cameraDistanceToPlayer.label = string.Format(CameraDistanceLabel.GetLocalizedString(), gameSettings.cameraDistance);
        }

        void SetupGraphicsQualitySetting()
        {
            RadioButtonGroup graphicsOptions = root.Q<RadioButtonGroup>("GraphicsQuality");
            graphicsOptions.value = (int)gameSettings.graphicsQuality;
            graphicsOptions.Focus();

            graphicsOptions.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetGraphicsQuality(ev.newValue);
            });
        }

        void SetupMusicVolumeSetting()
        {
            Slider musicVolumeSlider = root.Q<Slider>("MusicVolume");

            musicVolumeSlider.RegisterValueChangedCallback(ev =>
                       {
                           gameSettings.SetMusicVolume(ev.newValue);
                           musicVolumeSlider.label = String.Format(MusicVolumeLabel.GetLocalizedString(), ev.newValue);
                       });
            musicVolumeSlider.lowValue = 0f;
            musicVolumeSlider.highValue = 1f;
            musicVolumeSlider.value = gameSettings.musicVolume;
            musicVolumeSlider.label = String.Format(MusicVolumeLabel.GetLocalizedString(), gameSettings.musicVolume);

        }

        void SetupDisplayControlsSetting()
        {
            Toggle showControlsInHUD = root.Q<Toggle>("ShowControlsInHUD");

            showControlsInHUD.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetShouldShowPlayerHUD(ev.newValue);
            });
            showControlsInHUD.value = gameSettings.showControlsInHUD;
        }

        void SetupFooterActions()
        {
            menuFooter.GetFooterActionsContainer().Add(returnToSettingsButton.GetKey(starterAssetsInputs));
        }

        public bool IsActive()
        {
            return this.isActiveAndEnabled;
        }

        public void Close()
        {
            viewSettings.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
