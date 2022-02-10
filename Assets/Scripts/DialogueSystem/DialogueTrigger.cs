using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    //public Dialogue dialogue;

    public DialogueData dialogueData;
    public LevelLoader levelLoader;
    //Dialogue should only be triggered from either button or being attached to the EventSystem gameObject

    void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        
        if (name.Equals("EventSystem"))
        {
            if (levelLoader == null)
            {
                Debug.LogError("There is no level loader in this dialogue scene!");
            }
            //FindObjectOfType<DialogueManager>().StartDialogue(dialogueData);
            else
                StartCoroutine(EventTriggerDialogue());
        }
    }

    public void TiggerDialogue()
    {
        //FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogueData);
    }

    IEnumerator EventTriggerDialogue()
    {
        yield return new WaitForSeconds(levelLoader.levelFadeHandler.StartFade(-1));
        FindObjectOfType<DialogueManager>().StartDialogue(dialogueData);
        yield return null;
    }

}
