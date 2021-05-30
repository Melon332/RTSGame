using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class GroundUnits : Units
{
    protected override void Start()
    {
        base.Start();
        Debug.Log("We are ground units. We do ground stuff!");
    }
}
