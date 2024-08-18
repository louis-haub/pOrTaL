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

    public void Pickup(Transform player, Transform target)
    {
        // attach Object to player camera
        transform.SetParent(player, true);
        rigidBody.constraints = RigidbodyConstraints.FreezePosition;
        transform.position = target.position;
        rigidBody.useGravity = false;
    }

    public void Drop()
    {
        Debug.Log("Drop");
        // detatch Object form camera
        transform.SetParent(null);
        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.useGravity = true;
    }

    public void SnapToPlayer(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    // Update is called once per frame
    void Update() {}
}
