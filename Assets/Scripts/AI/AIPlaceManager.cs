using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlaceManager : MonoBehaviour {

    public List<GameObject> Racers;

    public List<GameObject> playerRacers = new List<GameObject>();
    public List<GameObject> AIRacers = new List<GameObject>();
    public GameObject leastAdvancedPlayer;

    public bool debugRacersPlace;

    [Range(1,10)]
    public int currentNodeDifference = 5;
    [Range(1, 10)]
    public float rubberBandCooldownTimer = 4f;
    [Range(1, 10)]
    [Tooltip("Bonus to speed for rubberbanding purposes")]
    public float speedBonus = 5f;
    private float rubberBandTimer;
    private int leastAdvancedPlayerCurrentLap;
    private bool foundRacers = false;
    private float originalSpeed;
    private float fastRubberbandSpeed, slowRubberbandSpeed;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (!foundRacers)
        {
            //Finds all Racers in the scene
            FindRacers();
            originalSpeed = FindObjectOfType<HoverCarController>().speed;
            fastRubberbandSpeed = originalSpeed + speedBonus;
            slowRubberbandSpeed = originalSpeed - speedBonus;
        }

        if (foundRacers)
        {
            if (debugRacersPlace)
                GetPlayersPlace();

            GetLeastAdvancedPlayer();

            if (leastAdvancedPlayer != null)
            {
                GetLeastAdvancedPlayerCurrentLap();

                CheckAIRacers();
            }
        }
	}

    void FindRacers()
    {
        HoverCarController[] RacerArray = FindObjectsOfType<HoverCarController>();

        foreach (HoverCarController hcc in RacerArray)
        {
            Racers.Add(hcc.gameObject);
            //Determine which of the racers are players or AI
            switch (hcc.driver)
            {
                case HoverCarController.driverEnum.Player:
                    playerRacers.Add(hcc.gameObject);
                    break;
                case HoverCarController.driverEnum.AI:
                    AIRacers.Add(hcc.gameObject);
                    break;
            }

        }
        foundRacers = true;
    }

    void GetPlayersPlace()
    {
        foreach (GameObject go in Racers)
        {
            Debug.Log(go.name + "'s Place = " + go.GetComponent<HoverCarController>().currentPlace);
        }
    }

    void GetLeastAdvancedPlayer()
    {
        float playerLastPlace = 1;

        //Determine which player is in last place
        foreach (GameObject player in playerRacers)
        {
            float playerCurrentPlace = player.GetComponent<HoverCarController>().currentPlace;
            if (playerRacers.Count == 1)
            {
                leastAdvancedPlayer = player;
                break;
            }
            if (playerCurrentPlace >= playerLastPlace)
            {
                leastAdvancedPlayer = player;
                playerLastPlace = playerCurrentPlace;
            }

        }
        //Debug.Log("Least advance Player: " + leastAdvancedPlayer.name + " in " + leastAdvancedPlayer.GetComponent<HoverCarController>().currentPlace + " place");
    }

    void GetLeastAdvancedPlayerCurrentLap()
    {
        leastAdvancedPlayerCurrentLap = leastAdvancedPlayer.GetComponent<Lapping>().currentLap;
    }

    void CheckAIRacers()
    {
        foreach (GameObject AI in AIRacers)
        {
            //If the AI is behind the least advanced player
            if (AI.GetComponent<HoverCarController>().currentPlace > leastAdvancedPlayer.GetComponent<HoverCarController>().currentPlace)
            {
                if (rubberBandTimer <= 0)
                    AIBehindPlayer(AI);
                else
                    rubberBandTimer -= Time.deltaTime;
            }
            /*
            else if (AI.GetComponent<HoverCarController>().currentPlace < leastAdvancedPlayer.GetComponent<HoverCarController>().currentPlace)
            {
                AIAheadPlayer(AI);
            }
            */
            else
            {
                rubberBandTimer -= Time.deltaTime;
                //AI.GetComponent<HoverCarController>().speed = originalSpeed;
            }
        }

        rubberBandTimer = Mathf.Clamp(rubberBandTimer, 0, rubberBandCooldownTimer);
    }

    void AIBehindPlayer(GameObject AIRacer)
    {
        CarAIController AIController = AIRacer.GetComponent<CarAIController>();
        CarAIController playerController = leastAdvancedPlayer.GetComponent<CarAIController>();

        if (Mathf.Abs(AIController.CurrentNode - playerController.CurrentNode) > currentNodeDifference)
        {
            //Debug.Log(AIRacer.name + " current place: " + AIRacer.GetComponent<HoverCarController>().currentPlace + " -> teleporting behind " + leastAdvancedPlayer.name + " in " + leastAdvancedPlayer.GetComponent<HoverCarController>().currentPlace + " place!");
            Lapping AI_Lapping = AIRacer.GetComponent<Lapping>();
            Lapping LAP_Lapping = leastAdvancedPlayer.GetComponent<Lapping>();


            AIRacer.GetComponent<HoverCarController>().ResetCarOnTrack(leastAdvancedPlayer.GetComponent<GravityController>().previousContactTrack);
            AI_Lapping.currentLap = LAP_Lapping.currentLap;
            AI_Lapping.HalfwayPoint = LAP_Lapping.HalfwayPoint;
            AIController.SetCurrentNode(playerController.CurrentNode - (currentNodeDifference / 2) + 2);

            rubberBandTimer = rubberBandCooldownTimer;
        }

        else
            AIRacer.GetComponent<HoverCarController>().speed = fastRubberbandSpeed;

    }

    void AIAheadPlayer(GameObject AIRacer)
    {
        CarAIController AIController = AIRacer.GetComponent<CarAIController>();
        CarAIController playerController = leastAdvancedPlayer.GetComponent<CarAIController>();

        if (Mathf.Abs(AIController.CurrentNode - playerController.CurrentNode) > currentNodeDifference)
        {
            //Debug.Log(AIRacer.name + " ahead player -> slowing down");
            AIRacer.GetComponent<HoverCarController>().speed = slowRubberbandSpeed;
        }
    }
}
