using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour {
    private Text timerText;
    private float timer;
    private float minutes, seconds, milliseconds;
    //private int minutes;
    bool timerStart;

    public float playerID;

    GameObject player;

	// Use this for initialization
	void Awake () {
        timerText = this.GetComponent<Text>();
        timerText.text = "00 : 00 : 00";
	}
	
	// Update is called once per frame
	void Update () {
        player = GameObject.FindGameObjectWithTag("Player" + playerID);

        if (player != null)
        {
            if (!player.GetComponent<HoverCarController>().raceStarted)
            {
                return;
            }
        }

        //if the player finishes the race, stop the timer
        if (player != null && !player.GetComponent<Lapping>().finishedRace)
        {
            timer += Time.deltaTime;
        }

        minutes = (int)(timer / 60);
        seconds = timer % 60;
        milliseconds = (timer * 100) % 100;

        //update timerText value
        timerText.text = string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, milliseconds);
        //Debug.Log(timer);
	}

    public bool TimerStart
    {
        get { return timerStart; }
        set { timerStart = value; }
    }

}
