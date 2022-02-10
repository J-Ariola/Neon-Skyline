using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HighScoreDataController : MonoBehaviour {

    public HighScoreData allHighScoreData;
    public bool restart;
    private string gameDataFileName = "HighScores.json";
    private string gameDataProjectFilePath = "/StreamingAssets/HighScores.json";

	// Use this for initialization
	void Start () {
        allHighScoreData = new HighScoreData();

        DontDestroyOnLoad(gameObject);
        if(restart)
        {
            RestartHighScoreData();
        }
        LoadHighScoreData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LoadHighScoreData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        //Read the text to deserialize to highScore
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            HighScoreData loadedData = JsonUtility.FromJson<HighScoreData>(dataAsJson);

            //Retrieve the highScores of loadedData
            allHighScoreData = loadedData;
        }
        else
        {
            Debug.LogError("Unable to load scores");
        }
    }

    private void SaveHighScoreData()
    {
        HighScoreData saveData = new HighScoreData();

        saveData.allScoreData = allHighScoreData.allScoreData;

        //Serialize GameData to Json
        string dataAsJson = JsonUtility.ToJson(saveData);
        //string dataAsJson = JsonHelper.arrayToJson(saveData.allScoreData);
        Debug.Log("Passed to Json:  " + dataAsJson);
        string filePath = Application.dataPath + gameDataProjectFilePath;
        File.WriteAllText(filePath, dataAsJson);
        Debug.Log("Data saved!");
    }
    private void RestartHighScoreData()
    {
        SaveHighScoreData();
    }
}
