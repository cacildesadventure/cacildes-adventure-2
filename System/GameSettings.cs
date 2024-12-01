using System;
using System.Collections.Generic;
using AF;
using AF.Events;
using CI.QuickSave;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;

[CreateAssetMenu(fileName = "GameSettings", menuName = "System/New Game Settings", order = 0)]
public class GameSettings : ScriptableObject
{
    public bool hasLoadedPreferences = false;
    public bool hasInitializedSettings = false;

    [Header("Gameplay Settings")]
    public float minimumCameraDistance = 0;
    public float maximumCameraDistance = 15;
    public float defaultCameraDistance = 4;
    public float cameraDistance = 4;
    public float zoomSpeed = 2f;

    public float minimumCameraSensitivity = 0f;
    public float maximumCameraSensitivity = 10f;
    public float defaultCameraSensitivity = 1f;
    public float cameraSensitivity = 1f;

    public bool showControlsInHUD = true;

    [Header("Graphics Settings")]
    public GraphicsQuality graphicsQuality = GraphicsQuality.GOOD;
    public enum GraphicsQuality { LOW = 0, MEDIUM = 1, GOOD = 2, ULTRA = 3 };

    [Header("Audio Settings")]
    public float musicVolume = 1f;

    public string characterName;

    public readonly string PLAYER_NAME_KEY = "PLAYER_NAME";
    public readonly string defaultPlayerName = "Cacildes";
    public readonly string HIDE_PLAYER_HUD_KEY = "HIDE_PLAYER_HUD_KEY";


    [Header("Keybindings")]
    public Dictionary<string, string> keybindings = new();


    const string GAME_PREFERENCES_SAVE_FILE_NAME = "GamePreferences";

#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Clear();
        }
    }
#endif

    void Clear()
    {
        hasInitializedSettings = false;
        hasLoadedPreferences = false;
    }

    /// <summary>
    /// Call when exiting the various sub-settings menus, since this is a write operation to the os, best to only run it once
    /// </summary>
    /// <param name="starterAssetsInputs"></param>
    public void SavePreferences()
    {
        QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create(GAME_PREFERENCES_SAVE_FILE_NAME);

        // Gameplay Settings
        quickSaveWriter.Write("cameraSensitivity", cameraSensitivity);
        quickSaveWriter.Write("cameraDistance", cameraDistance);
        quickSaveWriter.Write("showControlsInHUD", showControlsInHUD);

        // Language Settings
        quickSaveWriter.Write("gameLanguage", LocalizationSettings.SelectedLocale.Identifier.Code);

        // Graphics Quality Settings
        quickSaveWriter.Write("graphicsQuality", graphicsQuality);

        // Music Volume Settings
        quickSaveWriter.Write("musicVolume", musicVolume);

        // Keybinding Settings
        quickSaveWriter.Write("keybindings", keybindings);

        quickSaveWriter.TryCommit();
    }


    /// <summary>
    /// Call when loading up the game
    /// </summary>    
    public void LoadPreferences(StarterAssetsInputs starterAssetsInputs)
    {
        if (hasLoadedPreferences)
        {
            return;
        }

        if (!QuickSaveReader.RootExists(GAME_PREFERENCES_SAVE_FILE_NAME))
        {
            return;
        }

        QuickSaveReader quickSaveReader = QuickSaveReader.Create(GAME_PREFERENCES_SAVE_FILE_NAME);

        // Gameplay Settings
        quickSaveReader.TryRead("cameraSensitivity", out float cameraSensitivity);
        this.cameraSensitivity = cameraSensitivity;

        quickSaveReader.TryRead("cameraDistance", out float cameraDistance);
        this.cameraDistance = cameraDistance;

        quickSaveReader.TryRead("showControlsInHUD", out bool showControlsInHUD);
        SetShouldShowPlayerHUD(showControlsInHUD);

        // Language Settings
        quickSaveReader.TryRead("gameLanguage", out string gameLanguage);
        if (LocalizationSettings.AvailableLocales.Locales.Exists(x => x.Identifier == gameLanguage))
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(gameLanguage);
        }

        // Graphic Settings
        quickSaveReader.TryRead("graphicsQuality", out int graphicsQuality);
        SetGraphicsQuality(graphicsQuality);

        // Audio Settings
        quickSaveReader.TryRead("musicVolume", out float musicVolume);
        SetMusicVolume(musicVolume);

        // Control Settings
        quickSaveReader.TryRead("keybindings", out Dictionary<string, string> keybindings);
        this.keybindings = keybindings;
        SetInputOverrides(starterAssetsInputs);

        hasLoadedPreferences = true;
    }

    public void UpdatePlayerNameOnLocalizedAssets()
    {
        // Get our GlobalVariablesSource
        var source = LocalizationSettings
            .StringDatabase
            .SmartFormatter
            .GetSourceExtension<PersistentVariablesSource>();
        // Get the specific global variable
        var characterName =
            source["global"]["playerName"] as UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable;
        // Update the global variable
        characterName.Value = GetPlayerName();
    }


    public void SetShouldShowPlayerHUD(bool value)
    {
        showControlsInHUD = value;
        EventManager.EmitEvent(EventMessages.ON_PLAYER_HUD_VISIBILITY_CHANGED);
    }

    public string GetPlayerName()
    {
        if (!PlayerPrefs.HasKey(PLAYER_NAME_KEY))
        {
            return defaultPlayerName;
        }

        return PlayerPrefs.GetString(PLAYER_NAME_KEY);
    }

    public void SetPlayerName(string playerName)
    {

        PlayerPrefs.SetString(PLAYER_NAME_KEY, playerName);
        PlayerPrefs.Save();
        UpdatePlayerNameOnLocalizedAssets();
    }

    public void SetGraphicsQuality(int newValue)
    {
        // Ensure the newValue is within the bounds of the enum values
        if (Enum.IsDefined(typeof(GraphicsQuality), newValue))
        {
            SetGraphicsQuality((GraphicsQuality)newValue);
        }
        else
        {
            Debug.LogWarning($"Invalid graphics quality value: {newValue}. It must be between 0 and {Enum.GetValues(typeof(GraphicsQuality)).Length - 1}.");
        }
    }

    public void SetGraphicsQuality(GraphicsQuality newValue)
    {
        if (newValue == 0)
        {
            QualitySettings.SetQualityLevel(0);
        }
        else if ((int)newValue == 1)
        {
            QualitySettings.SetQualityLevel(2);
        }
        else if ((int)newValue == 2)
        {
            QualitySettings.SetQualityLevel(4);
        }
        else if ((int)newValue == 3)
        {
            QualitySettings.SetQualityLevel(5);
        }

        EventManager.EmitEvent(EventMessages.ON_GRAPHICS_QUALITY_CHANGED);
    }

    public void SetMusicVolume(float newValue)
    {
        musicVolume = newValue;
        EventManager.EmitEvent(EventMessages.ON_MUSIC_VOLUME_CHANGED);
    }

    public void UpdateKey(string actionName, string key)
    {
        if (keybindings.ContainsKey(actionName))
        {
            keybindings[actionName] = key;
        }
        else
        {
            keybindings.Add(actionName, key);
        }
    }

    public void SetInputOverrides(StarterAssetsInputs starterAssetsInputs)
    {
        foreach (var keyOverride in keybindings)
        {
            starterAssetsInputs.ApplyBindingOverride(keyOverride.Key, keyOverride.Value);
        }
    }
}
