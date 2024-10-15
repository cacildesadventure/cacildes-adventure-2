namespace AF
{
    using UnityEngine;

    public class PreferencesManager : MonoBehaviour
    {
        public GameSettings gameSettings;
        public StarterAssetsInputs starterAssetsInputs;

        private void Start()
        {
            if (!gameSettings.hasLoadedPreferences)
            {
                gameSettings.LoadPreferences(starterAssetsInputs);
            }
        }
    }
}