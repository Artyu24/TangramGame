using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sprite soundOn, soundOff;
    [SerializeField] private Image buttonImage;
    private bool soundActivate = true;

    public Sound[] sounds;

    public static AudioManager instance;
    
    private Dictionary<SoundState, List<Sound>> DicoActualSound = new Dictionary<SoundState, List<Sound>>();
    
    void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        //Create an Audio Source for each sound
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.outputAudioMixerGroup = s.audioMixer;
        }
    }

    private void Start()
    {
        foreach (Sound sound in sounds)
        {
            if (!DicoActualSound.ContainsKey(sound.ActualSound))
            {
                DicoActualSound.Add(sound.ActualSound, new List<Sound>());
            }
            DicoActualSound[sound.ActualSound].Add(sound);
        }
    }

    public void Stop(SoundState soundState)
    {
        if (DicoActualSound.ContainsKey(soundState))
        {
            foreach (Sound s in DicoActualSound[soundState])
            {
                if (s == null)
                {
                    Debug.LogWarning("Sound: " + name + " not found");
                    return;
                }
                s.source.Stop();
            }
        }
    }

    /// <summary>
    /// Allows a random sound to be played if you have several different sounds for the same action
    /// </summary>
    public void PlayRandom(SoundState soundState)
    {
        if (DicoActualSound.ContainsKey(soundState) && soundActivate)
        {
            int i = Random.Range(0, DicoActualSound[soundState].Count);

            Sound s = DicoActualSound[soundState][i];
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found");
                return;
            }
            s.source.Play();
        }
    }

    /// <summary>
    /// For the UI in Game
    /// </summary>
    public void SwitchSoundParameter()
    {
        soundActivate = !soundActivate;

        if (soundActivate)
            buttonImage.sprite = soundOn;
        
        else
            buttonImage.sprite = soundOff;
    }
}
