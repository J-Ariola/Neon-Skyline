using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {

    public GameObject FadeScreen;
    public GameObject loadingScreen;
    public Image LoadingFillBar;

    [ColorHtmlProperty]
    public Color beginFillColor;

    [ColorHtmlProperty]
    public Color endFillColor;

    GameObject manager;
    AudioManager audioManager;

    public LevelFadeHandler levelFadeHandler;

    void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("AudioManager");

        if (manager != null)
            audioManager = manager.GetComponent<AudioManager>();
        try
        {
            levelFadeHandler = FadeScreen.GetComponent<LevelFadeHandler>();
        }
        catch (System.Exception e)
        {
            Debug.LogError("levelFadeHandler has been unassigned. If the current scene is not a dialogue scene, please ignore this error");
        }
    }

    public void LoadLevel (string sceneName)
    {
        if(audioManager != null)
        audioManager.Play("menuNavigation");
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    public void isSinglePlayer(bool isSinglePlayer)
    {
        if(isSinglePlayer)
        {
            PlayerPrefs.SetInt("IsStoryMode", 1);
        }
        else
        {
            PlayerPrefs.SetInt("IsStoryMode", 0);
        }
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        //Fade to black
        if (levelFadeHandler != null && sceneName.ToLower().Contains("dialogue"))
            yield return new WaitForSeconds(levelFadeHandler.StartFade(1));
        else
            yield return null;

        if (sceneName.ToUpper().Equals("QUIT"))
            Application.Quit();
        //stores a 1 if we are going into story mode(will prevent multiplayer in levels)
        //if (sceneName.ToLower().Equals("dialoguescene1"))
        //    PlayerPrefs.SetInt("IsStoryMode", 1);
        ////stores a 0 if we are going into arcade mode
        //if (sceneName.ToLower().Equals("characterselect"))
        //    PlayerPrefs.SetInt("IsStoryMode", 0);


        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            //From 0 - 1 -> 0 - 0.5f;
            LoadingFillBar.fillAmount = progress * 0.5f;
            LoadingFillBar.color = Color.Lerp(beginFillColor, endFillColor, progress);
            //Debug.Log(progress);
            //Debug.Log(LoadingFillBar.fillAmount);
            //progressText.text = progress * 100f + "%";
            yield return null;
        }
        
    }

    void OnEnable()
    {
        if (levelFadeHandler != null)
            levelFadeHandler.StartFade(-1);
    }


}
