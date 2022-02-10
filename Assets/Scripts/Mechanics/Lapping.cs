using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lapping : MonoBehaviour {

    GameObject raceTrack;
    Transform[] trackPieces;
    public List<Transform> nodes = new List<Transform>();
    public List<Transform> startConnectors = new List<Transform>();
    public List<Transform> endConnectors = new List<Transform>();

    public float distanceOfTrack;
    public float distanceTravelled;

    float distanceBetweenPieces;

    public Transform currentNode;
    public Transform previousNode;
    public Transform playerNode;

    //keeps track of the current index in the nodes list
    public int currentIndex;
    //keeps track of the previous index in the nodes list
    public int previousIndex;

    //this will be the start/goal line
    public Transform startNode;
    //this will be a track the player must hit before hitting the goal line in order to complete a successful lap
    Transform halfwayNode;

    //is the player currently moving backwards
    public bool isBackwards;


    //NOTES: in order to prevent some strange lapping behavior, if the player crosses the goal point backwards after crossing the halfway point,
    //reset the halfwayPoint back to false
    //did the player hit the halfway point?
    bool halfwayPoint;
    //did the player hit the goal point after hitting the halfway point?
    bool goalPoint;

    //max laps will be 3 for now
    int maxLaps = 3;
    //keeps track of the current lap the player is on
    public int currentLap;
    int previousLap;
    public bool newLap;

    //find the player's lap counter in it's respective canvas
    Text playerLapCounter;

    public bool finishedRace;

    public float playerID;

	// Use this for initialization
	void Start () {

        //find player's id
        playerID = GetComponent<HoverCarController>().playerID;

        if(this.GetComponent<HoverCarController>().driver == HoverCarController.driverEnum.AI)
        {
            playerID = 0;
        }


        //find the current track in the scene
        raceTrack = GameObject.FindGameObjectWithTag("RaceTrack");
        //find the player's specific lap counter
        if (playerID != 0)
        {
            playerLapCounter = GameObject.FindGameObjectWithTag("Player" + playerID + "Laps").GetComponent<Text>();
        }

        if(playerLapCounter != null)
        playerLapCounter.text = "Laps: " + (currentLap + 1) + "/" + maxLaps;

        //create a list to store the track pieces from the race track
        trackPieces = raceTrack.GetComponentsInChildren<Transform>();

        //loop through the trackpiece list, and add to the nodes list if we find a track piece transform
        for (int i = 0; i < trackPieces.Length; i++)
        {
            if ((trackPieces[i].name.Contains("Straight") || trackPieces[i].name.Contains("Left") ||
                    trackPieces[i].name.Contains("Right") || trackPieces[i].name.Contains("Up") || trackPieces[i].name.Contains("Down")) &&
                    !trackPieces[i].name.Contains("Connector"))
            {
                nodes.Add(trackPieces[i]);
            }

            if (trackPieces[i].name.ToLower().Contains("endconnector") && !trackPieces[i].name.ToLower().Contains("clone"))
            {
                endConnectors.Add(trackPieces[i]);
            }

            if ((trackPieces[i].name.ToLower().Contains("startconnector") || trackPieces[i].name.ToLower().Contains("straightconnector")) && !trackPieces[i].name.ToLower().Contains("clone"))
            {
                startConnectors.Add(trackPieces[i]);
            }
        }

        ////loop through connectors and find distance between start and end, and add to the total distance counter
        //for (int i = 0; i < endConnectors.Count; i++)
        //{
        //    float distance = Vector3.Distance(startConnectors[i].position, endConnectors[i].position);
        //    distanceOfTrack += distance;
        //}

        distanceBetweenPieces = Vector3.Distance(startConnectors[0].position, endConnectors[0].position);
        distanceOfTrack = distanceBetweenPieces * nodes.Count;

        //starting/goal line
        startNode = nodes[0];
        halfwayNode = nodes[(nodes.Count - 1) / 2];

        //current node in the list that the player is on
        currentNode = nodes[0];
        //previous node in the list 
        previousNode = nodes[nodes.Count - 1];

        currentIndex = 0;
        previousIndex = nodes.Count - 1;

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //determine what track piece the player is on currently
        playerNode = this.GetComponent<GravityController>().lastContactTrack.transform;

        //if the current track piece does not equal the track piece the player is currently on, and the track piece the player is currently on
        //is not the previous track piece, that means we are moving forward
        if(currentNode != playerNode && playerNode != previousNode)
        {
            isBackwards = false;
            previousIndex++;
            if (previousIndex > nodes.Count - 1)
            {
                previousIndex = 0;
            }
            previousNode = nodes[previousIndex];
            distanceTravelled += distanceBetweenPieces;
            currentIndex++;
            if (currentIndex > nodes.Count - 1)
            {
                currentIndex = 0;
            }
            currentNode = nodes[currentIndex];         
        }
        //if the current track piece does not equal the track piece the player is currently on, and the track piece the player is currently on
        //IS the previous track piece, that means we are moving BACKWARDS
        else if (currentNode != playerNode && playerNode == previousNode)
        {
            isBackwards = true;
            previousIndex--;
            if (previousIndex < 0)
            {
                previousIndex = nodes.Count - 1;
            }
            previousNode = nodes[previousIndex];
            distanceTravelled -= distanceBetweenPieces;
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = nodes.Count - 1;
            }
            currentNode = nodes[currentIndex];
        }

        //this is the halfway trigger to ensure the player is going around the track
        if(currentNode == halfwayNode && !isBackwards)
        {
            halfwayPoint = true;
        }

        //prevents the player from going backwards from the halfway point, crossing the goal, then going forward
        if(currentNode == startNode && isBackwards)
        {
            halfwayPoint = false;
            goalPoint = false;
        }

        //successfully lapped course
        if(currentNode == startNode && !isBackwards && halfwayPoint)
        {
            halfwayPoint = false;
            goalPoint = true;
        }

        //increment lap counter
        if(goalPoint)
        {
            currentLap++;
            if (currentLap < maxLaps)
            {
                if (playerLapCounter != null)
                {
                    playerLapCounter.text = "Laps: " + (currentLap + 1) + "/" + maxLaps;
                }
            }
            goalPoint = false;
        }

        if(currentLap != previousLap)
        {
            newLap = true;
            previousLap = currentLap;
        }

        //completed all laps of course
        if(currentLap >= maxLaps)
        {
            this.GetComponent<HoverCarController>().raceStarted = false;
            //AI player ID, the following code is specific to players
            if(playerID != 0)
            {
                finishedRace = true;
            }
        }
    }

    public bool HalfwayPoint
    {
        get { return halfwayPoint; }
        set { halfwayPoint = value; }
    }
}
