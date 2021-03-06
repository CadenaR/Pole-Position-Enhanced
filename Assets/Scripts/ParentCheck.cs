﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ParentCheck : NetworkBehaviour
{
    [SerializeField]
    private List<GameObject> Checkpoints = new List<GameObject>();
    int n = 0;

    public void CheckpointTriggered(GameObject player)
    {
        //recibir la información del servidor()

        Debug.Log("Checkpoint " + n);
        n++;
        player.GetComponent<PlayerInfo>().CmdSetCheckpoint(n);        
        CheckLap(player);        
        Checkpoints[n%11].SetActive(true);        
    }

    private void CheckLap(GameObject player)
    {
        if (n == 12)
        {
            Debug.Log("He dado una vuelta");
            player.GetComponent<PlayerInfo>().CmdIncreaseLap();
            n = 0;
        }
    }
}
