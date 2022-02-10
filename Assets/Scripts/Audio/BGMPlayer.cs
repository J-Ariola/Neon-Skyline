using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMPlayer : MonoBehaviour {

    public AudioManager audioManager;
    public GameObject[] players;

    public bool songStarted;
    public string currentSongName;

	// Use this for initialization
	void Start () {
        //max number of players is 4
        players = new GameObject[4];
        
	}

    private void Update()
    {   if (!songStarted)
        {
            //find audio manager and player object
            audioManager = FindObjectOfType<AudioManager>();
            //player 1 will always be in the scene, so search for this player
            players[0] = GameObject.FindGameObjectWithTag("Player1");

            //finds the remaining players within the scene, start with 1 cause we already have player 1
            for (int i = 1; i < players.Length; i++)
            {
                if (GameObject.FindGameObjectWithTag("Player" + (i + 1)) != null)
                {
                    players[i] = GameObject.FindGameObjectWithTag("Player" + (i + 1));
                }
            }

            if (audioManager == null)
            {
                return;
            }

            //play music of tracks here
            if (players[0].GetComponent<HoverCarController>().raceStarted && !songStarted)
            {
                if (SceneManager.GetActiveScene().name.ToLower().Contains("0"))
                {
                    audioManager.Play("level3Song");
                    currentSongName = "level3Song";
                    songStarted = true;
                    //based on song BPM, change color pulse beat for character
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i] != null)
                            players[i].GetComponent<HealthController>().bpm = 145;
                    }
                }
                else if (SceneManager.GetActiveScene().name.ToLower().Contains("1"))
                {
                    audioManager.Play("level1Song");
                    currentSongName = "level1Song";
                    songStarted = true;
                    //based on song BPM, change color pulse beat for character
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i] != null)
                            players[i].GetComponent<HealthController>().bpm = 145;
                    }
                }

                else if (SceneManager.GetActiveScene().name.ToLower().Contains("2"))
                {
                    audioManager.Play("level2Song");
                    currentSongName = "level2Song";
                    songStarted = true;
                    //based on song BPM, change color pulse beat for character
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i] != null)
                            players[i].GetComponent<HealthController>().bpm = 160;
                    }
                }

                else if (SceneManager.GetActiveScene().name.ToLower().Contains("3") || SceneManager.GetActiveScene().name.ToLower().Contains("4"))
                {
                    audioManager.Play("level3Song");
                    currentSongName = "level3Song";
                    songStarted = true;
                    //based on song BPM, change color pulse beat for character
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i] != null)
                            players[i].GetComponent<HealthController>().bpm = 145;
                    }
                }

                else if(SceneManager.GetActiveScene().name.ToLower().Contains("procedural"))
                {
                    int songNum = Random.Range(1, 3);
                    audioManager.Play("level" + songNum + "Song");
                    currentSongName = "level" + songNum + "Song";
                    songStarted = true;
                    //based on song BPM, change color pulse beat for character
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i] != null)
                        {
                            if (currentSongName.Contains("1") || currentSongName.Contains("3"))
                            {
                                players[i].GetComponent<HealthController>().bpm = 145;
                            }
                            else if(currentSongName.Contains("2"))
                            {
                                players[i].GetComponent<HealthController>().bpm = 160;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (audioManager != null)
        {
            audioManager.Stop(currentSongName);
        }
    }
}
