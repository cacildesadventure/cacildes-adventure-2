namespace AF
{
    using System.Collections.Generic;
    using System.Linq;

    public static class QuestUtils
    {
        public static SceneTeleport GetClosestTeleportTowardsObjective(QuestParent trackedQuest, string currentScene, SceneTeleport[] sceneTeleports)
        {
            QuestObjective currentQuestObjective = trackedQuest.questObjectives[trackedQuest.questProgress];

            if (currentQuestObjective == null)
            {
                return null;
            }

            List<SceneTeleport> pathToObjective = FindPathToObjective(currentScene, currentQuestObjective.sceneLocation, sceneTeleports);

            if (pathToObjective.Count > 1)
            {
                // Return the next teleport after the current scene
                return pathToObjective[1];
            }
            else if (pathToObjective.Count == 1)
            {
                // If only one teleport is found, return it
                return pathToObjective[0];
            }

            return null;
        }

        public static List<SceneTeleport> FindPathToObjective(
            string startScene,
            string targetScene,
            SceneTeleport[] sceneTeleports
        )
        {
            Dictionary<string, bool> visitedScenes = new();
            Queue<string> sceneQueue = new();
            Dictionary<string, string> previousScene = new();

            // Start searching with the current scene
            sceneQueue.Enqueue(startScene);
            previousScene[startScene] = null;
            visitedScenes[startScene] = true; // Mark the starting scene as visited

            while (sceneQueue.Count > 0)
            {
                string currentScene = sceneQueue.Dequeue();

                // If we reach the current scene, build the path
                if (currentScene == targetScene)
                {
                    return BuildPathToObjective(previousScene, startScene, targetScene, sceneTeleports);
                }

                var currentSceneInstance = sceneTeleports.Where(sceneTeleport => sceneTeleport.name == currentScene).FirstOrDefault();
                if (currentSceneInstance == null)
                {
                    continue;
                }

                foreach (KeyValuePair<Location, string> teleport in currentSceneInstance.teleports)
                {
                    string nextDestination = teleport.Key.name;

                    // If visited scene already, skip
                    if (visitedScenes.ContainsKey(nextDestination))
                    {
                        continue;
                    }

                    // Assign the origin of the next destination to this current scene
                    previousScene[nextDestination] = currentScene;
                    sceneQueue.Enqueue(nextDestination);
                    visitedScenes[nextDestination] = true;
                }
            }


            return new();
        }

        public static List<SceneTeleport> BuildPathToObjective(
            Dictionary<string, string> previousScene,
            string startScene,
            string targetScene,
            SceneTeleport[] sceneTeleports
        )
        {
            List<SceneTeleport> path = new() { sceneTeleports.FirstOrDefault(
                    sceneTeleport => sceneTeleport.name == targetScene) };

            string currentScene = targetScene;

            while (!string.IsNullOrEmpty(currentScene) && currentScene != startScene)
            {
                string fromScene = previousScene[currentScene];

                SceneTeleport match = sceneTeleports.FirstOrDefault(
                    sceneTeleport =>
                        sceneTeleport.name == fromScene
                        && sceneTeleport.teleports.Any(entry => entry.Key.name == currentScene));

                if (match != null)
                {
                    path.Insert(0, match);
                }

                currentScene = fromScene;
            }

            return path;
        }
    }
}
