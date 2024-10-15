using System;
using AF.Events;
using GameAnalyticsSDK;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [System.Serializable]
    public class QuestObjective
    {
        public LocalizedString description;

        public string sceneLocation;
    }

    [CreateAssetMenu(menuName = "Data / New Quest")]

    public class QuestParent : ScriptableObject
    {
        [Header("Quest")]
        public LocalizedString questName;
        public Texture questIcon;
        public int questProgress = -1;

        [Header("Objectives")]
        public QuestObjective[] questObjectives;

        [Header("Databases")]
        public QuestsDatabase questsDatabase;

        [Header("Testing")]
        public bool useDefaultQuestProgress = false;
        public int defaultQuestProgress = 0;

#if UNITY_EDITOR 

        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Clear();
            }
        }
#endif

        public void Clear()
        {
            questProgress = useDefaultQuestProgress ? defaultQuestProgress : -1;
        }

        public bool IsCompleted()
        {
            return questProgress + 1 > questObjectives.Length;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="progress"></param>
        public void SetProgress(int progress)
        {
            if (!questsDatabase.ContainsQuest(this) && progress != -1)
            {
                questsDatabase.AddQuest(this);
            }

            questProgress = progress;

            EventManager.EmitEvent(EventMessages.ON_QUESTS_PROGRESS_CHANGED);

            if (questObjectives != null && questProgress >= 0 && questProgress < questObjectives.Length)
            {
                QuestObjective questObjective = questObjectives[questProgress];

                if (questObjective != null && questObjective.description != null)
                {
                    LogAnalytic(AnalyticsUtils.OnQuestProgressed(name, questObjective.description.GetLocalizedString()));
                }
            }
        }

        public void SetProgressIfHigher(int progress)
        {
            if (questProgress >= progress)
            {
                return;
            }

            SetProgress(progress);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void Track()
        {
            questsDatabase.SetQuestToTrack(this);
        }

        public bool IsObjectiveCompleted(QuestObjective questObjective)
        {
            return questProgress > Array.IndexOf(questObjectives, questObjective);
        }


        void LogAnalytic(string eventName)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName);
        }
    }
}
