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
            playerTimer.SaveTime(ID, lap);
            Debug.Log(playerTimer.TimeToText(playerTimer.lapTime[ID][lap-1]));
            lap++;
            
            //Debug.Log(playerTimer.lapTime.Count);
            //Debug.Log(playerTimer.lapTime[0].Count);
            
        }
        if(lap>maxLap)
        {   
            playerTimer.SaveTotalTime(ID);    
            Debug.Log(playerTimer.TimeToText(playerTimer.t));
            Debug.Log("Has terminado la carrera.");
            FindObjectOfType<PolePositionManager>().UpdateEnd(this.Name, this.playerTimer.TimeToText(playerTimer.lapTime[ID][lap-1]));
            RpcSwapCamera();
            for (int i = 0; i < playerTimer.lapTime[ID].Count; i++)
            {
                if(i < playerTimer.lapTime[ID].Count - 1)
                {
                    RpcAddLapsTime("\nLap " + (i+1)  + ":\n" + playerTimer.TimeToText(playerTimer.lapTime[ID][i]));
                }
                else
                {
                    RpcAddLapsTime("\n\nTotal time: "  + ":\n" + playerTimer.TimeToText(playerTimer.lapTime[ID][i]));
                }
            }


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
    public void RpcSwapCamera(){
        if(hasAuthority){
            
            FindObjectOfType<CameraController>().SwapToEndCamera();
        }
    }

    [ClientRpc]
    public void RpcAddLapsTime(string lap)
    {
        if(hasAuthority)
            FindObjectOfType<UIManager>().AppendLapTime(lap);
    }
    #endregion
    
}