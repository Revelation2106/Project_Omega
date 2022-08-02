using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class GameSettings_Internal
{
    public float m_MasterVolume, m_MusicVolume, m_DialogueVolume, m_AmbientVolume;
    public int m_QualityIndex, m_WindowModeIndex;
    public int[] m_Resolution;

    public GameSettings_Internal()
    {
        AudioManager.m_Instance.m_AudioMixer.GetFloat("MasterVolume", out m_MasterVolume);
        AudioManager.m_Instance.m_AudioMixer.GetFloat("MusicVolume", out m_MusicVolume);
        AudioManager.m_Instance.m_AudioMixer.GetFloat("DialogueVolume", out m_DialogueVolume);
        AudioManager.m_Instance.m_AudioMixer.GetFloat("AmbientVolume", out m_AmbientVolume);

        m_QualityIndex = QualitySettings.GetQualityLevel(); // TODO: Might need to set render pipeline here too
        m_WindowModeIndex = (int)Screen.fullScreenMode;

        m_Resolution = new int[3]
        {
            Screen.width,
            Screen.height,
            Screen.currentResolution.refreshRate
        };
    }
}

[System.Serializable]
public static class GameSettings
{
    public static float s_MasterVolume, s_MusicVolume, s_DialogueVolume, s_AmbientVolume;
    public static int s_QualityIndex, s_WindowModeIndex;
    public static int[] s_Resolution  = { 0, 0, 0 };

    public static string PATH = Application.persistentDataPath + "/settings.prefs";

    public static void SaveSettings()
    {
        GameSettings_Internal settings = new();
        SyncSettings(settings);

        BinaryFormatter formatter = new BinaryFormatter();

        string path = PATH;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, settings);
        stream.Close();
    }

    public static void LoadSettings()
    {
        if (!File.Exists(PATH))
            return;

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(PATH, FileMode.Open);

        GameSettings_Internal data = formatter.Deserialize(stream) as GameSettings_Internal;
        stream.Close();

        AudioManager.m_Instance.m_AudioMixer.SetFloat("MasterVolume", Mathf.Log10(data.m_MasterVolume) * 20);
        AudioManager.m_Instance.m_AudioMixer.SetFloat("MusicVolume", Mathf.Log10(data.m_MusicVolume) * 20);
        AudioManager.m_Instance.m_AudioMixer.SetFloat("DialogueVolume", Mathf.Log10(data.m_DialogueVolume) * 20);
        AudioManager.m_Instance.m_AudioMixer.SetFloat("AmbientVolume", Mathf.Log10(data.m_AmbientVolume) * 20);

        QualitySettings.SetQualityLevel(data.m_QualityIndex);
        Screen.SetResolution(data.m_Resolution[0], data.m_Resolution[1], (FullScreenMode)data.m_WindowModeIndex);

        SyncSettings(data);
    }

    public static void SyncSettings(GameSettings_Internal _settings)
    {
        s_MasterVolume = Mathf.Pow(10.0f, _settings.m_MasterVolume / 20.0f);
        s_MusicVolume = Mathf.Pow(10.0f, _settings.m_MusicVolume / 20.0f);
        s_DialogueVolume = Mathf.Pow(10.0f, _settings.m_DialogueVolume / 20.0f);
        s_AmbientVolume = Mathf.Pow(10.0f, _settings.m_AmbientVolume / 20.0f);

        s_QualityIndex = _settings.m_QualityIndex;
        s_WindowModeIndex = _settings.m_WindowModeIndex;
        s_Resolution = _settings.m_Resolution;
    }
}
