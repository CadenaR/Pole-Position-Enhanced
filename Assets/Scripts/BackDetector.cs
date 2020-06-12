using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackDetector : MonoBehaviour
{
    public GameObject marchaAtras;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "BackDetector")
        {
            if (other.GetComponentInParent<PlayerController>().hasAuthority) 
            {

                marchaAtras.SetActive(true);

            }
        }
        
            
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "BackDetector")
        {
            if (other.GetComponentInParent<PlayerController>().hasAuthority)
            {

                marchaAtras.SetActive(false);
                
            }
        }
    }
}
