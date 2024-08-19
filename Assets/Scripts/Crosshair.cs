using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private PortalReference portals;

    public Sprite nonEnabled;
    public Sprite blueEnabled;
    public Sprite yellowEnabled;
    public Sprite bothEnabled;

    public Image img;

    private void Start()
    {
        UpdatePortalColors();
    }

    public void UpdatePortalColors()
    {
        Sprite sprite = nonEnabled;
        if (portals.Portals[0].IsPlaced && portals.Portals[1].IsPlaced)
        {
            sprite = bothEnabled;
        }
        else if (portals.Portals[0].IsPlaced)
        {
            sprite = yellowEnabled;
        }
        else if (portals.Portals[1].IsPlaced)
        {
            sprite = blueEnabled;
        }

        img.sprite = sprite;
    }
}
