using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Units
{
    protected override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = minRangeToAttack - 3;
        canBeAttacked = true;
    }

    public override void OnClicked()
    {
        Debug.Log("This is an enemy, you cannot select it");
    }

    public override void OnHit(int damage)
    {
        damage += 50;
        base.OnHit(damage);
    }
}
