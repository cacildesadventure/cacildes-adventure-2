
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
    }
}
