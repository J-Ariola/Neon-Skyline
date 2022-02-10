using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour {

    EventSystem ES;
    public string[] controllers;
    public string controllerType;
    public float playerID;

    GameObject pauseUI;

    bool finishedRace;
    bool submitButton;

    GameObject currentSelectedButton;

    AudioManager audioManager;
    GameObject manager;

	// Use this for initialization
	void Start () {

        ES = this.GetComponent<EventSystem>();
        manager = GameObject.FindGameObjectWithTag("AudioManager");

        if (manager != null)
            audioManager = manager.GetComponent<AudioManager>();
        pauseUI = GameObject.FindGameObjectWithTag("PauseUI");

        controllers = Input.GetJoystickNames();
        controllerType = controllers[0];
        playerID = 1;

        currentSelectedButton = ES.firstSelectedGameObject;

    }
	
	// Update is called once per frame
	void Update () {

        ////helps put the cursor on the first button for controllers
        //if (pauseUI != null)
        //{
        //    if ((pauseUI.GetComponent<PauseUI>().finishedRace && pauseUI.GetComponent<PauseUI>().isStoryMode == 0))
        //    {
        //        if (!finishedRace)
        //        {
        //            ES.SetSelectedGameObject(null);
        //            ES.SetSelectedGameObject(pauseUI.GetComponent<PauseUI>().buttons[0]);
        //            pauseUI.GetComponent<PauseUI>().buttons[0].GetComponent<Button>().OnSelect(null);
        //            finishedRace = true;
        //        }
        //    }

        //    if (pauseUI.GetComponent<PauseUI>().finishedRace && pauseUI.GetComponent<PauseUI>().isStoryMode == 1)
        //    {
        //        if (pauseUI.GetComponent<PauseUI>().playerWon)
        //        {
        //            ES.firstSelectedGameObject = pauseUI.GetComponent<PauseUI>().winUI[1];
        //            pauseUI.GetComponent<PauseUI>().winUI[1].GetComponent<Button>().OnSelect(null);
        //        }
        //        else
        //        {
        //            ES.firstSelectedGameObject = pauseUI.GetComponent<PauseUI>().loseUI[1];
        //            pauseUI.GetComponent<PauseUI>().loseUI[1].GetComponent<Button>().OnSelect(null);
        //        }
        //    }
        //}

        if(currentSelectedButton != ES.currentSelectedGameObject)
        {
            currentSelectedButton = ES.currentSelectedGameObject;
            if(audioManager != null)
            audioManager.Play("menuNavigation");
        }

        if (controllers[(int)playerID - 1].Equals(""))
        {
            return;
        }

        if (controllerType.ToLower().Contains("xbox"))
        {
            ES.GetComponent<StandaloneInputModule>().horizontalAxis = "Axis 6 Player " + playerID;
            ES.GetComponent<StandaloneInputModule>().verticalAxis = "Axis 7 Player " + playerID;
            ES.GetComponent<StandaloneInputModule>().submitButton = "XboxConfirmPlayer" + playerID;
        }

        if(controllerType.ToLower().Contains("wireless"))
        {
            ES.GetComponent<StandaloneInputModule>().horizontalAxis = "Axis 7 Player " + playerID;
            ES.GetComponent<StandaloneInputModule>().verticalAxis = "Axis 8 Player " + playerID;
            ES.GetComponent<StandaloneInputModule>().submitButton = "WirelessConfirmPlayer" + playerID;
        }


	}
}
