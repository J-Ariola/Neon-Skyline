//Jarrod Ariola
//Based on the CarAI by EYEmaginary
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAIController : MonoBehaviour {

    public enum difficultyEnum { Easy, Medium};
    public difficultyEnum difficulty;
    public enum playStyleEnum { Passive, Aggressive};
    public playStyleEnum playstyle;

    public Transform destinationTransform;
    public Transform trackPiecesPath;

    public bool debugSteer;
    public bool showAvoidMultiplier;

    //Distance needed to set the next waypoint
    [Range(0.5f, 40)]
    public float switchWaypointDistance = 20;
    [Range(-1.5f, 0)]
    public float maxDecelClamp = 0.9f;
    public bool isBraking = false;
    public float minorAvoidMultiplier = 0.60f;
    public float minorAvoidPlayerMultiplier = 0.75f;
    public float majorAvoidMultiplier = 1.0f;
    [Range(1f, 10f)]
    public float resetTimerCooldown = 2f;
    

    [Header("Sensors")]
    public float sensorLength = 5f;
    public float sideSensorLength = 2.5f;
    public Transform sensors;
    private Transform frontSensorStartPoint, frontLeftSensorPoint, frontRightSensorPoint, leftBackSensorPoint, rightBackSensorPoint;
    public float frontSideSensorAngle = 30.0f;
    public bool showSensorRays;
    public bool avoiding = false;
    public bool frontCenterRayDetection, frontLeftRayDetection, frontRightRayDetection, backLeftRayDetection, backRightRayDetection;

    
    public enum trickEnum { None, Sometimes, Helf, Likely};
    [Header("Tricks")]
    public trickEnum trickProbability;
    [Range(1, 10)]
    public float trickTimerRangeMax = 5f;
    private float trickProb;

    private HoverCarController hoverCarController;
    private GravityController gravityController;
    private Rigidbody rb;
    private HealthController healthController;
    private List<Transform> nodes;
    public int currentNode = 0;
    public int nextNode = 1;
    private float steerRate;
    private float airTimer;
    private float driftTimer;
    private float driftPower;
    private float driftCooldown;
    private float accel;
    private float accelerationRate;
    private float decel;
    private float resetTimer;
    private float trickTimer;

    void Awake () {
        hoverCarController = GetComponent<HoverCarController>();
        gravityController = GetComponent<GravityController>();
        healthController = GetComponent<HealthController>();
        rb = GetComponent<Rigidbody>();
        trackPiecesPath = GameObject.FindGameObjectWithTag("RaceTrack").transform;

        steerRate = hoverCarController.turnSpeed;
        accelerationRate = hoverCarController.accelRate;

        FindSensors();

        SetTrickProb();

        GetTrackPieces();
        
    }

    void FixedUpdate()
    {
        if (!hoverCarController.raceStarted)
        {
            return;
        }

        CheckWaypointDistance();

        switch (hoverCarController.driver)
        {
            //AI is actually driving
            case HoverCarController.driverEnum.AI:
                Sensors();
                if (hoverCarController.isAlive)
                {

                    if (gravityController.InAir)
                    {
                        airTimer += Time.deltaTime;
                        if (airTimer < hoverCarController.airTimeMax)
                        {
                            ApplySteer();
                            Braking();
                            driftPower = 1.0f;
                            Drive();
                        }
                        else if (airTimer >= hoverCarController.carResetTimer)
                        {
                            hoverCarController.ResetCarOnTrack();
                            //Give it some time before resetting
                            airTimer = 0;
                        }
                        driftTimer -= Time.deltaTime;
                    }
                    else
                    {
                        ApplySteer();
                        Braking();
                        ApplyDrift();
                        Drive();
                        AIPowerUpHandling();
                        airTimer = 0;
                    }
                }
                break;
        }
        
        
        
    }

    void GetTrackPieces()
    {
        Transform[] trackPiecesTransform = trackPiecesPath.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();



        //Finds every EndConnector and Center of every track piece
        for (int i = 0; i < trackPiecesTransform.Length; i++)
        {
            //If not our own transform
            if (trackPiecesTransform[i] != trackPiecesPath.transform.gameObject)
            {
                switch (trickProbability)
                {
                    case trickEnum.None:
                        //Difficulty determines how well the path is stored and more accurated points to move towards
                        switch (difficulty)
                        {
                            case difficultyEnum.Easy:
                                if (trackPiecesTransform[i].name.Equals("EndConnector"))
                                    nodes.Add(trackPiecesTransform[i]);
                                break;
                            case difficultyEnum.Medium:
                                if (trackPiecesTransform[i].name.Equals("EndConnector") || trackPiecesTransform[i].name.Equals("Center"))
                                {
                                    nodes.Add(trackPiecesTransform[i]);
                                }
                                break;
                        }
                        break;
                    default:
                        int rampPointNum = Random.Range(1, 2);
                        string rampPointStr = "RampPoint" + rampPointNum;
                        switch (difficulty)
                        {
                            case difficultyEnum.Easy:
                                if (trackPiecesTransform[i].name.Equals("EndConnector") || trackPiecesTransform[i].name.Equals(rampPointStr))
                                    nodes.Add(trackPiecesTransform[i]);
                                break;
                            case difficultyEnum.Medium:
                                if (trackPiecesTransform[i].name.Equals("EndConnector") || trackPiecesTransform[i].name.Equals("Center") || trackPiecesTransform[i].name.Equals(rampPointStr))
                                {
                                    nodes.Add(trackPiecesTransform[i]);
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }

    void FindSensors()
    {
        Transform[] allSensors = sensors.GetComponentsInChildren<Transform>();

        foreach (Transform t in allSensors)
        {
            if (t.name == "CarFrontPoint")
                frontSensorStartPoint = t;
            else if (t.name == "CarFrontLeftPoint")
                frontLeftSensorPoint = t;
            else if (t.name == "CarFrontRightPoint")
                frontRightSensorPoint = t;
            else if (t.name == "CarLeftBackPoint")
                leftBackSensorPoint = t;
            else if (t.name.Equals("CarRightBackPoint"))
                rightBackSensorPoint = t;

        }
    }

    void Sensors()
    {
        RaycastHit hit;
        Ray frontCenterRay = new Ray(frontSensorStartPoint.position, transform.forward);
        Ray frontLeftRay = new Ray(frontLeftSensorPoint.position, transform.forward);
        Ray frontLeftAngleRay = new Ray(frontLeftSensorPoint.position, Quaternion.AngleAxis(-frontSideSensorAngle, transform.up) * transform.forward);
        Ray frontRightRay = new Ray(frontRightSensorPoint.position, transform.forward);
        Ray frontRightAngleRay = new Ray(frontRightSensorPoint.position, Quaternion.AngleAxis(frontSideSensorAngle, transform.up) * transform.forward);
        Ray leftBackRay = new Ray(leftBackSensorPoint.position, -transform.right);
        Ray rightBackRay = new Ray(rightBackSensorPoint.position, transform.right);

        float avoidMultiplier = 0;
        avoiding = false;

        //Back left sensor
        if (Physics.Raycast(leftBackRay, out hit, sideSensorLength))
        {
            switch(playstyle)
            {
                case playStyleEnum.Aggressive:
                    if (hit.transform.tag.Contains("NoRay"))
                    {
                        Debug.DrawLine(leftBackRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the left
                        avoidMultiplier += minorAvoidMultiplier;
                        backLeftRayDetection = true;
                    }
                    break;

                case playStyleEnum.Passive:
                    if (hit.transform.tag.Contains("NoRay"))
                    {
                        Debug.DrawLine(leftBackRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the left
                        avoidMultiplier += minorAvoidMultiplier;
                        backLeftRayDetection = true;
                    }
                    else if (hit.transform.tag.Contains("Player"))
                    {
                        Debug.DrawLine(leftBackRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the left
                        avoidMultiplier += minorAvoidMultiplier;
                        backLeftRayDetection = true;
                    }
                    break;
            }
        }

        //Back right sensor
        if (Physics.Raycast(rightBackRay, out hit, sideSensorLength))
        {
            switch (playstyle)
            {
                case playStyleEnum.Aggressive:
                    if (hit.transform.tag.Contains("NoRay"))
                    {
                        Debug.DrawLine(rightBackRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the right
                        avoidMultiplier -= minorAvoidMultiplier;
                        backRightRayDetection = true;
                    }
                    break;

                case playStyleEnum.Passive:
                    if (hit.transform.tag.Contains("NoRay"))
                    {
                        Debug.DrawLine(rightBackRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the left
                        avoidMultiplier -= minorAvoidMultiplier;
                        backRightRayDetection = true;
                    }
                    else if (hit.transform.tag.Contains("Player"))
                    {
                        Debug.DrawLine(rightBackRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the left
                        avoidMultiplier -= minorAvoidPlayerMultiplier;
                        backRightRayDetection = true;
                    }
                    break;
            }
        }


        //front left sensor
        if (Physics.Raycast(frontLeftRay, out hit, sensorLength))
        {
            
            switch (playstyle)
            {
                case playStyleEnum.Aggressive:
                    if (hit.transform.tag.Contains("NoRay"))
                    {
                        Debug.DrawLine(frontLeftRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the right
                        avoidMultiplier += majorAvoidMultiplier;
                        frontLeftRayDetection = true;
                    }
                    break;

                case playStyleEnum.Passive:
                    if (hit.transform.tag.Contains("NoRay") || hit.transform.root.tag.Contains("Player"))
                    {
                        Debug.DrawLine(frontLeftRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the right
                        if (hit.transform.root.tag.Contains("Player"))
                            avoidMultiplier += minorAvoidPlayerMultiplier;
                        else
                            avoidMultiplier += minorAvoidMultiplier;
                        frontLeftRayDetection = true;
                    }

                    break;
            }
        }

        //front left angle sensor
        else if (Physics.Raycast(frontLeftAngleRay, out hit, sensorLength))
        {
            switch (playstyle)
            {
                case playStyleEnum.Aggressive:
                    if (hit.transform.tag.Contains("NoRay"))
                    {
                        Debug.DrawLine(frontLeftAngleRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        avoidMultiplier += minorAvoidMultiplier;
                        frontLeftRayDetection = true;
                    }
                    break;

                case playStyleEnum.Passive:
                    if (hit.transform.tag.Contains("NoRay") || hit.transform.root.tag.Contains("Player"))
                    {
                        Debug.DrawLine(frontLeftAngleRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        if (hit.transform.root.tag.Contains("Player"))
                            avoidMultiplier += minorAvoidPlayerMultiplier;
                        else
                            avoidMultiplier += minorAvoidMultiplier;
                        frontLeftRayDetection = true;
                    }
                    break;
            }
            
        }
        else
            frontLeftRayDetection = false;

        //front right sensor
        if (Physics.Raycast(frontRightRay, out hit, sensorLength))
        {
            switch (playstyle)
            {
                case playStyleEnum.Aggressive:
                    if (hit.transform.tag.Contains("NoRay"))
                    {
                        Debug.DrawLine(frontRightRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the left
                        avoidMultiplier -= majorAvoidMultiplier;
                        frontRightRayDetection = true;
                    }
                    break;

                case playStyleEnum.Passive:
                    if (hit.transform.tag.Contains("NoRay") || hit.transform.root.tag.Contains("Player"))
                    {
                        Debug.DrawLine(frontRightRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        //Turn to the left
                        if (hit.transform.root.tag.Contains("Player"))
                            avoidMultiplier -= minorAvoidPlayerMultiplier;
                        else
                            avoidMultiplier -= minorAvoidMultiplier;
                        frontRightRayDetection = true;
                    }
                    break;
            }
        }

        //front right angle sensor
        else if (Physics.Raycast(frontRightAngleRay, out hit, sensorLength))
        {
            switch (playstyle)
            {

                case playStyleEnum.Aggressive:
                    if (hit.transform.tag.Contains("NoRay"))
                    {
                        Debug.DrawLine(frontRightAngleRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        if (hit.transform.root.tag.Contains("Player"))
                            avoidMultiplier -= minorAvoidPlayerMultiplier;
                        else
                            avoidMultiplier -= minorAvoidMultiplier;
                        frontRightRayDetection = true;
                    }
                    break;

                case playStyleEnum.Passive:
                    if (hit.transform.tag.Contains("NoRay") || hit.transform.root.tag.Contains("Player"))
                    {
                        Debug.DrawLine(frontRightAngleRay.origin, hit.point, Color.magenta);
                        avoiding = true;
                        if (hit.transform.root.tag.Contains("Player"))
                            avoidMultiplier -= minorAvoidPlayerMultiplier;
                        else
                            avoidMultiplier -= minorAvoidMultiplier;
                        frontRightRayDetection = true;
                    }
                    break;
            }
        }
        else
            frontRightRayDetection = false;

        //front center sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(frontCenterRay, out hit, sensorLength))
            {
                switch (playstyle)
                {
                    case playStyleEnum.Aggressive:
                        if (hit.transform.tag.Contains("NoRay"))
                        {
                            Debug.DrawLine(frontCenterRay.origin, hit.point, Color.magenta);
                            avoiding = true;
                            frontCenterRayDetection = true;
                            //Find the normal of the object and turn the appropriate direction
                            if (hit.normal.x < 0)
                                avoidMultiplier = -majorAvoidMultiplier;
                            else
                                avoidMultiplier = majorAvoidMultiplier;
                        }
                        break;

                    case playStyleEnum.Passive:
                        if (hit.transform.tag.Contains("NoRay") || hit.transform.root.tag.Contains("Player"))
                        {
                            Debug.DrawLine(frontCenterRay.origin, hit.point, Color.magenta);
                            avoiding = true;
                            frontCenterRayDetection = true;
                            //Find the normal of the object and turn the appropriate direction
                            if (hit.normal.x < 0)
                                avoidMultiplier = -majorAvoidMultiplier;
                            else
                                avoidMultiplier = majorAvoidMultiplier;
                        }
                        break;
                }
            }
            else
                frontCenterRayDetection = false;

            if (frontCenterRayDetection && frontLeftRayDetection && frontRightRayDetection)
            {
                //Reverse
                Accel(false);
            }
            else
            {
                Accel(true);
            }
            
        }

        //If there is an obstacle to avoid
        if (avoiding)
        {
            float newSteer = steerRate;
            if (accel >= decel)
                newSteer = steerRate * avoidMultiplier;
            //Steer the correct direction when reversing
            else
                newSteer = steerRate * -avoidMultiplier;
            if (showAvoidMultiplier)
                Debug.Log(name + "avoid Multiplier: " + avoidMultiplier);
            rb.AddRelativeTorque(0f, newSteer, 0f);
            isBraking = true;
        }
        else
            isBraking = false;

        //Show rays in scene
        if (showSensorRays)
        {
            Debug.DrawRay(frontCenterRay.origin, frontCenterRay.direction * sensorLength, Color.cyan);
            Debug.DrawRay(frontLeftRay.origin, frontLeftRay.direction * sensorLength, Color.cyan);
            Debug.DrawRay(frontLeftAngleRay.origin, frontLeftAngleRay.direction * sensorLength, Color.cyan);
            Debug.DrawRay(frontRightRay.origin, frontRightRay.direction * sensorLength, Color.cyan);
            Debug.DrawRay(frontRightAngleRay.origin, frontRightAngleRay.direction * sensorLength, Color.cyan);
        }
    }

    void ApplySteer()
    {
        if (avoiding)
            return;
        Vector3 relativeVector = Vector3.zero;
        switch (difficulty)
        {
            case difficultyEnum.Easy:
                relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
                break;
            case difficultyEnum.Medium:
                relativeVector = transform.InverseTransformPoint(nodes[nextNode].position);
                break;
        }
        relativeVector = relativeVector / relativeVector.magnitude;
        
        //Wheel Angle
        float newSteer = (relativeVector.x / relativeVector.magnitude) * steerRate;
        if (debugSteer)
        {
            Debug.Log(name + "RelavtiveVector: " + relativeVector);
            Debug.Log(name + " Steer: " + newSteer);
        }
        if (Mathf.Abs(newSteer) >= Mathf.PI)
            isBraking = true;
        else
            isBraking = false;
        rb.AddRelativeTorque(0f, newSteer, 0f);
    }

    void Drive()
    {
        rb.AddRelativeForce(0f, 0f, (accel - decel) * hoverCarController.speed * driftPower, ForceMode.Force);
        //Debug.Log(accel + decel);
    }

    //If the distance between the car and its current waypoint is a certain distance, set the current waypoint to the next waypoint
    void CheckWaypointDistance()
    {
        //Debug.Log(Vector3.Distance(transform.position, nodes[currentNode].position));
        /*
        //Prioritizing RampPoint and thus getting closer
        if (nodes[currentNode].name.Contains("RampPoint"))
        {
            if (Vector3.Distance(transform.position, nodes[currentNode].position) < switchWaypointDistance / 2)
            {
                if (currentNode == nodes.Count - 1)
                {
                    currentNode = 0;
                }
                if (nextNode == nodes.Count - 1)
                {
                    nextNode = 0;
                }
                else
                {
                    currentNode++;
                    nextNode++;
                    destinationTransform = nodes[currentNode];
                }
            }
        }*/

        if (Vector3.Distance(transform.position, nodes[currentNode].position) < switchWaypointDistance)
        {
            //If at the last waypoint, set the current waypoint to the first waypoint
            if (currentNode >= nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode ++;
                nextNode = currentNode + 1;
            }
            if (nextNode >= nodes.Count - 1)
            {
                nextNode = 0;
            }
            UpdateDestinationTransform();
            //Debug.Log(destinationTransform);
        }
    }

    public void SetCurrentNode(int nodeNum)
    {
        currentNode = nodeNum;
        nextNode = currentNode + 1;
        if (currentNode >= nodes.Count - 1)
        {
            currentNode = 0;
        }
        if (nextNode >= nodes.Count - 1)
        {
            nextNode = 0;
        }

        UpdateDestinationTransform();
    }

    public void UpdateDestinationTransform()
    {
        destinationTransform = nodes[currentNode];
    }

    

    void Braking()
    {
        if (isBraking)
        {
            Decel();
        }
        else
            decel = 0;

        //Debug.Log(decel);
    }

    void ApplyDrift()
    {
        if (driftCooldown > 0)
        {
            driftCooldown -= Time.deltaTime;
        }
        else
            driftCooldown = 0;

        if (isBraking && driftCooldown == 0)
        {
            if (driftTimer < hoverCarController.driftTimeMax)
                driftTimer += Time.deltaTime;
        }
        else
        {
            if (driftTimer > 0f)
            {
                driftTimer -= Time.deltaTime;
            }
            else
                driftTimer = 0;
        }

        if (driftTimer > hoverCarController.driftTimeMin)
        {
            if (!isBraking)
            {
                driftPower = 1.0f + (driftTimer * hoverCarController.driftMagnitude);
                driftCooldown = driftTimer;
            }
        }
        else
            driftPower = 1.0f;

         hoverCarController.DriftTimer = driftTimer;
            
    }

    void Accel(bool isAccelerating)
    {
        //As if you are slowing down, not really braking
        if (!isAccelerating)
        {
            accel -= (Time.deltaTime * accelerationRate);
        }
        else
            accel += (Time.deltaTime * accelerationRate);

        accel = Mathf.Clamp(accel, 0, 1f);
    }

    void Decel()
    {
        decel += (Time.deltaTime * accelerationRate);
        decel = Mathf.Clamp(decel, 0, maxDecelClamp);
    }

    void SetTrickProb()
    {
        switch(trickProbability)
        {
            case trickEnum.None:
                trickProb = 0f;
                break;
            case trickEnum.Sometimes:
                trickProb = 25f;
                break;
            case trickEnum.Helf:
                trickProb = 50f;
                break;
            case trickEnum.Likely:
                trickProb = 75f;
                break;
        }
    }

    void AIPowerUpHandling()
    {
        if (hoverCarController.hasPowerUp)
        {
            if (trickTimer <= 0)
            {
                hoverCarController.UsePowerUp();
            }
            else
                trickTimer -= Time.deltaTime;
        }

        hoverCarController.PowerUpInUse();
    }

    void SetTrickTimer()
    {
        trickTimer = Random.Range(0.5f, trickTimerRangeMax);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!enabled || hoverCarController.driver == HoverCarController.driverEnum.Player)
            return;

        switch (trickProbability)
        {
            case trickEnum.None:
                break;
            default:
                //For now, the AI will always do a trick when jumping off a ramp
                if (hoverCarController.canPerformTrick)
                {
                    //Debug.Log(name + " AI Perform Trick");

                    switch (difficulty)
                    {
                        case difficultyEnum.Easy:
                            RandomlyPerformTrick();
                            break;

                        case difficultyEnum.Medium:
                            int trickPerformNum = Random.Range(1, 100);
                            //Makes decision based on current situation
                            //If Aggressive, get Attack (40%)
                            if (trickPerformNum <= 40)
                                StartCoroutine(hoverCarController.PerformFrontflip());
                            //Not in 1st place -> get Boost
                            /*
                            else if ( )
                                hoverCarController.PerformCorkscrew();
                            */
                            //Current health is less than 0.5f -> get Restoration
                            else if (healthController.currentHealth <= 0.5f)
                                StartCoroutine(hoverCarController.PerformPopShoveIt());
                            //Else randomly choose the rest
                            else
                                RandomlyPerformTrick();

                            break;
                    }
                    SetTrickTimer();
                }
                else
                    hoverCarController.ResetAnims();
                break;
    }

    }

    void RandomlyPerformTrick()
    {
        int trickPerformNum = Random.Range(1, 100);
        switch (playstyle)
        {
            case playStyleEnum.Passive:
                //1-25 - Backflip
                if (trickPerformNum <= 25)
                {
                    StartCoroutine(hoverCarController.PerformBackflip());
                }
                //26-50 - Frontflip
                else if (trickPerformNum >= 26 && trickPerformNum <= 50)
                {
                    StartCoroutine(hoverCarController.PerformFrontflip());
                }
                //51 - 75 - Corkscrew
                else if (trickPerformNum >= 51 && trickPerformNum <= 75)
                {
                    StartCoroutine(hoverCarController.PerformCorkscrew());
                }
                //76 - 100 - PopShoveIt
                else
                {
                    StartCoroutine(hoverCarController.PerformPopShoveIt());
                }
                break;
            case playStyleEnum.Aggressive:
                //1-20 - Backflip
                if (trickPerformNum <= 20)
                    StartCoroutine(hoverCarController.PerformBackflip());
                //21 - 60 - Frontflip
                if (trickPerformNum >= 21 && trickPerformNum <= 60)
                    StartCoroutine(hoverCarController.PerformFrontflip());
                //61 - 80 - Corkscrew
                if (trickPerformNum >= 61 && trickPerformNum <= 80)
                    StartCoroutine(hoverCarController.PerformCorkscrew());
                //81 - 100 - PopShoveIt
                else
                    StartCoroutine(hoverCarController.PerformPopShoveIt());

                break;
        }
        
    }

    Vector3 FindMidPoint(Vector3 v1, Vector3 v2)
    {
        return ((v1 + v2) / 2);
    }

    public float DriftTimer
    {
        get { return driftTimer; }
        set { driftTimer = value; }
    }

    public int CurrentNode
    {
        get { return currentNode; }
        set { currentNode = value; }
    }

    public List<Transform> Nodes
    {
        get { return nodes; }
    }
}
