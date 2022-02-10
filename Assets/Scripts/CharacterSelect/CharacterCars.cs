using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCars : MonoBehaviour {

    public GameObject selectedCharacter;

    public GameObject currentCarModel;
    //size of car model
    public float rotationSpeed;
    bool carSpawned;

    public List<Material> carMaterials;
    public MeshRenderer[] carMeshes;

    public Material[] mats;

    int localColorIndex;


	// Use this for initialization
	void Start () {

        PlayerPrefs.SetString("CarColorPlayer" + selectedCharacter.GetComponent<CharacterSelect>().playerID, carMaterials[0].name);

    }
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(-Vector3.up, rotationSpeed * Time.deltaTime);

	}

    public void ChangeCars()
    {
        carSpawned = false;
        Destroy(currentCarModel);
    }

    public void SpawnCar()
    {
        if(carSpawned)
        {
            return;
        }
        carSpawned = true;
        //set car to be used in next scene
        
        if(selectedCharacter.GetComponent<CharacterSelect>().characterName.ToLower().Contains("adam"))
        {
            currentCarModel = Instantiate(Resources.Load("CarModels/AdamReedsCar"), this.transform) as GameObject;
            carMeshes = currentCarModel.GetComponentsInChildren<MeshRenderer>();
            //start at 1 to ignore first mesh renderer of this car
            for(int i = 0; i < carMeshes.Length; i++)
            {
                carMeshes[i].material = carMaterials[localColorIndex];
            }
            PlayerPrefs.SetString("CarModelPlayer" + selectedCharacter.GetComponent<CharacterSelect>().playerID, "AdamReeds");
        }
        else if (selectedCharacter.GetComponent<CharacterSelect>().characterName.ToLower().Contains("richard"))
        {
            currentCarModel = Instantiate(Resources.Load("CarModels/RichardProstCar"), this.transform) as GameObject;
            carMeshes = currentCarModel.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < carMeshes.Length; i++)
            {
                if (carMeshes[i].name.ToLower().Contains("pcube2"))
                {
                    mats = carMeshes[i].materials;
                }
            }
            for (int i = 0; i < carMeshes.Length; i++)
            {
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].name.ToLower().Contains("lambert"))
                    {
                        mats[j] = carMaterials[localColorIndex];
                    }
                }
                carMeshes[i].materials = mats;
            }
            PlayerPrefs.SetString("CarModelPlayer" + selectedCharacter.GetComponent<CharacterSelect>().playerID, "RichardProst");
        }
        else if(selectedCharacter.GetComponent<CharacterSelect>().characterName.ToLower().Contains("minivan"))
        {
            currentCarModel = Instantiate(Resources.Load("CarModels/ExtraCar"), this.transform) as GameObject;
            carMeshes = currentCarModel.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < carMeshes.Length; i++)
            {
                if (carMeshes[i].name.ToLower().Contains("ppipe7"))
                {
                    mats = carMeshes[i].materials;
                }
            }
            for (int i = 0; i < carMeshes.Length; i++)
            {
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].name.ToLower().Contains("lambert1"))
                    {
                        mats[j] = carMaterials[localColorIndex];
                    }
                }
                carMeshes[i].materials = mats;
            }
            PlayerPrefs.SetString("CarModelPlayer" + selectedCharacter.GetComponent<CharacterSelect>().playerID, "Minivan");
        }
        else
        {
            currentCarModel = Instantiate(Resources.Load("CarModels/DefaultCar"), this.transform) as GameObject;
            carMeshes = currentCarModel.GetComponentsInChildren<MeshRenderer>();    
            for(int i = 0; i < carMeshes.Length; i++)
            {
                if(carMeshes[i].name.ToLower().Contains("pcube2"))
                {
                    mats = carMeshes[i].materials;
                }
            }
            //default cars only have one mesh
            for (int i = 0; i < carMeshes.Length; i++)
            {
                for(int j = 0; j < mats.Length; j++)
                {
                    if(mats[j].name.ToLower().Contains("lambert"))
                    {
                        mats[j] = carMaterials[localColorIndex];
                    }
                }
                carMeshes[i].materials = mats;
            }
            PlayerPrefs.SetString("CarModelPlayer" + selectedCharacter.GetComponent<CharacterSelect>().playerID, "Default");
        }
    }

    public void ChangeCarColors(int currentColorIndex)
    {
        
        //change colors for adam reeds car
        if (selectedCharacter.GetComponent<CharacterSelect>().characterName.ToLower().Contains("adam"))
        {
            for (int i = 1; i < carMeshes.Length; i++)
            {
                carMeshes[i].material = carMaterials[currentColorIndex];
                PlayerPrefs.SetString("CarColorPlayer" + selectedCharacter.GetComponent<CharacterSelect>().playerID, carMaterials[currentColorIndex].name);
            }
        }
        else if (selectedCharacter.GetComponent<CharacterSelect>().characterName.ToLower().Contains("richard"))
        {
            for (int i = 0; i < carMeshes.Length; i++)
            {
                mats[1] = carMaterials[currentColorIndex];
                carMeshes[i].materials = mats;
                PlayerPrefs.SetString("CarColorPlayer" + selectedCharacter.GetComponent<CharacterSelect>().playerID, carMaterials[currentColorIndex].name);
            }
        }
        else if (selectedCharacter.GetComponent<CharacterSelect>().characterName.ToLower().Contains("minivan"))
        {
            for (int i = 0; i < carMeshes.Length; i++)
            {
                mats[0] = carMaterials[currentColorIndex];
                carMeshes[i].materials = mats;
                PlayerPrefs.SetString("CarColorPlayer" + selectedCharacter.GetComponent<CharacterSelect>().playerID, carMaterials[currentColorIndex].name);
            }
        }
        //change colors for default cars
        else
        {
            for (int i = 0; i < carMeshes.Length; i++)
            {
                mats[1] = carMaterials[currentColorIndex];
                carMeshes[i].materials = mats;
                PlayerPrefs.SetString("CarColorPlayer" + selectedCharacter.GetComponent<CharacterSelect>().playerID, carMaterials[currentColorIndex].name);
            }
        }

        localColorIndex = currentColorIndex;
    }

    
}
