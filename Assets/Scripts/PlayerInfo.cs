using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [SyncVar]
    public string Name;

    [SyncVar]
    public int lap;

    [SyncVar]
    public int maxLap;
    
    [SyncVar]
    public int checkpoint = 0;

    [SyncVar]
    public int ID;

    Timer playerTimer;

    public int CurrentPosition { get; set; }
    

    public override string ToString()
    {
        return Name;
    }

    #region Command

    [Command]
    public void CmdIncreaseLap()
    {
        playerTimer = FindObjectOfType<PolePositionManager>().time;
        if(lap <= maxLap){            
            playerTimer.SaveTime(FindObjectOfType<PlayerInfo>().lap);
            Debug.Log(playerTimer.TimeToText(playerTimer.lapTime[playerTimer.lapTime.Count-1]));
            lap++;
        }
        if (lap > maxLap)
        {
            Debug.Log(playerTimer.TimeToText(playerTimer.SaveTotalTime()));
            Debug.Log("Has terminado la carrera.");
        }
    }

    [Command]
    public void CmdSetCheckpoint(int c)
    {
        checkpoint = c;
    }


    #endregion
}