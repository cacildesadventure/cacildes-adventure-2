namespace AF
{
    using UnityEngine;

    public class PushObjectManager : MonoBehaviour
    {
        public PlayerManager playerManager;

        PushableObject objectToPush;

        public void StartPush(PushableObject objectToPush)
        {
            this.objectToPush = objectToPush;

            Vector3 lookDir = objectToPush.transform.position - playerManager.transform.position;
            lookDir.y = 0;

            playerManager.transform.rotation = Quaternion.LookRotation(lookDir);

            playerManager.PlayBusyAnimationWithRootMotion("Push Object");
        }

        public void OnPushObject()
        {
            if (objectToPush != null)
            {
                objectToPush.OnPush(playerManager.transform.position);
            }
        }


    }
}