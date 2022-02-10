using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.MultiAudioListener;

//script is used to manage the local audio coming from cars within a race, such as engine sounds, item usage, etc. this allows for more 3D sound

public class LocalAudioManager : MonoBehaviour {

    public Sound[] sounds;

    public List<MultiAudioSource> audioSources;

	// Use this for initialization
	void Awake () {

        foreach(Sound s in sounds)
        {
            MultiAudioSource currentAudioSource = gameObject.AddComponent<MultiAudioSource>();
            currentAudioSource.AudioClip = s.clip;
            currentAudioSource.Volume = s.volume;
            currentAudioSource.Pitch = s.pitch;
            currentAudioSource.Loop = s.loop;
            audioSources.Add(currentAudioSource);
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
