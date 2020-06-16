using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ParentCheck : NetworkBehaviour
{
    [SerializeField]
    private List<GameObject> Checkpoints = new List<GameObject>();
    int n = 0;

    public bool clasificacion;
    bool first = true;

    private void Awake()
    {
        clasificacion = FindObjectOfType<PoleNetworkManager>().clasif;
        
    }


    public void CheckpointTriggered(GameObject player)
    {
        //recibir la información del servidor()

        //Debug.Log("Checkpoint " + n);
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
            if (clasificacion)
            {         
                
                //Debug.Log("Id clasif: " + player.GetComponent<PlayerInfo>().ID);           
                player.GetComponent<PlayerController>().CmdSavePos(player.GetComponent<PlayerInfo>().ID);
                n = 0;
                return;
            }
            player.GetComponent<PlayerInfo>().CmdIncreaseLap();
            n = 0;
        }
        else if(n == 1 && !clasificacion && first){            
            FindObjectOfType<UIManager>().textPosition.transform.parent.gameObject.SetActive(true);
            first = false;        
        }
    }

    public void RestartCheckpoints ()
    {
        Checkpoints[n%11].SetActive(false);
        n = 0;
        Checkpoints[0].SetActive(true);
    }
}
