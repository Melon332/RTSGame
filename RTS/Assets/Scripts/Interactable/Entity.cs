using System;
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
    public bool isBuilding = false;
    [Header("Can be clicked by player? Picture of player")]
    public Sprite pictureOfObject;
    public bool isSelectable;
    [HideInInspector] public bool isDead = false;
    public bool canAttack;
    public bool isEnemy;
    #endregion

    [HideInInspector] public NavMeshAgent agent;
    public GameObject selectionBox;
    public bool hasBeenConstructed;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        selectionBox = GetComponentInChildren<SelectionBox>().gameObject;
        if (selectionBox == null) return;
        selectionBox.SetActive(false);
        selectionBox.transform.localScale = gameObject.transform.localScale * 2;
        if (isEnemy)
        {
            canBeAttacked = true;
        }
    }

    protected virtual void Start()
    {
        
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
