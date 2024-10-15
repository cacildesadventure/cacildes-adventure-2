namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Events;
    using QuantumTek.QuantumTravel;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class QuestTrackManager : MonoBehaviour
    {
        [Header("Scene Components")]
        public QT_CompassBar qT_CompassBar;

        [Header("Databases")]
        public QuestsDatabase questsDatabase;

        [Header("Templates")]
        public QT_MapMarkerData trackedQuestMarker;

        private List<SceneTeleport> sceneTeleports = new();
        private QT_MapObject currentMapObject;

        private void Awake()
        {
            sceneTeleports = Resources.LoadAll<SceneTeleport>("Teleports").ToList();
            EventManager.StartListening(EventMessages.ON_QUEST_TRACKED, HandleTrackedQuest);
            EventManager.StartListening(EventMessages.ON_QUESTS_PROGRESS_CHANGED, HandleTrackedQuest);
            HandleTrackedQuest();
        }

        public void HandleTrackedQuest()
        {
            ClearQuestData();

            var currentQuestObjective = questsDatabase.GetCurrentTrackedQuestObjective();
            if (currentQuestObjective == null)
            {
                return;
            }

            var activeSceneName = SceneManager.GetActiveScene().name;
            GameObject match = currentQuestObjective.sceneLocation == activeSceneName
                ? FindObjectivePointMatch()
                : FindTeleportMatch(activeSceneName);

            if (match != null)
            {
                AddQuestMarkerToCompass(match);
            }
        }

        private GameObject FindObjectivePointMatch()
        {
            var objectivePoint = FindObjectsByType<ObjectivePoint>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .FirstOrDefault(HasFoundObjectivePoint);

            return objectivePoint?.gameObject;
        }

        private GameObject FindTeleportMatch(string activeSceneName)
        {
            var closestTeleport = QuestUtils.GetClosestTeleportTowardsObjective(
                questsDatabase.GetTrackedQuest(),
                activeSceneName,
                sceneTeleports.ToArray());

            if (closestTeleport == null) return null;

            var teleportMatch = FindObjectsByType<EV_Teleport>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .FirstOrDefault(evTeleport => evTeleport.destinationSceneName == closestTeleport.name);

            return teleportMatch?.gameObject;
        }

        private bool HasFoundObjectivePoint(ObjectivePoint objectivePoint)
        {
            if (questsDatabase.IsQuestTracked(objectivePoint.questParent) &&
                     questsDatabase.GetTrackedQuest().questProgress == objectivePoint.questProgress)
            {
                return true;
            }

            return false;
        }

        private void AddQuestMarkerToCompass(GameObject match)
        {
            var newQtMapObject = match.AddComponent<QT_MapObject>();
            if (newQtMapObject == null)
            {
                return;
            }

            currentMapObject = newQtMapObject;
            currentMapObject.Data = trackedQuestMarker;
            qT_CompassBar.AddMarker(currentMapObject);
        }

        public void ClearQuestData()
        {
            RemoveMarkerFromCompass(currentMapObject);
            currentMapObject = null;

            ClearExistingMarkers<EV_Teleport>();
            ClearExistingMarkers<ObjectivePoint>();
        }

        private void ClearExistingMarkers<T>() where T : MonoBehaviour
        {
            foreach (var obj in FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Select(item => item.GetComponent<QT_MapObject>())
                .Where(mapObj => mapObj != null))
            {
                Destroy(obj);
            }
        }

        public void RemoveMarkerFromCompass(QT_MapObject obj)
        {
            var match = qT_CompassBar.Markers.FirstOrDefault(x => x.Object == obj);
            if (match == null) return;

            qT_CompassBar.Markers.Remove(match);
            Destroy(match.gameObject);
        }
    }
}
