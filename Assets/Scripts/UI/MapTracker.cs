using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTracker : MonoBehaviour {

    //from 0 - 7, indicating all racers
    public int racerNum;

    public List<GameObject> borders = new List<GameObject>();

    //use to grab initial list of racers
    public GameObject placeTracker;

    public GameObject currentRacer;

    public GameObject mapBar;

    //determines how much the image moves as the car progresses
    public float mapDistanceUnit;

    //starting image position;
    Vector3 startingPosition;

    //current distance travelled by player
    float currentDistanceTravelled;
    //keeps track of changing distance
    float newDistanceTravelled;

    float playerID;
    bool playerFound;

    //buffer time to allow the game to find all players before assigning them
    float timer = 1;

    public string[] controllers;


	// Use this for initialization
	void Start () {
        controllers = Input.GetJoystickNames();

        if (PlayerPrefs.GetInt("IsStoryMode") == 1)
        {
            mapBar.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, 0f);
            mapBar.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, 0f);
            mapBar.GetComponent<RectTransform>().pivot = new Vector2(.5f, 0f);
        }
        else if (controllers.Length > 1)
        {//fail safe to ensure single player for arcade mode
            if (controllers[1].Equals(""))
            {
                mapBar.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, 0f);
                mapBar.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, 0f);
                mapBar.GetComponent<RectTransform>().pivot = new Vector2(.5f, 0f);
            }
            else
            {
                mapBar.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, .5f);
                mapBar.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, .5f);
                mapBar.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
            }
        }
        else if (controllers.Length == 1)
        {
            //if there is one controller connected
            mapBar.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, 0f);
            mapBar.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, 0f);
            mapBar.GetComponent<RectTransform>().pivot = new Vector2(.5f, 0f);
        }
        /*
        //fail safe to ensure single player for arcade mode
            if (controllers.Length > 1)
        {
            if(controllers[1].Equals(""))
            {
                mapBar.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, 0f);
                mapBar.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, 0f);
                mapBar.GetComponent<RectTransform>().pivot = new Vector2(.5f, 0f);
            }
            else
            {
                mapBar.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, .5f);
                mapBar.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, .5f);
                mapBar.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
            }
        }*/
        

        placeTracker = GameObject.FindGameObjectWithTag("PlaceTracker");
        
        startingPosition = this.GetComponent<RectTransform>().localPosition;
        
	}
	
	// Update is called once per frame
	void Update () {

        if(timer > 0)
        timer -= Time.deltaTime;

        //determine if this racer slot is currently filled, if not destroy the object
        if (!playerFound)
        {
            if (placeTracker.GetComponent<CurrentPlace>().racers.Count <= racerNum || timer > 0)
            {
                GetComponent<RawImage>().enabled = false;
                return;
            }
            else
            {
                GetComponent<RawImage>().enabled = true;
                currentRacer = placeTracker.GetComponent<CurrentPlace>().racers[racerNum];
                mapDistanceUnit = (Mathf.Abs(this.GetComponent<RectTransform>().localPosition.x * 2)) / placeTracker.GetComponent<CurrentPlace>().trackLength;
                this.GetComponent<RawImage>().texture = currentRacer.GetComponentInChildren<NameTagHandler>().image;
                if (currentRacer.GetComponent<HoverCarController>().driver == HoverCarController.driverEnum.Player)
                {
                    playerID = currentRacer.GetComponent<HoverCarController>().playerID;
                    borders[(int)playerID - 1].SetActive(true);
                }
                playerFound = true;
            }
        }

        currentDistanceTravelled = currentRacer.GetComponent<Lapping>().distanceTravelled;

        if (currentDistanceTravelled > newDistanceTravelled)
        {
            newDistanceTravelled = currentDistanceTravelled;
            this.GetComponent<RectTransform>().localPosition = new Vector3(this.GetComponent<RectTransform>().localPosition.x + mapDistanceUnit, 
                this.GetComponent<RectTransform>().localPosition.y, this.GetComponent<RectTransform>().localPosition.z);
        }
        else if(currentDistanceTravelled < newDistanceTravelled)
        {
            newDistanceTravelled = currentDistanceTravelled;
            if (this.GetComponent<RectTransform>().localPosition.x > -510)
            {
                this.GetComponent<RectTransform>().localPosition = new Vector3(this.GetComponent<RectTransform>().localPosition.x - mapDistanceUnit,
                    this.GetComponent<RectTransform>().localPosition.y, this.GetComponent<RectTransform>().localPosition.z);
            }
        }

        if(currentRacer.GetComponent<Lapping>().newLap)
        {
            newDistanceTravelled = currentDistanceTravelled;
            this.GetComponent<RectTransform>().localPosition = startingPosition;
            //this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x + 10, this.GetComponent<RectTransform>().sizeDelta.y + 10);
            currentRacer.GetComponent<Lapping>().newLap = false;
        }

        if(currentRacer.GetComponent<HoverCarController>().driver == HoverCarController.driverEnum.AI)
        {
            if(currentRacer.GetComponent<Lapping>().currentNode == currentRacer.GetComponent<Lapping>().startNode)
            {
                newDistanceTravelled = currentDistanceTravelled;
                this.GetComponent<RectTransform>().localPosition = startingPosition;
            }
        }

    }
}
