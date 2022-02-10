using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public List<Camera> playerCameras;
    public string[] controllers;

	// Use this for initialization
	void Start () {

        if(playerCameras.Count != 0)
        {
            playerCameras[0].rect = new Rect(0, 0, 1, 1);
        }

        for (int i = 1; i < playerCameras.Count; i++)
        {
            playerCameras[i].enabled = false;
        }

        if (PlayerPrefs.GetInt("IsStoryMode") == 1)
        {
            return;
        }

        controllers = Input.GetJoystickNames();

        if (controllers.Length != 0)
        {
            //if one controller is plugged in, disable other cameras
            if (controllers.Length >= 1 && !controllers[0].Equals(""))
            {
                playerCameras[0].rect = new Rect(0, 0, 1, 1);
                for (int i = 1; i < playerCameras.Count; i++)
                {
                    playerCameras[i].enabled = false;
                }
            }
            //if two controllers are plugged in, enable camera 2 and disable others
            if (controllers.Length >= 2 && !controllers[1].Equals(""))
            {
                playerCameras[0].rect = new Rect(0, 0.5f, 1, 0.5f);
                playerCameras[1].enabled = true;
                playerCameras[1].rect = new Rect(0, 0, 1, 0.5f);
                for (int i = 2; i < playerCameras.Count; i++)
                {
                    playerCameras[i].enabled = false;
                }
            }
            //if three controllers are plugged in, enable camera 2 and 3 and disable others
            if (controllers.Length >= 3 && !controllers[2].Equals(""))
            {
                playerCameras[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                playerCameras[1].enabled = true;
                playerCameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                playerCameras[2].enabled = true;
                playerCameras[2].rect = new Rect(0, 0, 0.5f, 0.5f);
                playerCameras[3].enabled = true;
                playerCameras[3].rect = new Rect(0.5f, 0, 0.5f, 0.5f);
            }
            //if 4 controllers are plugged in, enable all cameras
            if (controllers.Length >= 4 && !controllers[3].Equals(""))
            {
                playerCameras[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                playerCameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                playerCameras[2].rect = new Rect(0, 0, 0.5f, 0.5f);
                playerCameras[3].rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                for (int i = 0; i < playerCameras.Count; i++)
                {
                    playerCameras[i].enabled = true;
                }
            }
        }
        //if there are no controllers plugged in(keyboard), then disable all cameras except for camera 1
        else
        {
            playerCameras[0].rect = new Rect(0, 0, 1, 1);
            
        }

    }
	
	// Update is called once per frame
	void Update () {

        

    }
}
