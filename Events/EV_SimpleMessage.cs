
namespace AF
{
    using System.Collections;
    using System.Linq;
    using AF.Dialogue;
    using UnityEngine;
    using UnityEngine.Localization.Settings;

    [RequireComponent(typeof(MonoBehaviourID))]
    public class EV_SimpleMessage : EventBase
    {
        public Character character;

        [TextAreaAttribute(minLines: 10, maxLines: 20)]
        [HideInInspector] public string message;
        [TextAreaAttribute(minLines: 10, maxLines: 20)]
        public string englishMessage;

        [TextAreaAttribute(minLines: 10, maxLines: 20)]
        public string portugueseMessage;

        [Header("Responses")]
        public Response[] responses;

        [HideInInspector] public MonoBehaviourID monoBehaviourID => GetComponent<MonoBehaviourID>();

        // Scene Refs
        UIDocumentDialogueWindow uIDocumentDialogueWindow;

        public override IEnumerator Dispatch()
        {
            // Only consider responses that are active - we hide responses based on composition of nested objects
            Response[] filteredResponses = responses.Where(response => response.gameObject.activeInHierarchy).ToArray();

            yield return GetUIDocumentDialogueWindow().DisplayMessage(
                character, LocalizationSettings.StringDatabase.GetLocalizedString("Dialogues", monoBehaviourID.ID), filteredResponses);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        UIDocumentDialogueWindow GetUIDocumentDialogueWindow()
        {
            if (uIDocumentDialogueWindow == null)
            {
                uIDocumentDialogueWindow = FindAnyObjectByType<UIDocumentDialogueWindow>(FindObjectsInactive.Include);
            }

            return uIDocumentDialogueWindow;
        }
    }
}
