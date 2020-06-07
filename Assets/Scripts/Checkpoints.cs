using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Checkpoints : NetworkBehaviour
{
    private ParentCheck parent;

    private void Start()
    {
        parent = FindObjectOfType<ParentCheck>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PlayerController>().hasAuthority)
            {
                this.gameObject.SetActive(false);
                parent.CheckpointTriggered(other.gameObject);
            }
        }
    }
}
