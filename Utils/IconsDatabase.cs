namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Icons Database", menuName = "System/New Icons Database", order = 0)]
    public class IconsDatabase : ScriptableObject
    {
        public Sprite physicalAttack;
        public Sprite fire;
        public Sprite frost;
        public Sprite magic;
        public Sprite lightning;
        public Sprite darkness;
        public Sprite water;
    }

}
