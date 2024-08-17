using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalReference : MonoBehaviour
{
    public Portal portal1;
    public Portal portal2;
    
    public List<Portal> Portals => new() { portal1, portal2 };
}
