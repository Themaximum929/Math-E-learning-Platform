using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    // Get and set Highscore
    public void SaveHighScore(string course_name, int HighScore)
    {
        PlayerPrefs.SetInt(course_name, HighScore);
    }
    public int GetHighScore(string course_name)
    {
        return PlayerPrefs.GetInt(course_name, 0);
    }
    
    // Get and Set User total points
    public void SavePoints(int points)
    {
        PlayerPrefs.SetInt("points", points);
    }
    public int GetPoints()
    {
        return PlayerPrefs.GetInt("points", 0);
    }

    // Get and set User difficulty
    public void SaveDifficulty(string course_name, int difficulty)
    {
        PlayerPrefs.SetInt(course_name + "difficulty", difficulty);
    }  
    public int GetDifficulty(string course_name)
    {
        return PlayerPrefs.GetInt(course_name + "difficulty", 0);
    }

    // Get and Set Background
    public void SaveBackground(string backgroundName)
    {
        PlayerPrefs.SetString("background", backgroundName);
    }
    public string GetBackground()
    {
        return PlayerPrefs.GetString("background", "");
    }
}
