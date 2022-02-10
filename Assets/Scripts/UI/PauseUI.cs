using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseUI : MonoBehaviour
{

    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> buttons;

    //used for story specific UI
    public List<GameObject> winUI;
    public List<GameObject> loseUI;

    public bool isPaused;
    public bool canBePaused;
    float pauseTimer;
    public float pauseTimerMax = 1f;
    public int isStoryMode;

    public bool finishedRace;
    public bool playerWon;


    bool buttonSet;

    public GameObject highlightImage;
    public Vector3 startingPos;

    EventSystem ES;

    // Use this for initialization
    void Start()
    {

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(false);

        }

        for (int i = 0; i < winUI.Count; i++)
        {
            winUI[i].SetActive(false);

        }

        for (int i = 0; i < loseUI.Count; i++)
        {
            loseUI[i].SetActive(false);

        }

        startingPos = highlightImage.transform.localPosition;
        highlightImage.SetActive(false);

        isStoryMode = PlayerPrefs.GetInt("IsStoryMode");

        //fail safe
        if (SceneManager.GetActiveScene().name.ToLower().Contains("story"))
        {
            isStoryMode = 1;
        }
        else
        {
            isStoryMode = 0;
        }

        ES = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
        ES.firstSelectedGameObject = buttons[0];
        buttons[0].GetComponent<Button>().OnSelect(null);

    }

    // Update is called once per frame
    void Update()
    {
        if (ES.currentSelectedGameObject != null)
        {
            //highlightImage.transform.parent = ES.currentSelectedGameObject.transform;
            highlightImage.transform.SetParent(ES.currentSelectedGameObject.transform, false);
            highlightImage.transform.SetSiblingIndex(0);
            highlightImage.transform.localPosition = startingPos;
        }

        if (!canBePaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                canBePaused = true;
                pauseTimer = pauseTimerMax;
                Debug.Log(pauseTimer);
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].GetComponent<Lapping>().finishedRace)
            {
                break;
            }

            if (i == players.Count - 1)
            {
                finishedRace = true;
            }
        }

        /*
        if (isPaused)
        {
            for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(true);
        }
        highlightImage.SetActive(true);
            
        }
        else
        {
             for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(true);
        }
        highlightImage.SetActive(true);
        }
        */

        if (finishedRace)
        {
            highlightImage.SetActive(true);
            if (players.Count == 1 && isStoryMode == 0)
            {
                highlightImage.GetComponent<RectTransform>().pivot = new Vector2(1, .5f);
                highlightImage.GetComponent<RectTransform>().anchorMax = new Vector2(1, .5f);
                highlightImage.GetComponent<RectTransform>().anchorMin = new Vector2(1, .5f);
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].SetActive(true);
                    if (players[0].GetComponent<Lapping>().finishedRace)
                    {
                        buttons[i].GetComponent<RectTransform>().pivot = new Vector2(1, .5f);
                        buttons[i].GetComponent<RectTransform>().anchorMax = new Vector2(1, .5f);
                        buttons[i].GetComponent<RectTransform>().anchorMin = new Vector2(1, .5f);                       
                    }
                }
            }
            else if(players.Count > 1 && isStoryMode == 0)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].SetActive(true);
                }
            }
            else if (isStoryMode == 1 && players.Count == 1)
            {
                if (players[0].GetComponent<Lapping>().finishedRace)
                {
                    if (players[0].GetComponent<HoverCarController>().currentPlace == 1)
                    {
                        playerWon = true;
                        for (int i = 0; i < winUI.Count; i++)
                        {
                            winUI[i].SetActive(true);
                        }
                    }
                    else
                    {
                        playerWon = false;
                        for (int i = 0; i < loseUI.Count; i++)
                        {
                            loseUI[i].SetActive(true);
                        }
                    }
                }
            }
        }

        if (finishedRace)
        {
            if ((finishedRace && isStoryMode == 0))
            {
                ES.firstSelectedGameObject = buttons[0];
            }

            if (finishedRace && isStoryMode == 1)
            {
                if (playerWon)
                {
                    ES.firstSelectedGameObject = winUI[1];
                }
                else
                {
                    ES.firstSelectedGameObject = loseUI[1];
                }
            }
        }

    }

    public void enablePauseButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(true);
        }
        highlightImage.SetActive(true);
    }

    public void disablePauseButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(false);
        }
        highlightImage.SetActive(false);
    }

}
