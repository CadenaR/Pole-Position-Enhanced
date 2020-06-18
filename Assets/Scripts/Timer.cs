using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Timer: NetworkBehaviour
{
    float startTime;
    public string timerText;
    public List<float> lapTime = new List<float>();
    public float t;
    public float total;
    bool end = false;

    void Start()
    {
        startTime = (float)NetworkTime.time;
    }


    public void UpdateTimer()
    {
        if (!end){
            t = (float)NetworkTime.time - startTime;
            timerText = TimeToText(t);
        }
    }

    public string TimeToText(float t){
        string minutes = ((int)t / 60).ToString("00");
        string seconds = ((int)t % 60).ToString("00");
        string milliseconds = ((int)(t * 1000) % 1000).ToString("000");

        string timerText = minutes + " : " + seconds + " : " + milliseconds;
        return timerText;
    }

    public void ResetTimer()
    {
        startTime = (float)NetworkTime.time;
    }

    public float SaveTotalTime(){
        end = true;
        //total = t;
        return t;
    }

    public void SaveTime(int lap)
    {
        float aux = t;
        if (lap == 1){
            lapTime.Add(t);
        }
        else{
            foreach(float lt in lapTime)
            {
                aux -= lt;
            }
            lapTime.Add(aux);
        }
    }
}