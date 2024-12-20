
namespace AF.UI
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.DualShock;
    using UnityEngine.InputSystem.Switch;
    using UnityEngine.InputSystem.XInput;
    using UnityEngine.Localization;
    using UnityEngine.UIElements;

    public enum ControlScheme
    {
        Keyboard,
        PS4,
        Xbox,
    }

    [CreateAssetMenu(fileName = "Action Button", menuName = "Data/New Action Button", order = 1)]
    public class ActionButton : ScriptableObject
    {
        [Tooltip("Name of the input action, must match exactly with what is defined in the Starter Assets Inputs (e.g., Jump, Attack, etc.)")]
        public string actionName;
        public string overrideKeyName;

        // Sprites for each control scheme
        public Sprite keyboardSprite;
        public Sprite ps4Sprite;
        public Sprite xboxSprite;
        public Sprite switchSprite;

        [Header("Visuals")]
        public bool useColor = true;
        public Color keyboardBackgroundColor;
        public Font font;

        [Header("Label")]
        public LocalizedString description;

        // Main method that coordinates the creation of the key VisualElement
        public VisualElement GetKey(StarterAssetsInputs starterAssetsInputs)
        {
            VisualElement container = CreateContainer();
            VisualElement buttonElement = CreateButtonElement();

            if (Gamepad.current == null)
            {
                VisualElement keyboardElement = CreateKeyboardElement(starterAssetsInputs);
                buttonElement.Add(keyboardElement);
            }
            else
            {
                VisualElement gamepadElement = CreateGamepadElement(GetCurrentGamepadSprite());
                buttonElement.Add(gamepadElement);
            }

            container.Add(buttonElement);

            if (!description.IsEmpty)
            {
                Label descriptionLabel = CreateDescriptionLabel();
                container.Add(descriptionLabel);
            }

            return container;
        }

        // Creates the root container element
        private VisualElement CreateContainer()
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.flexShrink = 0;
            container.style.marginRight = 5;
            return container;
        }

        // Creates the button element container
        private VisualElement CreateButtonElement()
        {
            VisualElement buttonElement = new VisualElement();
            buttonElement.style.flexDirection = FlexDirection.Row;
            buttonElement.style.alignItems = Align.Center;
            buttonElement.style.flexShrink = 0;
            return buttonElement;
        }

        // Creates the keyboard background and label
        private VisualElement CreateKeyboardElement(StarterAssetsInputs starterAssetsInputs)
        {
            VisualElement background = new VisualElement();
            background.style.backgroundImage = new StyleBackground(keyboardSprite);
            background.style.minWidth = 24;
            background.style.height = 24;
            background.style.unityBackgroundImageTintColor = keyboardBackgroundColor;

            string rebindedKey = starterAssetsInputs != null ? starterAssetsInputs.GetCurrentKeyBindingForAction(actionName) : "";
            Label keyLabel = new Label(rebindedKey);
            keyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            keyLabel.style.fontSize = 20;
            keyLabel.style.color = Color.white;
            keyLabel.style.unityFontDefinition = new StyleFontDefinition(font);
            keyLabel.style.marginTop = 0;
            keyLabel.style.marginRight = 0;
            keyLabel.style.marginBottom = 0;
            keyLabel.style.marginLeft = 0;

            if (!string.IsNullOrEmpty(overrideKeyName))
            {
                keyLabel.text = overrideKeyName;
            }

            background.Add(keyLabel);
            return background;
        }

        // Creates the gamepad icon element based on the detected gamepad
        private VisualElement CreateGamepadElement(Sprite sprite)
        {
            VisualElement gamepadIcon = new VisualElement();
            gamepadIcon.style.backgroundImage = new StyleBackground(sprite);
            gamepadIcon.style.width = 24;
            gamepadIcon.style.height = 24;

            if (useColor)
            {
                gamepadIcon.style.unityBackgroundImageTintColor = keyboardBackgroundColor;
            }

            return gamepadIcon;
        }

        // Detects the current gamepad and returns the appropriate sprite
        private Sprite GetCurrentGamepadSprite()
        {
            if (Gamepad.current is DualShockGamepad)
            {
                return ps4Sprite;
            }
            else if (Gamepad.current is XInputController)
            {
                return xboxSprite;
            }
            else if (Gamepad.current is SwitchProControllerHID)
            {
                return switchSprite;
            }

            return null;  // Default sprite if no specific gamepad detected
        }

        // Creates the description label if needed
        private Label CreateDescriptionLabel()
        {
            Label descriptionLabel = new Label();
            descriptionLabel.name = "Description";
            descriptionLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            descriptionLabel.style.fontSize = 20;
            descriptionLabel.style.color = Color.white;
            descriptionLabel.style.unityFontDefinition = new StyleFontDefinition(font);
            descriptionLabel.text = description.GetLocalizedString();
            descriptionLabel.style.flexShrink = 0;

            // Applying text shadow
            descriptionLabel.style.textShadow = new TextShadow
            {
                offset = new Vector2(2, 2),      // Offset for the shadow (X and Y)
                color = new Color(0, 0, 0, 0.5f), // Shadow color (black with 50% opacity)
                blurRadius = 1                   // Optional: blur radius for the shadow
            };

            return descriptionLabel;
        }

    }

}
