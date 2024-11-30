
namespace AF
{
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentPlayerHUDV2 : MonoBehaviour
    {
        [HideInInspector] public UIDocument uIDocument => GetComponent<UIDocument>();
        VisualElement root;

        [Header("Player HUD Components")]
        public GameObject compass;

        [Header("Components")]
        public EquipmentHUD equipmentHUD;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            this.root = this.uIDocument.rootVisualElement;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetHUD_RootOpacity(float value)
        {
            root.style.opacity = value;
        }

        public void HideHUD()
        {
            compass.gameObject.SetActive(false);
            DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 0f, 0.5f);
        }

        public void ShowHUD()
        {
            compass.gameObject.SetActive(true);
            DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 1f, 0.5f);
        }

        public bool IsEquipmentDisplayed()
        {
            if (!root.visible)
            {
                return false;
            }

            return equipmentHUD.IsVisible();
        }

    }
}
