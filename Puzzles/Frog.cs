namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GenericTrigger))]
    public class Frog : MonoBehaviour
    {
        Rigidbody rigidBody => GetComponent<Rigidbody>();

        public float sphereCastRadiusToLookForNeighbourFrogStand = 5f;
        public LayerMask frogStandLayer;
        public float jumpForce = 5f;
        public ForceMode jumpForceMode = ForceMode.Force;

        bool isMoving = false;
        Vector3 initialPosition;

        List<FrogStand> visitedFrogStands = new();

        GenericTrigger genericTrigger => GetComponent<GenericTrigger>();

        public UnityEvent onWin;

        Soundbank _soundbank;

        private void Awake()
        {
            initialPosition = transform.position;
            rigidBody.useGravity = true;
        }

        public void Move()
        {
            if (isMoving)
            {
                return;
            }

            isMoving = true;
            visitedFrogStands.Clear();

            StartCoroutine(HandlePuzzle_Coroutine());
        }

        IEnumerator HandlePuzzle_Coroutine()
        {
            FrogStand nextStand = FindNextFrogStand();

            while (nextStand != null && nextStand.isLast == false)
            {
                JumpTo(nextStand.frogStandRef.transform.position);
                yield return new WaitForSeconds(1f);
                nextStand = FindNextFrogStand();
            }

            if (nextStand != null && nextStand.isLast)
            {
                JumpTo(nextStand.frogStandRef.transform.position);
                yield return new WaitForSeconds(1f);
                Win();
                yield break;
            }

            JumpTo(transform.position + transform.right * -1f);
            rigidBody.isKinematic = false;
            yield return new WaitForSeconds(1f);

            rigidBody.isKinematic = true;
            transform.position = initialPosition;
            isMoving = false;
        }

        FrogStand FindNextFrogStand()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereCastRadiusToLookForNeighbourFrogStand, frogStandLayer);

            FrogStand closestStand = null;
            float minDistance = float.MaxValue;

            foreach (var hitCollider in hitColliders)
            {
                FrogStand stand = hitCollider.GetComponent<FrogStand>();
                if (stand != null && !visitedFrogStands.Contains(stand))
                {

                    float distance = Vector3.Distance(transform.position, stand.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestStand = stand;
                    }
                }
            }

            if (closestStand != null)
            {
                visitedFrogStands.Add(closestStand);
            }

            return closestStand;
        }

        void JumpTo(Vector3 targetPosition)
        {
            transform.position = targetPosition;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, sphereCastRadiusToLookForNeighbourFrogStand);
        }

        void Win()
        {
            genericTrigger.DisableCapturable();
            onWin?.Invoke();
            GetSoundbank().PlaySound(GetSoundbank().puzzleWon);
        }


        Soundbank GetSoundbank()
        {
            if (_soundbank == null)
            {
                _soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return _soundbank;
        }

    }
}
