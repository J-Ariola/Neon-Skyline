using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public Text nameText, dialogueText, narratorText;

    public Animator boxAnimator;

    public Image portrait;

    public LevelLoader levelLoader;

    public string nextSceneName;

    DialogueData dialogueData;

    int dialogueDataCounter, dialogueDataMax;

    Queue<string> sentences;

    bool isNarration;

    [Range(0, 20)]
    [Tooltip("How fast the text types each letter per second")]
    float typeLetterPerSecond = 15f;

	void Awake () {
        sentences = new Queue<string>();
	}
	
    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue.characterProfile != null)
        {
            //If the speaker is the narrator, clear the normal dialogue area and use the narratorText
            if (dialogue.characterProfile.name == "Narrator")
            {
                narratorText.color = dialogue.characterProfile.dialogueTextColor;

                dialogueText.color = Color.clear;
                dialogueText.color = Color.clear;
                nameText.color = Color.clear;
                portrait.sprite = null;
                portrait.color = Color.black;

                isNarration = true;
            }
            //else use the normal dialogue box and show the portrait
            else
            {
                narratorText.color = Color.clear;

                dialogueText.color = dialogue.characterProfile.dialogueTextColor;
                nameText.color = dialogue.characterProfile.dialogueTextColor;
                portrait.color = Color.white;

                isNarration = false;

                if (dialogue.characterProfile.portrait != null)
                {
                    portrait.sprite = Sprite.Create(dialogue.characterProfile.portrait, new Rect(0, 0, dialogue.characterProfile.portrait.width, dialogue.characterProfile.portrait.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
                else
                {
                    portrait.sprite = null;
                    portrait.color = Color.black;
                }
            }

            boxAnimator.SetBool("IsOpen", true);
            //outlineAnimator.SetBool("IsOpen", true);

            //Debug.Log("Starting conversation with: " + dialogue.name);
            nameText.text = dialogue.characterProfile.name;

            //Remove dialogue from the previous conversation
            sentences.Clear();

            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }
        else
            Debug.LogError("Dialogue is missing a character profile!");
    }
    
    public void StartDialogue(DialogueData Data)
    {
        //Load Dialogue Data
        if (Data != null)
        {
            dialogueData = Data;
            dialogueDataCounter = 0;
            StartDialogue(dialogueData.DialogueInfo[dialogueDataCounter]);
            dialogueDataMax = dialogueData.DialogueInfo.Count;
        }
        else
            Debug.LogError("Please insert Dialogue Data to Dialogue Trigger!");

    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        //dialogueText.text = sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    //Type dialogue sentence a letter at a time
    IEnumerator TypeSentence (string sentence)
    {
        if (isNarration)
        {
            narratorText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                narratorText.text += letter;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            dialogueText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForFixedUpdate();
            }
        }
    }

    void SwitchDialogue()
    {
        dialogueDataCounter++;
        Debug.Log("Switching Dialogue to: " + dialogueData.DialogueInfo[dialogueDataCounter].characterProfile.name);
        StartDialogue(dialogueData.DialogueInfo[dialogueDataCounter]);
    }

    void EndDialogue()
    {
        if (dialogueDataCounter < dialogueDataMax - 1)
        {
            SwitchDialogue();
        }
        else
        {
            boxAnimator.SetBool("IsOpen", false);
            //outlineAnimator.SetBool("IsOpen", false);
            Debug.Log("End of conversation");
            if (nextSceneName.ToLower().Contains("story"))
            {
                //Sets Player1 as Adam Reeds for story mode
                PlayerPrefs.SetString("Player1Profile", "Adam Reeds");
                PlayerPrefs.SetString("CharacterSelectedPlayer1", "Adam Reeds");
                PlayerPrefs.SetString("Player1Profile", "AdamReeds");

                levelLoader.isSinglePlayer(true);
            }
            else
                levelLoader.isSinglePlayer(false);
            levelLoader.LoadLevel(nextSceneName);   
        }
    }
}
