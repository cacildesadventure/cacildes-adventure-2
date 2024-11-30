using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class PushableObject : MonoBehaviour
    {
        public UnityEvent onPush;
        public float moveDuration = 1f; // Duration for the push movement
        public float pushForceUnit = 2f;

        PlayerManager _playerManager;

        bool canPush = true;

        public void BeginPush()
        {
            GetPlayerManager().pushObjectManager.StartPush(this);
        }

        public void OnPush(Vector3 pusherPosition)
        {
            if (!canPush)
            {
                return;
            }

            canPush = false;

            Vector3 direction = transform.position - pusherPosition;
            direction.y = 0; // Keep y-component zero for horizontal movement

            // Normalize the direction to find cardinal directions
            direction.Normalize();

            // Determine the closest cardinal direction
            Vector3 pushDirection = Vector3.zero;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z)) // Move left/right
            {
                pushDirection = new Vector3(Mathf.Sign(direction.x), 0, 0); // Only horizontal
            }
            else // Move up/down
            {
                pushDirection = new Vector3(0, 0, Mathf.Sign(direction.z)); // Only vertical
            }

            // Calculate the final direction based on pushForceUnit
            pushDirection *= pushForceUnit;

            onPush?.Invoke();
            StartCoroutine(MoveToTarget(pushDirection));
        }

        private IEnumerator MoveToTarget(Vector3 targetDirection)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + targetDirection; // Calculate the target position
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                // Calculate the percentage of time elapsed
                float t = elapsedTime / moveDuration;

                // Lerp between start position and target position
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                elapsedTime += Time.deltaTime; // Increase elapsed time
                yield return null; // Wait for the next frame
            }

            // Ensure the object ends up at the target position
            transform.position = targetPosition;

            canPush = true;
        }


        // Lazy initialization helper methods
        private PlayerManager GetPlayerManager()
        {
            return _playerManager ??= FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
        }

    }
}
