using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class JsonHelper {
    public static ScoreData[] getJsonArray<ScoreData>(string json)
    {
        string newJson = "{ \"array\": " + json + " }";
        Wrapper<ScoreData> wrapper = JsonUtility.FromJson<Wrapper<ScoreData>>(newJson);
        return wrapper.array;
    }

    public static string arrayToJson<ScoreData>(ScoreData[] array)
    {
        Wrapper<ScoreData> wrapper = new Wrapper<ScoreData>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    [System.Serializable]
    private class Wrapper<ScoreData>
    {
        public ScoreData[] array;
    }

}
