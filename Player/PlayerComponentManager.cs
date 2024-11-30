namespace AF
{
    using UnityEngine;

    public class PlayerComponentManager : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        public bool isInTutorial = false;
        public bool isInBonfire = false;

        public bool regainControlOnResetState = false;

        // Cache
        int nothingLayer;
        int enemyLayer;

        public bool isParalysed = false;

        private void Start()
        {
            nothingLayer = LayerMask.GetMask("Nothing");
            enemyLayer = LayerMask.GetMask("Enemy");
        }

        public bool PlayerMovementIsEnabled() => playerManager.thirdPersonController.enabled;
        public bool IsBusy() => isInTutorial || isInBonfire;

        public void ResetStates()
        {
            if (regainControlOnResetState)
            {
                regainControlOnResetState = false;

                EnablePlayerControl();
            }

            EnableCollisionWithEnemies();
        }

        public void EnableComponents()
        {
            if (isParalysed)
            {
                return;
            }

            playerManager.thirdPersonController.enabled = true;
            playerManager.thirdPersonController.canRotateCharacter = true;
            playerManager.thirdPersonController.canMove = true;
            playerManager.playerCombatController.enabled = true;
            playerManager.dodgeController.enabled = true;
            playerManager.playerBlockInput.enabled = true;
            playerManager.thirdPersonController.SetTrackFallDamage(true);
            playerManager.eventNavigator.SetCanInteract(true);
        }

        public void DisableComponents()
        {
            playerManager.thirdPersonController.StopMovement();
            playerManager.thirdPersonController.canMove = false;
            playerManager.thirdPersonController.canRotateCharacter = false;
            playerManager.playerCombatController.enabled = false;
            playerManager.dodgeController.enabled = false;
            playerManager.playerBlockInput.enabled = false;
            playerManager.thirdPersonController.SetTrackFallDamage(false);
            playerManager.eventNavigator.SetCanInteract(false);
        }

        public void DisableCharacterController()
        {
            playerManager.characterController.enabled = false;
        }

        public void EnableCharacterController()
        {
            playerManager.characterController.enabled = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisablePlayerControl()
        {
            DisableCharacterController();
            DisableComponents();
        }

        public void DisablePlayerControlAndRegainControlAfterResetStates()
        {
            DisableCharacterController();
            DisableComponents();
            regainControlOnResetState = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void EnablePlayerControl()
        {
            EnableCharacterController();
            EnableComponents();
        }

        public void CurePlayer()
        {
            playerManager.health.RestoreFullHealth();
            playerManager.staminaStatManager.RestoreStaminaPercentage(100);
        }

        public void UpdatePosition(Vector3 newPosition, Quaternion newRotation)
        {
            // Store the initial state of fall damage tracking
            bool originalTrackFallDamage = playerManager.thirdPersonController.GetTrackFallDamage();

            // Disable fall damage tracking temporarily
            playerManager.thirdPersonController.SetTrackFallDamage(false);

            // Disable character controller to avoid unintended collisions during position update
            DisableCharacterController();

            playerManager.characterController.transform.SetPositionAndRotation(newPosition, newRotation);
            EnableCharacterController();

            // Restore original fall damage tracking state
            playerManager.thirdPersonController.SetTrackFallDamage(originalTrackFallDamage);
        }

        public void EnableCollisionWithEnemies()
        {
            if (playerManager.characterController.excludeLayers != nothingLayer)
            {
                playerManager.characterController.excludeLayers = nothingLayer;
            }
        }

        public void DisableCollisionWithEnemies()
        {
            playerManager.characterController.excludeLayers = enemyLayer;
        }

        public void TeleportPlayer(Transform target)
        {
            UpdatePosition(target.TransformPoint(Vector3.zero), target.rotation);
        }

        public void SetIsParalysed(bool value)
        {
            isParalysed = value;

            if (!value)
            {
                EnablePlayerControl();
            }
            else
            {
                LockPlayerControl();
            }
        }

        public void LockPlayerControl()
        {
            DisableComponents();
            playerManager.thirdPersonController.enabled = false;
        }

        public void FaceObject(GameObject gameObject)
        {
            Vector3 desiredRot = gameObject.transform.position - playerManager.characterController.transform.position;
            desiredRot.y = 0;
            playerManager.characterController.transform.rotation = Quaternion.LookRotation(desiredRot);
        }
    }
}
