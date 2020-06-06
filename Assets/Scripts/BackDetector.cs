using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackDetector : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "BackDetector") {

            Debug.Log("esta marcha atras"); //flag aqui para el script de UI
        
        }
    }
}
