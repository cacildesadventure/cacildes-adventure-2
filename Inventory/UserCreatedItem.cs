namespace AF
{
    using System.Linq;
    using UnityEngine;

    [System.Serializable]
    public class UserCreatedItem
    {
        public string itemName;
        public string itemThumbnailName = "";
        public Sprite itemThumbnail;
        public StatusEffect[] positiveEffects;
        public StatusEffect[] negativeEffects;
        public int value = 0;
    }
}
