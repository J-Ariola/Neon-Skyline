using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Saves to /Assets/StreamingAssets/HighScores
//Helps display the values in the Unity Inspector and Editor
[System.Serializable]
public class HighScoreData{

    public ScoreData[] allScoreData = new ScoreData[20];

    public HighScoreData()
    {
        for (int i = 0; i < allScoreData.Length; i++)
            allScoreData[i] = new ScoreData();
    }
}
