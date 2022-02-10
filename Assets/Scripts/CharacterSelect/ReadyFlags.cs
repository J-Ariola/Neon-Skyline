using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyFlags : MonoBehaviour {

    //list will contain all character confirmed booleans of each player. if each bool is true, then pop up
    //the text and allow the player to continue
    public GameObject[] players;
    bool[] flagsChecked;
    public GameObject text;
    public GameObject startButton;

    public bool allFlagsClear;

	// Use this for initialization
	void Start () {

        players = GameObject.FindGameObjectsWithTag("Flag");
        flagsChecked = new bool[players.Length];

	}
	
	// Update is called once per frame
	void Update () {
		
        //loop through all character selects on each player canvas. If all characters have confirmed their characters,
        //then set allFlagsClear to true and enable the text and button
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].GetComponent<CharacterSelect>().characterConfirmed)
            {
                flagsChecked[i] = true;
            }
            else
            {
                flagsChecked[i] = false;
            }
        }

        for(int i = 0; i < flagsChecked.Length; i++)
        {
            if(flagsChecked[i] == false)
            {
                allFlagsClear = false;
                break;
            }
            else
            {
                allFlagsClear = true;
            }
        }

        if(allFlagsClear)
        {
            text.SetActive(true);
            startButton.SetActive(true);
        }
        else
        {
            text.SetActive(false);
            startButton.SetActive(false);
        }

	}
}
