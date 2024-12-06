
namespace AF
{
    using AF.Events;
    using TigerForge;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Tutorial Manager", menuName = "System/New Tutorial Manager", order = 0)]
    public class TutorialManager : ScriptableObject
    {
        public Tutorial currentTutorial;
        public Tutorial.TutorialStep currentStep;

        [Header("Data")]
        public Sprite uncheckIcon;
        public Sprite checkIcon;

        public Color goldColor;
        public Font font;

#if UNITY_EDITOR
        private void OnEnable()
        {

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                currentTutorial = null;
                currentStep = null;
            }
        }
#endif
        public void SetTutorial(Tutorial tutorial)
        {
            if (tutorial == null || tutorial.tutorialSteps.Length == 0)
            {
                return;
            }

            this.currentTutorial = tutorial;
            this.currentStep = tutorial.tutorialSteps[0];

            EventManager.EmitEvent(EventMessages.ON_TUTORIAL_SET);
        }
        public void EndTutorial()
        {
            this.currentTutorial = null;
        }
    }
}
