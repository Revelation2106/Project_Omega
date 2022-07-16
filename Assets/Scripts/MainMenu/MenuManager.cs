using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_MainMenuPanel, m_OptionsPanel;

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

        Application.Quit();
    }
}
