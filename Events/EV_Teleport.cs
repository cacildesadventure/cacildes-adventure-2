using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_Teleport : EventBase
    {
        public SceneTeleport currentSceneTeleports;
        public string destinationSceneName;

        // Scene Refs
        TeleportManager teleportManager;

        public override IEnumerator Dispatch()
        {
            yield return null;
            Teleport();
        }

        public void Teleport()
        {
            if (currentSceneTeleports.teleports.ContainsKey(destinationSceneName))
            {
                GetTeleportManager().Teleport(destinationSceneName, currentSceneTeleports.teleports[destinationSceneName]);
            }
        }

        TeleportManager GetTeleportManager()
        {
            if (teleportManager == null)
            {
                teleportManager = FindAnyObjectByType<TeleportManager>(FindObjectsInactive.Include);
            }

            return teleportManager;
        }
    }
}
