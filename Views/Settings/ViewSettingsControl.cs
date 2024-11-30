
namespace AF
{
    using System.Collections;
    using AF.UI;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.InputSystem;
    using UnityEngine.UIElements;

    public class ViewSettingsControl : ViewMenu, INestedView
    {
        [Header("Components")]
        public GameSettings gameSettings;

        [Header("UI Documents")]
        public ViewSettings viewSettings;

        [Header("Footer")]
        public MenuFooter menuFooter;
        public ActionButton returnToSettingsButton;

        [Header("Key Rebinding")]
        public ControlsHUD controlsHUD;
        VisualElement pressAnyKeyModal;
        public StarterAssetsInputs inputs;

        enum ViewMenu { Keyboard, Dualshock, Xbox }

        ViewMenu controlScheme = ViewMenu.Keyboard;

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

            pressAnyKeyModal = root.Q<VisualElement>("PressAnyKeyModal");
            pressAnyKeyModal.style.display = DisplayStyle.None;

            SetupControlSchemeButtons();

            SetupCustomizeCallbacks();

            menuFooter.SetupReferences();
            SetupFooterActions();

            UpdateUI();
        }

        void SetupControlSchemeButtons()
        {
            Button keyboardButton = root.Q<Button>("Keyboard");
            UIUtils.SetupButton(keyboardButton, () =>
            {
                controlScheme = ViewMenu.Keyboard;
                UpdateUI();
            }, () => { }, () => { }, false, soundbank);

            Button dualshockButton = root.Q<Button>("Dualshock");
            UIUtils.SetupButton(dualshockButton, () =>
            {
                controlScheme = ViewMenu.Dualshock;
                UpdateUI();
            }, () => { }, () => { }, false, soundbank);

            Button xboxButton = root.Q<Button>("Xbox");
            UIUtils.SetupButton(xboxButton, () =>
            {
                controlScheme = ViewMenu.Xbox;
                UpdateUI();
            }, () => { }, () => { }, false, soundbank);
        }

        void SetupCustomizeCallbacks()
        {
            SetupCustomizableKey("HeavyAttack");
            SetupCustomizableKey("Jump");
            SetupCustomizableKey("Dodge");
            SetupCustomizableKey("Sprint");
            SetupCustomizableKey("ChangeCombatStance");
        }

        void SetupCustomizableKey(string key)
        {
            VisualElement container = root.Q<VisualElement>("KeyboardList").Q<VisualElement>(key);
            container.Q("IconContainer").Q<Label>().text = starterAssetsInputs.GetCurrentKeyBindingForAction(key);

            Button customizeButton = container.Q<Button>("Customize");
            customizeButton.SetEnabled(Gamepad.current == null);
            UIUtils.SetupButton(customizeButton, () =>
            {
                StartCoroutine(
                    SelectKeyBinding(
                        key,
                        (bindingPayload) =>
                        {
                            gameSettings.UpdateKey(key, bindingPayload);
                            gameSettings.SetInputOverrides(starterAssetsInputs);
                        },
                        SetupCustomizeCallbacks
                    )
                );
            }, soundbank);
        }

        IEnumerator SelectKeyBinding(string actionName, UnityAction<string> onRebindSuccessPayload, UnityAction onFinish)
        {
            pressAnyKeyModal.style.display = DisplayStyle.Flex;

            yield return inputs.Rebind(actionName, (action) =>
            {
                onRebindSuccessPayload.Invoke(action);
            });

            pressAnyKeyModal.style.display = DisplayStyle.None;

            onFinish?.Invoke();

            controlsHUD.UpdateUI();
        }

        void UpdateUI()
        {
            Button keyboardButton = root.Q<Button>("Keyboard");
            keyboardButton.RemoveFromClassList("navbar-button-active");
            Button dualshockButton = root.Q<Button>("Dualshock");
            dualshockButton.RemoveFromClassList("navbar-button-active");
            Button xboxButton = root.Q<Button>("Xbox");
            xboxButton.RemoveFromClassList("navbar-button-active");

            VisualElement keyboardList = root.Q<VisualElement>("KeyboardList");
            keyboardList.style.display = DisplayStyle.None;

            VisualElement dualshockList = root.Q<VisualElement>("DualshockList");
            dualshockList.style.display = DisplayStyle.None;

            VisualElement xboxList = root.Q<VisualElement>("XboxList");
            xboxList.style.display = DisplayStyle.None;

            if (controlScheme == ViewMenu.Keyboard)
            {
                keyboardList.style.display = DisplayStyle.Flex;
                keyboardButton.AddToClassList("navbar-button-active");
            }
            else if (controlScheme == ViewMenu.Dualshock)
            {
                dualshockList.style.display = DisplayStyle.Flex;
                dualshockButton.AddToClassList("navbar-button-active");
            }
            else if (controlScheme == ViewMenu.Xbox)
            {
                xboxList.style.display = DisplayStyle.Flex;
                xboxButton.AddToClassList("navbar-button-active");
            }
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
