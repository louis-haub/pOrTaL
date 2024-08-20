using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RespawnableObject : MonoBehaviour
{
    private readonly float _respawnHeightDown = -100;
    private readonly float _respawnHeightUp = 500;
    private Vector3 _startPosition;
    // Start is called before the first frame update
    void Start()
    {
        this._startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < _respawnHeightDown)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        this.transform.position = _startPosition;
        var rb = this.GetComponent<Rigidbody>();
        if (rb)
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }
}
