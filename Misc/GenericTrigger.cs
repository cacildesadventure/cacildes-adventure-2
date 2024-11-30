using AF.Inventory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class GenericTrigger : MonoBehaviour, IEventNavigatorCapturable
    {

        [Header("Events")]
        public UnityEvent onActivate;

        [Header("Prompt")]
        public string action = "Pickup";


        [Header("Alchemy Pickable Info")]
        public Item item;

        [Header("Required Item to Open")]
        public Item requiredItemToOpen;
        public InventoryDatabase inventoryDatabase;

        bool canInteract = true;

        // Scene Refs
        UIDocumentKeyPrompt _uIDocumentKeyPrompt;
        Soundbank _soundbank;
        public void OnCaptured()
        {
            if (!canInteract)
            {
                return;
            }

            GetUIDocumentKeyPrompt().DisplayPrompt(action);
        }

        public void OnInvoked()
        {
            if (!canInteract)
            {
                return;
            }

            DisableKeyPrompt();

            HandleActivation();
        }

        public void OnReleased()
        {
            DisableKeyPrompt();
        }

        public void DisableKeyPrompt()
        {
            GetUIDocumentKeyPrompt().gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void TurnCapturable()
        {
            canInteract = true;
            //this.gameObject.layer = LayerMask.NameToLayer("IEventNavigatorCapturable");
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisableCapturable()
        {
            canInteract = false;
            //this.gameObject.layer = 0;
        }
        public void HandleActivation()
        {
            bool canActivate = true;

            if (requiredItemToOpen != null && inventoryDatabase != null)
            {
                if (inventoryDatabase.HasItem(requiredItemToOpen))
                {
                    inventoryDatabase.RemoveItem(requiredItemToOpen);
                    GetNotificationManager().ShowNotification($"{requiredItemToOpen.GetName()} " + LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "was lost with its use."));
                }
                else
                {
                    GetNotificationManager().ShowNotification($"{requiredItemToOpen.GetName()} " + LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "is required to activate."));
                    canActivate = false;
                }
            }

            if (canActivate)
            {
                onActivate?.Invoke();
            }

            GetSoundbank().PlaySound(GetSoundbank().uiDecision);
        }

        UIDocumentKeyPrompt GetUIDocumentKeyPrompt()
        {
            if (_uIDocumentKeyPrompt == null)
            {
                _uIDocumentKeyPrompt = FindAnyObjectByType<UIDocumentKeyPrompt>(FindObjectsInactive.Include);
            }

            return _uIDocumentKeyPrompt;
        }


        NotificationManager GetNotificationManager()
        {
            return FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
        }
        Soundbank GetSoundbank()
        {
            if (_soundbank == null)
            {
                _soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return _soundbank;
        }

    }
}
