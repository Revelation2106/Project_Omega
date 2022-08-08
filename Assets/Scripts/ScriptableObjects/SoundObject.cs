using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sound Object", menuName = "ScriptableObjects/SoundObject")]
public class SoundObject : ScriptableObject
{
    public string m_SoundName = "";
    public AudioClip m_Clip;

    [Range(0f, 1f)] public float m_Volume = 1.0f;
    [Range(.1f, 3f)] public float m_Pitch = 1.0f;

    public bool m_Loop = false;

    public AudioMixerGroup m_Group;
}
