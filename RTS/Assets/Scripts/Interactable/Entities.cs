using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
using UnityEngine.AI;
using Player;

public abstract class Entities : MonoBehaviour, IInteractable,ISubscriber,IDestructable
{
    #region Unit Variables
    public int hitPoints;
    public float minRangeToAttack;
    public float attackTimer;
    public string nameOfUnit;
    public bool canBeAttacked;
    #endregion

    [HideInInspector] public NavMeshAgent agent;

    protected virtual void Start()
    {
        PlayerSelectedUnits.SelectableUnits.Add(gameObject);
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void Subscribe(CharacterInput publisher)
    {
        publisher.hasClicked += null;
    }

    public virtual void UnSubscribe(CharacterInput publisher)
    {
        publisher.hasClicked -= null;
    }

    public virtual void OnHit(int damage)
    {
        if (hitPoints > 0)
        {
            hitPoints -= damage;   
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void OnClicked()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDeselect()
    {
        throw new NotImplementedException();
    }
}
