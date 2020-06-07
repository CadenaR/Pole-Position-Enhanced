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
    public int checkpoint = 0;

    public int ID { get; set; }

    public int CurrentPosition { get; set; }

    public int CurrentLap { get; set; }

    public override string ToString()
    {
        return Name;
    }

    [Command]
    public void CmdIncreaseLap()
    {
        lap++;
    }

    [Command]
    public void CmdSetCheckpoint(int c)
    {
        checkpoint = c;
    }
}