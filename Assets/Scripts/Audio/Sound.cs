using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip m_Clip;

    [Range(0f, 1f)] public float m_Volume;
    [Range(.1f, 3f)] public float m_Pitch;

    public string m_Name;
    public bool m_Loop;

    [HideInInspector] public AudioSource m_Source;

    public AudioMixerGroup m_Group;
}
