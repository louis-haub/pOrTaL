using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTest : MonoBehaviour
{
    private float _startTime;
    private Vector3 _startPos;
    
    void Start()
    {
        _startTime = Time.time;
        _startPos = transform.position;
    }

    void Update()
    {
        if (Time.time - _startTime > 1)
        {
            _startTime = Time.time;
            Debug.Log((transform.position - _startPos).magnitude);
            _startPos = transform.position;
        }
    }
}
