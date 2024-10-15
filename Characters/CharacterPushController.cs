using System.Collections;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterPushController : MonoBehaviour
    {
        public CharacterManager characterManager;

        bool isPushed = false;

        public UnityEvent onPush_Begin;
        public UnityEvent onPush_End;

        // TODO: Should derive from enemies        
        [Range(0, 1f)] public float pushForceAbsorption = 1;

        private void Awake()
        {
            characterManager.damageReceiver.onDamageEvent += OnDamageEvent;
        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage damage)
        {
            bool waitingForBackstab = (receiver as CharacterManager)?.characterBackstabController?.waitingForBackstab ?? false;

            if (!waitingForBackstab && damage != null && damage.pushForce > 0)
            {
                var targetPos = characterManager.transform.position - Camera.main.transform.position;
                targetPos.y = 0;
                characterManager.characterPushController.ApplyForceSmoothly(
                    targetPos.normalized,
                    Mathf.Clamp(damage.pushForce * pushForceAbsorption, 0, Mathf.Infinity) * 2.5f,
                    .25f);
            }

            return damage;
        }

        // Call this method when the character gets hit by the force
        public void ApplyForceSmoothly(Vector3 forceDirection, float pushForce, float duration)
        {
            if (!isPushed)
            {
                onPush_Begin?.Invoke();
                StartCoroutine(ApplyForceCoroutine(forceDirection, pushForce, duration));
            }
        }

        private IEnumerator ApplyForceCoroutine(Vector3 forceDirection, float pushForce, float duration)
        {
            float elapsed = 0f;
            isPushed = true;
            while (elapsed < duration)
            {
                float forceMagnitude = Mathf.Lerp(pushForce, 0f, elapsed / duration);

                if (characterManager.characterController.enabled)
                {
                    characterManager.characterController.Move(forceDirection * forceMagnitude * Time.deltaTime);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            onPush_End?.Invoke();
            isPushed = false;
        }
    }
}
