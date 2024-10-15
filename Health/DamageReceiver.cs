namespace AF
{
    using System;
    using AF.Combat;
    using AF.Health;
    using UnityEngine;
    using UnityEngine.Events;

    public class DamageReceiver : MonoBehaviour, IDamageable
    {

        [Header("Character")]
        public CharacterBaseManager character;

        [Header("Damage Effects")]
        public DamageEffect physicalDamageEffect;
        public DamageEffect fireDamageEffect;

        [Header("Flags")]
        public bool canTakeDamage = true;

        // Damage Event
        public Func<CharacterBaseManager, CharacterBaseManager, Damage, Damage> onDamageEvent;

        public void OnDamage(CharacterBaseManager attacker, Action onDamageInflicted)
        {
            // Don't allow character to hit itself if not confused
            if (character?.isConfused == false && attacker == character)
            {
                return;
            }

            HandleIncomingDamage(attacker, (incomingDamage) =>
            {
                onDamageInflicted();
            }, character != null ? character.isConfused : false);
        }

        public void ResetStates()
        {
            canTakeDamage = true;
        }

        public void HandleIncomingDamage(CharacterBaseManager attacker, UnityAction<Damage> onTakeDamage, bool ignoreSameFaction)
        {
            if (IsSameFactionAttack(attacker, ignoreSameFaction) || !CanTakeDamage())
            {
                return;
            }

            Damage incomingDamage = attacker.GetAttackDamage();

            // Call subscribers to modify incomingDamage
            if (onDamageEvent != null)
            {
                incomingDamage = onDamageEvent(attacker, character, incomingDamage); // Get modified damage
            }

            if (incomingDamage == null)
            {
                return;
            }

            ApplyDamage(incomingDamage);

            onTakeDamage?.Invoke(incomingDamage);
        }

        private bool IsSameFactionAttack(CharacterBaseManager damageOwner, bool ignoreSameFaction)
        {
            return !ignoreSameFaction && damageOwner.IsFromSameFaction(character);
        }

        public void ApplyDamage(Damage damage)
        {
            character.health.TakeDamage(damage.GetTotalDamage());

            if (character.statusController != null && damage.statusEffects != null && damage.statusEffects.Length > 0)
            {
                foreach (var statusEffectToApply in damage.statusEffects)
                {
                    character.statusController.InflictStatusEffect(statusEffectToApply.statusEffect, statusEffectToApply.amountPerHit, false);
                }
            }

            HandleDamageEffect(damage);
        }

        void HandleDamageEffect(Damage damage)
        {
            if (damage.physical > 0)
            {
                physicalDamageEffect.Play();
            }
        }

        public void TakeDamagePercentage(float damagePercentage)
        {
            int damageAmount = (int)damagePercentage * character.health.GetMaxHealth() / 100;

            ApplyDamage(
                new(
                    physical: damageAmount,
                    fire: 0,
                    frost: 0,
                    magic: 0,
                    lightning: 0,
                    darkness: 0,
                    water: 0,
                    poiseDamage: 1,
                    postureDamage: 2,
                    weaponAttackType: WeaponAttackType.Slash,
                    statusEffects: null,
                    pushForce: 0,
                    canNotBeParried: false,
                    ignoreBlocking: false));
        }

        bool CanTakeDamage()
        {
            if (!canTakeDamage)
            {
                return false;
            }

            // If is an object
            if (character == null)
            {
                return true;
            }

            if (character.health.GetCurrentHealth() <= 0)
            {
                return false;
            }

            if (character is PlayerManager player && player.climbController.climbState != Ladders.ClimbState.NONE)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetCanTakeDamage(bool value)
        {
            canTakeDamage = value;
        }
    }
}
