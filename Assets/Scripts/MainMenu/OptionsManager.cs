using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private GameObject m_MainMenuPanel, m_OptionsPanel, m_ConfirmGraphicsPanel;

    [SerializeField] private Slider m_MasterVolumeSlider, m_MusicVolumeSlider, m_DialogueVolumeSlider, m_AmbientVolumeSlider;
    [SerializeField] private TMP_Dropdown m_ResolutionDropdown, m_QualityDropdown, m_WindowModeDropdown;

    private void OnEnable()
    {
        m_MasterVolumeSlider.value = GameSettings.s_MasterVolume;
        m_MusicVolumeSlider.value = GameSettings.s_MusicVolume;
        m_DialogueVolumeSlider.value = GameSettings.s_DialogueVolume;
        m_AmbientVolumeSlider.value = GameSettings.s_AmbientVolume;

        m_QualityDropdown.value = GameSettings.s_QualityIndex;
        m_QualityDropdown.RefreshShownValue();

        //m_ResolutionDropdown.value = Array.IndexOf(Screen.resolutions, Screen.currentResolution);
        //m_ResolutionDropdown.RefreshShownValue();

        // TODO: update value on game load
        m_WindowModeDropdown.value = GameSettings.s_WindowModeIndex > 2 ? 2 : GameSettings.s_WindowModeIndex;
        m_WindowModeDropdown.RefreshShownValue();

        m_ConfirmGraphicsPanel.SetActive(false);
    }

    public void HideOptionsMenu()
    {
        GameSettings.SaveSettings();

        m_MainMenuPanel.SetActive(true);
        m_OptionsPanel.SetActive(false);
    }
}
