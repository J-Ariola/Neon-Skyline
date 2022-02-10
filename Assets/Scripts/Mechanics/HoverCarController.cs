using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kino;
using System;
using Assets.MultiAudioListener;

public class HoverCarController : MonoBehaviour {
    
    public enum driverEnum {Player, AI};
    [Header("Driver")]
    public driverEnum driver;
    
    [Header("Speed Variables")]
    [Range(1, 250)]
    public float speed = 50.0f;
    [Range(1, 250)]
    public float minSpeedForTrick;
    private Vector3 currentVelocity;
    public float currentSpeed;

    [Header("Turn Variables")]
    [Range(1, 250)]
    public float turnSpeed = 5.0f;
    private float defaultTurnSpeed;
    [Range(1, 500)]
    public float driftTurnSpeed;
    
    [Header("Air Time")]
    //Max air time the car can be before you can no longer accelerate
    [Range(0, 5)]
    public float airTimeMax = 0.5f;

    [Header("Drift Variables")]
    public float driftTimer;
    //Max drift time before you can apply acceleration
    [Range(0, 5)]
    public float driftTimeMax = 1.0f;
    [Range(0, 5)]
    public float driftTimeMin = 0.15f;
    //lets the drift timer decrement and prevents the player from boosting until the drift timer has decremented fully
    public float driftCooldown;
    [Range(1, 5)]
    public float driftMagnitude = 1.5f;
    //allows for the car to keep a bit of momentum when drifting
    [Range(0, 1)]
    public float driftClamp;

    [Header("Accerleration Variables")]
    [Range(1, 10)]
    //The time it takes to fully accelerate
    public float accelRate = 4.0f;
    public bool isAccelerating;

    [Header("Tricks and Power Ups")]
    [Range(0.5f, 5)]
    public float trickAnimationTime = 1.000f;
    [Range(0, 1)]
    public float boostMultiplier = 0.5f;
    [Range(0, 1)]
    public float damageReductionMultiplier = 0.25f;
    [Range(0, 1)]
    public float damageAmplificationMultiplier = 0.25f;
    public int amplifiedDamageDivisor;
    [Range(1,10)]
    public float powerUpTimerMax = 5.0f;
    public enum powerUpEnum { Boost, Restoration, Defense, Attack, None };
    public powerUpEnum storedPowerUp, currentPowerUp;
    private float powerUpTimer;
    private bool usingPowerUp;
    private bool currentlyPerformingTrick = false;

    [Header("Miscellaneous")]
    //Time it takes for the car to reset back on track
    [Range(1, 5)]
    public float carResetTimer = 3.5f;
    public bool resetCar;
    bool resetButton;

    //hold both of these buttons down to reset car
    bool L3ButtonDown;
    bool R3ButtonDown;
    float selfDestructTimer;
    //1 is player 1, 2 is player 2, 3 is player 3, 4 is player 4
    public float playerID;
    public int damageDivisor = 500;
    [Range(1, 5)]
    public float respawnTimerMax = 3.0f;
    public float respawnTimer;
    private float currentForceApplied;

    //Can be affected by the drag of the rigidbody
    private float powerInput;
    //Can be affected by the angular drag of the rigidbody
    private float turnInput;
    private bool driftInput;

    //controller related inputs
    private float accelInput;
    private float brakeInput;

    //inputs for tricks
    private float trickUpDownInput;
    private float trickLeftRightInput;

    //input for item usage
    private bool itemUse;

    private bool usingController;

    private Rigidbody carRigidbody;
    GravityController gravityController;
    LoadCharacterCar loadCar;
    GameObject currentModel;
    Animator currentAnimator;
    public Animator powerupAnimator;
    ParticleSystem boost;
    public RuntimeAnimatorController animController;

    public string[] controllers;
    //will store the current controller being used by this player
    public string controlsType;

    float airTimer;
    
    float driftPower;
    bool isDrifting;
    public bool canPerformTrick;
    public bool hasPowerUp;
    [Range(.1f, 10f)]
    public float resetPosOffset = 0.75f;
    [Range(1f, 10f)]
    public float resetTimerCooldown = 2f;
    private float resetTimer;
    public bool justResetCar;
    //private bool toResetCar;
    //check to see if race started
    public bool raceStarted;
    private float accelTimer;
    private float normalSpeed;
    private int normalDamageDivisor;
    public bool isAlive = true;

    //Variables relevant to teleporting in procedural races
    GameObject teleSource, teleDest;
    bool isTeleported;
    public GameObject myCamera;

    //pause game
    bool startButton;
    GameObject pauseUI;

    EventSystem ES;

    MultiAudioSource boostAcquired;
    MultiAudioSource boostDeployed;
    MultiAudioSource repairAcquired;
    MultiAudioSource repairDeployed;
    MultiAudioSource shieldAcquired;
    MultiAudioSource shieldDeployed;
    MultiAudioSource spikesAcquired;
    MultiAudioSource spikesDeployed;

    LocalAudioManager audioManager;

    public float currentPlace;

    void Awake() {

        carRigidbody = GetComponent<Rigidbody>();
        gravityController = GetComponent<GravityController>();

        defaultTurnSpeed = turnSpeed;

        canPerformTrick = false;

        normalSpeed = speed;
        normalDamageDivisor = damageDivisor;
        amplifiedDamageDivisor = (int)(normalDamageDivisor * (1.0f - damageAmplificationMultiplier));

        switch (driver)
        {
            case driverEnum.Player:
                //GetComponent<CarAIController>().enabled = false;
                break;
            case driverEnum.AI:
                GetComponent<CarAIController>().enabled = true;
                break;
        }

    }

    void Start()
    {
        
        //loadCar = GetComponent<LoadCharacterCar>();
        //currentModel = loadCar.currentCarModel;
        GameObject carFrame = transform.Find("CarFrame").gameObject;
        if (pauseUI == null)
        {
            pauseUI = GameObject.FindGameObjectWithTag("PauseUI");
        }

        if (carFrame.GetComponent<Animator>() != null)
        {
            Debug.Log("Successfully has access to the Animator of the car model");
            currentAnimator = carFrame.GetComponent<Animator>();
            if (animController != null)
            {
                currentAnimator.runtimeAnimatorController = animController;
                Debug.Log("Successfully set the animation controller to " + this.name);
            }
            powerupAnimator = carFrame.transform.Find("Powerups").GetComponent<Animator>();
            if (powerupAnimator != null)
                Debug.Log("Found the Powerup Animator as well.");
            boost = carFrame.transform.Find("Powerups").Find("Boost").GetComponent<ParticleSystem>();
            boost.Stop();
        }
        else
            Debug.LogError("Can't access the Animator of the carFrame of " + this.name);

        storedPowerUp = powerUpEnum.None;
        usingPowerUp = false;

        //Sets the carCanvas' parent to carFrame
        GameObject carCanvas = transform.Find("CarCanvas").gameObject;
        carCanvas.transform.SetParent(carFrame.transform, false);

        teleSource = GameObject.Find("TeleSource");
        teleDest = GameObject.Find("TeleDestination");

        if(this.driver != HoverCarController.driverEnum.AI)
        myCamera = GameObject.Find("Player" + playerID + "Camera");


        //audio stuff
        audioManager = this.GetComponent<LocalAudioManager>();

        for (int i = 0; i < audioManager.audioSources.Count; i++)
        {
            if (audioManager.audioSources[i].AudioClip.name.ToLower().Contains("boostacquired"))
            {
                boostAcquired = audioManager.audioSources[i];
                boostAcquired.VolumeRolloff = AudioRolloffMode.Linear;
                boostAcquired.MaxDistance = 50;
                boostAcquired.Volume = .75f;
            }

            if(audioManager.audioSources[i].AudioClip.name.ToLower().Contains("boostdeployed"))
            {
                boostDeployed = audioManager.audioSources[i];
                boostDeployed.VolumeRolloff = AudioRolloffMode.Linear;
                boostDeployed.MaxDistance = 50;
            }

            if (audioManager.audioSources[i].AudioClip.name.ToLower().Contains("repairacquired"))
            {
                repairAcquired = audioManager.audioSources[i];
                repairAcquired.VolumeRolloff = AudioRolloffMode.Linear;
                repairAcquired.MaxDistance = 50;
            }

            if (audioManager.audioSources[i].AudioClip.name.ToLower().Contains("repairdeployed"))
            {
                repairDeployed = audioManager.audioSources[i];
                repairDeployed.VolumeRolloff = AudioRolloffMode.Linear;
                repairDeployed.MaxDistance = 50;
            }

            if (audioManager.audioSources[i].AudioClip.name.ToLower().Contains("shieldacquired"))
            {
                shieldAcquired = audioManager.audioSources[i];
                shieldAcquired.VolumeRolloff = AudioRolloffMode.Linear;
                shieldAcquired.MaxDistance = 50;
            }

            if (audioManager.audioSources[i].AudioClip.name.ToLower().Contains("shielddeployed"))
            {
                shieldDeployed = audioManager.audioSources[i];
                shieldDeployed.VolumeRolloff = AudioRolloffMode.Linear;
                shieldDeployed.MaxDistance = 50;
            }

            if (audioManager.audioSources[i].AudioClip.name.ToLower().Contains("spikesacquired"))
            {
                spikesAcquired = audioManager.audioSources[i];
                spikesAcquired.VolumeRolloff = AudioRolloffMode.Linear;
                spikesAcquired.MaxDistance = 50;
            }

            if (audioManager.audioSources[i].AudioClip.name.ToLower().Contains("spikesdeployed"))
            {
                spikesDeployed = audioManager.audioSources[i];
                spikesDeployed.VolumeRolloff = AudioRolloffMode.Linear;
                spikesDeployed.MaxDistance = 50;
            }
        }
    }

    private void Update()
    {

        //transform.Rotate(-Vector3.up, 30f * Time.deltaTime);

        ControllerType();
        if (ES == null)
        {
            ES = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
        }

        if(GetComponent<Lapping>().finishedRace)
        {
            ES.GetComponent<MenuNavigation>().playerID = playerID;
            ES.GetComponent<MenuNavigation>().controllerType = controlsType;
        }

        if (startButton && pauseUI != null && pauseUI.GetComponent<PauseUI>().canBePaused)
        {
            //Debug.Log("Queue Pause");
            //pauseUI.GetComponent<PauseUI>().canBePaused = false;
            pauseUI.GetComponent<PauseUI>().isPaused = !pauseUI.GetComponent<PauseUI>().isPaused;
            Debug.Log(pauseUI.GetComponent<PauseUI>().isPaused);

            if (pauseUI.GetComponent<PauseUI>().isPaused)
                pauseUI.GetComponent<PauseUI>().enablePauseButtons();
            else
                pauseUI.GetComponent<PauseUI>().disablePauseButtons();

            pauseUI.GetComponent<PauseUI>().canBePaused = false;

            ES.SetSelectedGameObject(pauseUI.GetComponent<PauseUI>().buttons[0]);
            pauseUI.GetComponent<PauseUI>().buttons[0].GetComponent<Button>().OnSelect(null);
            ES.GetComponent<MenuNavigation>().playerID = playerID;
            ES.GetComponent<MenuNavigation>().controllerType = controlsType;
        }
    }

    void FixedUpdate()
    {
        if (!raceStarted)
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= .5f;
            }
            return;
        }

        

        controllers = Input.GetJoystickNames();
        switch (driver)
        {
            case driverEnum.Player:
                //checks whether a controller is plugged in 
                ControllerChecking();


                //controller inputs, checks through list of all controllers plugged in
                if (usingController)
                {
                    
                    //for(int i = 1; i <= controllers.Length; i++)
                    //{
                    //    accelInput = CrossPlatformInputManager.GetAxis("Axis 5 Player " + playerID);
                    //    brakeInput = CrossPlatformInputManager.GetAxis("Axis 4 Player " + playerID);
                    //    turnInput = CrossPlatformInputManager.GetAxis("Horizontal Player " + playerID);
                    //}
                }


                //keyboard inputs
                if (!usingController)
                {
                    powerInput = CrossPlatformInputManager.GetAxis("Vertical");
                    turnInput = CrossPlatformInputManager.GetAxis("Horizontal");
                    driftInput = CrossPlatformInputManager.GetButton("Drift");
                    //if no controllers are plugged in, disable other cars
                    for (int i = 2; i <= 4; i++)
                    {
                        if (playerID == i)
                        {
                            this.enabled = false;
                        }
                    }
                }
                break;

            case driverEnum.AI:
                break;
        }

        //Debug.Log(accelInput);

        //clamp values from 0 to 1, 0 meaning there is no pressure on trigger button, 1 meaning there is full pressure on trigger button
        accelInput = Mathf.Clamp(accelInput, 0, 1.0f);
        //accelInput = scale(-1f, 1f, 0f, 1f, accelInput);
        //brakeInput = scale(-1f, 1f, 0f, 1f, brakeInput);
        //Debug.Log(accelInput);
        brakeInput = Mathf.Clamp(brakeInput, 0, 1.0f);
        //Debug.Log(accelInput - brakeInput);

        if (isAlive)
        {
            switch (driver)
            {
                case driverEnum.Player:
                    //Check if car is drifting
                    DriftHandling();
                    //Handles driving mechanics
                    MotionHandling();
                    //reset car on race track
                    ResetCarHandling();
                    break;
                case driverEnum.AI:
                    break;
            }
        }
        else
        {
            if (respawnTimer < respawnTimerMax)
            {
                respawnTimer += Time.deltaTime;
            }
            else
            {
                respawnTimer = 0;
                ResetCarOnTrack();
                GetComponent<HealthController>().IncrementHealth(GetComponent<HealthController>().MaxHealth);
            }
        }

        //Find the speed of the car
        currentVelocity = carRigidbody.velocity;
        currentSpeed = currentVelocity.magnitude;


        //Perform tricks if you meet requirements
        if (canPerformTrick)
            TrickHandling();

        

        PowerUpHandling();
    }



    void ControllerChecking()
    {
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
        //Debug.Log(usingController);


    }

    //determines the controller used by the player and sets axes correctly
    public void ControllerType()
    {
        //determines the control type for this player
        if (playerID <= controllers.Length)
        {
            controlsType = controllers[(int)playerID - 1];
        }
        for (int i = 1; i <= controllers.Length; i++)
        {
            //xboxコントローラーを確認
            if (controlsType.ToLower().Contains("xbox"))
            {
                //Debug.Log(controlsType.ToLower() + " detected");
                //RT
                accelInput = CrossPlatformInputManager.GetAxis("Axis 10 Player " + playerID);
                //LT
                brakeInput = CrossPlatformInputManager.GetAxis("Axis 9 Player " + playerID);
                //Left Stick
                turnInput = CrossPlatformInputManager.GetAxis("Horizontal Player " + playerID);
                //DPad
                trickLeftRightInput = CrossPlatformInputManager.GetAxis("Axis 6 Player " + playerID);
                trickUpDownInput = CrossPlatformInputManager.GetAxis("Axis 7 Player " + playerID);
                //Right Bumper
                itemUse = CrossPlatformInputManager.GetButton("ColorRightPlayer" + playerID);
                startButton = CrossPlatformInputManager.GetButtonDown("XboxConfirmContinuePlayer" + playerID);
                //Debug.Log(startButton);
               // resetButton = CrossPlatformInputManager.GetButtonDown("XboxResetPlayer" + playerID);
                L3ButtonDown = CrossPlatformInputManager.GetButton("XboxL3Player" + playerID);
                R3ButtonDown = CrossPlatformInputManager.GetButton("XboxR3Player" + playerID);

                //if ()
            }
            else if (controlsType.ToLower().Contains("wireless"))
            {
                accelInput = CrossPlatformInputManager.GetAxis("Axis 5 Player " + playerID);
                //Debug.Log(accelInput);
                brakeInput = CrossPlatformInputManager.GetAxis("Axis 4 Player " + playerID);
                turnInput = CrossPlatformInputManager.GetAxis("Horizontal Player " + playerID);
                trickLeftRightInput = CrossPlatformInputManager.GetAxis("Axis 7 Player " + playerID);
                trickUpDownInput = CrossPlatformInputManager.GetAxis("Axis 8 Player " + playerID);
                itemUse = CrossPlatformInputManager.GetButton("ColorRightPlayer" + playerID);
                startButton = CrossPlatformInputManager.GetButtonDown("WirelessConfirmContinuePlayer" + playerID);
                //resetButton = CrossPlatformInputManager.GetButtonDown("WirelessResetPlayer" + playerID);
                L3ButtonDown = CrossPlatformInputManager.GetButton("WirelessL3Player" + playerID);
                R3ButtonDown = CrossPlatformInputManager.GetButton("WirelessR3Player" + playerID);
            }
        }
    }

    void DriftHandling()
    {
        if (usingController)
        {
            if (brakeInput > 0 && accelInput == 0)
            {
                turnInput = -turnInput;
            }

            //controller cornering
            if (brakeInput > 0 && accelInput > 0)
            {
                turnSpeed = driftTurnSpeed;
                brakeInput = Mathf.Clamp(brakeInput, 0, driftClamp);
                isDrifting = true;
            }
            else
            {
                turnSpeed = defaultTurnSpeed;
                isDrifting = false;
            }
        }

        else
        {
            //keyboard drift with shift
            if (driftInput)
            {
                turnSpeed = driftTurnSpeed;
                brakeInput = Mathf.Clamp(brakeInput, 0, driftClamp);
                isDrifting = true;
            }
            else
            {
                turnSpeed = defaultTurnSpeed;
                isDrifting = false;
            }
        }
    }

    void MotionHandling()
    {
        //Acceleration Handling
        float accelDivisor = AccelerationHandling();

        //if the car is in the air
        if (gravityController.InAir)
        {
            airTimer += Time.deltaTime;
            //Check to see if you been in the air based on airTimeMax, if exceeded airTimeMax, you no longer have the ability to accel
            if (airTimer < airTimeMax)
            {
                if (!usingController)
                {
                    currentForceApplied = powerInput * speed;
                    carRigidbody.AddRelativeForce(0f, 0f,currentForceApplied, ForceMode.Force);
                    carRigidbody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f, ForceMode.Force);
                }
                else
                {
                    currentForceApplied = ((accelInput - brakeInput) * speed) / accelDivisor;
                    carRigidbody.AddRelativeForce(0f, 0f, currentForceApplied, ForceMode.Force);
                    carRigidbody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f, ForceMode.Force);
                }
            }
            else if (airTimer >= carResetTimer)
            {
                ResetCarOnTrack();
                //Give it some time before resetting
                airTimer = 0;
            }
            //If the car is in the air, you still lose potential energy
            driftTimer -= Time.deltaTime;
        }
        else
        {
            if(driftCooldown > 0)
            {
                driftCooldown -= Time.deltaTime;
            }

            if(driftCooldown < 0)
            {
                driftCooldown = 0;
            }

            if (isDrifting && driftCooldown == 0)
            {
                if (driftTimer < driftTimeMax)
                    driftTimer += Time.deltaTime;
            }
            else
            {
                if (driftTimer > 0f)
                {
                    driftTimer -= Time.deltaTime;
                }
                if (driftTimer < 0f)
                {
                    driftTimer = 0;
                }
            }

            if (driftTimer > driftTimeMin)
            {
                //The moment you release the brake
                if (!isDrifting)
                {
                    driftPower = 1.0f + (driftTimer * driftMagnitude);
                    driftCooldown = driftTimer;
                }
            }
            else
                driftPower = 1.0f;

            if (!usingController)
            {
                if (!driftInput)
                {
                    currentForceApplied = powerInput * speed * driftPower;
                    carRigidbody.AddRelativeForce(0f, 0f, currentForceApplied);
                }
                carRigidbody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);
            }
            else
            {
                currentForceApplied = ((accelInput - brakeInput) * speed * driftPower) / accelDivisor;
                carRigidbody.AddRelativeForce(0f, 0f, ((accelInput - brakeInput) * speed * driftPower) / accelDivisor, ForceMode.Force);
                //Debug.Log(accelInput - brakeInput);
                carRigidbody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);
            }
            airTimer = 0;
        }
    }

    void ResetCarHandling()
    {
        //reset car
        if (L3ButtonDown && R3ButtonDown && selfDestructTimer <= 0)
        {
            this.GetComponent<HealthController>().ResetHealth();
            selfDestructTimer = 3;
        }

        if (selfDestructTimer > 0)
        {
            selfDestructTimer -= Time.deltaTime;
        }
    }

    float AccelerationHandling()
    {
        if (accelRate == 1)
        {
            return 1;
        }
        else
        {
            if (accelInput > 0 || brakeInput > 0)
            {
                isAccelerating = true;
            }
            else
                isAccelerating = false;

            if (isAccelerating)
            {
                accelTimer += Time.deltaTime;
            }
            else
                accelTimer -= Time.deltaTime;

            accelTimer = Mathf.Clamp(accelTimer, 0f, accelRate - 1);

            return Mathf.Abs(accelTimer - accelRate);
        }
    }

    void TrickHandling()
    {
        //When not in the middle of performing a trick
        if (!currentlyPerformingTrick)
        {//trick inputs
            if (trickUpDownInput != 0)
            {
                if (trickUpDownInput == -1)
                {
                    shieldAcquired.Play();
                    StartCoroutine(PerformBackflip());
                }
                if (trickUpDownInput == 1)
                {
                    spikesAcquired.Play();
                    StartCoroutine(PerformFrontflip());
                }
            }
            else if (trickLeftRightInput != 0)
            {
                if (trickLeftRightInput == -1)
                {
                    boostAcquired.Play();
                    StartCoroutine(PerformCorkscrew());
                }
                if (trickLeftRightInput == 1)
                {
                    repairAcquired.Play();
                    StartCoroutine(PerformPopShoveIt());
                }
            }
            else
            {
                ResetAnims();
            }
        }
    }

    void PowerUpHandling()
    {
        //use item
        if (itemUse && hasPowerUp)
        {
            UsePowerUp();
        }


        PowerUpInUse();
    }

    public void UsePowerUp()
    {
        //Debug.Log("Power Up Used");
        hasPowerUp = false;
        RefreshCar();

        switch (storedPowerUp)
        {
            case powerUpEnum.None:
                break;
            case powerUpEnum.Boost:
                boostDeployed.Play();
                UseBoostPowerUp();
                break;
            case powerUpEnum.Restoration:
                repairDeployed.Play();
                StartCoroutine(UseRestorationPowerUp());
                break;
            case powerUpEnum.Defense:
                shieldDeployed.Play();
                UseDefensePowerUp();
                break;
            case powerUpEnum.Attack:
                spikesDeployed.Play();
                UseAttackPowerUp();
                break;
        }
        usingPowerUp = true;
    }

    public void PowerUpInUse()
    {
        if (usingPowerUp)
        {
            if (powerUpTimer < powerUpTimerMax)
                powerUpTimer += Time.deltaTime;
            else if (powerUpTimer >= powerUpTimerMax)
            {
                RefreshCar();
                ResetPowerUpAnim();
            }
        }
    }

    public void RefreshCar()
    {
        usingPowerUp = false;
        powerUpTimer = 0f;

        //Speed Reset
        speed = normalSpeed;
        //Defenses reset
        damageDivisor = normalDamageDivisor;

        //Animation Reset
        ResetAnims();
        ResetPowerUpAnim();
        //Debug.Log("Car Refreshed!");
    }

    //If the car has fallen off the track, the car will reorient itself back on the last track it was in contact with
    public void ResetCarOnTrack()
    {
        GameObject resetTrack = GetComponent<GravityController>().lastContactTrack;
        Transform resetPoint = resetTrack.transform.Find("Center");
        Vector3 resetPos = resetPoint.transform.position + (resetPoint.transform.up * GetComponent<GravityController>().hoverHeight);
        Quaternion resetRot = resetPoint.transform.rotation;

        Debug.Log("Resetting " + transform.name + " to " + resetTrack.transform.name);
        transform.position = resetPos;
        transform.rotation = resetRot;
        //transform.forward = resetPoint.forward;
        RefreshCar();
        driftPower = 1;
        switch (driver)
        {
            case driverEnum.AI:
                GetComponent<CarAIController>().DriftTimer = 0;
                break;
        }
        isAlive = true;
        justResetCar = true;
    }

    //Mostly used for AI to reset at a specific track
    public void ResetCarOnTrack(GameObject resetTrack)
    {
        Transform resetPoint = resetTrack.transform.Find("Center");
        Vector3 resetPos = resetPoint.transform.position + (resetPoint.transform.up * GetComponent<GravityController>().hoverHeight);
        Quaternion resetRot = resetPoint.transform.rotation;

        Debug.Log("Resetting " + transform.name + " to " + resetTrack.transform.name);
        transform.position = resetPos;
        //transform.rotation = resetRot;
        //transform.forward = resetPoint.forward;
        //transform.up = resetPoint.up;
        RefreshCar();
        driftPower = 1;

        switch (driver)
        {
            case driverEnum.AI:
                GetComponent<CarAIController>().DriftTimer = 0;
                break;
        }
        isAlive = true;
        justResetCar = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.ToUpper().Contains("RAMP") && (currentSpeed >= minSpeedForTrick))
        {
            canPerformTrick = true;
        }

        if (other.gameObject == teleSource && !isTeleported)
        {
            //Teleport the player if they hit the teleport threshold on procedural tracks
            if (teleSource != null && other.gameObject == teleSource.gameObject)
            {
                Debug.Log("Teleporting " + name + " to the start.");
                //get car's position and rotation relative to the end tunnel
                transform.parent = teleSource.transform;
                Vector3 telePos = transform.localPosition;
                Quaternion teleRot = transform.localRotation;

                carRigidbody.isKinematic = true;

                //set car to same position relative to the start tunnel
                transform.parent = teleDest.transform;
                transform.localPosition = telePos;
                transform.localRotation = teleRot;

                if (this.driver != driverEnum.AI)
                {
                    myCamera.transform.parent = teleSource.transform;
                    Vector3 camPos = myCamera.transform.localPosition;
                    Quaternion camRot = myCamera.transform.localRotation;
                    //Debug.Log(myCamera.transform.localPosition);
                    //Debug.Log(camPos);

                    myCamera.transform.parent = teleDest.transform;
                    myCamera.transform.localPosition = camPos;
                    myCamera.transform.localRotation = camRot;
                }
                //Debug.Log(camPos);
                //Debug.Log(myCamera.transform.localPosition);

                //unparent car and unfuck anything else
                transform.parent = null;
                transform.localScale = Vector3.one;

                //Handles setting the car's momentum when going through the teleporter
                Vector3 carAngularVelocity = transform.InverseTransformDirection(carRigidbody.angularVelocity);
                Vector3 currentLocalVelocity = transform.InverseTransformDirection(currentVelocity);
                //Debug.Log("currentLocalVelocity: " + currentLocalVelocity);
                carRigidbody.angularVelocity = Vector3.zero;
                //carRigidbody.angularVelocity = transform.TransformDirection(carAngularVelocity);
                carRigidbody.velocity = Vector3.zero;
                //carRigidbody.velocity = transform.TransformDirection(currentLocalVelocity);
                carRigidbody.velocity = transform.forward * currentSpeed;
                carRigidbody.isKinematic = false;

                GetComponent<CarAIController>().CurrentNode = 0;
                GetComponent<CarAIController>().UpdateDestinationTransform();

                if (this.driver != driverEnum.AI)
                {
                    myCamera.transform.parent = GameObject.Find("CameraManager").transform;
                    myCamera.transform.localScale = Vector3.one;
                }

                isTeleported = true;
            }
        }

        if(other.gameObject == teleDest)
        {
            isTeleported = false;
        }
    }

    public IEnumerator PerformBackflip()
    {
        
        if (storedPowerUp != powerUpEnum.Defense)
            GetComponentInChildren<PowerUpHandler>().ResetPowerUpAnim();

        currentlyPerformingTrick = true;
        //Debug.Log("Backflip performed! Gained 'Defense' power up!");
        currentAnimator.SetBool("doBackflip", true);
        hasPowerUp = true;
        storedPowerUp = powerUpEnum.Defense;

        yield return new WaitForSeconds(trickAnimationTime);
        currentlyPerformingTrick = false;
    }

    public IEnumerator PerformFrontflip()
    {
        if (storedPowerUp != powerUpEnum.Attack)
            GetComponentInChildren<PowerUpHandler>().ResetPowerUpAnim();

        currentlyPerformingTrick = true;
        //Debug.Log("Frontflip performed! Gained 'Attack' power up!");
        currentAnimator.SetBool("doFrontflip", true);
        hasPowerUp = true;
        storedPowerUp = powerUpEnum.Attack;

        yield return new WaitForSeconds(trickAnimationTime);
        currentlyPerformingTrick = false;
    }

    public IEnumerator PerformCorkscrew()
    {
        if (storedPowerUp != powerUpEnum.Boost)
            GetComponentInChildren<PowerUpHandler>().ResetPowerUpAnim();

        currentlyPerformingTrick = true;
        //Debug.Log("Corkscrew performed! Gained 'Boost' power up!");
        currentAnimator.SetBool("doCorkscrew", true);
        hasPowerUp = true;
        storedPowerUp = powerUpEnum.Boost;

        

        yield return new WaitForSeconds(trickAnimationTime);
        currentlyPerformingTrick = false;
    }

    public IEnumerator PerformPopShoveIt()
    {
        if (storedPowerUp != powerUpEnum.Restoration)
            GetComponentInChildren<PowerUpHandler>().ResetPowerUpAnim();

        currentlyPerformingTrick = true;
        //Debug.Log("PopShoveIt performed! Gained 'Restoration' power up!");
        currentAnimator.SetBool("doPopShoveIt", true);
        hasPowerUp = true;
        storedPowerUp = powerUpEnum.Restoration;

        yield return new WaitForSeconds(trickAnimationTime);
        currentlyPerformingTrick = false;
    }

    void UseBoostPowerUp()
    {
        speed *= (1.0f + boostMultiplier);
        //Debug.Log("Increasing Speed!");
        //powerupAnimator.SetBool("isBoosting", true);
        if (!boost.gameObject.activeInHierarchy)
            boost.gameObject.SetActive(true);
        boost.Play();
        currentPowerUp = powerUpEnum.Boost;
        storedPowerUp = powerUpEnum.None;
    }

    IEnumerator UseRestorationPowerUp()
    {
        
        powerupAnimator.SetBool("isShielding", false);
        powerupAnimator.SetBool("isAttacking", false);
        powerupAnimator.SetBool("isRepair", true);
        currentPowerUp = powerUpEnum.Restoration;
        storedPowerUp = powerUpEnum.None;
        powerUpTimer = 0f;

        yield return new WaitForSeconds(1.0f);
        GetComponent<HealthController>().IncrementHealth(1);
        //Debug.Log("Health Restored");

        //RefreshCar();
    }

    void UseDefensePowerUp()
    {
        damageDivisor = (int)(damageDivisor * (1.0f + damageReductionMultiplier));
        powerupAnimator.SetBool("isShielding", true);
        //Debug.Log("Damage Reduction!");
        currentPowerUp = powerUpEnum.Defense;
        storedPowerUp = powerUpEnum.None;
    }

    void UseAttackPowerUp()
    {
        powerupAnimator.SetBool("isShielding", false);
        powerupAnimator.SetBool("isAttacking", true);
        //Debug.Log("Attacking Target");
        currentPowerUp = powerUpEnum.Attack;
        storedPowerUp = powerUpEnum.None;
    }

    public void ResetAnims()
    {
        currentAnimator.SetBool("doFrontflip", false);
        currentAnimator.SetBool("doBackflip", false);
        currentAnimator.SetBool("doCorkscrew", false);
        currentAnimator.SetBool("doPopShoveIt", false);
        
    }

    public void ResetPowerUpAnim()
    {
        powerupAnimator.SetBool("isShielding", false);
        powerupAnimator.SetBool("isAttacking", false);
        powerupAnimator.SetBool("isRepair", false);
        boost.Stop();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.ToUpper().Contains("RAMP"))
        {
            canPerformTrick = false;
            currentAnimator.SetBool("doFrontflip", false);
            currentAnimator.SetBool("doBackflip", false);
            currentAnimator.SetBool("doCorkscrew", false);
            currentAnimator.SetBool("doPopShoveIt", false);
        }
    }

    float scale(float oldMin, float oldMax, float newMin, float newMax, float value)
    {
        float oldRange = oldMax - oldMin;
        float newRange = newMax - newMin;
        float newVal = (((value - oldMin) * newRange) / oldRange) + newMin;

        return newVal;
    }

    public float DriftPower
    {
        get { return driftPower; }
    }

    public float DriftTimer
    {
        get { return driftTimer; }
        set { driftTimer = value; }
    }

    public float AirTimer
    {
        get { return airTimer; }
        set { airTimer = value; }
    }

    public bool UsingPowerUp
    {
        get { return usingPowerUp; }
    }

    public Vector3 CurrentVelocity
    {
        get { return currentVelocity;}
    }

}
