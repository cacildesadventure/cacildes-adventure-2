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

            HandleIncomingDamage(attacker);
        }

        public void ResetStates()
        {
            canTakeDamage = true;
        }

        public bool HandleIncomingDamage(CharacterBaseManager attacker)
        {
            if (!CanTakeDamage(attacker))
            {
                return false;
            }

            Damage incomingDamage = attacker.GetAttackDamage();

            return ApplyDamage(attacker, incomingDamage);
        }

        public bool ApplyDamage(CharacterBaseManager attacker, Damage damage)
        {
            // Call subscribers to modify incomingDamage
            if (onDamageEvent != null)
            {
                foreach (var subscriber in onDamageEvent.GetInvocationList())
                {
                    var modifiedDamage = (Damage)subscriber.DynamicInvoke(attacker, character, damage);

                    if (modifiedDamage == null)
                    {
                        damage = null;
                    }

                    damage = modifiedDamage;
                }
            }

            if (damage == null)
            {
                return false;
            }

            character.health.TakeDamage(damage.GetTotalDamage());

            if (character.statusController != null && damage.statusEffects != null && damage.statusEffects.Length > 0)
            {
                foreach (var statusEffectToApply in damage.statusEffects)
                {
                    character.statusController.InflictStatusEffect(statusEffectToApply.statusEffect, statusEffectToApply.amountPerHit, false);
                }
            }

            HandleDamageEffect(damage);

            return true;
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
                null,
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

        bool CanTakeDamage(CharacterBaseManager attacker)
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

            if (attacker.IsFromSameFaction(character))
            {
                return false;
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
