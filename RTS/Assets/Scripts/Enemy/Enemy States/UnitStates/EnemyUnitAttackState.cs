using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class EnemyUnitAttackState : EnemyUnitBaseState
{
    public override void EnterState(Units entity)
    {
        entity.AttackAndMove = entity.StartCoroutine(entity.AggroAttack());
    }

    public override void Update(Units entity)
    {

    }
}
