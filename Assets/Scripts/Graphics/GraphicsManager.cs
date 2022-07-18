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

    public TMPro.TMP_Dropdown m_QualityDropdown, m_ResolutionDropdown, m_WindowModeDropdown;

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

    public void SetGraphicsQuality(int _qualityIndex)
    {
        Debug.Log("SETGRAPHICSQUALITY index: " + _qualityIndex);
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
        switch(_windowModeIndex)
        {
            case 0:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
                break;
            case 1:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.Windowed);
                break;
            case 2:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
                break;
            default:
                Debug.Log("Failed to set window mode - index out of range!");
                break;        
        }
    }
}
