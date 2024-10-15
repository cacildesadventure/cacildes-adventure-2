namespace AF
{
    using AF.Health;
    using UnityEngine;

    public class CharacterBackstabController : MonoBehaviour
    {
        public readonly int hashBackstabExecuted = Animator.StringToHash("AI Humanoid - Backstabbed");

        public bool waitingForBackstab = false;
        public bool canBeBackstabbed = true;

        public CharacterBaseManager character;

        public float backstabDamageMultiplier = 3f;

        private void Awake()
        {
            character.damageReceiver.onDamageEvent += OnDamageEvent;
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

                character.PlayBusyHashedAnimationWithRootMotion(hashBackstabExecuted);

                damage.physical = (int)(damage.physical * backstabDamageMultiplier);
            }

            return damage;
        }
    }
}