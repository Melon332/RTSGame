using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class AirUnits : Units
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Invoke("FlyToAir",2f);
    }

    protected override void MoveToTarget(RaycastHit target)
    {
        base.MoveToTarget(target);
    }

    private void FlyToAir()
    {
        transform.position = new Vector3(transform.position.x, 5f, transform.position.z); 
    }
}
