using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] m_Sounds;

    public static AudioManager m_Instance;

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in m_Sounds)
        {
            s.m_Source = gameObject.AddComponent<AudioSource>();
            s.m_Source.clip = s.m_Clip;
            s.m_Source.loop = s.m_Loop;
            s.m_Source.outputAudioMixerGroup = s.m_Group;
        }
    }

    void Start()
    {
        //Play("MenuMusic");
    }

    public void Play(string _name)
    {
        Sound s = Array.Find(m_Sounds, sound => sound.m_Name == _name);

        if (s == null)
        {
            // TODO: Debug, remove later
            Debug.LogWarning("Sound: " + name + " not found!");

            return;
        }

        s.m_Source.volume = s.m_Volume;
        s.m_Source.pitch = s.m_Pitch;

        s.m_Source.Play();
    }

    public void StopPlaying(string _sound)
    {
        Sound s = Array.Find(m_Sounds, item => item.m_Name == _sound);
        if (s == null)
        {
            // TODO: Debug, remove later
            Debug.LogWarning("Sound: " + name + " not found!");

            return;
        }

        s.m_Source.Stop();
    }
}
