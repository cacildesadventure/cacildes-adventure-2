namespace AF
{
    using AYellowpaper.SerializedCollections;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Scene Teleports", menuName = "Data / New Scene Teleports")]
    public class SceneTeleport : ScriptableObject
    {
        [SerializedDictionaryAttribute("Destination Scene", "Spawn Gameobject Name Reference")]
        public SerializedDictionary<Location, string> teleports;

    }
}
