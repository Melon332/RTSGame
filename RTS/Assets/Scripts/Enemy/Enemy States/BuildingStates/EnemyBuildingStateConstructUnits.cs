using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class EnemyBuildingStateConstructUnits : EnemyBuildingBaseState
{
    public override void EnterState(Factory factory)
    {
        //Construct 5 units
        while (factory.GetComponent<Factory>().unitQueue.Count < 5)
        {
            UnitManager.Instance.EnemyBuildUnits("Tank", factory.GetComponent<Factory>());
        }
        
    }
}
