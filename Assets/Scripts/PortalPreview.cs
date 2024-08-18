using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PortalPreview : MonoBehaviour
{
    public Material intersectingMaterial;
    public Material validMaterial;

    /**
     * Set by portal when placed
     */
    public Collider wallCollider;
    
    public bool valid = true;

    private MeshRenderer renderer;

    public void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    public void Init()
    {
        valid = true;
        renderer.material = validMaterial;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == wallCollider) return;
        valid = false;
        renderer.material = intersectingMaterial;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == wallCollider) return;
        valid = true;
        renderer.material = validMaterial;
    }
}
