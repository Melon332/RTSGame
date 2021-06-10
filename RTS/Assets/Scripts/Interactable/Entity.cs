﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
using Managers;
using UnityEngine.AI;
using Player;
using UnityEditor;
using UnityEngine.UI;

public abstract class Entity : MonoBehaviour, IInteractable,ISubscriber,IDestructable
{
    #region Unit Variables
    [Header("General Unit Variables")]
    public float maxHitPoints;
    public float hitPoints;
    public string nameOfUnit;
    public bool canBeAttacked;
    public bool isBuilding = false;
    public Sprite pictureOfObject;
    public bool isSelectable;
    [HideInInspector] public bool isDead = false;
    public bool canAttack;
    #endregion

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public GameObject selectionBox;
    public bool hasBeenConstructed;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        selectionBox = GetComponentInChildren<SelectionBox>().gameObject;
        if (selectionBox == null) return;
        selectionBox.SetActive(false);
        selectionBox.transform.localScale = gameObject.transform.localScale * 2;
    }

    public virtual void Subscribe(CharacterInput publisher)
    {
        
    }

    public virtual void UnSubscribe(CharacterInput publisher)
    {
        
    }

    public virtual void OnHit(int damage)
    {
        if (hitPoints <= 0)
        {  
            isDead = true;
            OnDeselect();
            Destroy(gameObject,2f);
            gameObject.SetActive(false);
            UnitManager.SelectableUnits.Remove(gameObject);
            UnitManager.Instance.selectedAttackingUnits.Remove(gameObject);
            UnitManager.Instance.selectedNonLethalUnits.Remove(gameObject);
        }
        else
        {
            hitPoints -= damage;
        }
    }

    public virtual void OnClicked()
    {
        if (pictureOfObject != null)
        {
            UIManager.Instance.PictureOfSelectedUnits(pictureOfObject);
        }
        else
        {
            Debug.Log("Sorry, you are missing a picture for this unit " + nameOfUnit + " Maybe you forgot to add it?");
        }
        selectionBox.SetActive(true);
    }

    public virtual void OnDeselect()
    {
       selectionBox.SetActive(false);
       if (PlayerManager.Instance.hasSelectedUnits || PlayerManager.Instance.hasSelectedNonLethalUnits) return;
       UIManager.Instance.PictureOfSelectedUnits(null);
    }
}
