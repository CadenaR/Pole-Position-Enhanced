using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [SyncVar]
    public string Name;

    [SyncVar]
    public int lap = 1;

    [SyncVar]
    public int maxLap;
    
    [SyncVar]
    public int checkpoint = 0;

    [SyncVar]
    public int ID;

    public Timer playerTimer;

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
            playerTimer.SaveTotalTime();
            RpcSaveTotalTime();
            Debug.Log("Has terminado la carrera.");
            FindObjectOfType<CameraController>().SwapToEndCamera();            
        }
    }

    [Command]
    public void CmdSetCheckpoint(int c)
    {
        checkpoint = c;
    }
    #endregion

    #region Rpc

    [ClientRpc]
    public void RpcSaveTotalTime(){
        FindObjectOfType<UIManager>().UpdateEnd();
        Debug.Log(playerTimer.TimeToText(playerTimer.t));
    }

    #endregion    
}