using System;
using UnityEngine;
using System.Collections.Generic;

public class WeightCalculator : MonoBehaviour
{
    private List<Rigidbody> objectsOnScale = new List<Rigidbody>();
    public GameObject objectToDisappear;  // Reference to the object that should disappear;
    public float weightThreshold;
    private float totalWeight;
    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        Debug.Log("test");

        if (rb != null)
        {
            Debug.Log("Not Null");
            objectsOnScale.Add(rb);
            UpdateWeight();
        }
        else if (other.name == "GroundCheck")
        {
            rb = other.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                objectsOnScale.Add(rb);
                UpdateWeight();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody rb;
        if (other.name == "GroundCheck")
        {
            rb = other.GetComponentInParent<Rigidbody>();
        }
        else
        {
            rb = other.GetComponent<Rigidbody>();

        }

        if (rb != null && objectsOnScale.Contains(rb))
        {
            objectsOnScale.Remove(rb);
            UpdateWeight();
        }

    }

    private void UpdateWeight()
    {
        totalWeight = 0f;
        foreach (Rigidbody rb in objectsOnScale)
        {
            totalWeight += rb.mass;
        }
        CheckWeightThreshold();  // Check if the weight exceeds the threshold
    }
    
    private void CheckWeightThreshold()
    {
        if (totalWeight >= weightThreshold)
        {
            if (objectToDisappear != null)
            {
                objectToDisappear.SetActive(false);  // Make the object disappear
                Debug.Log("Object has disappeared because the weight threshold was exceeded.");
            }
        }
    }

    public float GetTotalWeight()
    {
        return totalWeight;
    }
    
}