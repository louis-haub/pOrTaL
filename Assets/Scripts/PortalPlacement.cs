using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortalPlacement : MonoBehaviour
{
    [SerializeField]
    private PortalReference portals;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Crosshair crosshair;

    public float scale = 1;
    public float minScale;
    public float maxScale;

    public Transform banana;

    public Camera playerCamera;

    public int currentPreview = -1;

    private void Update()
    {
        if (currentPreview != -1)
        {
            PreviewPortal(currentPreview, transform.position, playerCamera.transform.forward, 250.0f);
        }
    }

    public void SetScale(InputAction.CallbackContext ctx)
    {
        var scaleChange = ctx.ReadValue<Vector2>();
        scale += scaleChange.y / 3000f;
        scale = Mathf.Clamp(scale, minScale, maxScale);
        banana.localScale = Vector3.one * scale;
    }

    public void PreviewPortal1(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            PreviewPortal(0, playerCamera.transform.position, playerCamera.transform.forward, 250.0f);
        }
    }
    
    public void PreviewPortal2(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            PreviewPortal(1, playerCamera.transform.position, playerCamera.transform.forward, 250.0f);
        }
    }

    private void PreviewPortal(int portalID, Vector3 pos, Vector3 dir, float distance)
    {
        Debug.DrawRay(pos, dir);
        currentPreview = portalID;
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(ray, out var hit, distance, layerMask);

        if(hit.collider != null)
        {
            // Orient the portal according to camera look direction and surface direction.
            var portalRight = transform.right;
            
            if(Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
            {
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            }
            else
            {
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;
            }

            var portalForward = -hit.normal;
            var portalUp = -Vector3.Cross(portalRight, portalForward);

            var portalRotation = Quaternion.LookRotation(portalForward, portalUp);
            
            // Attempt to place the portal.
            portals.Portals[portalID].PreviewPortal(hit.collider, hit.point, portalRotation, scale);

        }
    }

    public void PlacePortal1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            PlacePortal(0);
        }
    }
    
    public void PlacePortal2(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            PlacePortal(1);
        }
    }
    
    private void PlacePortal(int portalId)
    {
        currentPreview = -1;
        var wasPlaced = portals.Portals[portalId].TryPlacingPortal();
        if(wasPlaced)
        {
            crosshair.SetPortalPlaced(portalId, true);
        }
    }
}
