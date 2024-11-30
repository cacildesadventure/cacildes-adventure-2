namespace AF
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using DG.Tweening;
    using AF.UI;

    public class UIDocumentKeyPrompt : MonoBehaviour
    {
        public UIDocument uiDocument => GetComponent<UIDocument>();

        [Header("Components")]
        public Soundbank soundbank;
        public StarterAssetsInputs starterAssetsInputs;
        public ActionButton interactButton;

        VisualElement root;
        VisualElement actionButtonContainer;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void DisplayPrompt(string action)
        {
            this.gameObject.SetActive(true);

            root = uiDocument.rootVisualElement;
            actionButtonContainer = root.Q<VisualElement>("ActionButtonContainer");

            // Clean up action button in case gamepad / keyboard input changes
            actionButtonContainer.Clear();
            var resultKey = interactButton.GetKey(starterAssetsInputs);
            actionButtonContainer.Add(resultKey);

            root.Q<Label>("InputText").text = action;

            soundbank.PlaySound(soundbank.uiHover);

            DOTween.To(
                  () => root.contentContainer.style.opacity.value,
                  (value) => root.contentContainer.style.opacity = value,
                  1,
                  .25f
            );
        }
    }
}
