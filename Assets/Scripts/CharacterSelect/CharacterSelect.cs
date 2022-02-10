using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour {

    //current selected character
    public string characterName; 

    //player selecting
    public int playerID;
    public bool characterConfirmed;

    //enable image when character is selected
    public Image darkenScreen;

	// Use this for initialization
	void Start () {


		
	}
	
	// Update is called once per frame
	void Update () {

        if(this.GetComponent<Image>().sprite == null)
        {
            this.GetComponent<Image>().enabled = false;
        }
        else
            this.GetComponent<Image>().enabled = true;

        if (characterConfirmed)
        {
            //enable an image that will darken the screen a bit
            darkenScreen.enabled = true;
            
        }
        else
        {
            //disable above image
            darkenScreen.enabled = false;
        }

    }

    public void ConfirmCharacter()
    {
        PlayerPrefs.SetString("CharacterSelectedPlayer" + playerID, characterName);
        if(GetComponent<Image>().sprite != null)
        PlayerPrefs.SetString("Player" + playerID + "Profile", GetComponent<Image>().sprite.name);

        Debug.Log(characterName);
       

        if (this.GetComponent<Image>().sprite == null)
        {
            //if player doesn't select a character, pop up message
        }
        else
        {
            characterConfirmed = true;
        }

        //when all players have confirmed their characters, pop up text that says press start to continue. any player can press start
    }

    public void Back()
    {
        if(characterConfirmed)
        {
            darkenScreen.enabled = false;
            characterConfirmed = false;
        }
        else
        {
            GetComponent<ButtonToScene>().TransitionToScene("StudioMainMenuScene");
        }
    }
}
