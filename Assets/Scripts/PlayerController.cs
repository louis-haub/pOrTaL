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
    public float speed, sensitivity, density, maxForce, jumpForce;
    private Vector2 move, look;
    public Vector3 previousScale;
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
        if (_pickedUpObject == null && _focusedObject != null)
        {
            _focusedObject.Pickup(camHolder.transform, pickupLocation.transform);
            _pickedUpObject = _focusedObject;
            _focusedObject = null;
        }
        else if (_pickedUpObject != null)
        {
            _pickedUpObject.Drop();
            _pickedUpObject = null;
        }
        else
        {
            // Debug.Log("None");
        }
    }

    void Smaller()
    {
        transform.localScale *= 9f / 10f;
    }

    void Bigger()
    {
        transform.localScale *= 10f / 9f;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SetObjectMass(rb.mass);
        previousScale = transform.localScale;

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
            rb.SetDensity(density);
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
        grounded = state;
    }
    
    public override void Warp()
    {
        base.Warp();
        var angle = Vector3.Angle(transform.up, Vector3.up);
        var normal = Vector3.Cross(transform.up, Vector3.up);
        transform.RotateAround(transform.position, normal, angle);
        var scaling = inPortal.Size / outPortal.Size;
        transform.localScale *= scaling;
    }
}
