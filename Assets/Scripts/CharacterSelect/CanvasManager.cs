using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{

    public List<GameObject> playerCanvas;
    public string[] controllers;

    // Use this for initialization
    void Start()
    {
        for (int i = 1; i < playerCanvas.Count; i++)
        {
            playerCanvas[i].SetActive(false);
        }

        controllers = Input.GetJoystickNames();

        if (controllers.Length != 0)
        {
            //if two controllers are plugged in, enable camera 2 and disable others
            if (controllers.Length >= 2 && !controllers[1].Equals(""))
            {
                playerCanvas[1].SetActive(true);
            }
            //if three controllers are plugged in, enable camera 2 and 3 and disable others
            if (controllers.Length >= 3 && !controllers[2].Equals(""))
            {
                playerCanvas[1].SetActive(true);
                playerCanvas[2].SetActive(true);
            }
            //if 4 controllers are plugged in, enable all cameras
            if (controllers.Length >= 4 && !controllers[3].Equals(""))
            {
                for (int i = 0; i < playerCanvas.Count; i++)
                {
                    playerCanvas[i].SetActive(true);
                }
            }
        }
        //if there are no controllers plugged in(keyboard), then disable all cameras except for camera 1

    }
}
