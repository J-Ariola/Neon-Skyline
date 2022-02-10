using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPrefab : MonoBehaviour
{

    public int playerID;
    public GameObject currentCar;

    public string carModelName;
    string[] controllers;

    //determines if model we are loading is an AI or not
    public bool isAI;

    // Use this for initialization
    void Start()
    {

        controllers = Input.GetJoystickNames();



        carModelName = PlayerPrefs.GetString("CarModelPlayer" + playerID);
        /*
        if (PlayerPrefs.GetInt("IsStoryMode") == 1 && playerID != 1)
        {
            return;
        }
        */

        SetRacers();

        Debug.Log("IsStoryMode = " + PlayerPrefs.GetInt("IsStoryMode") + " and isAI: " + isAI);

        if (PlayerPrefs.GetInt("IsStoryMode") == 1 && !isAI)
        {
            //if in storymode, always initiate as Adam Reeds
            currentCar = Instantiate(Resources.Load("Prefabs/AdamReedsCarPlayer" + playerID)) as GameObject;
        }
        else if (!isAI)
        {
            currentCar = Instantiate(Resources.Load("Prefabs/" + carModelName + "CarPlayer" + playerID)) as GameObject;
        }
        else if (isAI)
        {
            currentCar = Instantiate(Resources.Load("Prefabs/AIRacer")) as GameObject;
            currentCar.GetComponent<CarAIController>().playstyle = (CarAIController.playStyleEnum)Random.Range(0, System.Enum.GetValues(typeof(CarAIController.playStyleEnum)).Length);
            Debug.Log(currentCar.name + " playstyle = " + currentCar.GetComponent<CarAIController>().playstyle);
        }

        //spawn point of car will be based on this objects position and rotation
        currentCar.transform.position = this.transform.position;
        currentCar.transform.rotation = this.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {



        //if (carModelName != "")
        //{
        //    currentCar = Instantiate(Resources.Load("Prefabs/" + carModelName + "CarPlayer" + playerID), this.transform) as GameObject;
        //}
        ////fail safe
        //else
        //{
        //    currentCar = Instantiate(Resources.Load("Prefabs/DefaultCarPlayer" + playerID), this.transform) as GameObject;
        //}

    }

    void SetRacers()
    {
        if (playerID == 0)
        {
            isAI = true;
        }
        else if (playerID != 1 && controllers.Length < playerID)
        {
            isAI = true;
        }
        else if (controllers.Length >= playerID)
        {
            if (controllers[playerID - 1].Equals(""))
            {
                isAI = true;
            }
        }
    }
}
