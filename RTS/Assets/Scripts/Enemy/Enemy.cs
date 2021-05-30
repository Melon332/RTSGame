using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Units
{
    protected override void Start()
    {
        canBeAttacked = true;
        _selectionBox = GetComponentInChildren<SelectionBox>().gameObject;
        if (_selectionBox != null)
        {
            _selectionBox.SetActive(false);
        }
    }

    public override void OnClicked()
    {
        Debug.Log("This is an enemy, you cannot select it");
        
        _selectionBox.SetActive(true);
    }

    public override void OnDeselect()
    {
        _selectionBox.SetActive(false);
        base.OnDeselect();
    }

    public override void OnHit(int damage)
    {
        damage += 50;
        base.OnHit(damage);
    }
}
