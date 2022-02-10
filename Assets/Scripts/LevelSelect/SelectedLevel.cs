using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedLevel : MonoBehaviour
{

    //current selected character
    public string levelName;

    //player selecting
    public int playerID;

    //In charge of loading levels
    public Transform levelLoader;

    public GameObject seedInput;
    public GameObject trackPiecesInput;

    // Use this for initialization
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

        if (this.GetComponent<Image>().sprite == null)
        {
            this.GetComponent<Image>().enabled = false;
        }
        else
            this.GetComponent<Image>().enabled = true;

    }

    public void ConfirmLevel()
    {
        //GetComponent<ButtonToScene>().TransitionToScene(levelName);
        PlayerPrefs.SetString("SeedInput", seedInput.GetComponent<InputField>().text);
        if(trackPiecesInput.GetComponent<InputField>().text == "")
        {
            trackPiecesInput.GetComponent<InputField>().text = "0";
        }
        PlayerPrefs.SetInt("TrackPieces", int.Parse(trackPiecesInput.GetComponent<InputField>().text));
        levelLoader.GetComponent<LevelLoader>().LoadLevel(levelName);
    }

    public void Back()
    {
       //GetComponent<ButtonToScene>().TransitionToScene("CharacterSelect");
       levelLoader.GetComponent<LevelLoader>().LoadLevel("CharacterSelect");
    }
}
