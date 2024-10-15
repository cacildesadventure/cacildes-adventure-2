using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AF
{
    public class CameraQuality : MonoBehaviour
    {
        public GameSettings gameSettings;
        public new Camera camera;

        UniversalAdditionalCameraData universalAdditionalCameraData;

        private void Awake()
        {
            universalAdditionalCameraData = Camera.main.GetUniversalAdditionalCameraData();

            Evaluate();

            EventManager.StartListening(EventMessages.ON_GRAPHICS_QUALITY_CHANGED, Evaluate);
        }

        void Evaluate()
        {
            if (gameSettings.graphicsQuality == GameSettings.GraphicsQuality.LOW)
            {
                universalAdditionalCameraData.antialiasing = AntialiasingMode.None;
                universalAdditionalCameraData.renderPostProcessing = false;
                universalAdditionalCameraData.renderShadows = false;
            }
            else if (gameSettings.graphicsQuality == GameSettings.GraphicsQuality.MEDIUM)
            {
                universalAdditionalCameraData.antialiasing = AntialiasingMode.None;
                universalAdditionalCameraData.renderPostProcessing = true;
                universalAdditionalCameraData.renderShadows = true;
            }
            else if (gameSettings.graphicsQuality == GameSettings.GraphicsQuality.GOOD)
            {
                universalAdditionalCameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                universalAdditionalCameraData.antialiasingQuality = AntialiasingQuality.Medium;
                universalAdditionalCameraData.renderPostProcessing = true;
                universalAdditionalCameraData.renderShadows = true;
            }
            else
            {
                universalAdditionalCameraData.antialiasing = AntialiasingMode.TemporalAntiAliasing;
                universalAdditionalCameraData.antialiasingQuality = AntialiasingQuality.High;
                universalAdditionalCameraData.renderPostProcessing = true;
                universalAdditionalCameraData.renderShadows = true;
            }
        }
    }
}
