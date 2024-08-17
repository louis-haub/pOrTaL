using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public PlayerController playerController;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
        {
            return;
        }
        Debug.Log("Enter Ground");
        playerController.SetGrounded(true);
    }
    
    void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject == playerController.gameObject)
        {
            return;
        }
        Debug.Log("Exit Ground");
        playerController.SetGrounded(false);
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
        {
            return;
        }
        Debug.Log("Stay Ground");

        playerController.SetGrounded(true);
    }
}
