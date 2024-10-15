
namespace AF
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Localization;
    using UnityEngine.UIElements;

    public class ViewModal : MonoBehaviour
    {

        [Header("Localization")]
        public LocalizedString modalTitle;
        public LocalizedString modalDescription;
        public LocalizedString confirmButton;
        public LocalizedString cancelButton;

        [Header("Components")]
        public MenuManager menuManager;
        public Soundbank soundbank;

        UnityAction onConfirmCallback;
        UnityAction onCancelCallback;
        bool isSetup = false;

        private void OnEnable()
        {
            isSetup = false;
        }

        private void OnDisable()
        {
            isSetup = false;
        }

        void Setup(VisualElement containerRoot)
        {
            VisualElement modalContainer = containerRoot.Q<VisualElement>("Modal");

            UIUtils.SetupButton(
            modalContainer.Q<Button>("ExitButton"), () =>
            {
                HideModal(containerRoot);
            }, soundbank);

            modalContainer.Q<Label>("SectionTitle").text = modalTitle.GetLocalizedString();
            modalContainer.Q<Label>("Description").text = modalDescription.GetLocalizedString();

            Button confirmButtonInstance = modalContainer.Q<Button>("Confirm");
            confirmButtonInstance.text = confirmButton.GetLocalizedString();

            UIUtils.SetupButton(confirmButtonInstance, () =>
            {
                onConfirmCallback?.Invoke();
                HideModal(containerRoot);
            }, soundbank);

            Button cancelButtonInstance = modalContainer.Q<Button>("Cancel");
            cancelButtonInstance.text = cancelButton.GetLocalizedString();
            UIUtils.SetupButton(cancelButtonInstance, () => HideModal(containerRoot), soundbank);

            isSetup = true;
        }

        public void ShowModal(VisualElement containerRoot, UnityAction onConfirm, UnityAction onCancel)
        {
            this.onConfirmCallback = onConfirm;
            this.onCancelCallback = onCancel;

            if (!isSetup)
            {
                Setup(containerRoot);
            }
            VisualElement modalContainer = containerRoot.Q<VisualElement>("Modal");

            modalContainer.Q<Button>("Confirm").Focus();
            modalContainer.style.display = DisplayStyle.Flex;

            // Disable the content outside the modal from being picked up
            VisualElement outsideContent = containerRoot.Q<VisualElement>("Content");
            if (outsideContent != null)
            {
                outsideContent.style.display = DisplayStyle.None;
            }

            menuManager.isShowingModal = true;
        }

        public void HideModal(VisualElement containerRoot)
        {
            containerRoot.Q<VisualElement>("Modal").style.display = DisplayStyle.None;

            // Allow the content outside the modal to be picked up again
            VisualElement outsideContent = containerRoot.Q<VisualElement>("Content");
            if (outsideContent != null)
            {
                outsideContent.style.display = DisplayStyle.Flex;
            }

            onCancelCallback?.Invoke();

            this.onConfirmCallback = null;
            this.onCancelCallback = null;
            menuManager.isShowingModal = false;
        }
    }
}
