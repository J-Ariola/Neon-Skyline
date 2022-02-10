//Jarrod Ariola
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class HighScoresEditor : EditorWindow {

    public HighScoreData highScoreData;

    private string highScoreDataProjectFilePath = "/StreamingAssets/HighScores.json";

    Vector2 scrollPos;
    [MenuItem("Window/High Score Viewer (Read-Only)")]

    static void Init()
    {
        EditorWindow.GetWindow(typeof(HighScoresEditor)).Show();
    }

    void OnGUI()
    {
        //Scrolling
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(EditorWindow.GetWindow(typeof(HighScoresEditor)).position.width), GUILayout.Height(EditorWindow.GetWindow(typeof(HighScoresEditor)).position.height));

        if (highScoreData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("highScoreData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Reset Scores (BEWARE)"))
            {
                ResetHighScores();
            }
        }
        else
        {
            //ResetHighScores();
            highScoreData = new HighScoreData();
            ResetHighScores();
            //SaveScores();
        }
        if (GUILayout.Button("Load Scores"))
        {
            LoadScores();
            //ResetHighScores();
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void LoadScores()
    {
        string filePath = Application.dataPath + highScoreDataProjectFilePath;

        //Read the text to deserialize to highScore
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            HighScoreData loadedData = JsonUtility.FromJson<HighScoreData>(dataAsJson);
        }
        else
        {
            highScoreData = new HighScoreData();
        }
    }

    private void SaveScores()
    {
        //Serialize GameData to Json
        string dataAsJson = JsonUtility.ToJson(highScoreData);

        string filePath = Application.dataPath + highScoreDataProjectFilePath;
        File.WriteAllText(filePath, dataAsJson);
    }

    private void ResetHighScores()
    {
        foreach (ScoreData score in highScoreData.allScoreData)
        {
            score.playerName = "";
            score.minutes = 0;
            score.seconds = 0;
            score.milliseconds = 0;
        }
        SaveScores();
    }
       
}
