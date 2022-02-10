using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SOURCE: https://github.com/GaelVanhalst/MultiAudioListener
using Assets.MultiAudioListener;

public class CarSound : MonoBehaviour {

    HoverCarController racer;

    float minPitch = .5f;
    float maxPitch = 4f;

    MultiAudioSource lowAccel;
    LocalAudioManager audioManager;

    void Start()
    {
        racer = this.GetComponent<HoverCarController>();
        audioManager = this.GetComponent<LocalAudioManager>();
        
        for(int i = 0; i < audioManager.audioSources.Count; i++)
        {
            if(audioManager.audioSources[i].AudioClip.name.ToLower().Contains("accel"))
            {
                lowAccel = audioManager.audioSources[i];
            }
        }


        lowAccel.VolumeRolloff = AudioRolloffMode.Linear;
        lowAccel.MaxDistance = 50;
        //if (this.GetComponent<HoverCarController>().driver == HoverCarController.driverEnum.AI)
        //lowAccel.OnlyPlayForClosestCamera = true;
        lowAccel.Play();

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        float pitch = racer.currentSpeed / 10;

        lowAccel.Pitch = pitch;

        if (lowAccel.Pitch < minPitch)
        {
            lowAccel.Pitch = minPitch;
        }

        if (lowAccel.Pitch > maxPitch)
        {
            lowAccel.Pitch = maxPitch;
        }




    }

}
