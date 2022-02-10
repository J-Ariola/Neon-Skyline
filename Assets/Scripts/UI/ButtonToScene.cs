using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonToScene : MonoBehaviour {

    public string sceneName;
	
    public void TransitionToScene()
    {
        if (sceneName.ToUpper().Equals("QUIT"))
        {
            Debug.Log("Quitting Application");
            Application.Quit();
        }
        else
        {
            /*
            if (this.gameObject.name == "MechanicsSceneButton")
                SceneManager.LoadScene("MechanicsScene");
            else if (this.gameObject.name == "GUISceneButton")
                SceneManager.LoadScene("DialogueSystemTestScene");
            else if(this.gameObject.name == "MainMenuButton")
            {
                SceneManager.LoadScene("StudioMainMenuScene");
            }
            else
                Application.Quit();
            */
            SceneManager.LoadScene(sceneName);

        }
    }

    public void TransitionToScene(string scene)
    {
        if (sceneName.ToUpper().Equals("QUIT"))
        {
            Debug.Log("Quitting Application");
            Application.Quit();
        }
        else
        {
            /*
            if (this.gameObject.name == "MechanicsSceneButton")
                SceneManager.LoadScene("MechanicsScene");
            else if (this.gameObject.name == "GUISceneButton")
                SceneManager.LoadScene("DialogueSystemTestScene");
            else if(this.gameObject.name == "MainMenuButton")
            {
                SceneManager.LoadScene("StudioMainMenuScene");
            }
            else
                Application.Quit();
            */
            SceneManager.LoadScene(scene);

        }
    }
}
