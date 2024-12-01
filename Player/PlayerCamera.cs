
using Unity.Cinemachine;
using UnityEngine;

namespace AF
{
    public class PlayerCamera : MonoBehaviour
    {
        private CinemachineCamera cinemachineVirtualCamera;
        public GameSettings gameSettings;


        void Start()
        {
            cinemachineVirtualCamera = GetComponent<CinemachineCamera>();

            UpdateCameraDistance();
        }

        public void UpdateCameraDistance()
        {
            cinemachineVirtualCamera.GetComponent<CinemachineThirdPersonFollow>().CameraDistance = gameSettings.cameraDistance;
        }

        public void UpdateZoom(float value)
        {
            gameSettings.cameraDistance = Mathf.Clamp(value, gameSettings.minimumCameraDistance, gameSettings.maximumCameraDistance);
            UpdateCameraDistance();
        }

        public void ZoomIn(float scrollDelta)
        {
            UpdateZoom(gameSettings.cameraDistance + scrollDelta * gameSettings.zoomSpeed);
        }

        public void ZoomOut(float scrollDelta)
        {
            UpdateZoom(gameSettings.cameraDistance - scrollDelta * gameSettings.zoomSpeed);
        }
    }
}
