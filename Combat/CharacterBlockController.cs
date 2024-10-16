using AF.Health;
using UnityEngine;

namespace AF
{
    public class CharacterBlockController : CharacterAbstractBlockController
    {

        [Header("Settings")]
        public bool shouldFaceTargetWhenBlockingAttack = true;

        private void Awake()
        {
            characterManager.damageReceiver.onDamageEvent += OnDamageEvent;
        }

        public override void BlockAttack(Damage damage)
        {
            if (shouldFaceTargetWhenBlockingAttack)
            {
                (characterManager as CharacterManager)?.FaceTarget();
            }

            base.BlockAttack(damage);

            // If is enemy, mark isBlocking as false, otherwise the enemy will always block player attacks repeatedly without exiting this guard state
            isBlocking = false;
        }

        public override int GetPostureDamageFromParry()
        {
            return basePostureDamageFromParry;
        }

        public override float GetUnarmedParryWindow()
        {
            return baseUnarmedParryWindow;
        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage incomingDamage)
        {
            if (incomingDamage == null || !CanBlockDamage(incomingDamage))
            {
                return incomingDamage;
            }


            if (CanParry(incomingDamage))
            {
                (characterManager as CharacterManager)?.FaceTarget();
                HandleParryEvent();
                attacker.characterBlockController.HandleParriedEvent(GetPostureDamageFromParry());
                return null;
            }

            characterManager.characterBlockController.BlockAttack(incomingDamage);
            return null;
        }
    }
}
