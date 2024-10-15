using System.Collections;
using AF.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class StaminaStatManager : MonoBehaviour
    {

        [Header("Regeneration Settings")]
        public float STAMINA_REGENERATION_RATE = 20f;
        public float STAMINA_REGENERATION_RATE_BONUS = 0f;
        public float negativeStaminaRegenerationBonus = 0f;
        public const float EMPTY_STAMINA_REGENERATION_DELAY = 0.5f;
        public bool shouldRegenerateStamina = false;

        [Header("Combat Stamina")]
        public int unarmedLightAttackStaminaCost = 15;
        public int unarmedHeavyAttackStaminaCost = 35;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;
        public PlayerStaminaUI playerStaminaUI;

        public StarterAssetsInputs inputs;


        public PlayerManager playerManager;

        [HideInInspector] public UnityEvent onStaminaChanged;

        private void Start()
        {
            if (playerStatsDatabase.currentStamina == -1)
            {
                SetCurrentStamina(GetMaxStamina());
            }
            else if (playerStatsDatabase.currentStamina < GetMaxStamina())
            {
                shouldRegenerateStamina = true;
            }
        }

        public int GetMaxStamina()
        {
            return playerStatsDatabase.maxStamina + playerStatsBonusController.staminaBonus + Mathf.RoundToInt((
                playerStatsDatabase.endurance + playerStatsBonusController.enduranceBonus) * playerStatsDatabase.levelMultiplierForStamina);
        }

        public float GetCurrentStaminaPercentage()
        {
            return playerStatsDatabase.currentStamina * 100 / GetMaxStamina();
        }

        public void DecreaseStamina(float amount)
        {
            shouldRegenerateStamina = false;

            SetCurrentStamina(Mathf.Clamp(playerStatsDatabase.currentStamina - amount, 0, GetMaxStamina()));

            StartCoroutine(RegenerateEmptyStamina());
        }

        IEnumerator RegenerateEmptyStamina()
        {
            yield return new WaitForSeconds(EMPTY_STAMINA_REGENERATION_DELAY);

            shouldRegenerateStamina = true;
        }

        private void Update()
        {
            if (shouldRegenerateStamina)
            {
                if (inputs.sprint)
                {
                    return;
                }

                HandleStaminaRegen();
            }
        }

        float GetStaminaRegenerationRate()
        {
            float value = STAMINA_REGENERATION_RATE + playerStatsBonusController.staminaRegenerationBonus - negativeStaminaRegenerationBonus + STAMINA_REGENERATION_RATE_BONUS;

            if (GetCurrentStaminaPercentage() <= 25)
            {
                value *= 1.75f;
            }
            else if (GetCurrentStaminaPercentage() <= 50)
            {
                value *= 1.25f;
            }

            return value;
        }

        void HandleStaminaRegen()
        {
            var finalRegenerationRate = GetStaminaRegenerationRate();


            if (playerManager.characterBlockController.isBlocking)
            {
                finalRegenerationRate = finalRegenerationRate / 4;
            }

            SetCurrentStamina(Mathf.Clamp(playerStatsDatabase.currentStamina + finalRegenerationRate * Time.deltaTime, 0f, GetMaxStamina()));

            if (playerStatsDatabase.currentStamina >= GetMaxStamina())
            {
                shouldRegenerateStamina = false;
            }
        }

        public bool HasEnoughStaminaForAction(float actionStaminaCost)
        {
            bool canPerform = playerStatsDatabase.currentStamina - actionStaminaCost > 0;
            if (!canPerform)
            {
                playerStaminaUI.DisplayInsufficientStamina();
            }

            return canPerform;
        }

        public void RestoreStaminaPercentage(float amount)
        {
            var percentage = this.GetMaxStamina() * amount / 100;
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentStamina + percentage, 0, this.GetMaxStamina());

            SetCurrentStamina(nextValue);
        }

        public void RestoreStaminaPoints(float amount)
        {
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentStamina + amount, 0, this.GetMaxStamina());

            SetCurrentStamina(nextValue);
        }


        public float GetStaminaPointsForGivenEndurance(int endurance)
        {
            return playerStatsDatabase.maxStamina + (int)Mathf.Ceil(endurance * playerStatsDatabase.levelMultiplierForStamina);
        }

        public void DecreaseLightAttackStamina()
        {
            DecreaseStamina(
                equipmentDatabase.GetCurrentWeapon() != null
                    ? equipmentDatabase.GetCurrentWeapon().lightAttackStaminaCost
                    : unarmedLightAttackStaminaCost);
        }
        public void DecreaseHeavyAttackStamina()
        {
            DecreaseStamina(
                equipmentDatabase.GetCurrentWeapon() != null
                    ? equipmentDatabase.GetCurrentWeapon().heavyAttackStaminaCost
                    : unarmedHeavyAttackStaminaCost);
        }

        public bool HasEnoughStaminaForLightAttack()
        {
            var staminaCost = equipmentDatabase.GetCurrentWeapon() != null
                ? equipmentDatabase.GetCurrentWeapon().lightAttackStaminaCost : unarmedLightAttackStaminaCost;

            return HasEnoughStaminaForAction(staminaCost);
        }

        public bool HasEnoughStaminaForHeavyAttack()
        {
            var staminaCost = equipmentDatabase.GetCurrentWeapon() != null
                ? equipmentDatabase.GetCurrentWeapon().heavyAttackStaminaCost : unarmedHeavyAttackStaminaCost;

            return HasEnoughStaminaForAction(staminaCost);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetNegativeStaminaRegenerationBonus(int value)
        {
            negativeStaminaRegenerationBonus = value;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void ResetNegativeStaminaRegenerationBonus()
        {
            negativeStaminaRegenerationBonus = 0f;
        }

        public void SetStaminaRegenerationBonus(float value)
        {
            this.STAMINA_REGENERATION_RATE_BONUS = value;
        }

        void SetCurrentStamina(float value)
        {
            playerStatsDatabase.currentStamina = value;

            onStaminaChanged?.Invoke();
        }
    }
}
