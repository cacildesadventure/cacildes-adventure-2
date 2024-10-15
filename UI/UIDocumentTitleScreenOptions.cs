using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreenOptions : MonoBehaviour
    {
        VisualElement root => GetComponent<UIDocument>().rootVisualElement;
        UIDocumentTitleScreen uIDocumentTitleScreen;


        [Header("Components")]
        public Soundbank soundbank;

        private void Awake()
        {
            uIDocumentTitleScreen = FindAnyObjectByType<UIDocumentTitleScreen>(FindObjectsInactive.Include);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {

            root.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                Close();
            });

            UIUtils.SetupButton(root.Q<Button>("CloseBtn"), () =>
            {
                Close();
            }, soundbank);

        }

        /// <summary>
        /// UnityEvent
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled)
            {
                Close();
            }
        }

        void Close()
        {
            uIDocumentTitleScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
