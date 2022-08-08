using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject m_MainMenuPanel, m_OptionsPanel;

    // TODO: Debug, remove later
    [SerializeField] private SoundObject m_MusicVolTest, m_DialogueVolTest, m_AmbientVolTest;

    private void Start()
    {
        GameSettings.LoadSettings();
        SetAudioVolume();

        // TODO: Debug, remove later
        InstanceManager.Get<AudioManager>().PlayMusic(m_MusicVolTest);
        InstanceManager.Get<AudioManager>().PlayDialogue(m_DialogueVolTest);
        InstanceManager.Get<AudioManager>().PlayAmbient(m_AmbientVolTest);
        // -----------------------------
    }

    private void SetAudioVolume()
    {
        InstanceManager.Get<AudioManager>().SetMasterVolume(GameSettings.s_MasterVolume);
        InstanceManager.Get<AudioManager>().SetMusicVolume(GameSettings.s_MusicVolume);
        InstanceManager.Get<AudioManager>().SetDialogueVolume(GameSettings.s_DialogueVolume);
        InstanceManager.Get<AudioManager>().SetAmbientVolume(GameSettings.s_AmbientVolume);
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
