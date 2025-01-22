using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    public AudioMixerGroup audioMixer;
    [HideInInspector]
    public AudioSource source;
    [SerializeField] private SoundState actualSound;
    public SoundState ActualSound { get { return actualSound; } }
}
