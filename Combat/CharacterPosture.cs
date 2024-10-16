namespace AF
{
    using AF.Events;
    using TigerForge;

    public class CharacterPosture : CharacterAbstractPosture
    {
        int curentMaxPosture = 100;

        public GameSession gameSession;

        private void Awake()
        {
            characterBaseManager.damageReceiver.onDamageEvent += OnDamageEvent;

            curentMaxPosture = GetCombatantBasePosture();
        }

        private void Start()
        {
            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, () =>
            {
                curentMaxPosture = GetCombatantBasePosture();
            });
        }

        public override bool CanPlayPostureDamagedEvent()
        {
            return true;
        }

        public override int GetMaxPostureDamage()
        {
            return Utils.ScaleWithCurrentNewGameIteration(curentMaxPosture, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
        }

        public override float GetPostureDecreateRate()
        {
            return 1f;
        }

        public void ResetPosture()
        {
            this.currentPostureDamage = 0;
        }
        public override bool TakePostureDamage(int extraPostureDamage)
        {
            bool hasBrokenPosture = base.TakePostureDamage(extraPostureDamage);

            // If taking posture damage, increase the max posture
            if (hasBrokenPosture)
            {
                curentMaxPosture += (int)(curentMaxPosture / 2);
            }

            return hasBrokenPosture;
        }


        int GetCombatantBasePosture()
        {
            return (characterBaseManager as CharacterManager)?.combatant?.posture ?? 1;
        }
    }
}
