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
    [Header("Hitpoints and name")]
    public float maxHitPoints;
    public float hitPoints;
    public string nameOfUnit;
    [Header("Cost and power consumption")]
    public int objectCost;
    public int costOfPower;
    [Header("Is it a building and is it friendly?")]
    public bool canBeAttacked;
    public bool isEnemy;
    public bool isBuilding = false;
    [Header("Can be clicked by player? Picture of player")]
    public Sprite pictureOfObject;
    public bool isSelectable;
    [HideInInspector] public bool isDead = false;
    public bool canAttack;
    #endregion

    [HideInInspector] public NavMeshAgent agent;
    public GameObject selectionBox;
    public bool hasBeenConstructed;
    [HideInInspector] public bool hasBeenPickedUpByPool;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        selectionBox = GetComponentInChildren<SelectionBox>().gameObject;
        if (selectionBox == null) return;
        selectionBox.SetActive(false);
        selectionBox.transform.localScale = gameObject.transform.localScale * 2;
    }

    protected virtual void Start()
    {
        if (!isEnemy) return;
        canBeAttacked = true;
        hitPoints = maxHitPoints;
        GameManager._enemyManager.AddEnemyToList(this);
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
            if (!isEnemy)
            {
                isDead = true;
                OnDeselect();
                gameObject.SetActive(false);
                UnitManager.SelectableUnits.Remove(gameObject);
                UnitManager.Instance.selectedAttackingUnits.Remove(gameObject);
                UnitManager.Instance.selectedNonLethalUnits.Remove(gameObject);
                hasBeenPickedUpByPool = false;
            }
            else
            {
                isDead = true;
                OnDeselect();
                gameObject.SetActive(false);
                GameManager._enemyManager.RemoveEnemyFromList(this);
            }
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
        if (PlayerManager.Instance.hasSelectedUnits) return;
        UIManager.Instance.PictureOfSelectedUnits(null);
    }

    public virtual void OnDisable()
    {
        
    }

    public virtual void OnEnable()
    {
        
    }
}
