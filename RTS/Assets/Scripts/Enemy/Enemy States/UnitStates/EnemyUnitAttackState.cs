using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class EnemyUnitAttackState : EnemyUnitBaseState
{
    //Starts a coroutine to attack a unit
    public override void EnterState(Units unit)
    {
        //Start attacking an unit
        if (!unit.hasBeenConstructed) return;
        unit.AttackAndMove = unit.StartCoroutine(unit.AggroAttack());
    }

    public override void Update(Units unit)
    {

    }
}
