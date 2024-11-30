using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace AF.Tests
{
    public class QuestUtils_Tests
    {
        // Shared sceneTeleports array for all tests
        private SceneTeleport[] sceneTeleports;
        SceneTeleport map1, map2, map3, map4;

        QuestParent fetchApplesInMap4;

        // SetUp method to initialize the teleports for all test cases
        [SetUp]
        public void SetUp()
        {
            map1 = ScriptableObject.CreateInstance<SceneTeleport>();

            var Map1 = ScriptableObject.CreateInstance<Location>();
            var Map2 = ScriptableObject.CreateInstance<Location>();
            var Map3 = ScriptableObject.CreateInstance<Location>();
            var Map4 = ScriptableObject.CreateInstance<Location>();

            map1.name = "Map1";
            map1.teleports = new AYellowpaper.SerializedCollections.SerializedDictionary<Location, string>()
            {
                {Map2, "A"},
                {Map3, "A"},
            };

            map2 = ScriptableObject.CreateInstance<SceneTeleport>();
            map2.name = "Map2";
            map2.teleports = new AYellowpaper.SerializedCollections.SerializedDictionary<Location, string>()
            {
                {Map1, "A"},
                {Map3, "B"},
            };

            map3 = ScriptableObject.CreateInstance<SceneTeleport>();
            map3.name = "Map3";
            map3.teleports = new AYellowpaper.SerializedCollections.SerializedDictionary<Location, string>()
            {
                {Map1, "B"},
                {Map2, "B"},
                {Map4, "A"},
            };

            map4 = ScriptableObject.CreateInstance<SceneTeleport>();
            map4.name = "Map4";
            map4.teleports = new AYellowpaper.SerializedCollections.SerializedDictionary<Location, string>()
            {
                {Map3, "C"},
            };

            sceneTeleports = new SceneTeleport[] { map1, map2, map3, map4 };


            fetchApplesInMap4 = ScriptableObject.CreateInstance<QuestParent>();
            fetchApplesInMap4.questProgress = 0;

            QuestObjective questObjective1 = new()
            {
                sceneLocation = "Map4"
            };

            QuestObjective questObjective2 = new()
            {
                sceneLocation = "Map1"
            };

            fetchApplesInMap4.questObjectives = new[] { questObjective1, questObjective2 };

        }

        [Test]
        public void FromMap1_FindsTheShortestPathToObjective()
        {
            // Act
            var result = QuestUtils.FindPathToObjective(
                "Map1",
                "Map4",
                sceneTeleports
            );

            // Assert
            Assert.AreEqual(result, new List<SceneTeleport>() { map1, map3, map4 });
        }

        [Test]
        public void FromMap2_FindsTheShortestPathToObjective()
        {
            // Act
            var result = QuestUtils.FindPathToObjective(
                "Map2",
                "Map4",
                sceneTeleports
            );

            // Assert
            Assert.AreEqual(result, new List<SceneTeleport>() { map2, map3, map4 });
        }

        [Test]
        public void FromMap3_FindsTheShortestPathToObjective()
        {
            // Act
            var result = QuestUtils.FindPathToObjective(
                "Map3",
                "Map4",
                sceneTeleports
            );

            // Assert
            Assert.AreEqual(result, new List<SceneTeleport>() { map3, map4 });
        }

        [Test]
        public void FromMap1_ReturnMap3_ToFetchApplesObjective()
        {
            fetchApplesInMap4.questProgress = 0;

            SceneTeleport closestTeleportToCurrentScene = QuestUtils.GetClosestTeleportTowardsObjective(
               fetchApplesInMap4,
               "Map1",
               sceneTeleports
           );

            Assert.AreEqual(closestTeleportToCurrentScene, map3);
        }

        [Test]
        public void FromMap2_ReturnMap3_ToFetchApplesObjective()
        {
            fetchApplesInMap4.questProgress = 0;

            SceneTeleport closestTeleportToCurrentScene = QuestUtils.GetClosestTeleportTowardsObjective(
               fetchApplesInMap4,
               "Map2",
               sceneTeleports
           );

            Assert.AreEqual(closestTeleportToCurrentScene, map3);
        }

        [Test]
        public void FromMap3_ReturnMap4_ToFetchApplesObjective()
        {
            fetchApplesInMap4.questProgress = 0;

            SceneTeleport closestTeleportToCurrentScene = QuestUtils.GetClosestTeleportTowardsObjective(
               fetchApplesInMap4,
               "Map3",
               sceneTeleports
           );

            Assert.AreEqual(closestTeleportToCurrentScene, map4);
        }

        [Test]
        public void FromMap4_ReturnMap4_ToFetchApplesObjective()
        {
            fetchApplesInMap4.questProgress = 0;

            SceneTeleport closestTeleportToCurrentScene = QuestUtils.GetClosestTeleportTowardsObjective(
               fetchApplesInMap4,
               "Map4",
               sceneTeleports
           );

            Assert.AreEqual(closestTeleportToCurrentScene, map4);
        }

        [Test]
        public void FromMap4_ReturnMap3_ToReturnObjective()
        {
            fetchApplesInMap4.questProgress = 1;

            SceneTeleport closestTeleportToCurrentScene = QuestUtils.GetClosestTeleportTowardsObjective(
               fetchApplesInMap4,
               "Map4",
               sceneTeleports
           );

            Assert.AreEqual(closestTeleportToCurrentScene, map3);
        }

        [Test]
        public void FromMap3_ReturnMap1_ToReturnObjective()
        {
            fetchApplesInMap4.questProgress = 1;

            SceneTeleport closestTeleportToCurrentScene = QuestUtils.GetClosestTeleportTowardsObjective(
               fetchApplesInMap4,
               "Map3",
               sceneTeleports
           );

            Assert.AreEqual(closestTeleportToCurrentScene, map1);
        }

        [Test]
        public void FromMap2_ReturnMap1_ToReturnObjective()
        {
            fetchApplesInMap4.questProgress = 1;

            SceneTeleport closestTeleportToCurrentScene = QuestUtils.GetClosestTeleportTowardsObjective(
               fetchApplesInMap4,
               "Map2",
               sceneTeleports
           );

            Assert.AreEqual(closestTeleportToCurrentScene, map1);
        }
    }
}
