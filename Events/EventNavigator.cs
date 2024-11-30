namespace AF
{
    using UnityEngine;

    public class EventNavigator : MonoBehaviour
    {
        [Header("Layer Options")]
        public LayerMask eventNavigatorCapturableLayer;

        [Header("Components")]
        public Transform playerTransform;
        public UIManager uiManager;
        public MomentManager momentManager;
        public GameSettings gameSettings;
        public UIDocumentKeyPrompt uIDocumentKeyPrompt;

        IEventNavigatorCapturable currentTarget;

        bool canInteract = true;

        public void SetCanInteract(bool value)
        {
            if (value)
            {
                canInteract = true;
            }
            else
            {
                canInteract = false;
                uIDocumentKeyPrompt.gameObject.SetActive(false);
            }
        }

        bool CanInteract()
        {
            if (uiManager.IsShowingFullScreenGUI() || momentManager.HasMomentOnGoing)
            {
                return false;
            }

            return canInteract;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnInteract()
        {
            if (!CanInteract())
            {
                return;
            }

            currentTarget?.OnInvoked();
        }

        private void Update()
        {
            if (!CanInteract())
            {
                return;
            }

            bool hitSomething = Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward, out var hitInfo, gameSettings.cameraDistance + 1, eventNavigatorCapturableLayer);

            if (hitSomething)
            {
                var eventNavigatorCapturable = hitInfo.collider.GetComponent<IEventNavigatorCapturable>();

                if (eventNavigatorCapturable != currentTarget)
                {
                    var hitPosition = hitInfo.transform.position;
                    hitPosition.y = playerTransform.transform.position.y;
                    Vector3 dist = hitPosition - playerTransform.transform.position;
                    float angle = Vector3.Angle(dist.normalized, playerTransform.transform.forward);

                    if (angle / 2 <= 50)
                    {
                        currentTarget = eventNavigatorCapturable;

                        if (uiManager.CanShowGUI() && momentManager.HasMomentOnGoing == false)
                        {
                            currentTarget?.OnCaptured();
                        }
                    }
                }
            }
            else
            {
                if (currentTarget != null)
                {
                    currentTarget?.OnReleased();
                }

                if (uIDocumentKeyPrompt.isActiveAndEnabled)
                {
                    uIDocumentKeyPrompt.gameObject.SetActive(false);
                }

                currentTarget = null;
            }
        }
    }
}
