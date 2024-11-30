using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Misc / Status / New Status")]
    [System.Serializable]
    public class StatusEffect : ScriptableObject
    {
        public LocalizedString displayName;
        public LocalizedString displayNameWhenApplied;

        public Sprite icon;
        public Color barColor;
        public bool isPositive = false;
        public bool isAppliedImmediately = false;

        public string GetName()
        {
            if (displayName == null || displayName.IsEmpty)
            {
                return "";
            }

            return displayName.GetLocalizedString();
        }

        public string GetAppliedName()
        {
            if (displayNameWhenApplied == null || displayNameWhenApplied.IsEmpty)
            {
                return "";
            }

            return displayNameWhenApplied.GetLocalizedString();
        }

    }
}