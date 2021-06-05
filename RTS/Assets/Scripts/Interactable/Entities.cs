using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
using Managers;
using UnityEngine.AI;
using Player;
using UnityEngine.UI;

public abstract class Entities : MonoBehaviour, IInteractable,ISubscriber,IDestructable
{
    #region Unit Variables
    public float hitPoints;
    public float maxHitPoints;
    public string nameOfUnit;
    public bool canBeAttacked;
    public Sprite pictureOfObject;
    [HideInInspector] public bool isDead = false;
    protected bool hasFoundTarget = false;
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
        if (hitPoints <= 0)
        {  
            isDead = true;
            OnDeselect();
            Destroy(gameObject,2f);
            gameObject.SetActive(false);
            UnitManager.SelectableUnits.Remove(gameObject);
            UnitManager.Instance.selectedUnits.Remove(gameObject);
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
        _selectionBox.SetActive(true);
    }

    public virtual void OnDeselect()
    {
       _selectionBox.SetActive(false);
       UIManager.Instance.PictureOfSelectedUnits(null);
    }
}
