using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishedRaceUI : MonoBehaviour {

    GameObject player;
    public float playerID;

    //need to move these specific texts to center of canvas
    public GameObject finalTimeText;
    public GameObject finalPlaceText;

    //darken screen when race is finished
    public GameObject finishedRaceImage;
    
	// Use this for initialization
	void Start () {

        
		
	}
	
	// Update is called once per frame
	void Update () {

        player = GameObject.FindGameObjectWithTag("Player" + playerID);

        if(player == null)
        {
            return;
        }

        if(player.GetComponent<Lapping>().finishedRace)
        {
            finishedRaceImage.SetActive(true);

            finalTimeText.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, .5f);
            finalTimeText.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, .5f);
            finalTimeText.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);

            finalPlaceText.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, .5f);
            finalPlaceText.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, .5f);
            finalPlaceText.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
        }

        

    }
}
