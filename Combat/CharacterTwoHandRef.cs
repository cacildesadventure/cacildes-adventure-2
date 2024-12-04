namespace AF
{
    using UnityEngine;

    public class CharacterTwoHandRef : MonoBehaviour
    {

        public bool useTwoHandingTransform = true;

        [Header("Idle Settings")]
        public Vector3 twoHandingPosition;
        public Vector3 twoHandingRotation;

        [Header("Block Settings")]
        public bool useCustomBlockRefs = false;
        public Vector3 blockPosition;
        public Vector3 blockRotation;

        Vector3 originalPosition;
        Quaternion originalRotation;

        [Header("Components")]
        public PlayerManager playerManager;
        public EquipmentDatabase equipmentDatabase;

        private void Awake()
        {
            this.originalPosition = transform.localPosition;
            this.originalRotation = transform.localRotation;
        }

        private void OnEnable()
        {
            playerManager.twoHandingController.onTwoHandingModeChanged += EvaluateTwoHandingUpdate;
            playerManager.characterBlockController.onBlockChanged += EvaluateTwoHandingUpdate;
            playerManager.characterBlockController.onBlockChanged += UseBlockTransform;

            EvaluateTwoHandingUpdate();
        }

        private void OnDisable()
        {
            playerManager.twoHandingController.onTwoHandingModeChanged -= EvaluateTwoHandingUpdate;
            playerManager.characterBlockController.onBlockChanged -= EvaluateTwoHandingUpdate;
            playerManager.characterBlockController.onBlockChanged -= UseBlockTransform;
        }


        public void EvaluateTwoHandingUpdate()
        {
            if (equipmentDatabase.isTwoHanding == false)
            {
                UseOneHandTransform();
                return;
            }

            if (playerManager.characterBlockController.isBlocking && equipmentDatabase.isTwoHanding)
            {
                UseBlockTransform();
                return;
            }

            UseTwoHandTransform();
        }

        public void UseOneHandTransform()
        {
            transform.SetLocalPositionAndRotation(originalPosition, originalRotation);
        }

        public void UseTwoHandTransform()
        {
            if (useTwoHandingTransform == false)
            {
                return;
            }

            transform.localPosition = twoHandingPosition;
            transform.localEulerAngles = twoHandingRotation;
        }

        public void UseBlockTransform()
        {
            if (equipmentDatabase.isTwoHanding == false || useCustomBlockRefs == false || playerManager.characterBlockController.isBlocking == false || equipmentDatabase.isUsingShield)
            {
                return;
            }

            this.transform.localPosition = blockPosition;
            this.transform.localEulerAngles = blockRotation;
        }
    }
}
