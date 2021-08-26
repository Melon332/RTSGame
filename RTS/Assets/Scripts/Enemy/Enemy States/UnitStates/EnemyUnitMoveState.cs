using System.Collections;
using System.Collections.Generic;
using Interactable;
using Managers;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnitMoveState : EnemyUnitBaseState
{
    private Vector3 posToMoveTo;
    public override void EnterState(Units unit)
    {
        //Search for a position to go to. Depends on the map.
        if (!unit.hasBeenConstructed) return;
        posToMoveTo = new Vector3(Random.Range(0,MapManager.Instance.ReturnSizeOfMap().x), 0,
            Random.Range(0,MapManager.Instance.ReturnSizeOfMap().y));
        unit.agent.SetDestination(posToMoveTo);
    }

    public override void Update(Units unit)
    {
        //Search for a position to go to. Depends on the map.
        if (!unit.hasBeenConstructed) return;
        if(unit.agent.hasPath) return;
        posToMoveTo = new Vector3(Random.Range(-MapManager.Instance.ReturnSizeOfMap().x,MapManager.Instance.ReturnSizeOfMap().x), 0,
            Random.Range(-MapManager.Instance.ReturnSizeOfMap().y,MapManager.Instance.ReturnSizeOfMap().y));
        unit.agent.CalculatePath(posToMoveTo, unit.path);
        if (unit.path.status == NavMeshPathStatus.PathPartial)
        {
            posToMoveTo = new Vector3(Random.Range(-MapManager.Instance.ReturnSizeOfMap().x, MapManager.Instance.ReturnSizeOfMap().x), 0,
                Random.Range(-MapManager.Instance.ReturnSizeOfMap().y,MapManager.Instance.ReturnSizeOfMap().y));
        }
        else
        {
            unit.agent.SetDestination(posToMoveTo);   
        }

    }
}
