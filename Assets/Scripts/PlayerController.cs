using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PortalableObject
{

    public Rigidbody rb;

    public GameObject camHolder;
    public GameObject pickupLocation;
    public float speed, sensitivity, maxForce,jumpForce;
    private Vector2 move, look;
    private float lookRotation;
    public bool grounded;

    public float scale;
    // TODO: remove "angesehendes Objekt"
    private PickupObject _focusedObject;
    private PickupObject _pickedUpObject;

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
        Jump();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Interact();
    }

    private void FixedUpdate()
    {
        Move();
        
        // add ray for picking up objects
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("PickupObject");
        // Does the ray intersect any objects excluding the player layer
        if (_pickedUpObject == null)
        {
            if (Physics.Raycast(camHolder.transform.position, camHolder.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, mask))
            {
                _focusedObject = hit.collider.GetComponent<PickupObject>();
            }
            else
            {
                _focusedObject = null;
            }
        }
    }

    void Jump()
    {
        Vector3 jumpForces = Vector3.zero;
        if (grounded)
        {
            jumpForces = Vector3.up * jumpForce;
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
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity *= speed;

        targetVelocity = transform.TransformDirection(targetVelocity);

        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);
        //Limit force
        Vector3.ClampMagnitude(velocityChange, maxForce);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Interact()
    {
        Debug.Log($"Picked up Object: ," +
                  $" focused Object: ");
        Debug.Log(_pickedUpObject == null);
        Debug.Log(_focusedObject != null);
        if (_pickedUpObject == null && _focusedObject != null)
        {
            Debug.Log("Pickup");
            _focusedObject.Pickup(camHolder.transform, pickupLocation.transform);
            _pickedUpObject = _focusedObject;
        }
        else if (_pickedUpObject != null)
        {
            Debug.Log("Drop");
            _pickedUpObject.Drop();
            _pickedUpObject = null;
        }
        else
        {
            // Debug.Log("None");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
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
        grounded = state;
    }
    
    public override void Warp()
    {
        base.Warp();
        // cameraMove.ResetTargetRotation();
    }
}
