using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class EnemyBuildingStateConstructUnits : EnemyBuildingBaseState
{
    public override void EnterState(Factory entity)
    {
        while (entity.GetComponent<Factory>().unitQueue.Count < 5)
        {
            UnitManager.Instance.EnemyBuildUnits("Tank", entity.GetComponent<Factory>());
        }
    }
}
