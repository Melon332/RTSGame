using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Units
{
    public override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = minRangeToAttack - 3;
    }

    public override void OnClicked()
    {
        Debug.Log("This is an enemy, you cannot select it");
    }
}
