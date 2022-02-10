using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToPlaceTracker : MonoBehaviour {

    GameObject placeTracker;
    GameObject pauseUI;
    bool addedToList;

    Vector3 lastPosition;

    bool pauseUIFound = true;

	// Use this for initialization
	void Start () {


        
    }
	
	// Update is called once per frame
	void Update () {

        placeTracker = GameObject.FindGameObjectWithTag("PlaceTracker");

        if(GetComponent<HoverCarController>().driver == HoverCarController.driverEnum.AI)
        {
            pauseUIFound = true;
        }

        pauseUI = GameObject.FindGameObjectWithTag("PauseUI");

        if(pauseUI != null)
        {
            pauseUIFound = true;
        }


        if(placeTracker != null && !addedToList)
        {
            placeTracker.GetComponent<CurrentPlace>().racers.Add(this.gameObject);

            if (pauseUIFound)
                if (GetComponent<HoverCarController>().driver == HoverCarController.driverEnum.Player)
                    pauseUI.GetComponent<PauseUI>().players.Add(this.gameObject);

            addedToList = true;
        }
        /*
        if (pauseUIFound && addedToList)
        {
            if (GetComponent<HoverCarController>().driver == HoverCarController.driverEnum.Player)
            {
                pauseUI.GetComponent<PauseUI>().players.Add(this.gameObject);
            }
        }*/
        

    }
}
