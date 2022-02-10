using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartRaceCounter : MonoBehaviour {

    public float countdown = 3;
    public float timerToNextNumber;
    float timer;

    public float playerID;
    GameObject player;

    GameObject[] aiRacers;

    GameObject manager;
    AudioManager audioManager;

    bool audioMaster;

	// Use this for initialization
	void Start () {

        manager = GameObject.FindGameObjectWithTag("AudioManager");

        if (manager != null)
            audioManager = manager.GetComponent<AudioManager>();

        if(playerID == 1)
        {
            audioMaster = true;
        }

        this.GetComponent<Text>().text = countdown + "";
        if (audioMaster && audioManager != null)
        {
            audioManager.Play("countdown" + countdown);
        }
        timer = timerToNextNumber;

	}
	
	// Update is called once per frame
	void Update () {
        player = GameObject.FindGameObjectWithTag("Player" + playerID);

        aiRacers = GameObject.FindGameObjectsWithTag("Player0");
        

        if(player == null)
        {
            return;
        }

        if (audioManager == null)
        {
            player.GetComponent<HoverCarController>().raceStarted = true;
            for (int i = 0; i < aiRacers.Length; i++)
            {
                aiRacers[i].GetComponent<HoverCarController>().raceStarted = true;
            }
            Destroy(this.gameObject);
        }

        timer -= Time.deltaTime;

        if(timer < 0)
        {
           
            timer = timerToNextNumber;
            countdown--;

            if (countdown > 0)
            {
                this.GetComponent<Text>().text = countdown + "";
                if (audioMaster && audioManager != null)
                {
                    audioManager.Play("countdown" + countdown);
                }
            }


            if (countdown == 0)
            {
                this.GetComponent<Text>().text = "GO!";
                if (audioMaster && audioManager != null)
                {
                    audioManager.Play("countdownGo");
                    for(int i = 0; i < aiRacers.Length; i++)
                    {
                        aiRacers[i].GetComponent<HoverCarController>().raceStarted = true;
                    }
                }
                player.GetComponent<HoverCarController>().raceStarted = true;
            }
            else if(countdown == -1)
            {
                this.GetComponent<Text>().text = "";
            }
            else if(countdown < -1)
            {
                Destroy(this.gameObject);
            }
        }
		
	}
}
