namespace AF.Health
{
    using AF.Companions;
    using AF.Events;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterHealth : CharacterBaseHealth
    {
        public CharacterHealthUI characterHealthUI;
        public GameSession gameSession;
        public CharacterManager characterManager;
        public CompanionsDatabase companionsDatabase;

        [HideInInspector] public UnityEvent onHealthSettingsChanged;

        [SerializeField] private float currentHealth;

        private bool hasTriggeredHalfHealthEvent = false;

        [Header("Events")]
        public UnityEvent onHalfHealth;
        public UnityEvent onRevive;

        [Header("Options")]
        public int bonusHealth = 0;
        public int bonusHealthFromCompanions = 0;

        public float CurrentHealth
        {
            get => currentHealth;
            set
            {
                currentHealth = Mathf.Clamp(value, 0, GetMaxHealth());
                characterHealthUI.UpdateUI(); // Ensure UI updates whenever health changes.
            }
        }

        private void Awake()
        {
            CurrentHealth = GetMaxHealth();
            EventManager.StartListening(EventMessages.ON_PARTY_CHANGED, UpdateHealthSettings);
        }

        private void OnDestroy()
        {
            EventManager.StopListening(EventMessages.ON_PARTY_CHANGED, UpdateHealthSettings);
        }

        private void UpdateHealthSettings()
        {
            if (CurrentHealth <= 0) return;

            bool wasAtFullHealth = IsAtFullHealth();

            UpdateBonusHealthFromCompanions();

            if (wasAtFullHealth)
            {
                RestoreFullHealth();
            }

            onHealthSettingsChanged?.Invoke();
        }

        private bool IsAtFullHealth() => CurrentHealth >= GetMaxHealth();

        private void UpdateBonusHealthFromCompanions()
        {
            if (!characterManager.IsCompanion() && companionsDatabase.TryGetCompanionCount(out int companionCount) && companionCount > 0)
            {
                bonusHealthFromCompanions = HealthUtils.GetExtraHealthBasedOnCompanionsInParty(companionCount);
            }
            else
            {
                bonusHealthFromCompanions = 0;
            }
        }

        public override void RestoreHealth(float value)
        {
            CurrentHealth += value;
            onRestoreHealth?.Invoke();
        }

        public override void TakeDamage(float value)
        {
            if (value <= 0 || CurrentHealth <= 0) return;

            CurrentHealth -= value;
            HandleHalfHealthTrigger();
            onTakeDamage?.Invoke();

            if (CurrentHealth <= 0)
            {
                HandleDeath();
            }
        }

        private void HandleHalfHealthTrigger()
        {
            if (!hasTriggeredHalfHealthEvent && CurrentHealth <= GetMaxHealth() / 2)
            {
                hasTriggeredHalfHealthEvent = true;
                onHalfHealth?.Invoke();
            }
        }

        private void HandleDeath()
        {
            PlayDeath();
            characterManager.PlayBusyAnimationWithRootMotion(hashDeath);
            characterManager.DisableComponents();
            characterManager.characterLoot.GiveLoot();
            CheckIfHasBeenKilledWithRightWeapon();
            EventManager.EmitEvent(EventMessages.ON_CHARACTER_KILLED);
            onDeath?.Invoke();
        }

        public override int GetMaxHealth()
        {
            int scaledMaxHealth = Utils.ScaleWithCurrentNewGameIteration(
                characterManager.combatant.health + bonusHealth + bonusHealthFromCompanions,
                gameSession.currentGameIteration,
                gameSession.newGamePlusScalingFactor
            );

            return hasHealthCutInHalf ? scaledMaxHealth / 2 : scaledMaxHealth;
        }

        public override float GetCurrentHealth() => CurrentHealth;

        public override void RestoreFullHealth() => CurrentHealth = GetMaxHealth();

        public void Revive()
        {
            hasTriggeredHalfHealthEvent = false;
            RestoreFullHealth();
            onRevive?.Invoke();
            characterManager.EnableComponents();
        }

        public override void SetCurrentHealth(float value) => CurrentHealth = value;


        public void IncreaseBonusHealth(int value)
        {
            bonusHealth += value;
            UpdateHealthSettings();
        }

        public override void SetHasHealthCutInHealth(bool value)
        {
            base.SetHasHealthCutInHealth(value);
            UpdateHealthSettings();
        }
    }
}
