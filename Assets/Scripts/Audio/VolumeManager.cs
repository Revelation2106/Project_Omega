using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    [SerializeField]
    AudioMixer m_AudioMixer;

    public void SetMasterVolume(float _volume)
    {
        // TODO: Debug, remove later
        Debug.Log("Master volume set to " + _volume);

        m_AudioMixer.SetFloat("MasterVolume", Mathf.Log10(_volume) * 20); // Converts to -80 decibel logarithmic range
    }

    public void SetMusicVolume(float _volume)
    {
        // TODO: Debug, remove later
        Debug.Log("Music volume set to " + _volume);

        m_AudioMixer.SetFloat("MusicVolume", Mathf.Log10(_volume) * 20); // Converts to -80 decibel logarithmic range
    }

    public void SetDialogueVolume(float _volume)
    {
        // TODO: Debug, remove later
        Debug.Log("Dialogue volume set to " + _volume);

        m_AudioMixer.SetFloat("DialogueVolume", Mathf.Log10(_volume) * 20); // Converts to -80 decibel logarithmic range
    }

    public void SetAmbientVolume(float _volume)
    {
        // TODO: Debug, remove later
        Debug.Log("Ambient volume set to " + _volume);

        m_AudioMixer.SetFloat("AmbientVolume", Mathf.Log10(_volume) * 20); // Converts to -80 decibel logarithmic range
    }
}
