using System.Collections.Generic;
using System.Linq;
using AF.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class ViewQuestsMenu : ViewMenu
    {
        [Header("Prefabs")]
        public VisualTreeAsset questPrefabButton;
        public VisualTreeAsset questObjectivePrefab;

        [Header("Quests")]
        public QuestsDatabase questsDatabase;
        [Header("Footer")]
        public MenuFooter menuFooter;
        public ActionButton trackQuestButton, exitMenuButton;

        ScrollView questsScrollView;

        VisualElement questPreview, questIcon, questObjectivesContainer;
        Label questTitle;

        int elementToFocusIndex = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            SetupRefs();

            menuFooter.SetupReferences();
            SetupFooterActions();
            RedrawUI();
        }

        void SetupRefs()
        {
            questsScrollView = root.Q<VisualElement>("QuestlogContainer").Q<ScrollView>();
            questPreview = root.Q<VisualElement>("QuestPreview");
            questPreview.style.opacity = 0;

            questIcon = questPreview.Q<VisualElement>("QuestIcon");
            questTitle = questPreview.Q<Label>("QuestTitle");
            questObjectivesContainer = questPreview.Q<VisualElement>("QuestObjectivesContainer");
            questObjectivesContainer.Clear();
        }

        void RedrawUI()
        {
            DrawQuestsMenu();
        }

        void DrawQuestsMenu()
        {
            questsScrollView.Clear();

            List<QuestParent> questParentsReversed = questsDatabase.questsReceived.ToList();
            questParentsReversed.Reverse();

            for (int i = 0; i < questsDatabase.questsReceived.Count; i++)
            {
                var quest = questParentsReversed[i];

                VisualElement clone = questPrefabButton.CloneTree();
                clone.Q<Label>("QuestName").text = quest.questName.IsEmpty ? quest.name : quest.questName.GetLocalizedString();
                clone.Q<VisualElement>("TrackIcon").style.display = questsDatabase.IsQuestTracked(quest) ? DisplayStyle.Flex : DisplayStyle.None;

                int index = i; // Store the current value of 'i' in a separate variable to avoid closure issues

                UIUtils.SetupButton(
                    clone.Q<Button>("QuestButton"),
                    () =>
                    {
                        questsDatabase.SetQuestToTrack(quest);
                        elementToFocusIndex = index;
                        RedrawUI();
                    },
                    () =>
                    {
                        PreviewQuest(quest);
                        questsScrollView.ScrollTo(clone.Q<Button>("QuestButton"));
                    },
                    () =>
                    {
                        questPreview.style.opacity = 0;
                    },
                    true,
                    soundbank
                );

                if (quest.IsCompleted())
                {
                    clone.style.opacity = 0.5f;
                }

                questsScrollView.Add(clone);
            }

            if (questsScrollView.childCount > 0 && questsScrollView.childCount >= elementToFocusIndex && questsScrollView.ElementAt(elementToFocusIndex) != null)
            {
                var btn = questsScrollView.ElementAt(elementToFocusIndex).Q<Button>("QuestButton");
                btn.Focus();
                questsScrollView.ScrollTo(btn);
            }
        }

        void PreviewQuest(QuestParent questParent)
        {
            questIcon.style.backgroundImage = new StyleBackground(questParent.questIcon as Texture2D);
            questTitle.text = questParent.questName.IsEmpty ? questParent.name : questParent.questName.GetLocalizedString();

            questObjectivesContainer.Clear();

            int idx = 0;
            foreach (var questObjective in questParent.questObjectives)
            {
                var questObjectiveEntry = questObjectivePrefab.CloneTree();
                questObjectiveEntry.Q<Label>("QuestObjectiveLabel").text = questParent.questObjectives[idx].description.IsEmpty ? "" : questParent.questObjectives[idx].description.GetLocalizedString();
                questObjectiveEntry.Q<Label>("QuestObjectiveLocation").text = "";

                bool isCompleted = questParent.IsObjectiveCompleted(questObjective);

                questObjectiveEntry.Q<VisualElement>("QuestObjectiveComplete").style.display = isCompleted ? DisplayStyle.Flex : DisplayStyle.None;
                questObjectiveEntry.Q<VisualElement>("QuestObjectiveIncomplete").style.display = !isCompleted ? DisplayStyle.Flex : DisplayStyle.None;

                questObjectiveEntry.style.opacity = 1;

                if (idx == questParent.questProgress)
                {
                    questObjectiveEntry.Q<Label>("QuestObjectiveLabel").style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
                }

                if (!isCompleted && idx > questParent.questProgress)
                {
                    questObjectiveEntry.style.opacity = 0.15f;
                }

                questObjectivesContainer.Add(questObjectiveEntry);

                idx++;
            }

            questObjectivesContainer.style.opacity = 1;
            questPreview.style.opacity = 1;
        }

        void SetupFooterActions()
        {
            menuFooter.GetFooterActionsContainer().Add(trackQuestButton.GetKey(starterAssetsInputs));
            menuFooter.GetFooterActionsContainer().Add(exitMenuButton.GetKey(starterAssetsInputs));
        }
    }
}
