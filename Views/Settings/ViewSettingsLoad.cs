
namespace AF
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using AF.UI;
    using UnityEngine;
    using UnityEngine.Localization;
    using UnityEngine.UIElements;

    public class ViewSettingsLoad : ViewMenu, INestedView
    {
        [Header("UI Documents")]
        public ViewSettings viewSettings;
        public VisualTreeAsset saveFileButtonPrefab;
        ScrollView scrollPanel;

        public ViewModal viewModal;

        [Header("Footer")]
        public MenuFooter menuFooter;
        public ActionButton returnToSettingsButton;
        public ActionButton deleteSaveFileButton;
        VisualElement deleteSaveFileButtonInstance;
        public ActionButton confirmLoadSaveFileButton;
        VisualElement confirmLoadSaveFileButtonInstance;

        // Last scroll position
        int lastScrollElementIndex = -1;
        string selectedSaveFile = "";
        string saveFileWaitingForDeletion = "";

        [Header("Localization")]
        public LocalizedString OpenSavesFolder_LocalizedString;

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

            UIUtils.SetupButton(root.Q<Button>("OpenSavesFolder"), () =>
            {
                // Open the folder using the default file explorer
                Process.Start(Application.persistentDataPath + "/" + SaveUtils.SAVE_FILES_FOLDER);
            }, soundbank);

            scrollPanel = root.Q<ScrollView>("SaveFilesContainer");

            DrawUI();

            menuFooter.SetupReferences();
            SetupFooterActions();
        }


        void DrawUI()
        {
            scrollPanel.Clear();
            selectedSaveFile = "";

            UIUtils.SetupSlider(scrollPanel);

            foreach (var saveFileName in SaveUtils.GetSaveFileNames(SaveUtils.SAVE_FILES_FOLDER))
            {
                var saveFileInstance = saveFileButtonPrefab.CloneTree();

                saveFileInstance.Q<Label>("SaveFileName").text = saveFileName;

                Texture2D screenshotThumbnail = SaveUtils.GetScreenshotFilePath(SaveUtils.SAVE_FILES_FOLDER, saveFileName);
                saveFileInstance.Q<VisualElement>("SaveScreenshot").style.backgroundImage = screenshotThumbnail;
                saveFileInstance.Q<VisualElement>("SaveScreenshot").Q<Label>("SaveFileNotFoundLabel").style.display =
                    screenshotThumbnail == null ? DisplayStyle.Flex : DisplayStyle.None;

                UIUtils.SetupButton(saveFileInstance.Q<Button>("Button"), () =>
                {
                    saveManager.LoadSaveFile(saveFileName);
                }, () =>
                {
                    selectedSaveFile = saveFileName;
                    deleteSaveFileButtonInstance.style.display = DisplayStyle.Flex;
                    confirmLoadSaveFileButtonInstance.style.display = DisplayStyle.Flex;
                    scrollPanel.ScrollTo(saveFileInstance.Q<Button>());
                },
                () =>
                {
                    selectedSaveFile = "";
                    deleteSaveFileButtonInstance.style.display = DisplayStyle.None;
                    confirmLoadSaveFileButtonInstance.style.display = DisplayStyle.None;
                }, false, soundbank);

                scrollPanel.Add(saveFileInstance);
            }

            Invoke(nameof(GiveFocus), 0f);
        }

        void GiveFocus()
        {
            if (lastScrollElementIndex == -1)
            {
                root.Q<Button>("ExitButton").Focus();
                return;
            }

            UIUtils.ScrollToLastPosition(
                lastScrollElementIndex,
                scrollPanel,
                () =>
                {
                    lastScrollElementIndex = -1;
                }
            );
        }


        void SetupFooterActions()
        {
            deleteSaveFileButtonInstance = deleteSaveFileButton.GetKey(starterAssetsInputs);
            deleteSaveFileButtonInstance.style.display = DisplayStyle.None;
            menuFooter.GetFooterActionsContainer().Add(deleteSaveFileButtonInstance);

            confirmLoadSaveFileButtonInstance = confirmLoadSaveFileButton.GetKey(starterAssetsInputs);
            confirmLoadSaveFileButtonInstance.style.display = DisplayStyle.None;
            menuFooter.GetFooterActionsContainer().Add(confirmLoadSaveFileButtonInstance);

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

        public void OnDeleteSaveFile()
        {
            if (string.IsNullOrEmpty(selectedSaveFile))
            {
                return;
            }

            saveFileWaitingForDeletion = selectedSaveFile;

            viewModal.ShowModal(root, () =>
            {
                SaveUtils.DeleteSaveFile(SaveUtils.SAVE_FILES_FOLDER, saveFileWaitingForDeletion);
                saveFileWaitingForDeletion = "";
                DrawUI();
            },
            () =>
            {
                saveFileWaitingForDeletion = "";
            });
        }
    }
}
