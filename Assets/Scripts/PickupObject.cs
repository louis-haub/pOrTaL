using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : PortalableObject
{
    // Start is called before the first frame update
    public Rigidbody rigidBody;
    public Material normalMat;
    public Material highlighted;

    private Transform parentTransform;
    private Transform targetTransform;
    private Transform joint;
    private Vector3 portedJointPosition;
    private bool isWarped = false;

    private Portal inPortalPickup;
    private Portal outPortalPickup;
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    
    
    public PlayerController playerController;

    void Start()
    {
        this.normalMat = this.GetComponent<Renderer>().material;
        gameObject.layer = LayerMask.NameToLayer("PickupObject");
    }

    public void Pickup(Transform player, Transform target, Transform joint)
    {
        this.joint = joint;
        parentTransform = player;
        targetTransform = target;
        rigidBody.useGravity = false;
        rigidBody.drag = 10;
        
        Vector3 direction = targetTransform.position - parentTransform.position;
        bool wasObjectHit = Physics.Raycast(parentTransform.position, direction, out var objectHit, 100,
            LayerMask.GetMask("PickupObject"));
        bool wasPortalHit = Physics.Raycast(parentTransform.position, direction, out var portalHit, 100,
            LayerMask.GetMask("PortalSurface"));
        
        bool normal = (wasObjectHit && wasPortalHit)
            ? (portalHit.distance < objectHit.distance)
            : wasPortalHit && !wasObjectHit;
        
        Vector3 newJoint = normal ? getWarpedJointPosition(): targetTransform.position;
        // if object picked up through portal
        if (newJoint != joint.position)
        {
            this.joint.position = newJoint;
            isWarped = true;
        }
    }

    public void Drop()
    {
        joint.position = targetTransform.position;
        isWarped = false;
        parentTransform = null;
        targetTransform = null;
        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.useGravity = true;
        rigidBody.drag = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // // if object is currently picked up
        if (parentTransform != null && targetTransform != null && isWarped)
        {
            joint.position = getWarpedJointPosition();
            // was just dropped by moving out of portal (now formally drop)
            if (!isWarped) playerController.dropPickedUpObject();
        }
    }

    private Vector3 getWarpedJointPosition()
    {
        // vector from player to pickup loaction (where object should be)
        Vector3 direction = targetTransform.position - parentTransform.position;
        bool wasPortalHit = Physics.Raycast(parentTransform.position, direction, out var portalHit, 100,
            LayerMask.GetMask("PortalSurface"));

        if (wasPortalHit)
        {
            // get collider of hit
            inPortalPickup = portalHit.collider.GetComponentInParent<Portal>();
            outPortalPickup = inPortalPickup.OtherPortal;
            var inTransform = inPortalPickup.transform;
            var outTransform = outPortalPickup.transform;
            Vector3 relativePos = inTransform.InverseTransformPoint(targetTransform.position);

            relativePos = halfTurn * relativePos;

            var newTarget = outTransform.TransformPoint(relativePos);
            return newTarget;
        }
        // no hit
        else
        {
            isWarped = false;
            return targetTransform.position;
        }
    }

    public override void Warp()
    {
        base.Warp();
        var scaling = base.inPortal.Size / base.outPortal.Size;
        transform.localScale *= scaling;
        //picked up
        if (parentTransform != null && targetTransform != null)
        {
            isWarped = !isWarped;
            joint.position = isWarped ? getWarpedJointPosition() : targetTransform.position;
        }
    }
}