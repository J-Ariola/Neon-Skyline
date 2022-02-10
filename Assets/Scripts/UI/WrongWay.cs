using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WrongWay : MonoBehaviour {

    //player's ID
    public float playerID;
    GameObject player;

	// Use this for initialization
	void Start () {

        

    }
	
	// Update is called once per frame
	void Update () {

        //find player object
        player = GameObject.FindGameObjectWithTag("Player" + playerID);

        //error detection, don't worry about this script if player doesn't exist
        if (player == null)
        {
            return;
        }

        //if the player is going backwards, display text
        if (player.GetComponent<Lapping>().isBackwards)
        {
            this.GetComponent<Text>().text = "!WRONG WAY!";
        }
        else
        {
            this.GetComponent<Text>().text = "";
        }

    }
}
