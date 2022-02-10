using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class MoveCursorLevel : MonoBehaviour
{

    public List<GameObject> levels;
    public GameObject selectedLevel;
    public int playerID;
    int currentLevelIndex;

    bool usingController;
    public string[] controllers;
    public string controlsType;

    //left stick on controller
    float leftStickAxis;
    float leftStickUpDown;
    //left and right on d pad
    float horizontalDPadAxis;
    float verticalDPadAxis;
    //X or A button on controller
    bool confirmButton;
    //B or O on controller
    bool backButton;

    public GameObject xButtonImage;
    public GameObject aButtonImage;
    public GameObject oButtonImage;
    public GameObject bButtonImage;

    //keeps the cursor from moving too fast
    public float moveTimer;
    float cursorTimer;
    float colorTimer;

    public GameObject generationUI;
    bool generationUIUp;

    GameObject manager;
    AudioManager audioManager;

    // Use this for initialization
    void Start()
    {

        if (levels != null)
        {
            this.GetComponent<RectTransform>().position = levels[0].GetComponent<RectTransform>().position;
        }

        manager = GameObject.FindGameObjectWithTag("AudioManager");

        if (manager != null)
            audioManager = manager.GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

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
        if (!usingController)
        {
            //move the cursor left
            if (CrossPlatformInputManager.GetButtonDown("MoveCursorLeft"))
            {
                currentLevelIndex--;
            }

            //move the cursor right
            if (CrossPlatformInputManager.GetButtonDown("MoveCursorRight"))
            {
                currentLevelIndex++;
            }
        }

        //controller input
        if (usingController)
        {
            //move the cursor left
            if ((leftStickAxis < -.5f || horizontalDPadAxis < -.5f) /*&& !generationUIUp*/)
            {
                cursorTimer -= Time.deltaTime;
                if (cursorTimer < 0)
                {
                    currentLevelIndex--;
                    if(audioManager != null)
                        audioManager.Play("menuNavigation");
                    cursorTimer = moveTimer;
                }
            }
            //move the cursor right
            else if ((leftStickAxis > .5f || horizontalDPadAxis > .5f) /*&& !generationUIUp*/)
            {
                cursorTimer -= Time.deltaTime;
                if (cursorTimer < 0)
                {
                    currentLevelIndex++;
                    if (audioManager != null)
                        audioManager.Play("menuNavigation");
                    cursorTimer = moveTimer;
                }
            }
            //move the cursor Down
            else if ((leftStickUpDown > .5f || verticalDPadAxis < -.5f) /*&& !generationUIUp*/)
            {
                cursorTimer -= Time.deltaTime;
                if (cursorTimer < 0)
                {
                    if (currentLevelIndex + 3 < levels.Count)
                        currentLevelIndex += 3;
                    if (audioManager != null)
                        audioManager.Play("menuNavigation");
                    cursorTimer = moveTimer;
                }
            }
            //move the cursor Up
            else if ((leftStickUpDown < -.5f || verticalDPadAxis > .5f) /*&& !generationUIUp*/)
            {
                cursorTimer -= Time.deltaTime;
                if (cursorTimer < 0)
                {
                    if (currentLevelIndex - 3 >= 0)
                        currentLevelIndex -= 3;
                    if (audioManager != null)
                        audioManager.Play("menuNavigation");
                    cursorTimer = moveTimer;
                }
            }

            else
            {
                cursorTimer = 0;
            }
            
            if (/*generationUIUp & */confirmButton)
            {
                levels[currentLevelIndex].GetComponent<ChangeSelectedImage>().selectedLevel.GetComponent<SelectedLevel>().ConfirmLevel();
                this.GetComponent<SpriteRenderer>().enabled = false;
            }

            //player confirms character
            if (confirmButton)
            {
                if (audioManager != null)
                    audioManager.Play("menuNavigation");
                if (!selectedLevel.GetComponent<SelectedLevel>().levelName.ToLower().Contains("procedural"))
                {
                    levels[currentLevelIndex].GetComponent<ChangeSelectedImage>().selectedLevel.GetComponent<SelectedLevel>().ConfirmLevel();
                    this.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    //generation UI appears
                    //generationUI.SetActive(true);
                    //generationUIUp = true;
                }
            }

            //player hits back button
            if (backButton)
            {
                if (audioManager != null)
                    audioManager.Play("menuNavigation");
                if (!generationUIUp)
                {
                    levels[currentLevelIndex].GetComponent<ChangeSelectedImage>().selectedLevel.GetComponent<SelectedLevel>().Back();
                }
                else
                {
                    //generation UI disappears
                    //generationUI.SetActive(false);
                    //generationUIUp = false;
                }
            }

            
        }

        //if the cursor can't move left anymore, wrap around to the end
        if (currentLevelIndex < 0)
        {
            currentLevelIndex = levels.Count - 1;
        }

        //if the cursor can't move right anymore, wrap around to the front
        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
        }





        //move the cursor
        this.GetComponent<RectTransform>().position = levels[currentLevelIndex].GetComponent<RectTransform>().position;
        levels[currentLevelIndex].GetComponent<ChangeSelectedImage>().SelectedLevel();
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
                leftStickUpDown =  CrossPlatformInputManager.GetAxis("MoveCursorPlayerUp");
                horizontalDPadAxis = CrossPlatformInputManager.GetAxis("Axis 6 Player " + playerID);
                verticalDPadAxis = CrossPlatformInputManager.GetAxis("Axis 7 Player " + playerID);
                confirmButton = CrossPlatformInputManager.GetButtonDown("XboxConfirmPlayer" + playerID);
                backButton = CrossPlatformInputManager.GetButtonDown("XboxBackPlayer" + playerID);

                xButtonImage.SetActive(false);
                aButtonImage.SetActive(true);
                oButtonImage.SetActive(false);
                bButtonImage.SetActive(true);
            }
            else if (controlsType.ToLower().Contains("wireless"))
            {
                //setup controls
                leftStickAxis = CrossPlatformInputManager.GetAxis("MoveCursorPlayer" + playerID);
                leftStickUpDown =  CrossPlatformInputManager.GetAxis("MoveCursorPlayerUp");
                horizontalDPadAxis = CrossPlatformInputManager.GetAxis("Axis 7 Player " + playerID);
                verticalDPadAxis = CrossPlatformInputManager.GetAxis("Axis 8 Player " + playerID);
                confirmButton = CrossPlatformInputManager.GetButtonDown("WirelessConfirmPlayer" + playerID);
                backButton = CrossPlatformInputManager.GetButtonDown("WirelessBackPlayer" + playerID);

                //change buttons
                xButtonImage.SetActive(true);
                aButtonImage.SetActive(false);
                oButtonImage.SetActive(true);
                bButtonImage.SetActive(false);
            }
            else
            {
                xButtonImage.SetActive(false);
                aButtonImage.SetActive(false);
                oButtonImage.SetActive(false);
                bButtonImage.SetActive(false);
            }
        }
    }
}
