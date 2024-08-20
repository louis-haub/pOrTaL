using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var respawnable = other.gameObject.GetComponent<RespawnableObject>();
        if (respawnable != null)
        {
            respawnable.Respawn();
        }
    }
}
