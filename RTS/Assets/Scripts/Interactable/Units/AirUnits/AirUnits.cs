using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class AirUnits : Units
{
    // Start is called before the first frame update
    public override void Start()
    {
        Debug.Log("I am an air unit :D");
    }

    protected override void MoveToTarget(RaycastHit target)
    {
        transform.Translate(target.point.x,transform.position.y,target.point.z);
    }
}
