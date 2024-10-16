using AF.Events;
using AF.Health;
using AF.Stats;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class PlayerHealth : CharacterBaseHealth
    {

        [Header("Components")]
        public PlayerManager playerManager;
        public StatsBonusController playerStatsBonusController;
        public NotificationManager notificationManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public GameSession gameSession;


        [Header("Events")]
        public UnityEvent onDyingInArena;

        [Header("Options")]
        public bool isTutorialScene = false;


        private void Awake()
        {
            playerManager.damageReceiver.onDamageEvent += OnDamageEvent;

            // Initialize Health
            if (playerStatsDatabase.currentHealth == -1)
            {
                SetCurrentHealth(GetMaxHealth());
            }
        }


        public override int GetMaxHealth()
        {
            int baseValue = Formulas.CalculateStatForLevel(
                playerStatsDatabase.maxHealth + playerStatsBonusController.healthBonus,
                playerStatsBonusController.GetCurrentVitality(),
                playerStatsDatabase.levelMultiplierForHealth);

            if (hasHealthCutInHalf)
            {
                return (int)baseValue / 2;
            }

            return baseValue;
        }


        public void SubtractAmountMultipliedByTimeDeltaTime(float amount)
        {
            TakeDamage(amount * Time.deltaTime);
        }

        public void RestoreHealthPercentage(int amount)
        {
            var percentage = GetMaxHealth() * amount / 100;
            var nextValue = Mathf.Clamp(
                playerStatsDatabase.currentHealth + percentage, 0, GetMaxHealth());

            SetCurrentHealth(nextValue);
        }

        public float GetHealthPointsForGivenVitality(int vitality)
        {
            return Formulas.CalculateStatForLevel((int)GetCurrentHealth(), vitality, playerStatsDatabase.levelMultiplierForHealth);
        }

        public override void RestoreHealth(float value)
        {
            SetCurrentHealth(Mathf.Clamp(
                playerStatsDatabase.currentHealth + value, 0, GetMaxHealth())
            );

            onRestoreHealth?.Invoke();
        }

        public override void TakeDamage(float value)
        {
            if (value <= 0 || GetCurrentHealth() <= 0)
            {
                return;
            }

            SetCurrentHealth(
                Mathf.Clamp(
                playerStatsDatabase.currentHealth - value, 0, GetMaxHealth())
            );

            onTakeDamage?.Invoke();

            if (GetCurrentHealth() <= 0)
            {
                if (isTutorialScene)
                {
                    RestoreFullHealth();
                    return;
                }

                if (value < 999 && playerStatsBonusController.chanceToRestoreHealthUponDeath && Random.Range(0, 1f) >= 0.5f)
                {
                    RestoreHealthPercentage(50);
                    notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "You were saved from death."));
                    return;
                }

                HandleDeath();
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamageWithoutOnTakeDamageEvent(float amount)
        {
            SetCurrentHealth(Mathf.Clamp(
                playerStatsDatabase.currentHealth - amount, 0, GetMaxHealth()));

            if (GetCurrentHealth() <= 0)
            {
                HandleDeath();
            }
        }

        void HandleDeath()
        {
            onDeath?.Invoke();
        }

        public override float GetCurrentHealth()
        {
            return playerStatsDatabase.currentHealth;
        }

        public float GetExtraAttackBasedOnCurrentHealth()
        {
            var percentage = playerStatsDatabase.currentHealth * 100 / GetMaxHealth() * 0.01;

            if (percentage > 0.9)
            {
                return 0;
            }
            else if (percentage > 0.8)
            {
                return 0.05f;
            }
            else if (percentage > 0.7)
            {
                return 0.1f;
            }
            else if (percentage > 0.6)
            {
                return 0.2f;
            }
            else if (percentage > 0.5)
            {
                return 0.5f;
            }
            else if (percentage > 0.4)
            {
                return 0.6f;
            }
            else if (percentage > 0.3)
            {
                return 0.8f;
            }
            else if (percentage > 0.2)
            {
                return 1.2f;
            }
            else if (percentage > 0.1)
            {
                return 1.5f;
            }
            else if (percentage > 0)
            {
                return 2f;
            }

            return 0f;
        }

        public override void RestoreFullHealth()
        {
            RestoreHealthPercentage(100);
        }

        public override void SetCurrentHealth(float value)
        {
            this.playerStatsDatabase.currentHealth = value;
            onHealthChanged?.Invoke();
        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage damage)
        {
            if (attacker is PlayerManager playerManager)
            {
                int healthRestoredWithEachHit = (int)(playerManager.playerWeaponsManager?.currentWeaponInstance?.weapon?.healthRestoredWithEachHit ?? 0);
                playerManager.health.RestoreHealth(healthRestoredWithEachHit);
            }

            return damage;
        }
    }
}
