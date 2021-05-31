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
    public string nameOfUnit;
    public bool canBeAttacked;
    #endregion

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public GameObject _selectionBox;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _selectionBox = GetComponentInChildren<SelectionBox>().gameObject;
        if (_selectionBox == null) return;
        _selectionBox.SetActive(false);
        _selectionBox.transform.localScale = gameObject.transform.localScale * 2;
    }

    public virtual void Subscribe(CharacterInput publisher)
    {
        
    }

    public virtual void UnSubscribe(CharacterInput publisher)
    {
        
    }

    public virtual void OnHit(int damage)
    {
        if (hitPoints >= 0)
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
        _selectionBox.SetActive(true);
    }

    public virtual void OnDeselect()
    {
       _selectionBox.SetActive(false);
    }
}
