using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GraphicsManager : MonoBehaviour
{
    private Resolution[] m_Resolutions;

    [SerializeField]
    private RenderPipelineAsset[] m_RenderPipelineAssets;

    [SerializeField]
    private GameObject m_ConfirmPanel;

    private float m_TimeRemaining = 10.0f;

    public TMPro.TMP_Dropdown m_QualityDropdown, m_ResolutionDropdown, m_WindowModeDropdown;
    public TMPro.TMP_Text m_CountdownText;

    private void Start()
    {
        m_Resolutions = Screen.resolutions;
        m_ResolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for(int i = 0; i < m_Resolutions.Length; i++)
        {
            string option = m_Resolutions[i].width + " x " + 
                            m_Resolutions[i].height + " @ " + 
                            m_Resolutions[i].refreshRate + "hz";

            options.Add(option);

            if (m_Resolutions[i].width == Screen.width &&
                m_Resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        m_ResolutionDropdown.AddOptions(options);
        m_ResolutionDropdown.value = currentResolutionIndex;
        m_ResolutionDropdown.RefreshShownValue();
    }

    private void Update()
    {
        if (!m_ConfirmPanel.activeSelf)
            return;

        m_TimeRemaining -= Time.deltaTime;
        m_CountdownText.text = ((int)m_TimeRemaining % 60).ToString();

        if(m_TimeRemaining <= 0.0f)
        {
            m_ConfirmPanel.SetActive(false);
            RevertGraphics();
        }
    }

    public void ShowConfirmGraphicsPanel()
    {
        m_ConfirmPanel.SetActive(true);
        m_TimeRemaining = 10.0f;
    }

    public void HideConfirmGraphicsPanel()
    {
        m_ConfirmPanel.SetActive(false);
    }

    public void ConfirmGraphics()
    {
        GameSettings.SaveSettings();
    }

    public void RevertGraphics()
    {
        SetGraphicsQuality(GameSettings.s_QualityIndex);
        SetWindowMode(GameSettings.s_WindowModeIndex);

        int currentResolutionIndex = 0;

        for (int i = 0; i < m_Resolutions.Length; i++)
        {
            if (m_Resolutions[i].width == GameSettings.s_Resolution[0] &&
                m_Resolutions[i].height == GameSettings.s_Resolution[1] &&
                m_Resolutions[i].refreshRate == GameSettings.s_Resolution[2])
            {
                currentResolutionIndex = i;
            }
        }

        m_QualityDropdown.value = GameSettings.s_QualityIndex;
        m_QualityDropdown.RefreshShownValue();

        m_ResolutionDropdown.value = currentResolutionIndex;
        m_ResolutionDropdown.RefreshShownValue();

        m_WindowModeDropdown.value = GameSettings.s_WindowModeIndex;
        m_WindowModeDropdown.RefreshShownValue();

        SetScreenResolution(currentResolutionIndex);
    }

    public void SetGraphicsQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(_qualityIndex);
        QualitySettings.renderPipeline = m_RenderPipelineAssets[_qualityIndex];
    }

    public void SetScreenResolution(int _resolutionIndex)
    {
        Resolution resolution = m_Resolutions[_resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetWindowMode(int _windowModeIndex)
    {
        //TODO: apply resolution correctly when changing window mode
        switch(_windowModeIndex)
        {
            case 0:
                Screen.fullScreen = true;
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
                break;
            case 1:
                Screen.fullScreen = false;
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
                break;
            case 2:
                Screen.fullScreen = false;
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
                break;
            default:
                Debug.Log("Failed to set window mode - index out of range!");
                break;        
        }
    }
}
