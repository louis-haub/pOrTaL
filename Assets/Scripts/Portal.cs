using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [field: SerializeField]
    public Portal OtherPortal { get; private set; }

    [SerializeField]
    private Renderer outlineRenderer;

    [field: SerializeField]
    public Color PortalColour { get; private set; }

    [SerializeField]
    private LayerMask placementMask;
    
    public Transform floor;
    
    [field: SerializeField]
    public float Size { get; private set; }

    private List<PortalableObject> portalObjects = new List<PortalableObject>();
    public bool IsPlaced { get; private set; } = false;
    public bool IsPreview { get; private set; } = false;
    
    private Collider wallCollider;

    // Components.
    public Renderer Renderer;
    private new BoxCollider collider;

    private PortalPreview preview;

    public GameObject outline;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        preview = GetComponentInChildren<PortalPreview>();
    }

    private void Start()
    {
        outlineRenderer.material.SetColor("_OutlineColour", PortalColour);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        Renderer.enabled = IsPlaced && OtherPortal.IsPlaced;

        for (int i = 0; i < portalObjects.Count; ++i)
        {
            Vector3 objPos = transform.InverseTransformPoint(portalObjects[i].transform.position);

            if (objPos.z > 0.0f)
            {
                portalObjects[i].Warp();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        if (obj != null)
        {
            portalObjects.Add(obj);
            obj.SetIsInPortal(this, OtherPortal, wallCollider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();

        if(portalObjects.Contains(obj))
        {
            portalObjects.Remove(obj);
            obj.ExitPortal(wallCollider);
        }
    }

    public void PreviewPortal(Collider wallCollider, Vector3 pos, Quaternion rot, float scale)
    {
        // set this to be a preview for now. in update we will check if we can change it to actually place it again
        IsPlaced = false;
        preview.wallCollider = wallCollider;
        preview.placementMask = placementMask;
        preview.test = transform;
        if (!IsPreview)
        {
            preview.Init();
        }
        outline.SetActive(false);
        
        this.wallCollider = wallCollider;
        transform.position = pos;
        transform.rotation = rot;
        // transform.position -= transform.forward * 0.001f;
        Size = scale;
        transform.position = new Vector3(transform.position.x, transform.position.y * Size, transform.position.z);
        transform.localScale = new Vector3(Size, Size, 1);

        gameObject.SetActive(true);
        IsPreview = true;
    }

    public bool TryPlacingPortal()
    {
        IsPreview = false;
        if (preview.Valid)
        {
            IsPlaced = true;
            preview.Hide();
            outline.SetActive(true);
            transform.position -= transform.forward * 0.001f;
            return true;
        }
        else
        {
            preview.Hide();
            gameObject.SetActive(false);
            return false;
        }
    }

    public void RemovePortal()
    {
        gameObject.SetActive(false);
        IsPlaced = false;
    }
}