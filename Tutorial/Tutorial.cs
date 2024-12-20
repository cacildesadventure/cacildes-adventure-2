
namespace AF
{
    using System;
    using System.Collections.Generic;
    using AF.UI;
    using UnityEngine;
    using UnityEngine.Localization;
    using UnityEngine.UIElements;

    [CreateAssetMenu(fileName = "New Tutorial", menuName = "Data/New Tutorial", order = 0)]
    public class Tutorial : ScriptableObject
    {
        [System.Serializable]
        public class TutorialStep
        {
            public LocalizedString objectives;

            public ActionButton[] actionButtons;

            public TutorialEventMessage completionEvent;
        }

        [System.Serializable]
        public class RenderingElement
        {
            public string text;
            public ActionButton button;
        }

        [Header("Components")]
        public TutorialManager tutorialManager;

        [Header("Data")]
        public LocalizedString title;
        public LocalizedString description;
        public TutorialStep[] tutorialSteps;

        public VisualElement GetRenderedObjective(TutorialStep tutorialStep, int currentStep, StarterAssetsInputs starterAssetsInputs)
        {
            bool isComplete = currentStep == -1 || currentStep > Array.IndexOf(tutorialSteps, tutorialStep);

            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;

            VisualElement checkIcon = new VisualElement();
            checkIcon.style.width = 15;
            checkIcon.style.height = 15;
            checkIcon.style.unityBackgroundImageTintColor = isComplete ? tutorialManager.goldColor : Color.white;
            checkIcon.style.backgroundImage = new StyleBackground(isComplete ? tutorialManager.checkIcon : tutorialManager.uncheckIcon);

            row.Add(checkIcon);

            string word = tutorialStep.objectives.GetLocalizedString();

            List<RenderingElement> renderingElements = TutorialUtils.GetRenderingElements(tutorialStep, word);

            foreach (var renderingElement in renderingElements)
            {
                if (renderingElement.button != null)
                {
                    VisualElement key = renderingElement.button.GetKey(starterAssetsInputs);
                    key.Q<Label>("Description").style.display = DisplayStyle.None;
                    key.style.marginLeft = 5;
                    row.Add(key);
                }
                else
                {
                    Label label = new();
                    label.style.unityTextAlign = TextAnchor.MiddleCenter;
                    label.style.fontSize = 20;
                    label.style.color = isComplete ? tutorialManager.goldColor : Color.white;
                    label.style.unityFontDefinition = new StyleFontDefinition(tutorialManager.font);
                    label.style.marginTop = 0;
                    label.style.marginRight = 0;
                    label.style.marginBottom = 0;
                    label.style.marginLeft = 0;
                    label.text = renderingElement.text;

                    row.Add(label);
                }
            }

            return row;
        }

    }
}
