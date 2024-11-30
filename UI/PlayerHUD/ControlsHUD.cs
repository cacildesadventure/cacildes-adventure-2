namespace AF
{
    using System.Collections.Generic;
    using AF.Events;
    using AF.UI;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UIElements;

    public class ControlsHUD : MonoBehaviour
    {
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        VisualElement root;
        VisualElement inputActionsContainer, inputActions;

        [Header("Databases")]
        public GameSettings gameSettings;

        [Header("Actions")]
        public List<ActionButton> actionButtons = new();

        [Header("Components")]
        public StarterAssetsInputs starterAssetsInputs;

        private void Awake()
        {
            SetupRefs();

            EventManager.StartListening(EventMessages.ON_USE_CUSTOM_INPUT_CHANGED, UpdateUI);
            EventManager.StartListening(EventMessages.ON_PLAYER_HUD_VISIBILITY_CHANGED, UpdateUI);
        }

        void OnEnable()
        {
            InputSystem.onDeviceChange += HandleDeviceChangeCallback;

            UpdateUI();
        }

        private void SetupRefs()
        {
            this.root = uIDocumentPlayerHUDV2.uIDocument.rootVisualElement;

            inputActionsContainer = root.Q<VisualElement>("InputActionsContainer");
            inputActions = root.Q<VisualElement>("InputActions");

        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= HandleDeviceChangeCallback;
        }

        private void HandleDeviceChangeCallback(InputDevice device, InputDeviceChange change)
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (!isActiveAndEnabled || root == null) return;

            if (gameSettings.showControlsInHUD)
            {
                UpdateInputsHUD();
                ShowControlHints();
            }
            else
            {
                HideControlHints();
            }

        }


        public void ShowControlHints()
        {
            inputActionsContainer.style.opacity = 1;
        }

        public void HideControlHints()
        {
            inputActionsContainer.style.opacity = 0;
        }

        void UpdateInputsHUD()
        {
            inputActions.Clear();

            foreach (var actionButton in actionButtons)
            {
                VisualElement keyToAdd = actionButton.GetKey(starterAssetsInputs);
                keyToAdd.style.flexDirection = FlexDirection.RowReverse;
                inputActions.Add(keyToAdd);
            }
        }

    }
}
