using System.Collections;
using System.Collections.Generic;
using Interactable;
using Managers;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnitMoveState : EnemyUnitBaseState
{
    private Vector3 posToMoveTo;
    public override void EnterState(Units entity)
    {
        if (!entity.hasBeenConstructed) return;
        posToMoveTo = new Vector3(Random.Range(0,MapManager.Instance.ReturnSizeOfMap().x), 0,
            Random.Range(0,MapManager.Instance.ReturnSizeOfMap().y));
        entity.agent.SetDestination(posToMoveTo);
    }

    public override void Update(Units entity)
    {
        if (!entity.hasBeenConstructed) return;
        if(entity.agent.hasPath) return;
        posToMoveTo = new Vector3(Random.Range(-MapManager.Instance.ReturnSizeOfMap().x,MapManager.Instance.ReturnSizeOfMap().x), 0,
            Random.Range(-MapManager.Instance.ReturnSizeOfMap().y,MapManager.Instance.ReturnSizeOfMap().y));
        entity.agent.CalculatePath(posToMoveTo, entity.path);
        if (entity.path.status == NavMeshPathStatus.PathPartial)
        {
            posToMoveTo = new Vector3(Random.Range(-MapManager.Instance.ReturnSizeOfMap().x, MapManager.Instance.ReturnSizeOfMap().x), 0,
                Random.Range(-MapManager.Instance.ReturnSizeOfMap().y,MapManager.Instance.ReturnSizeOfMap().y));
        }
        else
        {
            entity.agent.SetDestination(posToMoveTo);   
        }

    }
}
