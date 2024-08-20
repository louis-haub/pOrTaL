using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoureGrounded : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (!player.grounded)
            {
                player.rb.velocity = Vector3.zero;
            }
        }
    }
}
