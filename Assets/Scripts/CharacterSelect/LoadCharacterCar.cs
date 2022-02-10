using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacterCar : MonoBehaviour {

    public GameObject currentCarModel;
    string carModelName = "";

    MeshRenderer[] carMeshes;
    Material[] mats;
    string materialName;

    public int playerID;

    public Material[] carColors;

    string[] controllers;

	void Awake () {

        controllers = Input.GetJoystickNames();

        //if(currentCarModel != null)
        //{
        //    Destroy(currentCarModel);
        //}

        ////destroys other player cars not currently being controller
        //if(playerID != 1 && controllers.Length < playerID)
        //{
        //    Destroy(this.gameObject);
        //}

        //if(playerID != 1 && controllers.Length >= playerID)
        //{
        //    if(controllers[playerID - 1] == "" || PlayerPrefs.GetInt("IsStoryMode") == 1)
        //    {
        //        Destroy(this.gameObject);
        //    }
        //}

        if(this.GetComponent<HoverCarController>().driver == HoverCarController.driverEnum.Player)
        carModelName = PlayerPrefs.GetString("CarModelPlayer" + playerID);

        carMeshes = currentCarModel.GetComponentsInChildren<MeshRenderer>();

        if(carModelName.ToLower().Contains("adam"))
        {
            for (int i = 0; i < carMeshes.Length; i++)
            {
                if (carMeshes[i].name.ToLower().Contains("carbody"))
                {
                    mats = carMeshes[i].materials;
                }
            }
            for (int i = 0; i < carMeshes.Length; i++)
            {
                //QUICK FIX
                if (carMeshes[i].name.ToLower().Contains("spikes") || carMeshes[i].name.ToLower().Contains("base") || carMeshes[i].name.ToLower().Contains("shield") ||
                    carMeshes[i].name.ToLower().Contains("repair") || carMeshes[i].name.ToLower().Contains("boost") || carMeshes[i].name.ToLower().Contains("thruster"))
                {
                    continue;
                }
                    materialName = PlayerPrefs.GetString("CarColorPlayer" + playerID);
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].name.ToLower().Contains("glow"))
                    {
                        mats[j] = Resources.Load("Materials/" + materialName) as Material;
                        break;
                    }
                }
                carMeshes[i].materials = mats;
            }
        }
        else if (carModelName.ToLower().Contains("richard"))
        {
            for (int i = 0; i < carMeshes.Length; i++)
            {
                if (carMeshes[i].name.ToLower().Contains("carbody"))
                {
                    mats = carMeshes[i].materials;
                }
            }
            for (int i = 0; i < carMeshes.Length; i++)
            {
                //QUICK FIX
                if (carMeshes[i].name.ToLower().Contains("spikes") || carMeshes[i].name.ToLower().Contains("base") || carMeshes[i].name.ToLower().Contains("shield") ||
                    carMeshes[i].name.ToLower().Contains("repair") || carMeshes[i].name.ToLower().Contains("boost") || carMeshes[i].name.ToLower().Contains("thruster"))
                {
                    continue;
                }
                materialName = PlayerPrefs.GetString("CarColorPlayer" + playerID);
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].name.ToLower().Contains("glow"))
                    {
                        mats[j] = Resources.Load("Materials/" + materialName) as Material;
                        break;
                    }
                }
                carMeshes[i].materials = mats;
            }
        }
        else if(carModelName.ToLower().Contains("default"))
        {
            for (int i = 0; i < carMeshes.Length; i++)
            {
                if (carMeshes[i].name.ToLower().Contains("carbody"))
                {
                    mats = carMeshes[i].materials;
                }
            }
            for (int i = 0; i < carMeshes.Length; i++)
            {
                //QUICK FIX
                if (carMeshes[i].name.ToLower().Contains("spikes") || carMeshes[i].name.ToLower().Contains("base") || carMeshes[i].name.ToLower().Contains("shield") ||
                    carMeshes[i].name.ToLower().Contains("repair") || carMeshes[i].name.ToLower().Contains("boost") || carMeshes[i].name.ToLower().Contains("thruster"))
                {
                    continue;
                }
                materialName = PlayerPrefs.GetString("CarColorPlayer" + playerID);
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].name.ToLower().Contains("glow"))
                    {
                        if (materialName != "")
                        {
                            mats[j] = Resources.Load("Materials/" + materialName) as Material;
                            break;
                        }
                        //fail safe
                        else
                        {
                            mats[j] = Resources.Load("Materials/CarGlowRed") as Material;
                        }
                    }
                }
                carMeshes[i].materials = mats;
            }
        }
        else if (carModelName.ToLower().Contains("minivan"))
        {
            for (int i = 0; i < carMeshes.Length; i++)
            {
                if (carMeshes[i].name.ToLower().Contains("carbody"))
                {
                    mats = carMeshes[i].materials;
                }
            }
            for (int i = 0; i < carMeshes.Length; i++)
            {
                //QUICK FIX
                if (carMeshes[i].name.ToLower().Contains("spikes") || carMeshes[i].name.ToLower().Contains("base") || carMeshes[i].name.ToLower().Contains("shield") ||
                    carMeshes[i].name.ToLower().Contains("repair") || carMeshes[i].name.ToLower().Contains("boost") || carMeshes[i].name.ToLower().Contains("thruster"))
                {
                    continue;
                }
                materialName = PlayerPrefs.GetString("CarColorPlayer" + playerID);
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].name.ToLower().Contains("glow"))
                    {
                        if (materialName != "")
                        {
                            mats[j] = Resources.Load("Materials/" + materialName) as Material;
                            break;
                        }
                        //fail safe
                        else
                        {
                            mats[j] = Resources.Load("Materials/CarGlowRed") as Material;
                        }
                    }
                }
                carMeshes[i].materials = mats;
            }
        }
        else
        {
            for (int i = 0; i < carMeshes.Length; i++)
            {
                if (carMeshes[i].name.ToLower().Contains("carbody"))
                {
                    mats = carMeshes[i].materials;
                }
            }
            for (int i = 0; i < carMeshes.Length; i++)
            {
                //QUICK FIX
                if (carMeshes[i].name.ToLower().Contains("spikes") || carMeshes[i].name.ToLower().Contains("base") || carMeshes[i].name.ToLower().Contains("shield") ||
                    carMeshes[i].name.ToLower().Contains("repair") || carMeshes[i].name.ToLower().Contains("boost") || carMeshes[i].name.ToLower().Contains("thruster"))
                {
                    continue;
                }
                carColors = Resources.LoadAll<Material>("Materials");
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].name.ToLower().Contains("glow"))
                    {
                        mats[j] = carColors[Random.Range(0, carColors.Length)];
                    }
                }
                carMeshes[i].materials = mats;
            }
        }

      //  RuntimeAnimatorController animController = Resources.Load("Animations/CarAnimations/CarAnimations") as RuntimeAnimatorController;
        //GetComponent<Animator>().runtimeAnimatorController = animController;

        //Sets the colliders of the car as a child to the car's model
       // Transform colliders = transform.Find("Colliders");
       // colliders.parent = currentCarModel.transform;

        //Sets the CarCanvas of the car as a child to the car's model
        //Transform carCanvas = transform.Find("CarCanvas");
        //carCanvas.parent = currentCarModel.transform;
       // carCanvas.SetParent(currentCarModel.transform, false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
