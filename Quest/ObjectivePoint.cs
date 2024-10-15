namespace AF
{
    using UnityEngine;

    public class ObjectivePoint : MonoBehaviour
    {
        [Tooltip("The related quest of this objective point")]
        public QuestParent questParent;

        [Tooltip("The required progress of the quest for this objective point to appear")]
        public int questProgress;
    }
}
