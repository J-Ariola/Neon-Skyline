using UnityEngine.Audio;
using UnityEngine;
using System;


//Original code by Brackeys
//This implementation and instructions by Hunter Jones

//How to use: Place the AudioManager prefab in every scene optimally, but at least the main menu
//To play a sound in any other script, type the following line:

//FindObjectOfType<AudioManager>().Play("SoundName");

//Obviously replace SoundName with whatever the name of the sound file is in the manager object.
//If you plan on calling sounds many times within a script, you may want to store the sound manager in a variable to save processing power
//In Start():

//AudioManager am = FindObjectOfType<AudioManager>();

//And whenever a sound needs to be called:

//am.Play("SoundName");

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void PlayRandPitch(string name)
    {
        System.Random rand = new System.Random();

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        float pitchMod = rand.Next(10);
        float originalPitch = s.source.pitch;
        pitchMod = ((0.4f * originalPitch) / pitchMod) + (0.8f*originalPitch);
        s.source.pitch = pitchMod;
        s.source.Play();
        s.source.pitch = originalPitch;
    }
    
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void Stop()
    {
        foreach(Sound s in sounds)
        {
            s.source.Stop();
        }
    }

}
