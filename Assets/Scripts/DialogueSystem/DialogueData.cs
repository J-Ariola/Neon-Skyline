using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Dialogue Data", order = 1)]
public class DialogueData : ScriptableObject {
    //Be sure to attach DialogueData to the DialogueTrigger
    public List<Dialogue> DialogueInfo;
}
