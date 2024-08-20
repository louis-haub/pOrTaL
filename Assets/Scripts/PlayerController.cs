using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PortalableObject
{

    public Rigidbody rb;
    public GameObject player;
    public GameObject camHolder;
    public GameObject pickupLocation;
    public float maxPickupDistance;
    
    public float speed, sensitivity, density, maxForce, jumpForce;
    private Vector2 move, look;
    public Vector3 previousScale;
    private float lookRotation;
    public bool grounded;
    public Transform groundTest;
    
    public float scale;
    public float maxScale;
    public float minScale;
    public float initialScale = 1;
    public float pickupDistance;
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private PickupObject _focusedObject;
    private PickupObject _oldFocusedObject;
    private PickupObject _pickedUpObject;
    private SpringJoint joint;
    private AnimationHandler animation;
    public MusicController music;
    private float threshold = 0.1f;
    

    
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Jump();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started) Interact();
    }

    public void OnGetBigger(InputAction.CallbackContext context)
    {
        Bigger();
    }

    public void OnGetSmaller(InputAction.CallbackContext context)
    {
        Smaller();
    }

    private void FixedUpdate()
    {
      Move();

        LayerMask objectMask = LayerMask.GetMask("PickupObject");
        // Does the ray intersect any objects excluding the player layer
        if (_pickedUpObject == null)
        {
            bool wasObjectHit = Physics.Raycast(camHolder.transform.position,
                camHolder.transform.TransformDirection(Vector3.forward), out var objectHit, pickupDistance * scale,
                objectMask);
            bool wasPortalHit = Physics.Raycast(camHolder.transform.position,
                camHolder.transform.TransformDirection(Vector3.forward),
                out var portalHit, pickupDistance * scale, LayerMask.GetMask("PortalSurface"));
            bool noObjectBetweenPlayerAndPortal = (wasObjectHit && wasPortalHit)
                ? (portalHit.distance < objectHit.distance)
                : wasPortalHit && !wasObjectHit;
            
            // check if portal focused
            if (noObjectBetweenPlayerAndPortal)
            {
                var distance = portalHit.distance;
                inPortal = portalHit.collider.GetComponentInParent<Portal>();
                outPortal = inPortal.OtherPortal;
                var inTransform = inPortal.transform;
                var outTransform = outPortal.transform;
                Vector3 relativeDir =
                    inTransform.InverseTransformDirection(camHolder.transform.TransformDirection(Vector3.forward));
                Vector3 relativePos = inTransform.InverseTransformPoint(portalHit.point);
                relativePos = halfTurn * relativePos;
                relativeDir = halfTurn * relativeDir;
                var newOrigin = outTransform.TransformPoint(relativePos);
                var newDirection = outTransform.TransformDirection(relativeDir);

                // cast ray with remaining distance from other portal
                if (Physics.Raycast(newOrigin, newDirection, out objectHit, (pickupDistance * scale) - distance, objectMask))
                {
                    _focusedObject = objectHit.collider.GetComponent<PickupObject>();

                    if (_oldFocusedObject && _oldFocusedObject.GetInstanceID() != _focusedObject.GetInstanceID())
                    {
                        _oldFocusedObject.GetComponent<Renderer>().material = _oldFocusedObject.normalMat;
                    }

                    _oldFocusedObject = objectHit.collider.GetComponent<PickupObject>();

                    _focusedObject.GetComponent<Renderer>().material = _focusedObject.highlighted;
                }
                else
                {
                    if (_focusedObject)
                    {
                        _focusedObject.GetComponent<Renderer>().material = _focusedObject.normalMat;
                    }

                    _focusedObject = null;
                }
                //     // pickup position anpassen, wird an normale gesetzt
                //     _focusedObject = objectHit.collider.GetComponent<PickupObject>();
                // }
                // else
                // {
                //     _focusedObject = null;
                // }
            }
            else
            {
                if (wasObjectHit)
                {
                    _focusedObject = objectHit.collider.GetComponent<PickupObject>();

                    if (_oldFocusedObject && _oldFocusedObject.GetInstanceID() != _focusedObject.GetInstanceID())
                    {
                        _oldFocusedObject.GetComponent<Renderer>().material = _oldFocusedObject.normalMat;
                    }

                    _oldFocusedObject = objectHit.collider.GetComponent<PickupObject>();

                    _focusedObject.GetComponent<Renderer>().material = _focusedObject.highlighted;
                }
                else
                {
                    if (_focusedObject)
                    {
                        _focusedObject.GetComponent<Renderer>().material = _focusedObject.normalMat;
                    }

                    _focusedObject = null;
                }
            }
        }else if ((this.joint.transform.position - _pickedUpObject.transform.position).magnitude >
                  this.maxPickupDistance)
        {
            // Debug.Log((this.joint.transform.position - _pickedUpObject.transform.position).magnitude);
            // Debug.Log(this.joint.transform.position);
            // Debug.Log(this._pickedUpObject.transform.position);   
            dropPickedUpObject();
        }
    }

    public void dropPickedUpObject()
    {
        joint.connectedBody = null;
        _pickedUpObject.Drop();
        _pickedUpObject.GetComponent<Renderer>().material = _pickedUpObject.normalMat;
        _pickedUpObject = null;
        music.triggerBoxDrop();


    }
    void Jump()
    {
        Vector3 jumpForces = Vector3.zero;
        if (grounded)
        {
            jumpForces = Vector3.up * jumpForce *  Mathf.Pow(scale, 0.26f);
            animation.Jump();
            music.triggerJump();
        }
        rb.AddForce(jumpForces,ForceMode.VelocityChange);
        
    }

    void Look()
    {
        transform.Rotate(Vector3.up * look.x * sensitivity);

        //Look

        lookRotation += (-look.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y,
            camHolder.transform.eulerAngles.z);
    }

    void Move()
    {
        Debug.DrawRay(this.transform.position, this.GetComponent <Rigidbody> ().velocity, Color.black, .1f);

        if ( Vector3.Dot(this.transform.TransformDirection(new Vector3(move.x, 0, move.y)) * 5,this.transform.TransformDirection(new Vector3(move.x, 0, move.y))) - Vector3.Dot(this.GetComponent<Rigidbody>().velocity, this.transform.TransformDirection(new Vector3(move.x, 0, move.y))) > 0)
        {
            
            this.GetComponent<Rigidbody>().AddForce(this.transform.TransformDirection(new Vector3(move.x, 0, move.y)) * 15 * Mathf.Pow(scale, 0.3f));
            return;
        }

        if (grounded && this.GetComponent<Rigidbody>().velocity.magnitude > 0)
        {
                music.playingFootsteps=true;
                animation.Move(this.GetComponent<Rigidbody>().velocity * .5f);
        }
        else
        {
            music.playingFootsteps = false;
        }
       
        // Vector3 currentVelocity = rb.velocity;
        // Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        // targetVelocity *= speed;
        //
        // animation.Move(targetVelocity);
        // targetVelocity = transform.TransformDirection(targetVelocity);
        //
        // Vector3 velocityChange = (targetVelocity - currentVelocity);
        // velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);
        // //Limit force
        // Vector3.ClampMagnitude(velocityChange, maxForce);
        // rb.AddForce(velocityChange, ForceMode.VelocityChange);
        // //Check if movement is horizontal:
        // // Debug.Log(move);
        // if(Mathf.Abs(move.x)> threshold || Mathf.Abs(move.y) > threshold)
        // {
        //     music.playingFootsteps=true;
        // }
        // else
        // {
        //     music.playingFootsteps = false;
        // }
        
    }

    void Interact()
    {
        if (_pickedUpObject == null && _focusedObject != null)
        {
            _focusedObject.Pickup(camHolder.transform, pickupLocation.transform, joint.transform);
            _focusedObject.playerController = this;
            _pickedUpObject = _focusedObject;
            joint.connectedBody = _pickedUpObject.GetComponent<Rigidbody>();
            _focusedObject = null;
            music.triggerBoxLift();
        }
        else if (_pickedUpObject != null)
        {
            dropPickedUpObject();
        }
        else
        {
            // Debug.Log("None");
        }
    }

    void Smaller()
    {
        Scale(9f / 10f);
    }

    void Bigger()
    {
        Scale(10f / 9f);
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SetObjectMass(rb.mass);
        previousScale = transform.localScale;
        joint = GetComponentInChildren<SpringJoint>();
        animation = GetComponentInChildren<AnimationHandler>();
        Scale(initialScale);
    }

    void SetObjectMass(float mass)
    {
        Collider col = player.GetComponent<Collider>();
        if (rb != null && col != null)
        {
            float volume = CalculateColliderVolume(col);
            density = mass / volume;
            rb.SetDensity(density);
        }
    }

    float CalculateColliderVolume(Collider col)
    {
        if (col is BoxCollider box)
        {
            Vector3 size = box.size;
            return size.x * size.y * size.z;
        }

        if (col is SphereCollider sphere)
        {
            float radius = sphere.radius;
            return (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3);
        }

        if (col is CapsuleCollider capsule)
        {
            float radius = capsule.radius;
            float height = capsule.height;
            return Mathf.PI * Mathf.Pow(radius, 2) * ((4f / 3f) * radius + height);
        }

        // Handle other collider types as needed
        return 1f; // Default volume if unknown

    }

    void Update()
    {
        if (transform.localScale != previousScale)
        {
            // Update the mass based on the new scale
            //rb.SetDensity(density);
            Debug.Log("current Mass" + rb.mass);
            Debug.Log("current density" + density);

            // Update the previous scale
            previousScale = transform.localScale;
        }
        // if (_pickedUpObject != null)
        // {
        //     _pickedUpObject.SnapToPlayer(gameObject.transform.position + (gameObject.transform.forward * 2 + gameObject.transform.right + Vector3.up));
        // }
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        Look();
    }

    public void SetGrounded(bool state)
    {
        if (state && !grounded)
        {
            animation.Land();
            music.triggerLanding();
            music.triggerFootstep();
        }
        grounded = state;

    }

    public void OnCollisionEnter(Collision other)
    {
        print(other.gameObject.name);
    }

    public override void Warp()
    {
        Debug.Log(this.GetComponent<Rigidbody>().velocity);
        base.Warp();
        Debug.DrawRay(this.transform.position, this.GetComponent <Rigidbody> ().velocity, Color.cyan, 10f);
        var angle = Vector3.Angle(transform.up, Vector3.up);
        var normal = Vector3.Cross(transform.up, Vector3.up);
        transform.RotateAround(transform.position, normal, angle);
        Debug.DrawRay(this.transform.position, this.GetComponent <Rigidbody> ().velocity, Color.cyan, 10f);
    
        //this.GetComponent<Rigidbody>().velocity = this.transform.InverseTransformDirection(global_velocity);
        var scaling = inPortal.Size / outPortal.Size;
        Scale(scaling);
        Debug.DrawRay(this.transform.position, this.GetComponent <Rigidbody> ().velocity, Color.green, 10f);
    
        Collider col = player.GetComponent<Collider>();
        
    }

    private void Scale(float scale)
    {
        if (this.scale * scale >= minScale && this.scale * scale <= maxScale)
        {
            var oldGroundPos = groundTest.localPosition;
            transform.localScale *= scale;
            this.scale *= scale;

            groundTest.localScale *= 1f / scale;
            groundTest.localPosition = oldGroundPos;
        }
        else
        {
            if (scale < 1)
            {
                // float actualScale = minScale / this.scale;
                // Scale(actualScale);
                SetScaleToValue(minScale);
            }
            else if (scale > 1)
            {
                SetScaleToValue(maxScale);
            }
            Debug.Log("Out of Bounds");
        }
    }

    private void SetScaleToValue(float scale)
    {
        float actualScale = scale / this.scale;
        var oldGroundPos = groundTest.localPosition;
        // scale player
        transform.localScale = new Vector3(scale, scale, scale);
        // this.scale is still old
        groundTest.localScale *= 1f / actualScale;
        groundTest.localPosition = oldGroundPos;
        
        this.scale = scale;
    }

    protected override bool UseClone()
    {
        return false;
    }
}
