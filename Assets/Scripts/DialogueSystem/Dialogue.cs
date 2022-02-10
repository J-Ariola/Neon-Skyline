using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue {

    //public string name;

    //public Texture2D portrait;

    //[ColorHtmlProperty]
    //public Color dialogueTextColor = Color.white;

    //public string portaitInfo

    public CharacterProfile characterProfile;

    [TextArea(3, 10)]
    public string[] sentences;

    
}



