using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "Character Profile", order = 1)]
public class CharacterProfile : ScriptableObject {

    public string name;

    public Texture2D portrait;

    [ColorHtmlProperty]
    public Color dialogueTextColor = Color.white;

    
}
