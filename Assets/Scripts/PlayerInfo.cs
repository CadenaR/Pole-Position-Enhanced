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

    public int CurrentPosition { get; set; }

    public int CurrentLap { get; set; }

    public override string ToString()
    {
        return Name;
    }

    #region Command

    [Command]
    public void CmdIncreaseLap()
    {
        if(lap <= maxLap){
            lap++;
        }else{
            //acabar carrera;
        }
    }

    [Command]
    public void CmdSetCheckpoint(int c)
    {
        checkpoint = c;
    }


    #endregion
}