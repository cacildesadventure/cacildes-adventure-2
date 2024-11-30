
namespace AF
{
    using AF.Events;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class QuestHUD : MonoBehaviour
    {
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        VisualElement root;

        VisualElement currentObjectiveContainer;
        Label currentObjectiveValue;

        public QuestsDatabase questsDatabase;


        private void Awake()
        {
            SetupRefs();

            EventManager.StartListening(
                EventMessages.ON_QUEST_TRACKED,
                () => UpdateUI(true));

            EventManager.StartListening(
                EventMessages.ON_QUESTS_PROGRESS_CHANGED,
                () => UpdateUI(true));

        }

        void OnEnable()
        {
            UpdateUI(false);
        }

        private void SetupRefs()
        {
            this.root = uIDocumentPlayerHUDV2.uIDocument.rootVisualElement;

            currentObjectiveContainer = root.Q<VisualElement>("ObjectiveContainer");
            currentObjectiveValue = root.Q<Label>("CurrentObjectiveValue");

            currentObjectiveValue.text = "";
        }


        void UpdateUI(bool playPopupAnimation)
        {
            currentObjectiveValue.text = "";

            var currentQuestObjective = questsDatabase.GetCurrentTrackedQuestObjective();

            if (currentQuestObjective != null && !currentQuestObjective.description.IsEmpty)
            {
                currentObjectiveContainer.Q<VisualElement>("QuestObjectiveIcon").style.display = DisplayStyle.None; //.backgroundImage = new StyleBackground((Texture2D)questsDatabase.GetTrackedQuest().questIcon);
                currentObjectiveValue.text = currentQuestObjective.description.GetLocalizedString();
                currentObjectiveContainer.style.display = DisplayStyle.Flex;

                if (playPopupAnimation)
                {
                    UIUtils.PlayPopAnimation(currentObjectiveValue);
                }
            }
            else
            {
                currentObjectiveContainer.style.display = DisplayStyle.None;
            }
        }


    }
}
