namespace AF
{
    using UnityEngine;
    using UnityEngine.Localization;

    [CreateAssetMenu(fileName = "New Location", menuName = "Data / New Location")]
    public class Location : ScriptableObject
    {
        public LocalizedString locationName;

        public Sprite locationThumbnail;

        public string GetLocationDisplayName()
        {
            if (locationName.IsEmpty)
            {
                return name;
            }

            return locationName.GetLocalizedString();
        }
    }
}
