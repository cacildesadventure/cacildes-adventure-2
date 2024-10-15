using System.Collections;
using System.Collections.Generic;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class OnDamageCollisionAbstractManager : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public Projectile projectile;

        [Header("Damage Settings")]
        public Damage damage;
        List<DamageReceiver> damageReceivers = new();
        Coroutine ResetDamageReceiversCoroutine;
        public CharacterBaseManager damageOwner;
        public float damageCooldown = 1f;

        [Header("Events")]
        public UnityEvent onParticleDamage;

        [Header("Nighttime Options")]
        public bool doubleDamageOnNightTime = false;
        public GameSession gameSession;

        [Header("Healing Options")]
        public float healingAmount = -1f;

        private void OnEnable()
        {
            damageReceivers.Clear();
        }

        public void OnCollision(GameObject other)
        {
            other.TryGetComponent<DamageReceiver>(out var damageReceiver);

            if (damageReceiver == null && other.TryGetComponent<CharacterManager>(out var characterManager))
            {
                damageReceiver = characterManager.damageReceiver;
            }

            HandleDamage(damageReceiver);
        }

        void HandleDamage(DamageReceiver damageReceiver)
        {
            if (damageOwner != null && damageOwner.damageReceiver == damageReceiver)
            {
                return;
            }

            if (damageReceivers.Contains(damageReceiver))
            {
                return;
            }

            damageReceivers.Add(damageReceiver);

            if (projectile != null)
            {
                projectile.HandleCollision(damageReceiver);
            }
            else if (healingAmount >= 0f)
            {
                damageReceiver.character.health.RestoreHealth(healingAmount);
            }
            else if (damage != null && damageReceiver != null)
            {
                if (doubleDamageOnNightTime && gameSession != null && gameSession.IsNightTime())
                {
                    damage.physical *= 2;
                    damage.fire *= 2;
                    damage.frost *= 2;
                    damage.magic *= 2;
                    damage.darkness *= 2;
                    damage.lightning *= 2;
                    damage.water *= 2;
                }

                damageReceiver.ApplyDamage(damage);

                if (damageOwner != null && damageReceiver.character is CharacterManager aiCharacter && aiCharacter.targetManager != null)
                {
                    aiCharacter.targetManager.SetTarget(damageOwner);
                }

                if (damageOwner is PlayerManager)
                {
                    damageReceiver?.character?.health?.onDamageFromPlayer?.Invoke();
                }
            }

            onParticleDamage?.Invoke();

            if (ResetDamageReceiversCoroutine != null)
            {
                StopCoroutine(ResetDamageReceiversCoroutine);
            }

            ResetDamageReceiversCoroutine = StartCoroutine(ResetDamageReceivers_Coroutine());
        }

        IEnumerator ResetDamageReceivers_Coroutine()
        {
            yield return new WaitForSeconds(damageCooldown);

            damageReceivers.Clear();
        }
    }
}
