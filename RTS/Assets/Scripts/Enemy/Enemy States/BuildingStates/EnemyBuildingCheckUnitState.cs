using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class EnemyBuildingCheckUnitState : EnemyBuildingBaseState
{
    int tanksOnMap;
    public override void EnterState(Factory factory)
    {
        tanksOnMap += factory.unitQueue.Count;
        //Check if there is less than 4 units on the map. If there is, switch state.
        Debug.Log("I am now checking how many tanks there are: ");
        foreach (var enemyTanks in EnemyManager.Instance.enemiesOnMap)
        {
            if (!enemyTanks.GetComponent<Tank>()) continue;
            tanksOnMap++;
            Debug.Log(tanksOnMap);
        }
        if (tanksOnMap <= 4)
        {
            factory.TransisitonToState(factory.constructUnits);
        }

        if (tanksOnMap <= 0)
        {
            factory.TransisitonToState(factory.constructUnits);
        }
        tanksOnMap = 0;
    }
}
