using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
}
