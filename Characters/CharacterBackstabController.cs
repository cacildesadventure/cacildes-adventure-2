namespace AF
{
    using AF.Health;
    using UnityEngine;

    public class CharacterBackstabController : MonoBehaviour
    {
        public readonly int hashBackstabExecuted = Animator.StringToHash("AI Humanoid - Backstabbed");

        public bool waitingForBackstab = false;
        public bool isBeingBackstabbed = false;

        public CharacterBaseManager character;

        public float backstabDamageMultiplier = 3f;

        private void Awake()
        {
            character.damageReceiver.onDamageEvent += OnDamageEvent;
        }

        public void ResetStates()
        {
            isBeingBackstabbed = false;
        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage damage)
        {
            if (damage == null)
            {
                return damage;
            }

            if (waitingForBackstab)
            {
                waitingForBackstab = false;
                isBeingBackstabbed = true;

                character.PlayBusyHashedAnimationWithRootMotion(hashBackstabExecuted);

                damage.physical = (int)(damage.physical * backstabDamageMultiplier);
            }

            return damage;
        }

        public bool CanBeBackstabbed()
        {
            return (character as CharacterManager)?.combatant?.allowBackstabs ?? false;
        }
    }
}
