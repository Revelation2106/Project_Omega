using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : ManagedInstance
{
    [SerializeField] private AudioMixer m_AudioMixer;

    public AudioMixer AudioMixer
    {
        get
        {
            return m_AudioMixer;
        }
    }

    [SerializeField] private AudioSource m_MusicSource, m_DialogueSource, m_AmbientSource;

    void Awake()
    {
        InstanceManager.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
    }

    // TODO: Temporary functions, remove (AudioManager should never play music directly, audio sources should be passed a SoundObject)

    public void PlayMusic(SoundObject _sound)
    {
        SetMusicValues(_sound);
        m_MusicSource.Play();
    }

    public void PlayDialogue(SoundObject _sound)
    {
        SetDialogueValues(_sound);
        m_DialogueSource.Play();
    }

    public void PlayAmbient(SoundObject _sound)
    {
        SetAmbientValues(_sound);
        m_AmbientSource.Play();
    }

    public void StopPlaying(SoundObject _sound)
    {
        if (m_MusicSource.isPlaying)
            m_MusicSource.Stop();

        if (m_DialogueSource.isPlaying)
            m_DialogueSource.Stop();

        if (m_AmbientSource.isPlaying)
            m_AmbientSource.Stop();
    }

    private void SetMusicValues(SoundObject _sound)
    {
        m_MusicSource.name = _sound.name;
        m_MusicSource.clip = _sound.m_Clip;
        m_MusicSource.volume = _sound.m_Volume;
        m_MusicSource.pitch = _sound.m_Pitch;
        m_MusicSource.loop = _sound.m_Loop;
        m_MusicSource.outputAudioMixerGroup = _sound.m_Group;
    }

    private void SetDialogueValues(SoundObject _sound)
    {
        m_DialogueSource.name = _sound.name;
        m_DialogueSource.clip = _sound.m_Clip;
        m_DialogueSource.volume = _sound.m_Volume;
        m_DialogueSource.pitch = _sound.m_Pitch;
        m_DialogueSource.loop = _sound.m_Loop;
        m_DialogueSource.outputAudioMixerGroup = _sound.m_Group;
    }

    private void SetAmbientValues(SoundObject _sound)
    {
        m_AmbientSource.name = _sound.name;
        m_AmbientSource.clip = _sound.m_Clip;
        m_AmbientSource.volume = _sound.m_Volume;
        m_AmbientSource.pitch = _sound.m_Pitch;
        m_AmbientSource.loop = _sound.m_Loop;
        m_AmbientSource.outputAudioMixerGroup = _sound.m_Group;
    }

    // -----------------------------------------------------------

    // TODO: Convert to new event system

    public void SetMasterVolume(float _volume)
    {
        m_AudioMixer.SetFloat("MasterVolume", Mathf.Log10(_volume) * 20); // Converts to -80 decibel logarithmic range
    }

    public void SetMusicVolume(float _volume)
    {
        m_AudioMixer.SetFloat("MusicVolume", Mathf.Log10(_volume) * 20); // Converts to -80 decibel logarithmic range
    }

    public void SetDialogueVolume(float _volume)
    {
        m_AudioMixer.SetFloat("DialogueVolume", Mathf.Log10(_volume) * 20); // Converts to -80 decibel logarithmic range
    }

    public void SetAmbientVolume(float _volume)
    {
        m_AudioMixer.SetFloat("AmbientVolume", Mathf.Log10(_volume) * 20); // Converts to -80 decibel logarithmic range
    }

    // ------------------------------------------
}
