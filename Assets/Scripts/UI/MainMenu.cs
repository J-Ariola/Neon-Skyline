using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    public AudioManager audioManager; 

	// Use this for initialization
	void Start () {
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("Title Song - Neon Skyline");
        //メニューでマウス使いなくなる
        //Cursor.lockState = CursorLockMode.Locked;
	}

    void OnDestroy()
    {
        audioManager.Stop("Title Song - Neon Skyline");
    }
}
