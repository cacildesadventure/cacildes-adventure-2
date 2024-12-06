using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class PlayerBlockController : CharacterAbstractBlockController
    {
        public PlayerManager playerManager;

        bool canCounterAttack = false;

        public const string counterAttackAnimation = "CounterAttack";

        public UnityEvent onCounterAttack;

        public bool isCounterAttacking = false;

        private void Awake()
        {
            playerManager.damageReceiver.onDamageEvent += OnDamageEvent;
        }

        public void ResetStates()
        {
            canCounterAttack = false;
            isCounterAttacking = false;
        }

        public override float GetUnarmedParryWindow()
        {
            return baseUnarmedParryWindow + playerManager.statsBonusController.parryPostureWindowBonus;
        }

        public override int GetPostureDamageFromParry()
        {
            return basePostureDamageFromParry + playerManager.statsBonusController.parryPostureDamageBonus;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnCounterAttack()
        {
            if (this.canCounterAttack)
            {
                this.canCounterAttack = false;

                playerManager.PlayBusyAnimationWithRootMotion(counterAttackAnimation);

                isCounterAttacking = true;
            }
        }

        public void SetCanCounterAttack(bool value)
        {
            this.canCounterAttack = value;
        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage incomingDamage)
        {
            if (incomingDamage == null || !CanBlockDamage(incomingDamage) || attacker == null)
            {
                return incomingDamage;
            }

            if (CanParry(incomingDamage))
            {
                HandleParryEvent();
                attacker.characterBlockController.HandleParriedEvent(GetPostureDamageFromParry());
                return null;
            }

            if (playerManager.staminaStatManager.HasEnoughStaminaForAction(playerManager.playerWeaponsManager.GetCurrentBlockStaminaCost()))
            {
                incomingDamage = HandleShieldBlock(incomingDamage, attacker);
                playerManager.staminaStatManager.DecreaseStamina((int)playerManager.playerWeaponsManager.GetCurrentBlockStaminaCost());
                return incomingDamage;
            }

            return incomingDamage;
        }


        private Damage HandleShieldBlock(Damage incomingDamage, CharacterBaseManager attacker)
        {
            incomingDamage = playerManager.playerWeaponsManager.GetCurrentShieldDefenseAbsorption(incomingDamage);

            if (attacker is CharacterManager enemy)
            {
                playerManager.playerWeaponsManager.ApplyShieldDamageToAttacker(enemy);
            }

            playerManager.characterBlockController.BlockAttack(incomingDamage);

            if (playerManager.characterBlockController is PlayerBlockController playerBlockController)
            {
                playerBlockController.SetCanCounterAttack(true);
            }

            return incomingDamage;
        }

    }
}
