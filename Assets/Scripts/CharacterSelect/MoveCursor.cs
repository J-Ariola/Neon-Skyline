using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class MoveCursor : MonoBehaviour {

    public List<GameObject> characters;
    public GameObject selectedCharacter;
    public GameObject characterCar;
    public int playerID;
    int currentCharacterIndex;

    //change colors of car
    int currentColorIndex;

    bool usingController;
    public string[] controllers;
    public string controlsType;

    //left stick on controller
    float leftStickAxis;
    //left and right on d pad
    float horizontalDPadAxis;
    //X or A button on controller
    bool confirmButton;
    bool characterConfirmed;
    //B or O on controller
    bool backButton;
    //start button on controller
    bool startButton;
    //LB and RB of controller to change colors of car
    bool changeColorLeftButton;
    bool changeColorRightButton;

    public GameObject xButtonImage;
    public GameObject aButtonImage;
    public GameObject oButtonImage;
    public GameObject bButtonImage;
    public GameObject l1ButtonImage;
    public GameObject r1ButtonImage;
    public GameObject lbButtonImage;
    public GameObject rbButtonImage;

    //keeps the cursor from moving too fast
    public float moveTimer;
    float cursorTimer;
    float colorTimer;

    //ensure that all player's flags have been checked
    public GameObject readyFlags;

    GameObject manager;
    AudioManager audioManager;

    // Use this for initialization
    void Start () {

        if(characters != null)
        {
            this.GetComponent<RectTransform>().position = characters[0].GetComponent<RectTransform>().position;
        }
        //not in story mode if in character select, so fail safe check that we are in arcade
        PlayerPrefs.SetInt("IsStoryMode", 0);
        cursorTimer = .1f;

        manager = GameObject.FindGameObjectWithTag("AudioManager");

        if (manager != null)
            audioManager = manager.GetComponent<AudioManager>();
    }
	
	// Update is called once per frame
	void Update () {

        controllers = Input.GetJoystickNames();
        ControllerType();

        //checks whether a controller is plugged in 
        if (controllers.Length != 0)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (!controllers[i].Equals(""))
                {
                    usingController = true;
                    break;
                }
                else
                {
                    usingController = false;
                }
            }
        }
        else
        {
            usingController = false;
        }

        //keyboard input
        if (!usingController && !characterConfirmed)
        {
            //move the cursor left
            if (CrossPlatformInputManager.GetButtonDown("MoveCursorLeft"))
            {
                currentCharacterIndex--;
            }

            //move the cursor right
            if (CrossPlatformInputManager.GetButtonDown("MoveCursorRight"))
            {
                currentCharacterIndex++;
            }
        }
        
        //controller input
        if(usingController)
        {
            //move the cursor left
            if ((leftStickAxis < -.5f || horizontalDPadAxis < -.5f) && !characterConfirmed)
            {
                cursorTimer -= Time.deltaTime;
                if (cursorTimer < 0)
                {
                    currentCharacterIndex--;
                    if(audioManager != null)
                    audioManager.Play("menuNavigation");
                    cursorTimer = moveTimer;
                    //change cars
                    characterCar.GetComponent<CharacterCars>().ChangeCars();
                }
            }
            //move the cursor right
            else if ((leftStickAxis > .5f || horizontalDPadAxis > .5f) && !characterConfirmed)
            {
                cursorTimer -= Time.deltaTime;
                if (cursorTimer < 0)
                {
                    currentCharacterIndex++;
                    if(audioManager != null)
                    audioManager.Play("menuNavigation");
                    cursorTimer = moveTimer;
                    //change cars
                    characterCar.GetComponent<CharacterCars>().ChangeCars();
                }
            }
            else
            {
                cursorTimer = 0;
            }

            //player confirms character
            if(confirmButton && !characterConfirmed)
            {
                if (audioManager != null)
                    audioManager.Play("menuNavigation");
                selectedCharacter.GetComponent<CharacterSelect>().ConfirmCharacter();
                characterConfirmed = true;
            }

            //player hits back button
            if(backButton)
            {
                if (audioManager != null)
                    audioManager.Play("menuNavigation");
                selectedCharacter.GetComponent<CharacterSelect>().Back();
                characterConfirmed = false;
            }

            if(startButton && readyFlags.GetComponent<ReadyFlags>().allFlagsClear)
            {
                selectedCharacter.GetComponent<ButtonToScene>().TransitionToScene();
            }

            if(changeColorLeftButton)
            {
                colorTimer -= Time.deltaTime;
                if (colorTimer < 0)
                {
                    currentColorIndex--;
                    if(audioManager != null)
                    audioManager.Play("menuNavigation");
                    //move to last entry in array
                    if (currentColorIndex < 0)
                    {
                        currentColorIndex = characterCar.GetComponent<CharacterCars>().carMaterials.Count - 1;
                    }
                    colorTimer = moveTimer;
                    //change cars
                    characterCar.GetComponent<CharacterCars>().ChangeCarColors(currentColorIndex);
                }
            }
            else if(changeColorRightButton)
            {
                colorTimer -= Time.deltaTime;
                if (colorTimer < 0)
                {
                    currentColorIndex++;
                    if(audioManager != null)
                    audioManager.Play("menuNavigation");
                    //move back to first entry in array
                    if (currentColorIndex >= characterCar.GetComponent<CharacterCars>().carMaterials.Count)
                    {
                        currentColorIndex = 0;
                    }
                    colorTimer = moveTimer;
                    //change cars
                    characterCar.GetComponent<CharacterCars>().ChangeCarColors(currentColorIndex);
                }
            }
            else
            {
                colorTimer = 0;
            }
        }

        //if the cursor can't move left anymore, wrap around to the end
        if (currentCharacterIndex < 0)
        {
            currentCharacterIndex = characters.Count - 1;
        }

        //if the cursor can't move right anymore, wrap around to the front
        if(currentCharacterIndex >= characters.Count)
        {
            currentCharacterIndex = 0;
        }

        

        

        //move the cursor
        this.GetComponent<RectTransform>().position = characters[currentCharacterIndex].GetComponent<RectTransform>().position;
        characters[currentCharacterIndex].GetComponent<ChangeSelectedImage>().SelectedCharacter();

        //car spawn
        characterCar.GetComponent<CharacterCars>().SpawnCar();
    }

    public void ControllerType()
    {
        //determines the control type for this player
        if (playerID <= controllers.Length)
        {
            if (controllers.Length != 0)
            {
                controlsType = controllers[(int)playerID - 1];
            }
        }
        for (int i = 1; i <= controllers.Length; i++)
        {
            if (controlsType.ToLower().Contains("xbox"))
            {
                //setup controls
                leftStickAxis = CrossPlatformInputManager.GetAxis("MoveCursorPlayer" + playerID);
                horizontalDPadAxis = CrossPlatformInputManager.GetAxis("Axis 6 Player " + playerID);
                confirmButton = CrossPlatformInputManager.GetButtonDown("XboxConfirmPlayer" + playerID);
                backButton = CrossPlatformInputManager.GetButtonDown("XboxBackPlayer" + playerID);
                changeColorLeftButton = CrossPlatformInputManager.GetButtonDown("ColorLeftPlayer" + playerID);
                changeColorRightButton = CrossPlatformInputManager.GetButtonDown("ColorRightPlayer" + playerID);
                startButton = CrossPlatformInputManager.GetButtonDown("XboxConfirmContinue");

                xButtonImage.SetActive(false);
                aButtonImage.SetActive(true);
                oButtonImage.SetActive(false);
                bButtonImage.SetActive(true);
                //l1ButtonImage.SetActive(false);
                //lbButtonImage.SetActive(true);
                //r1ButtonImage.SetActive(false);
                //rbButtonImage.SetActive(true);
            }
            else if (controlsType.ToLower().Contains("wireless"))
            {
                //setup controls
                leftStickAxis = CrossPlatformInputManager.GetAxis("MoveCursorPlayer" + playerID);
                horizontalDPadAxis = CrossPlatformInputManager.GetAxis("Axis 7 Player " + playerID);
                confirmButton = CrossPlatformInputManager.GetButtonDown("WirelessConfirmPlayer" + playerID);
                backButton = CrossPlatformInputManager.GetButtonDown("WirelessBackPlayer" + playerID);
                startButton = CrossPlatformInputManager.GetButtonDown("WirelessConfirmContinue");
                changeColorLeftButton = CrossPlatformInputManager.GetButtonDown("ColorLeftPlayer" + playerID);
                changeColorRightButton = CrossPlatformInputManager.GetButtonDown("ColorRightPlayer" + playerID);

                //change buttons
                xButtonImage.SetActive(true);
                aButtonImage.SetActive(false);
                oButtonImage.SetActive(true);
                bButtonImage.SetActive(false);
                //l1ButtonImage.SetActive(true);
                //lbButtonImage.SetActive(false);
                //r1ButtonImage.SetActive(true);
                //rbButtonImage.SetActive(false);

            }
            else
            {
                xButtonImage.SetActive(false);
                aButtonImage.SetActive(false);
                oButtonImage.SetActive(false);
                bButtonImage.SetActive(false);
                //l1ButtonImage.SetActive(false);
                //lbButtonImage.SetActive(false);
                //r1ButtonImage.SetActive(false);
                //rbButtonImage.SetActive(false);
            }
        }
    }
}
