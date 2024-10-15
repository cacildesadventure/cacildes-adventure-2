namespace AF
{
    using UnityEngine.Localization.Settings;

    public static class Glossary
    {
        public static string Get(string key)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", key);
        }
    }
}
