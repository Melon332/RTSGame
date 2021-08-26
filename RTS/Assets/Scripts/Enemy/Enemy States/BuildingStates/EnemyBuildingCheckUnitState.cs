using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class EnemyBuildingCheckUnitState : EnemyBuildingBaseState
{
    public override void EnterState(Factory entity)
    {
        Debug.Log("I am now checking how many tanks there are: ");
        foreach (var amountOfTanks in EnemyManager.Instance.enemiesOnMap)
        {
            var tanksOnMap = 0;
            if (!amountOfTanks.GetComponent<Tank>()) continue;
            tanksOnMap++;
            if (tanksOnMap <= 4)
            {
                   entity.TransisitonToState(entity.constructUnits);
            }
            Debug.Log(tanksOnMap);
        }
    }
}
