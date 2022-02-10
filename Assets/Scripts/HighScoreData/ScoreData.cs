//Jarrod Ariola
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreData
{
    public string playerName;
    public int minutes;
    public int seconds;
    public int milliseconds;
    /******If we implement a score system based on tricks, total time, etc
    public int score;
    */

    public ScoreData()
    {
        playerName = "";
        minutes = 0;
        seconds = 42;
        milliseconds = 0;
    }
}