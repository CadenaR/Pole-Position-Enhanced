using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColliderController : MonoBehaviour
{


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<WheelCollider>() != null || collision.gameObject.tag == "Player")
        {
            UnityEngine.Debug.Log("Me estoy chocando rueda");
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), this.GetComponent<Collider>());
        }
    }
}
