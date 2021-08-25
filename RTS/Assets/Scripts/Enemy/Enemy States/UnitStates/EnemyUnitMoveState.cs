using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class EnemyUnitMoveState : EnemyUnitBaseState
{
    private Vector3 posToMoveTo;
    public override void EnterState(Units entity)
    {
        posToMoveTo = new Vector3(Random.Range(entity.transform.position.x, 100), 0,
            Random.Range(entity.transform.position.z, 100));
        entity.agent.SetDestination(posToMoveTo);
    }

    public override void Update(Units entity)
    {
        if(entity.agent.hasPath) return;
        posToMoveTo = new Vector3(Random.Range(entity.transform.position.x, 100), 0,
                Random.Range(entity.transform.position.z, 100));
            entity.agent.SetDestination(posToMoveTo);
        
    }
}
