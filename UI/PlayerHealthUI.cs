namespace AF
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using System.Collections;

    public class PlayerHealthUI : MonoBehaviour
    {
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;

        VisualElement healthContainer;
        VisualElement healthFill;
        VisualElement healthFillDelay;

        [Header("Graphic Settings")]
        public float healthContainerBaseWidth = 180;
        float _containerMultiplierPerLevel = 10f;

        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Health Delay Settings")]
        public float delayBeforeDecrease = 0.5f;  // Time before the delay bar starts decreasing
        public float delayDecreaseSpeed = 0.1f;   // Speed at which the delay bar decreases (as percentage per second)

        bool isSetup = false;
        Coroutine UpdateHealthDelayCoroutine;

        float previousHealthPercentage = 100f;  // Initially, 100% health

        void Start()
        {
            playerManager.health.onHealthChanged.AddListener(UpdateUI);
            UpdateUI();
        }

        private void OnEnable()
        {
            if (!isSetup)
            {
                SetupRefs();
            }
        }

        void SetupRefs()
        {
            healthContainer = uIDocumentPlayerHUDV2.uIDocument.rootVisualElement.Q<VisualElement>("Health");
            healthFill = healthContainer.Q<VisualElement>("Fill");
            healthFillDelay = healthContainer.Q<VisualElement>("FillDelay");
            isSetup = true;
        }

        void UpdateUI()
        {
            if (!isSetup)
            {
                SetupRefs();
            }

            float healthPercentage = playerManager.health.GetCurrentHealthPercentage() * (playerManager.health.hasHealthCutInHalf ? 0.5f : 1f);

            // Update the container width based on player's vitality
            healthContainer.style.width = (healthContainerBaseWidth +
                playerManager.statsBonusController.GetCurrentVitality() * _containerMultiplierPerLevel) *
                (playerManager.health.hasHealthCutInHalf ? 0.5f : 1f);

            // Immediately update the current health bar (green)
            healthFill.style.width = new Length(healthPercentage, LengthUnit.Percent);

            // Start the coroutine to update the delay bar (black bar showing previous health)
            if (UpdateHealthDelayCoroutine != null)
            {
                StopCoroutine(UpdateHealthDelayCoroutine);
            }
            UpdateHealthDelayCoroutine = StartCoroutine(UpdateHealthDelay(previousHealthPercentage, healthPercentage));

            previousHealthPercentage = healthPercentage;
        }

        IEnumerator UpdateHealthDelay(float previousHealthPercentage, float nextHealthPercentage)
        {
            // Set the black bar (delayed fill) to the previous health percentage before it gradually decreases
            healthFillDelay.style.width = new Length(previousHealthPercentage, LengthUnit.Percent);

            // Wait for the delay before starting the reduction of the black bar
            yield return new WaitForSeconds(delayBeforeDecrease);

            // Gradually reduce the delayed health bar
            while (previousHealthPercentage > nextHealthPercentage)
            {
                previousHealthPercentage -= delayDecreaseSpeed * Time.deltaTime;
                previousHealthPercentage = Mathf.Max(previousHealthPercentage, nextHealthPercentage); // Clamp to target percentage

                healthFillDelay.style.width = new Length(previousHealthPercentage, LengthUnit.Percent);

                yield return null;  // Wait until the next frame to continue the decrease
            }
        }
    }
}
