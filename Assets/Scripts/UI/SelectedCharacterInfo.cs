using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacterInfo : MonoBehaviour {

	// Use this for initialization
	void Start () {

        GetComponent<TextMesh>().text = PlayerPrefs.GetString("CharacterSelectedPlayer1");
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
