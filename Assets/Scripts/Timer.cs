using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Timer
{
    float startTime;
    public string timerText;
    public string[] lapTime;
    public float t;

    void Start()
    {
        startTime = (float)NetworkTime.time;
    }


    public void UpdateTimer()
    {
        t = (float)NetworkTime.time - startTime;
        string minutes = ((int)t / 60).ToString("00");
        string seconds = ((int)t % 60).ToString("00");
        string milliseconds = ((int)(t * 1000) % 1000).ToString("000"); ;

        timerText = minutes + " : " + seconds + " : " + milliseconds;
    }

    public void ResetTimer()
    {
        startTime = (float)NetworkTime.time;
    }

    public void SaveTime(int lap)
    {
        lapTime[lap] = timerText;
    }
}