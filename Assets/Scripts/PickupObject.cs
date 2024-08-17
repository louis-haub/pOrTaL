using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rigidBody;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("PickupObject");
    }

    public void Pickup()
    {
        rigidBody.useGravity = false;
    }

    public void Drop()
    {
        rigidBody.useGravity = true;
    }

    public void SnapToPlayer(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    // Update is called once per frame
    void Update() {}
}
