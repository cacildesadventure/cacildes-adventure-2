namespace AF
{
    using UnityEngine;

    public class CharacterBlendshapes : MonoBehaviour
    {

        public SkinnedMeshRenderer skinnedMeshRenderer;

        public int mouthClosedIndex = 45;
        public int mouthClosedValue = 0;

        private void Update()
        {
            skinnedMeshRenderer.SetBlendShapeWeight(mouthClosedIndex, mouthClosedValue);
        }

    }
}