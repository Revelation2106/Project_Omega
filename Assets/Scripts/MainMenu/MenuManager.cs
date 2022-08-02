using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject m_MainMenuPanel, m_OptionsPanel;

    private void Start()
    {
        GameSettings.LoadSettings();
        SetAudioVolume();

        // TODO: Debug, remove later
        AudioManager.m_Instance.Play("Hillbilly");
        AudioManager.m_Instance.Play("Raving");
        AudioManager.m_Instance.Play("Deed");
    }

    private void SetAudioVolume()
    {
        AudioManager.m_Instance.SetMasterVolume(GameSettings.s_MasterVolume);
        AudioManager.m_Instance.SetMusicVolume(GameSettings.s_MusicVolume);
        AudioManager.m_Instance.SetDialogueVolume(GameSettings.s_DialogueVolume);
        AudioManager.m_Instance.SetAmbientVolume(GameSettings.s_AmbientVolume);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameWorld");
    }

    public void DisplayOptionsMenu()
    {
        m_MainMenuPanel.SetActive(false);
        m_OptionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        // TODO: Debug, remove later
#if UNITY_EDITOR
        Debug.Log("Quitting game...");
        EditorApplication.isPlaying = false;
#endif

        GameSettings.SaveSettings();
        Application.Quit();
    }
}
