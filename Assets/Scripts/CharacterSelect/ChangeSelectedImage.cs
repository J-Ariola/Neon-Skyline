using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSelectedImage : MonoBehaviour {

    //keeps track of the main image on the select screen
    public GameObject selectedCharacter;
    public GameObject selectedLevel;
    //keeps track of the name on the select screen
    public GameObject characterName;
    public string levelName;
    public Sprite levelImage;
    public int characterNum;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectedCharacter()
    {
        selectedCharacter.GetComponent<CharacterSelect>().characterName = this.name;
        //changes image on select screen
        selectedCharacter.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
        //changes name on select screen
        characterName.GetComponent<Text>().text = this.name;
    }

    public void SelectedLevel()
    {
        selectedLevel.GetComponent<SelectedLevel>().levelName = levelName;
        //changes image on select screen
        selectedLevel.GetComponent<Image>().sprite = levelImage;
        //changes name on select screen
        characterName.GetComponent<Text>().text = this.name;
    }
}
