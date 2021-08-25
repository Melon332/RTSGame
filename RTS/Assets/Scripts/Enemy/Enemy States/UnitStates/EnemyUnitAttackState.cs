using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class EnemyUnitAttackState : EnemyUnitBaseState
{
    public override void EnterState(Units entity)
    {
        Debug.Log("Hello");
        entity.AttackAndMove = entity.StartCoroutine(entity.EnemyAttack());
    }

    public override void Update(Units entity)
    {
        if(entity.unitToAttack == null)
        {
            entity.TransisitonToState(entity.moveState);
        } ;
    }
}
