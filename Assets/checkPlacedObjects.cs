using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class checkPlacedObjects : MonoBehaviour
{
    private List<GameObject> objectsOn = new List<GameObject>();
    public List<GameObject> neededObjects = new List<GameObject>();
    public List<GameObject> objectsToSpawn = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        objectsOn.Add(other.gameObject);
        CheckAllObjectsOn();
    }
    
    void OnTriggerExit(Collider other)
    {
        if (objectsOn.Contains(other.gameObject))
        {
            objectsOn.Remove(other.gameObject);
        }
    }
    private void CheckAllObjectsOn()
    {
        bool allElementsExist = neededObjects.All(item => objectsOn.Contains(item));
        
        if (allElementsExist)
        {
            foreach (GameObject obj in objectsToSpawn)
            {
                Instantiate(obj, transform.position, Quaternion.identity);
            }
        }
    }
}
