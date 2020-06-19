using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Esta clase sirve para contar el tiempo dentro de la partida y poder imprimirselo en la interfaz a los jugadores
public class Timer: NetworkBehaviour
{
    float startTime;
    public string timerText;
    public List<float> lapTime = new List<float>();
    public float t;    
    bool end = false;

    void Start()
    {
        startTime = (float)NetworkTime.time;
    }


    //Dentro de este método se actualizará el tiempo mientras no haya terminado la carrera
    public void UpdateTimer()
    {
        if (!end){
            t = (float)NetworkTime.time - startTime;
            timerText = TimeToText(t);
        }
    }

    //Formatea un float en segundos a un string que separa el tiempo en minutos, segundos y milisegundos
    public string TimeToText(float t){
        string minutes = ((int)t / 60).ToString("00");
        string seconds = ((int)t % 60).ToString("00");
        string milliseconds = ((int)(t * 1000) % 1000).ToString("000");

        string timerText = minutes + " : " + seconds + " : " + milliseconds;
        return timerText;
    }

    //Reinicia la variable startTime
    public void ResetTimer()
    {
        startTime = (float)NetworkTime.time;
    }

    //Al terminar de correr, el jugador llama a este método y se deja de contar el tiempo por lo que se queda almacenado en t
    //el tiempo tota de la vuelta
    public float SaveTotalTime(){
        end = true;        
        return t;
    }

    //Guarda el tiempo por vuelta en la lista lapTime para poder mostrarlo luego en las estadísticas del jugador.
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