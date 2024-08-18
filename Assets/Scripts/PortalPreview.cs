using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PortalPreview : MonoBehaviour
{
    public Material intersectingMaterial;
    public Material validMaterial;

    public Transform[] testPoints = new Transform[4];

    /**
     * Set by portal when placed
     */
    public Collider wallCollider;
    
    private bool _valid = true;
    public bool Valid => _valid && IsFullyCovered();

    private MeshRenderer renderer;
    private Collider ownCollider;
    public LayerMask placementMask;
    [FormerlySerializedAs("testPosition")] public Transform test;

    public void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        ownCollider = GetComponent<Collider>();
    }

    public void Init()
    {
        _valid = true;
        gameObject.SetActive(true);
    }

    private void SetMaterial()
    {
        renderer.material = _valid && IsFullyCovered() ? validMaterial : intersectingMaterial;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        SetMaterial();
    }

    private bool IsFullyCovered()
    {
        return testPoints.All(p => wallCollider.bounds.Contains(p.position));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == wallCollider) return;
        _valid = false;
        SetMaterial();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == wallCollider) return;
        _valid = true;
        SetMaterial();
    }
}
