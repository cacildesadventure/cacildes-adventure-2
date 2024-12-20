namespace AF
{
    using System;
    using System.Collections;
    using AF.Events;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentTutorial : MonoBehaviour
    {
        public UIDocument uIDocument;

        VisualElement root;
        VisualElement stepsContent;

        public TutorialManager tutorialManager;

        [Header("Components")]
        public StarterAssetsInputs starterAssetsInputs;

        public Soundbank soundbank;

        private void Awake()
        {
            root = uIDocument.rootVisualElement;
            stepsContent = root.Q<VisualElement>("StepsContent");

            root.style.display = DisplayStyle.None;

            EventManager.StartListening(EventMessages.ON_TUTORIAL_SET, () =>
            {
                ShowGUI();
            });

        }

        void ShowGUI()
        {
            root.style.display = DisplayStyle.Flex;

            UIUtils.PlayFadeInAnimation(root.Q<VisualElement>("InfoContainer"), .5f);

            DrawTutorial(tutorialManager.currentTutorial);
        }

        void DrawTutorial(Tutorial currentTutorial)
        {
            stepsContent.Clear();

            VisualElement titleContainer = root.Q<VisualElement>("Title");
            titleContainer.Q<Label>("TitleLabel").text = currentTutorial.title.GetLocalizedString();

            int currentIndex = Array.IndexOf(currentTutorial.tutorialSteps, tutorialManager.currentStep);
            bool isComplete = currentIndex == -1 || currentIndex >= currentTutorial.tutorialSteps.Length;

            titleContainer.Q<Label>("TitleLabel").style.color = isComplete ? tutorialManager.goldColor : Color.white;

            titleContainer.Q<VisualElement>("UncompleteCheck").style.display = isComplete ? DisplayStyle.None : DisplayStyle.Flex;
            titleContainer.Q<VisualElement>("CompleteCheck").style.display = isComplete ? DisplayStyle.Flex : DisplayStyle.None;

            root.Q<VisualElement>("Description").Q<Label>().text = currentTutorial.description.GetLocalizedString();

            foreach (var tutorialStep in currentTutorial.tutorialSteps)
            {
                VisualElement row = currentTutorial.GetRenderedObjective(
                    tutorialStep,
                    Array.IndexOf(currentTutorial.tutorialSteps, tutorialManager.currentStep),
                    starterAssetsInputs
                );

                stepsContent.Add(row);
            }
        }

        public void OnStepComplete(TutorialEventMessage stepCompleteMessage)
        {
            if (stepCompleteMessage == null)
            {
                return;
            }

            Tutorial currentTutorial = tutorialManager.currentTutorial;
            if (currentTutorial == null)
            {
                return;
            }

            int currentStepIndex = Array.IndexOf(tutorialManager.currentTutorial.tutorialSteps, tutorialManager.currentStep);

            if (tutorialManager.currentStep.completionEvent == stepCompleteMessage)
            {
                int nextIndex = currentStepIndex + 1;

                tutorialManager.currentStep = nextIndex < tutorialManager.currentTutorial.tutorialSteps.Length
                    ? tutorialManager.currentTutorial.tutorialSteps[nextIndex]
                    : null;

                if (tutorialManager.currentStep == null)
                {
                    DrawTutorial(tutorialManager.currentTutorial);

                    soundbank.PlaySound(soundbank.companionJoin);

                    // End tutorial
                    tutorialManager.EndTutorial();


                    StartCoroutine(CloseGUICoroutine());
                }
                else
                {
                    soundbank.PlaySound(soundbank.craftSuccess);
                }
            }
        }

        IEnumerator CloseGUICoroutine()
        {
            yield return new WaitForSeconds(.5f);

            UIUtils.PlayFadeOutAnimation(root.Q<VisualElement>("InfoContainer"), 1.5f);

            yield return new WaitForSeconds(1);
            HideGUI();
        }

        void HideGUI()
        {
            root.style.display = DisplayStyle.None;
        }
    }
}
