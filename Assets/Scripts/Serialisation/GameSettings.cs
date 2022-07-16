using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public float m_MasterVolume, m_MusicVolume, m_DialogueVolume, m_AmbientVolume;
    public int m_QualityIndex, m_WindowModeIndex;
    public float[] m_Resolution;
    public bool m_IsFullscreen, m_IsBorderless;

    public GameSettings(float _masterVolume, float _musicVolume, float _dialogueVolume, float _ambientVolume, 
                        int _qualityIndex, int _windowModeIndex, 
                        float[] _resolution, 
                        bool _isFullscreen, bool _isBorderless)
    {
        m_MasterVolume = _masterVolume;
        m_MusicVolume = _musicVolume;
        m_DialogueVolume = _dialogueVolume;
        m_AmbientVolume = _ambientVolume;

        m_QualityIndex = _qualityIndex;
        m_WindowModeIndex = _windowModeIndex;

        m_Resolution = new float[3];
        m_Resolution = _resolution;

        m_IsFullscreen = _isFullscreen;
        m_IsBorderless = _isBorderless;
    }
}
