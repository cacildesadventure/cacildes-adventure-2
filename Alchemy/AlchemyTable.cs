namespace AF
{
    using UnityEngine;

    [RequireComponent(typeof(GenericTrigger))]
    public class AlchemyTable : MonoBehaviour
    {
        public GameObject alchemyCamera;
        public PlayerManager playerManager;

        public string alchemyAnimation = "Alchemy";

        public Transform alchemyStandRef;
        public Transform alchemyStandDirectionRef;

        public AlchemyMortarRef[] mortarRefs;

        public GameObject lightsContainer;
        [Header("Components")]
        public UIDocumentAlchemy uIDocumentAlchemy;

        GenericTrigger alchemyTrigger => GetComponent<GenericTrigger>();


        private void Awake()
        {
            uIDocumentAlchemy.onAlchemyEnd.AddListener(OnAlchemyEnd);
            enabled = false;

            mortarRefs = FindObjectsByType<AlchemyMortarRef>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        public void StartAlchemy()
        {
            playerManager.playerComponentManager.TeleportPlayer(alchemyStandRef);

            Vector3 lookDir = alchemyStandDirectionRef.transform.position - playerManager.transform.position;
            lookDir.y = 0;

            playerManager.transform.rotation = Quaternion.LookRotation(lookDir);
            playerManager.playerComponentManager.DisablePlayerControl();

            playerManager.PlayCrossFadeBusyAnimationWithRootMotion(alchemyAnimation, 0.5f);
            playerManager.playerWeaponsManager.HideEquipment();

            if (mortarRefs.Length > 0)
            {
                foreach (var mortarRef in mortarRefs)
                {
                    mortarRef.gameObject.SetActive(true);
                }
            }

            alchemyCamera.SetActive(true);

            lightsContainer.SetActive(true);
            enabled = true;
            alchemyTrigger.DisableCapturable();
        }

        public void OnAlchemyEnd()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            alchemyCamera.SetActive(false);
            playerManager.playerComponentManager.EnablePlayerControl();
            playerManager.playerWeaponsManager.ShowEquipment();

            if (mortarRefs.Length > 0)
            {
                foreach (var mortarRef in mortarRefs)
                {
                    mortarRef.gameObject.SetActive(false);
                }
            }

            playerManager.PlayCrossFadeBusyAnimationWithRootMotion("Idle Walk Run Blend", 0.5f);
            alchemyTrigger.TurnCapturable();
            lightsContainer.SetActive(false);
            enabled = false;
        }

    }
}
