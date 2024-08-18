using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rigidBody;
    private Transform parentTransform;
    private Transform targetTransform;

    private Portal inPortal;
    private Portal outPortal;
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("PickupObject");
    }

    public void Pickup(Transform player, Transform target)
    {
        // attach Object to player camera
        transform.SetParent(player, true);
        parentTransform = player;
        targetTransform = target;
        rigidBody.constraints = RigidbodyConstraints.FreezePosition;
        transform.position = target.position;
        rigidBody.useGravity = false;
    }

    public void Drop()
    {
        // detatch Object form camera
        parentTransform = null;
        targetTransform = null;
        transform.SetParent(null);
        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.useGravity = true;
    }

    public void SnapToPlayer(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // if object is currently picked up
        if (parentTransform != null && targetTransform != null)
        {
            Vector3 direction = targetTransform.position - parentTransform.position;
            if (Physics.Raycast(parentTransform.position, direction, out var hit, direction.magnitude,
                    LayerMask.GetMask("Portal")))
            {
                // get collider of hit
                inPortal = hit.collider.GetComponent<Portal>();
                outPortal = inPortal.OtherPortal;
                var inTransform = inPortal.transform;
                var outTransform = outPortal.transform;
                Vector3 relativePos = inTransform.InverseTransformPoint(targetTransform.position);
                if (relativePos.z > 0)
                {
                    relativePos = halfTurn * relativePos;

                    var newTarget = outTransform.TransformPoint(relativePos);
                    transform.position = newTarget;
                }
                else
                {
                    transform.position = targetTransform.position;
                }
            }
            else
            {
                transform.position = targetTransform.position;
            }
        }
    }
}