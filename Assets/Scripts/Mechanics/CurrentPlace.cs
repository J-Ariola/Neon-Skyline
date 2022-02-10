using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlace : MonoBehaviour {

    //needs a list of the current track pieces
    GameObject raceTrack;
    Transform[] trackPieces;
    //public List<Transform> nodes = new List<Transform>();
    public List<Transform> endConnectors = new List<Transform>();


    //assign the placement gameobject within the scene under each player's canvas in order from 1-4
    public List<Text> placesUI = new List<Text>();

    

    //list will have all available racers in the current race
    public List<GameObject> racers = new List<GameObject>();

    //keeps track of the currentPieces end point
    Transform currentPieceTransform;

    public float trackLength;



	// Use this for initialization
	void Start () {

        //find the current track in the scene
        raceTrack = GameObject.FindGameObjectWithTag("RaceTrack");

        //create a list to store the track pieces from the race track
        trackPieces = raceTrack.GetComponentsInChildren<Transform>();

        //loop through the trackpiece list, and add to the nodes list if we find a track piece transform
        for (int i = 0; i < trackPieces.Length; i++)
        {
            //if ((trackPieces[i].name.Contains("Straight") || trackPieces[i].name.Contains("Left") ||
            //        trackPieces[i].name.Contains("Right") || trackPieces[i].name.Contains("Up") || trackPieces[i].name.Contains("Down")) &&
            //        !trackPieces[i].name.Contains("Connector"))
            //{
            //    nodes.Add(trackPieces[i]);
            //}

            //store all the endpoints of each track piece
            if (trackPieces[i].name.ToLower().Contains("endconnector") && !trackPieces[i].name.ToLower().Contains("clone"))
            {
                endConnectors.Add(trackPieces[i]);
                trackLength++;
            }
        }



    }
	
	// Update is called once per frame
	void Update () {

        SortRacers();
        AssignPlaceTexts();
		
	}

    //sorts racers in the order of the who's furthest in the race
    void SortRacers()
    {
        //loop through all the players currently in the race and assign their weights accordingly
        for(int i = 0; i < racers.Count - 1; i++)
        {
            for(int j = i + 1; j > 0; j--)
            {
                //if this racer has already finished the race, no need to compare it to the other racers to determine it's place
                if(racers[j - 1].GetComponent<Lapping>().finishedRace)
                {
                    continue;
                }
                //sort the players based on who's on the furthest track piece
                if(racers[j - 1].GetComponent<Lapping>().distanceTravelled < racers[j].GetComponent<Lapping>().distanceTravelled)
                {
                    GameObject temp = racers[j - 1];
                    racers[j - 1] = racers[j];
                    racers[j] = temp;
                }

                //if players are on the same piece, sort based on who's closest to the end connector of the current piece
                if(racers[j - 1].GetComponent<Lapping>().distanceTravelled == racers[j].GetComponent<Lapping>().distanceTravelled)
                {
                    currentPieceTransform = endConnectors[racers[j - 1].GetComponent<Lapping>().currentIndex];
                    float distance1 = Vector3.Distance(racers[j - 1].transform.position, currentPieceTransform.position);
                    currentPieceTransform = endConnectors[racers[j].GetComponent<Lapping>().currentIndex];
                    float distance2 = Vector3.Distance(racers[j].transform.position, currentPieceTransform.position);

                    if(distance1 > distance2)
                    {
                        GameObject temp = racers[j - 1];
                        racers[j - 1] = racers[j];
                        racers[j] = temp;
                    }
                }
            }

            ////calculates distance to current end connector
            //currentPieceTransform = endConnectors[currentList[i].GetComponent<Lapping>().currentIndex];

            //float distance = Vector3.Distance(currentList[i].transform.position, currentPieceTransform.position);
            ////currentList[i].GetComponent<AddToPlaceTracker>().weight = distance;
        }
    }

    void AssignPlaceTexts()
    {
        string suffix = "";
        for(int i = 0; i < racers.Count; i++)
        {
            //determine suffix of number 
            if(i == 0)
            {
                suffix = "st";
            }
            else if(i == 1)
            {
                suffix = "nd";
            }
            else if(i == 2)
            {
                suffix = "rd";
            }
            else
            {
                suffix = "th";
            }

            /*//only need to worry about showing the place of players, so check to see if the racer is a player
            if(racers[i].GetComponent<HoverCarController>().driver == HoverCarController.driverEnum.Player)
            {
                
            }*/

            //assign text values to UI elements
            switch(racers[i].GetComponent<HoverCarController>().driver)
            {
                case HoverCarController.driverEnum.Player:
                    if (racers[i].GetComponent<HoverCarController>().playerID == 1 && !racers[i].GetComponent<Lapping>().finishedRace)
                    {
                        placesUI[0].text = (i + 1) + suffix;
                    }
                    if (racers[i].GetComponent<HoverCarController>().playerID == 2 && !racers[i].GetComponent<Lapping>().finishedRace)
                    {
                        placesUI[1].text = (i + 1) + suffix;
                    }
                    if (racers[i].GetComponent<HoverCarController>().playerID == 3 && !racers[i].GetComponent<Lapping>().finishedRace)
                    {
                        placesUI[2].text = (i + 1) + suffix;
                    }
                    if (racers[i].GetComponent<HoverCarController>().playerID == 4 && !racers[i].GetComponent<Lapping>().finishedRace)
                    {
                        placesUI[3].text = (i + 1) + suffix;
                    }
                        break;
            }

            racers[i].GetComponent<HoverCarController>().currentPlace = i + 1;
            
        }
    }
}
