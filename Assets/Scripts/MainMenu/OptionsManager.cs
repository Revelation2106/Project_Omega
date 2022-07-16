using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_MainMenuPanel, m_OptionsPanel;

    public void HideOptionsMenu()
    {
        m_MainMenuPanel.SetActive(true);
        m_OptionsPanel.SetActive(false);
    }
}
